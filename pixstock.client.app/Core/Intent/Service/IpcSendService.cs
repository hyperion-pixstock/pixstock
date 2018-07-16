using System;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.Models;
using static pixstock.apl.app.core.ScreenManager;

namespace pixstock.apl.app.core.Intent.Service
{
  public class IpcSendService : IMessagingServiceExtention
  {
    private ILogger mLogger;

    public ServiceType ServiceType => ServiceType.FrontendIpc;

    public Container Container { get; set; }

    public void Execute(string message, object parameter)
    {
      this.mLogger.LogDebug(LoggingEvents.Undefine, "[Execute] Parameter={Parameter}", message);

      if (message == "UpdateView")
      {
        // IPCメッセージを作成する
        var messageparameter = parameter as UpdateViewResponse;
        var ipcMessage = new IpcMessage();
        object obj = new
        {
          UpdateList = messageparameter.ViewEventList,
          Parameter = messageparameter.Parameter
        };
        ipcMessage.Body = JsonConvert.SerializeObject(obj, Formatting.Indented);
        Console.WriteLine("    Body(JSON)=" + ipcMessage.Body);

        var mainWindow = Electron.WindowManager.BrowserWindows.First();
        Electron.IpcMain.Send(mainWindow, "IPC_UPDATEVIEW", ipcMessage);
      }
      else if (message == "UpdateProp")
      {
        // UpdateProp
        //    キャッシュから任意のキーのオブジェクトを取得します。
        var propertyName = parameter.ToString();
        var memCache = Container.GetInstance<IMemoryCache>();

        object cachedObject;
        if (memCache.TryGetValue(propertyName, out cachedObject))
        {
          var ipcMessage = new IpcMessage();
          object obj = new
          {
            PropertyName = propertyName,
            Value = JsonConvert.SerializeObject(cachedObject)
          };

          ipcMessage.Body = JsonConvert.SerializeObject(obj, Formatting.Indented);
          var mainWindow = Electron.WindowManager.BrowserWindows.First();
          Electron.IpcMain.Send(mainWindow, "IPC_UPDATEPROP", ipcMessage);
        }
        else
        {
          this.mLogger.LogWarning(LoggingEvents.Undefine, "[Execute] Faile MemCache");
        }
      }
      else if (message == "CategoryTree")
      {
        string cacheKey = "CategoryTree";
        var categoryId = long.Parse(parameter.ToString());
        cacheKey += categoryId;

        var memCache = Container.GetInstance<IMemoryCache>();

        // MemCacheから、更新通知を行うカテゴリオブジェクトを取得
        if (memCache.TryGetValue(cacheKey, out Category[] cachedObject))
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
          this.mLogger.LogWarning(LoggingEvents.Undefine, "[Execute] Failer MemCache (CacheKey={CacheKey})", cacheKey);
        }

      }
      else if (message == "Alive")
      {
        var ipcMessage = new IpcMessage();
        var mainWindow = Electron.WindowManager.BrowserWindows.First();
        Electron.IpcMain.Send(mainWindow, "IPC_ALIVE", ipcMessage);
      }
    }

    public void InitializeExtention()
    {
      // EMPTY
    }

    public void Verify()
    {
      ILoggerFactory loggerFactory = this.Container.GetInstance<ILoggerFactory>();
      this.mLogger = loggerFactory.CreateLogger(this.GetType().FullName);
    }
  }
}
