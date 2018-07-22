using ElectronNET.API;
using Newtonsoft.Json;
using pixstock.apl.app.core.Infra;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static pixstock.apl.app.core.ScreenManager;

namespace pixstock.client.app.Core.IpcApi.Handler
{
  public class UpdateViewHandler : IResolveDeclare
  {
    public string ResolveName => "UpdateView";

    public Type ResolveType => typeof(Handler);

    public class Handler : PackageResolveHandler
    {
      public override void Handle(object param)
      {
        IpcSendServiceParam serviceParam = (IpcSendServiceParam)param;

        var messageparameter = serviceParam.Data as UpdateViewResponse;

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
    }
  }
}
