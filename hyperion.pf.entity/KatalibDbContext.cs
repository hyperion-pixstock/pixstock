using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Hyperion.Pf.Entity
{
    /// <summary>
    ///
    /// </summary>
    public abstract class KatalibDbContext : DbContext
    {

        public KatalibDbContext()
        {
        }

        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                if (entry.State == EntityState.Added)
                {
                    OnCreate(entry);
                }

                ProcAuditableEntity(entry);
                ProcSaveEntity(entry);
            }

            // 削除対象のEntityで、IDeleteEntityを実装しているオブジェクトを見つける
            var deleteEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IDeleteEntity
                    && (x.State == EntityState.Deleted));
            foreach (var entry in deleteEntries)
            {
                ProcDeleteEntity(entry);
            }

            return base.SaveChanges();
        }

        /// <summary>
        /// 新規エンティティの作成契機に呼び出します。
        /// オーバーライドして任意の処理を追加します。
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void OnCreate(EntityEntry entry)
        {

        }

        protected virtual void ProcAuditableEntity(EntityEntry entry)
        {
            string identityName = "SYS";//Thread.CurrentPrincipal.Identity.Name;
            DateTime now = DateTime.Now;

            IAuditableEntity auditableEntity = entry.Entity as IAuditableEntity;
            if (auditableEntity != null)
            {
                if (entry.State == EntityState.Added)
                {
                    auditableEntity.CreatedBy = identityName;
                    auditableEntity.CreatedDate = now;
                }
                else
                {
                    base.Entry(auditableEntity).Property(x => x.CreatedBy).IsModified = false;
                    base.Entry(auditableEntity).Property(x => x.CreatedDate).IsModified = false;
                }

                auditableEntity.UpdatedBy = identityName;
                auditableEntity.UpdatedDate = now;
            }
        }

        protected virtual void ProcSaveEntity(EntityEntry entry)
        {
            // ISaveEntity
            ISaveEntity saveEntity = entry.Entity as ISaveEntity;
            if (saveEntity != null)
                saveEntity.OnSave(this);
        }

        protected virtual void ProcDeleteEntity(EntityEntry entry)
        {
            // IDeleteEntity
            IDeleteEntity deleteEntity = entry.Entity as IDeleteEntity;
            if (deleteEntity != null)
                deleteEntity.OnDelete(this);
        }

    }
}