using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using Newtonsoft.Json;
using NLog;
using pixstock.apl.app.core.Infra;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using static pixstock.apl.app.core.ScreenManager;

namespace pixstock.client.app.Core.IpcApi.Handler {
  public class UpdateViewHandler : IResolveDeclare {
    readonly ILogger mLogger;

    public string ResolveName => "UpdateView";

    public Type ResolveType => typeof (Handler);

    public UpdateViewHandler () {
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    public class Handler : PackageResolveHandler {
      readonly ILogger mLogger;

      public Handler () {
        this.mLogger = LogManager.GetCurrentClassLogger ();
      }

      public override void Handle (object param) {
        IpcSendServiceParam serviceParam = (IpcSendServiceParam) param;

        var messageparameter = serviceParam.Data as UpdateViewResponse;

        var ipcMessage = new IpcMessage ();
        object obj = new {
          UpdateList = messageparameter.ViewEventList,
          Parameter = messageparameter.Parameter,
          NextScreenName = messageparameter.NextScreenName
        };
        ipcMessage.Body = JsonConvert.SerializeObject (obj, Formatting.Indented);
        mLogger.Info ("Body(JSON)=" + ipcMessage.Body);

        var mainWindow = Electron.WindowManager.BrowserWindows.First ();
        Electron.IpcMain.Send (mainWindow, "IPC_UPDATEVIEW", ipcMessage);
      }
    }
  }
}
