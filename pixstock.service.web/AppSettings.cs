namespace Pixstock.Service.App
{
    public class AppSettings
    {
        /// <summary>
        /// アプリケーションが使用するフォルダの基本パスを設定、または取得します。
        /// </summary>
        /// <returns></returns>
        public string ApplicationDirectoryBasePath { get; set; }

        /// <summary>
        /// "ApplicationDirectoryBasePath"が示すディレクトリが、絶対パスを示しているか
        /// </summary>
        /// <returns></returns>
        public bool AbsoluteApplicationDirectoryBase { get; set; }

        /// <summary>
        /// データベース初期化で使用するSQLファイルのリソースパス
        /// </summary>
        /// <returns></returns>
        public string InitializeSqlAppDb { get; set; }

        /// <summary>
        /// デフォルトワークスペース
        /// </summary>
        /// <returns></returns>
        public DefaultWorkspace Workspace { get; set; }

        public class DefaultWorkspace
        {
            public string Name { get; set; }
            public bool RelativeApplicationDirectoryBasePath { get; set; }
            public string VirtualPath { get; set; }
            public string PhysicalPath { get; set; }
        }
    }
}