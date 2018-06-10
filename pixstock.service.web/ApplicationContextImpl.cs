using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Hyperion.Pf.Entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NLog;
using Pixstock.Nc.Common;
using Pixstock.Service.Gateway;
using Pixstock.Service.Gateway.Repository;
using Pixstock.Service.Infra;
using Pixstock.Service.Model;
using SimpleInjector.Lifestyles;

[assembly: InternalsVisibleTo("Pixstock.Service.App.Tests")]
namespace Pixstock.Service.App
{
    public class ApplicationContextImpl : IApplicationContext
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public IBuildAssemblyParameter _AssemblyParameter;

        private string _ApplicationDirectoryPath;

        //private bool _alreadyDisposed = false;

        private SimpleInjector.Container _Container;

        public string ApplicationDirectoryPath => _ApplicationDirectoryPath;

        public System.Diagnostics.FileVersionInfo ApplicationFileVersionInfo
        {
            get;
            private set;
        }

        public string DatabaseDirectoryPath => Path.Combine(ApplicationDirectoryPath, @"db");

        public string ExtentionDirectoryPath => Path.Combine(ApplicationDirectoryPath, @"extention");

        public ApplicationContextImpl(IBuildAssemblyParameter parameter)
        {
            _ApplicationDirectoryPath = "";

            _AssemblyParameter = parameter;

            if (parameter.Params["AbsoluteApplicationDirectoryBase"] == "true")
            {
                _ApplicationDirectoryPath = _AssemblyParameter.Params["ApplicationDirectoryPath"];
            }
            else
            {
                _ApplicationDirectoryPath = Path.Combine(
                    System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    _AssemblyParameter.Params["ApplicationDirectoryPath"]);
            }

            this.ApplicationFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _logger.Info("_ApplicationDirectoryPath = " + _ApplicationDirectoryPath);
        }

        public void SetDiContainer(SimpleInjector.Container container)
        {
            this._Container = container;
        }

        public void Initialize()
        {
            try
            {
                CreateSettingSQLite();

                InitializeDirectory();
                InitializeAppDatabase();
                InitializeThumbnailDatabase();
            }
            catch (Exception expr)
            {
                _logger.Fatal("初期化に失敗しました.", expr.Message);
            }
        }

        /// <summary>
        /// SQLiteを使用するための設定を読み込みます
        /// </summary>
        void CreateSettingSQLite()
        {
            SqliteConnectionStringBuilder builder_AppDb = new SqliteConnectionStringBuilder();
            builder_AppDb.DataSource = Path.Combine(DatabaseDirectoryPath, "pixstock.db");

            // TODO: BuilderをDatabaseContextに設定する
        }

        /// <summary>
        /// 必要なディレクトリを作成する初期化処理
        /// </summary>
        private void InitializeDirectory()
        {
            // アプリケーションが使用する各種ディレクトリの作成
            System.IO.Directory.CreateDirectory(ApplicationDirectoryPath);
            System.IO.Directory.CreateDirectory(DatabaseDirectoryPath);
            System.IO.Directory.CreateDirectory(ExtentionDirectoryPath);
        }

        /// <summary>
        /// データベースに関する初期化処理
        /// </summary>
        private void InitializeAppDatabase()
        {
            AppMetaInfo apMetadata = null;
            bool isMigrate = false;

            const string appdb_structure_version_key = "APPDB_VER";

            var @dbc = (AppDbContext)_Container.GetInstance<IAppDbContext>(); // DIコンテナがリソースの開放を行う
            bool isInitializeDatabase = false;
            var @repo = new AppAppMetaInfoRepository(@dbc);
            try
            {
                apMetadata = @repo.FindBy(p => p.Key == appdb_structure_version_key).FirstOrDefault();
                if (apMetadata == null) isInitializeDatabase = true;
            }
            catch (Exception)
            {
                isInitializeDatabase = true;
            }

            if (isInitializeDatabase)
            {
                // データベースにテーブルなどの構造を初期化する
                string sqltext = "";
                System.Reflection.Assembly assm = System.Reflection.Assembly.GetExecutingAssembly();

                using (var stream = assm.GetManifestResourceStream(string.Format("Pixstock.Service.App.Assets.Sql.{0}.Initialize_sql.txt", "App")))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        sqltext = reader.ReadToEnd();
                    }
                }

                _logger.Trace("SQLファイルから、CREATEを読み込みます");
                @dbc.Database.ExecuteSqlCommand(sqltext);
                @dbc.SaveChanges();

                // 初期データを格納する
                var repo_Category = new CategoryRepository(@dbc);
                repo_Category.Add(new Category
                {
                    Id = 1L,
                    Name = "ROOT"
                });

                apMetadata = @repo.FindBy(p => p.Key == appdb_structure_version_key).FirstOrDefault();
            }

            if (apMetadata == null)
            {
                apMetadata = new AppMetaInfo { Key = appdb_structure_version_key, Value = "1.0.0" };
                @repo.Add(apMetadata);
                @repo.Save();

                // AppMetadataが存在しない場合のみ、初期値の書き込みを行う。
                // 初期値は、キッティングから取得したリソースパスが示すSQLファイルからデータベースの初期値を読み込む。
                if (this._AssemblyParameter.Params.ContainsKey("InitializeSqlAppDb"))
                {
                    System.Reflection.Assembly assm = System.Reflection.Assembly.GetExecutingAssembly();
                    var initializeSqlFilePath = this._AssemblyParameter.Params["InitializeSqlAppDb"];
                    _logger.Info($"アプリケーションデータベース初期化SQLファイルを読み込みます path:{initializeSqlFilePath}");
                    string sqltext = "";
                    using (var stream = assm.GetManifestResourceStream(initializeSqlFilePath))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            sqltext = reader.ReadToEnd();
                        }
                    }

                    @dbc.Database.ExecuteSqlCommand(sqltext);
                    @dbc.SaveChanges();
                }
            }

            string currentVersion = apMetadata.Value;
            string nextVersion = currentVersion;
            do
            {
                currentVersion = nextVersion;
                nextVersion = UpgradeFromResource("App", currentVersion, @dbc);
                if (nextVersion != currentVersion) isMigrate = true;
            } while (nextVersion != currentVersion);

            if (isMigrate)
            {
                apMetadata.Value = nextVersion;

                @repo.Save();
            }

            @dbc.SaveChanges();
        }

        /// <summary>
        /// データベースに関する初期化処理
        /// </summary>
        private void InitializeThumbnailDatabase()
        {
            AppMetaInfo apMetadata = null;
            bool isMigrate = false;

            const string appdb_structure_version_key = "APPDB_VER";

            var @dbc = (ThumbnailDbContext)_Container.GetInstance<IThumbnailDbContext>(); // DIコンテナがリソースの開放を行う
            bool isInitializeDatabase = false;
            var @repo = new ThumbnailAppMetaInfoRepository(@dbc);
            try
            {
                apMetadata = @repo.FindBy(p => p.Key == appdb_structure_version_key).FirstOrDefault();
                if (apMetadata == null) isInitializeDatabase = true;
            }
            catch (Exception)
            {
                isInitializeDatabase = true;
            }

            if (isInitializeDatabase)
            {
                // データベースにテーブルなどの構造を初期化する
                string sqltext = "";
                System.Reflection.Assembly assm = System.Reflection.Assembly.GetExecutingAssembly();

                using (var stream = assm.GetManifestResourceStream(string.Format("Pixstock.Service.App.Assets.Sql.{0}.Initialize_sql.txt", "Thumbnail")))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        sqltext = reader.ReadToEnd();
                    }
                }

                _logger.Trace("SQLファイルから、CREATEを読み込みます");
                @dbc.Database.ExecuteSqlCommand(sqltext);
                @dbc.SaveChanges();

                apMetadata = @repo.FindBy(p => p.Key == appdb_structure_version_key).FirstOrDefault();
            }

            if (apMetadata == null)
            {
                apMetadata = new AppMetaInfo { Key = appdb_structure_version_key, Value = "1.0.0" };
                @repo.Add(apMetadata);

                @repo.Save();
            }

            string currentVersion = apMetadata.Value;
            string nextVersion = currentVersion;
            do
            {
                currentVersion = nextVersion;
                nextVersion = UpgradeFromResource("Thumbnail", currentVersion, @dbc);
                if (nextVersion != currentVersion) isMigrate = true;
            } while (nextVersion != currentVersion);

            if (isMigrate)
            {
                apMetadata.Value = nextVersion;

                @repo.Save();
            }

            @dbc.SaveChanges();
        }

        /// <summary>
        /// 現在のバージョンからマイグレーションするファイルがリソースファイルにあるか探します。
        /// リソースファイルがある場合はそのファイルに含まれるSQLを実行し、ファイル名からマイグレーション後のバージョンを取得します。
        /// </summary>
        /// <param name="version">現在のバージョン。アップグレード元のバージョン。</param>
        /// <returns>次のバージョン番号。マイグレーションを実施しなかった場合は、versionの値がそのまま帰ります。</returns>
        private string UpgradeFromResource(string dbselect, string version, KatalibDbContext @dbc)
        {
            System.Reflection.Assembly assm = System.Reflection.Assembly.GetExecutingAssembly();

            string currentVersion = version;
            var mss = assm.GetManifestResourceNames();

            // この方法で読み込みができるリソースファイルの種類は「埋め込みリソース」を設定したもののみです。
            var r = new Regex(string.Format("Pixstock.Service.App.Assets.Sql.{0}.{1}", dbselect, "upgrade - " + currentVersion + "-(.+)\\.txt"));
            foreach (var rf in assm.GetManifestResourceNames())
            {
                var matcher = r.Match(rf);
                if (matcher.Success && matcher.Groups.Count > 1)
                {
                    _logger.Info("{0}データベースのアップデート({1} -> {2})", dbselect, version, matcher.Groups[1].Value);
                    UpgradeDatabase(rf, @dbc);
                    currentVersion = matcher.Groups[1].Value; // 正規表現にマッチした箇所が、マイグレート後のバージョンになります。
                }
            }

            return currentVersion;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="dbc">データベース</param>
        private void UpgradeDatabase(string resourcePath, KatalibDbContext dbc)
        {
            string sqltext = "";
            System.Reflection.Assembly assm = System.Reflection.Assembly.GetExecutingAssembly();

            using (var stream = assm.GetManifestResourceStream(resourcePath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    sqltext = reader.ReadToEnd();
                }
            }

            dbc.Database.ExecuteSqlCommand(sqltext);
        }
    }
}