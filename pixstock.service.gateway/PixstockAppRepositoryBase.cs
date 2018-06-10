using System;
using System.Linq;
using Hyperion.Pf.Entity;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using NLog;
using Pixstock.Service.Infra.Model.Eav;
using Pixstock.Service.Model;

namespace Pixstock.Service.Gateway.Repository
{

    public abstract class PixstockAppRepositoryBase<T, INTERFACE> : GenericRepository<T>
            where T : class
            where INTERFACE: IEntity<long>
    {
        private static NLog.Logger LOG = LogManager.GetCurrentClassLogger();

        private readonly string mEavEntityTypeName;

        private string mCurrentCategoryName = "SYS";

        public PixstockAppRepositoryBase(DbContext context, string eavEntityTypeName) : base(context)
        {
            this.mEavEntityTypeName = eavEntityTypeName;
        }

        public void SetCategoryName(string currentCategoryName)
        {
            this.mCurrentCategoryName = currentCategoryName;
        }

        public IEavInteger GetEavInteger(INTERFACE entity, string key)
        {
            var eavDataset = this.Entities.Set<EavInteger>();
            var r = eavDataset.Where(prop => prop.EntityTypeName == mEavEntityTypeName)
                .Where(prop => prop.EntityId == entity.Id)
                .Where(prop => prop.CategoryName == mCurrentCategoryName)
                .Where(prop => prop.Key == key);
            return r.AsNoTracking().FirstOrDefault();
        }

        public void SetEavInteger(INTERFACE entity, string key, int value)
        {
            var eavDataset = this.Entities.Set<EavInteger>();
            var eav = GetEavInteger(entity, key);
            if (eav == null)
            {
                var inst = new EavInteger
                {
                    EntityTypeName = mEavEntityTypeName,
                    EntityId = entity.Id,
                    CategoryName = mCurrentCategoryName,
                    Key = key
                };
                eavDataset.Add(inst);
                eav = inst;
            }
            else
            {
                eavDataset.Update((EavInteger)eav);
            }

            eav.Value = value;
        }

        public IEavText GetEavText(INTERFACE entity, string key)
        {
            var eavDataset = this.Entities.Set<EavText>();
            var r = eavDataset.Where(prop => prop.EntityTypeName == mEavEntityTypeName)
                .Where(prop => prop.EntityId == entity.Id)
                .Where(prop => prop.CategoryName == mCurrentCategoryName)
                .Where(prop => prop.Key == key);
            return r.AsNoTracking().FirstOrDefault();
        }

        public void SetEavText(INTERFACE entity, string key, string value)
        {
            var eavDataset = this.Entities.Set<EavText>();
            var eav = GetEavText(entity, key);
            if (eav == null)
            {
                var inst = new EavText
                {
                    EntityTypeName = mEavEntityTypeName,
                    EntityId = entity.Id,
                    CategoryName = mCurrentCategoryName,
                    Key = key
                };
                eavDataset.Add(inst);
                eav = inst;
            }
            else
            {
                eavDataset.Update((EavText)eav);
            }

            eav.Value = value;
        }

        public IEavBool GetEavBool(INTERFACE entity, string key)
        {
            var eavDataset = this.Entities.Set<EavBool>();
            var r = eavDataset.Where(prop => prop.EntityTypeName == mEavEntityTypeName)
                .Where(prop => prop.EntityId == entity.Id)
                .Where(prop => prop.CategoryName == mCurrentCategoryName)
                .Where(prop => prop.Key == key);
            return r.AsNoTracking().FirstOrDefault();
        }

        public void SetEavBool(INTERFACE entity, string key, bool value)
        {
            var eavDataset = this.Entities.Set<EavBool>();
            var eav = GetEavBool(entity, key);
            if (eav == null)
            {
                var inst = new EavBool
                {
                    EntityTypeName = mEavEntityTypeName,
                    EntityId = entity.Id,
                    CategoryName = mCurrentCategoryName,
                    Key = key
                };
                eavDataset.Add(inst);
                eav = inst;
            }
            else
            {
                eavDataset.Update((EavBool)eav);
            }

            eav.Value = value;
        }

        public IEavDate GetEavDate(INTERFACE entity, string key)
        {
            var eavDataset = this.Entities.Set<EavDate>();
            var r = eavDataset.Where(prop => prop.EntityTypeName == mEavEntityTypeName)
                .Where(prop => prop.EntityId == entity.Id)
                .Where(prop => prop.CategoryName == mCurrentCategoryName)
                .Where(prop => prop.Key == key);
            return r.AsNoTracking().FirstOrDefault();
        }

        public void SetEavDate(INTERFACE entity, string key, DateTime? value)
        {
            var eavDataset = this.Entities.Set<EavDate>();
            var eav = GetEavDate(entity, key);
            if (eav == null)
            {
                var inst = new EavDate
                {
                    EntityTypeName = mEavEntityTypeName,
                    EntityId = entity.Id,
                    CategoryName = mCurrentCategoryName,
                    Key = key
                };
                eavDataset.Add(inst);
                eav = inst;
            }
            else
            {
                eavDataset.Update((EavDate)eav);
            }

            eav.Value = value;
        }
    }
}