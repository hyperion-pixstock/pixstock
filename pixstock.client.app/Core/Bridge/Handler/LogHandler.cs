using System;
using pixstock.apl.app.core.Infra;

namespace pixstock.apl.app.core.Bridge.Handler {
    /// <summary>
    /// ログ出力用のIPCハンドラ
    /// </summary>
    public class LogHandler : IRequestHandler
    {
        public void Handle(IpcMessage param)
        {
            Console.WriteLine("Execute LogHandler.Handle");
        }
    }
}
