using System;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.core.Bridge.Handler;

namespace pixstock.apl.app.core.Bridge.Message
{
    /// <summary>
    /// この名前空間に、IPCメッセージ処理用のプラグインを追加してください。
    /// </summary>
    public class HelloIntentIpc : IIpcExtention
    {
        public string IpcMessageName => "HELLO_INTENT";

        public Type RequestHandler => typeof(PixstockIntentHandler);
    }
}
