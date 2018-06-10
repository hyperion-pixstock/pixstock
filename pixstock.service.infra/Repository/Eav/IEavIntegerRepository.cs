using Pixstock.Service.Infra.Model.Eav;

namespace Pixstock.Service.Infra.Repository
{
    public interface IEavIntegerRepository<T>
    {
        IEavInteger GetEavInteger(T entity, string key);
        
        void SetEavInteger(T entity, string key, int value);
    }
}