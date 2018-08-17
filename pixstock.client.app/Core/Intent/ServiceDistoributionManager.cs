using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.core.IpcApi;
using pixstock.client.app.Core.Intent;
using SimpleInjector;

namespace pixstock.apl.app.core.Intent {
  public class ServiceDistoributionManager : IServiceDistoributor {
    private ILogger mLogger;

    private readonly ServiceDistributionResolveHandlerFactory mFactory;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ServiceDistoributionManager (ServiceDistributionResolveHandlerFactory factory) {
      this.mFactory = factory;
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    public void ExecuteService (ServiceType service, string intentName, object parameter) {
      this.mLogger.Debug ("[ExecuteService] ServiceType={ServiceType} IntentName={IntentName}", service, intentName);
      try {
        // 各サービスへは、Intentパラメータとして処理を呼び出す
        var serviceObj = mFactory.CreateNew (service.ToString ());
        serviceObj.Handle (new IntentParam (intentName) { ExtraData = parameter });
      } catch (Exception expr) {
        mLogger.Error (expr, "[ExecuteService] Failer Service. {@IntentName}の処理でエラーが発生しました。", intentName);
        mLogger.Error (expr.StackTrace);
      }
    }

  }
}
