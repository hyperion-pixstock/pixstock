using System;
using Pixstock.Service.Infra.Model.Eav;

namespace Pixstock.Service.Infra.Repository
{
    public interface IEavDateRepository<T>
    {
        IEavDate GetEavDate(T entity, string key);
        
        void SetEavDate(T entity, string key, DateTime? value);
    }
}