using Pixstock.Service.Infra.Model.Eav;

namespace Pixstock.Service.Infra.Repository
{
    public interface IEavTextRepository<T>
    {
        IEavText GetEavText(T entity, string key);
        
        void SetEavText(T entity, string key, string value);
    }
}