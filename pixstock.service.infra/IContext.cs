using Pixstock.Service.Infra.Repository;

namespace Pixstock.Nc.Common
{
    public interface IContext
    {
        Type InjectionInstance<Type>() where Type : class;
    }
}