using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using pixstock.apl.app.core.Infra;

namespace pixstock.apl.app.core.IpcApi.Handler
{
  /// <summary>
  /// IPCメッセージを、Intentメッセージとして登録するIPCハンドラ
  /// </summary>
  public class PixstockIntentHandler : IRequestHandler
  {
    readonly IIntentManager mIntentManager;
    readonly ILogger mLogger;

    public PixstockIntentHandler(IIntentManager intentManager, ILoggerFactory loggerFactory)
    {
      this.mIntentManager = intentManager;
      mLogger = loggerFactory.CreateLogger<PixstockIntentHandler>();
    }

    public void Handle(IpcMessage param)
    {
      mLogger.LogDebug("[Handle(IpcMessage)] - IN " + param);

      try
      {
        // IPCメッセージから、PixstockのIntentメッセージを取り出す処理
        var message = JsonConvert.DeserializeObject<IntentMessage>(param.Body.ToString());
        mIntentManager.AddIntent(message.ServiceType, message.MessageName, message.Parameter);
      }catch(Exception expr)
      {
        mLogger.LogError(expr, "[Handle(IpcMessage)] Faile Deserialize");
      }
    }
  }
}
