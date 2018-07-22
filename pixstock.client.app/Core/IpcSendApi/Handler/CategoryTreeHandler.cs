using ElectronNET.API;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.Models;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.IpcApi.Handler
{
  public class CategoryTreeHandler : IResolveDeclare
  {
    public string ResolveName => "CategoryTree";

    public Type ResolveType => typeof(Handler);

    public class Handler : PackageResolveHandler
    {
      readonly IMemoryCache mMemoryCache;

      public Handler(IMemoryCache memoryCache)
      {
        this.mMemoryCache = memoryCache;
      }

      public override void Handle(object param)
      {
        IpcSendServiceParam serviceParam = (IpcSendServiceParam)param;

        string cacheKey = "CategoryTree";
        var categoryId = long.Parse(serviceParam.Data.ToString());
        cacheKey += categoryId;

        // MemCacheから、更新通知を行うカテゴリオブジェクトを取得
        if (this.mMemoryCache.TryGetValue(cacheKey, out Category[] cachedObject))
        {
          var ipcMessage = new IpcMessage();
          object obj = new
          {
            PropertyName = "CategoryTree",
            Hint = categoryId,
            Value = JsonConvert.SerializeObject(cachedObject)
          };

          // Viewへ更新通知メッセージを送信する
          ipcMessage.Body = JsonConvert.SerializeObject(obj, Formatting.Indented);
          var mainWindow = Electron.WindowManager.BrowserWindows.First();
          Electron.IpcMain.Send(mainWindow, "IPC_UPDATEPROP", ipcMessage);
        }
        else
        {
          //this.mLogger.LogWarning(LoggingEvents.Undefine, "[Execute] Failer MemCache (CacheKey={CacheKey})", cacheKey);
        }
      }
    }
  }
}
