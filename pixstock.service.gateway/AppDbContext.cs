using System;
using System.IO;
using Hyperion.Pf.Entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NLog;
using Pixstock.Nc.Srv.Ext;
using Pixstock.Service.Infra;
using Pixstock.Service.Infra.Model;
using Pixstock.Service.Model;

namespace Pixstock.Service.Gateway
{
    public class AppDbContext : KatalibDbContext, IAppDbContext
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private IApplicationContext context;

        private ExtentionManager extentionManager;

        public DbSet<AppMetaInfo> AppMetaInfos { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Content> Contents { get; set; }

        public DbSet<FileMappingInfo> FileMappingInfos { get; set; }

        public DbSet<Label> Labels { get; set; }

        public DbSet<EavInteger> EavInts { get; set; }

        public DbSet<EavText> EavTexts { get; set; }

        public DbSet<EavBool> EavBools { get; set; }

        public DbSet<EavDate> EavDates { get; set; }

        public AppDbContext(IApplicationContext context, ExtentionManager extentionManager)
        {
            this.context = context;
            this.extentionManager = extentionManager;
        }

        protected IApplicationContext Context { get => context; }

        protected override void OnCreate(EntityEntry entry)
        {
            // 各エンティティごとのCreateEntityカットポイントを呼び出します
            if (entry.Entity is ICategory)
            {

            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseFilePath = Path.Combine(this.context.DatabaseDirectoryPath, "pixstock.db");
            optionsBuilder.UseSqlite("Data Source=" + databaseFilePath);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Label2Content>()
                .HasKey(t => new { t.LabelId, t.ContentId });

            modelBuilder.Entity<Label2Content>()
                .HasOne(pt => pt.Content)
                .WithMany(p => p.Labels)
                .HasForeignKey(pt => pt.ContentId);

            modelBuilder.Entity<Label2Content>()
                .HasOne(pt => pt.Label)
                .WithMany(t => t.Contents)
                .HasForeignKey(pt => pt.LabelId);

            modelBuilder.Entity<Label2Category>()
                .HasKey(t => new { t.LabelId, t.CategoryId });

            modelBuilder.Entity<Label2Category>()
                .HasOne(pt => pt.Category)
                .WithMany(p => p.Labels)
                .HasForeignKey(pt => pt.CategoryId);

            modelBuilder.Entity<Label2Category>()
                .HasOne(pt => pt.Label)
                .WithMany(t => t.Categories)
                .HasForeignKey(pt => pt.LabelId);

            // EAV(複合キー)
            modelBuilder.Entity<EavInteger>()
                .HasKey(c => new { c.CategoryName, c.EntityTypeName, c.Key });
            modelBuilder.Entity<EavText>()
                .HasKey(c => new { c.CategoryName, c.EntityTypeName, c.Key });
            modelBuilder.Entity<EavBool>()
                .HasKey(c => new { c.CategoryName, c.EntityTypeName, c.Key });
            modelBuilder.Entity<EavDate>()
                .HasKey(c => new { c.CategoryName, c.EntityTypeName, c.Key });
        }
    }
}