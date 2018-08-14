using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NLog;
using pixstock.apl.app.core.Infra;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;

namespace pixstock.client.app.Core.IpcApi.Handler {
  public class UpdatePropHandler : IResolveDeclare {
    public string ResolveName => "UpdateProp";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      readonly Logger mLogger;

      readonly IMemoryCache mMemoryCache;

      /// <summary>
      /// コンストラクタ
      /// </summary>
      /// <param name="memoryCache"></param>
      public Handler (IMemoryCache memoryCache) {
        this.mLogger = LogManager.GetCurrentClassLogger ();
        this.mMemoryCache = memoryCache;
      }

      public override void Handle (object param) {
        IpcSendServiceParam serviceParam = (IpcSendServiceParam) param;

        var propertyName = serviceParam.Data.ToString ();
        object cachedObject;
        if (mMemoryCache.TryGetValue (propertyName, out cachedObject)) {
          var ipcMessage = new IpcMessage ();
          object obj = new {
            PropertyName = propertyName,
            Value = JsonConvert.SerializeObject (cachedObject)
          };

          ipcMessage.Body = JsonConvert.SerializeObject (obj, Formatting.Indented);
          this.mLogger.Debug ("UpdateProp 送信本文={0}", ipcMessage.Body);

          var mainWindow = Electron.WindowManager.BrowserWindows.First ();
          Electron.IpcMain.Send (mainWindow, "IPC_UPDATEPROP", ipcMessage);
        } else {
          this.mLogger.Warn ("[Execute] Faile MemCache");
        }
      }
    }
  }
}
