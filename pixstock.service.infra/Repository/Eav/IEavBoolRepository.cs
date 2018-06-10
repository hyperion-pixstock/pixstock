using Pixstock.Service.Infra.Model.Eav;

namespace Pixstock.Service.Infra.Repository
{
    public interface IEavBoolRepository<T>
    {
        IEavBool GetEavBool(T entity, string key);
        
        void SetEavBool(T entity, string key, bool value);
    }
}