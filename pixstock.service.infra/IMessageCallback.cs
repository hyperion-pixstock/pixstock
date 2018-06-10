using Pixstock.Service.Infra.Repository;
using SimpleInjector;

namespace Pixstock.Nc.Common
{
    /// <summary>
    /// メッセージングフレームワークで使用するコールバックメソッドを持つインターフェース
    /// </summary>
    public delegate void MessageCallback(IMessageContext context);

    public interface IMessageContext : IContext
    {
        int getInt();
        long getLong();
        string getString();
    }
}