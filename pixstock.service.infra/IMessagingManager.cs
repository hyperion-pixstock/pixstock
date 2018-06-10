using Pixstock.Service.Infra.Extention;

namespace Pixstock.Nc.Common
{
    /// <summary>
    /// メッセージマネージャのインターフェースを使用するのは拡張機能からのみを想定
    /// </summary>
    public interface IMessagingManager
    {
        void RegisterMessage(string messageName, IExtentionMetaInfo extention, MessageCallback callback);
        void UnegisterMessage(string messageName, IExtentionMetaInfo extention, MessageCallback callback);
        void Dispatcher(string messageName, int param);
        void Dispatcher(string messageName, long param);
        void Dispatcher(string messageName, string param);
    }
}