using System.IO;
using Hyperion.Pf.Entity;
using Microsoft.EntityFrameworkCore;
using Pixstock.Service.Infra;
using Pixstock.Service.Model;

namespace Pixstock.Service.Gateway
{
    /// <summary>
    /// サムネイル情報のデータベース
    /// </summary>
    public class ThumbnailDbContext : KatalibDbContext, IThumbnailDbContext
    {
        IApplicationContext context;

        public DbSet<AppMetaInfo> AppMetaInfos { get; set; }

        public DbSet<Thumbnail> Thumbnails { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context"></param>
        public ThumbnailDbContext(IApplicationContext context)
        {
            this.context = context;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseFilePath = Path.Combine(this.context.DatabaseDirectoryPath, "thumb.db");
            optionsBuilder.UseSqlite("Data Source=" + databaseFilePath);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
        }
    }
}