using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.core.IpcApi;
using pixstock.client.app.Core.Intent;
using SimpleInjector;

namespace pixstock.apl.app.core.Intent
{
  public class ServiceDistoributionManager : IServiceDistoributor
  {
    private ILogger mLogger;

    private readonly ServiceDistributionResolveHandlerFactory mFactory;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ServiceDistoributionManager(ServiceDistributionResolveHandlerFactory factory, ILoggerFactory loggerFactory)
    {
      this.mFactory = factory;
      this.mLogger = loggerFactory.CreateLogger(this.GetType().FullName);
    }
    
    public void ExecuteService(ServiceType service, string intentName, object parameter)
    {
      this.mLogger.LogDebug(LoggingEvents.Undefine, "[ExecuteService] ServiceType={ServiceType} IntentName={IntentName}", service, intentName);
      try
      {
        // 各サービスへは、Intentパラメータとして処理を呼び出す
        var serviceObj = mFactory.CreateNew(service.ToString());
        serviceObj.Handle(new IntentParam(intentName) { ExtraData = parameter });
      }
      catch (Exception expr)
      {
        mLogger.LogError(LoggingEvents.Undefine, expr, "[ExecuteService] Failer Service");
      }
    }
    
  }
}
