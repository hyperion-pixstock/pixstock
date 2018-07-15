using System;
using System.Collections.Generic;
using System.Linq;
using ElectronNET.API;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
//using NLog;
using pixstock.apl.app.core.Infra;
using SimpleInjector;

namespace pixstock.apl.app.core.IpcApi
{
  public class IpcBridge
  {
    const string IPCEXTENTION_NAMESPACE = "pixstock.apl.app.core.IpcApi.Message";

    private readonly ILogger mLogger;

    readonly Container mContainer;

    /// <summary>
    ///
    /// </summary>
    /// <param name="container"></param>
    public IpcBridge(Container container, ILoggerFactory loggerFactory)
    {
      this.mContainer = container;
      this.mLogger = loggerFactory.CreateLogger<IpcBridge>();
    }

    /// <summary>
    /// Ipcハンドラの初期化
    /// </summary>
    public RequestHandlerFactory Initialize()
    {
      RequestHandlerFactory requestHandlerFactory = new RequestHandlerFactory(mContainer);
      Container localContainer = new Container();

      var repositoryAssembly = typeof(IpcBridge).Assembly;

      // IPCメッセージ処理プラグイン登録
      // IIpcExtentionインターフェースを実装するクラスのみ登録可能とする
      var registrations =
          from type in repositoryAssembly.GetExportedTypes()
          where type.Namespace == IPCEXTENTION_NAMESPACE
          where type.GetInterfaces().Contains(typeof(IIpcExtention))
          select new { Service = type.GetInterfaces().Single(), Implementation = type };

      List<Type> implementationList = new List<Type>();
      foreach (var reg in registrations)
      {
        mLogger.LogInformation("[Initialize] Register");

        implementationList.Add(reg.Implementation);
      }
      localContainer.RegisterCollection<IIpcExtention>(implementationList);
      mLogger.LogInformation("[Initialize] Register Complete");

      localContainer.Verify();

      // IPCメッセージハンドラ登録
      foreach (var ext in localContainer.GetAllInstances<IIpcExtention>())
      {
        requestHandlerFactory.Add(ext.IpcMessageName, ext.RequestHandler);
        mLogger.LogInformation("[Initialize] " + ext.IpcMessageName);

        Electron.IpcMain.On(ext.IpcMessageName, (param) =>
        {
          mLogger.LogInformation(string.Format("[IPC][Receive] {0}", ext.IpcMessageName));

          var factory = mContainer.GetInstance<IRequestHandlerFactory>();
          var handler = factory.CreateNew(ext.IpcMessageName);
          var requestParam = ((JObject)param).ToObject<IpcMessage>();
          handler.Handle(requestParam);
        });
      }

      return requestHandlerFactory;
    }
  }
}

