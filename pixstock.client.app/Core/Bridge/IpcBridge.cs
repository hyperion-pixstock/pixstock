using System;
using System.Collections.Generic;
using System.Linq;
using ElectronNET.API;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
//using NLog;
using pixstock.apl.app.core.Infra;
using SimpleInjector;

namespace pixstock.apl.app.core.Bridge
{
  /// <summary>
  /// フロントエンドからのIPC通信によるメッセージを受信するブリッジ
  /// </summary>
  public class IpcBridge
  {
    const string IPCEXTENTION_NAMESPACE = "pixstock.apl.app.core.Bridge.Message";

    private readonly ILogger mLogger;

    readonly Container mContainer;

    /// <summary>
    /// コンストラクタ
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
    public IpcRequestHandlerFactory Initialize()
    {
      IpcRequestHandlerFactory requestHandlerFactory = new IpcRequestHandlerFactory(mContainer);
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
      localContainer.Verify();
      mLogger.LogInformation("[Initialize] Register Complete");

      // 受信したIPCメッセージを処理するハンドラを登録
      foreach (var ext in localContainer.GetAllInstances<IIpcExtention>())
      {
        mLogger.LogInformation("[Initialize] " + ext.IpcMessageName);

        requestHandlerFactory.Add(ext.IpcMessageName, ext.RequestHandler);
        Electron.IpcMain.On(ext.IpcMessageName, ExecuteHandler);

        // IPC受信時の処理
        void ExecuteHandler(object param)
        {
          mLogger.LogInformation(string.Format("[IPC][Receive] {0}", ext.IpcMessageName));

          var factory = mContainer.GetInstance<IRequestHandlerFactory>();
          var handler = factory.CreateNew(ext.IpcMessageName);
          var requestParam = ((JObject)param).ToObject<IpcMessage>();
          handler.Handle(requestParam);
        }
      }

      return requestHandlerFactory;
    }
  }
}

