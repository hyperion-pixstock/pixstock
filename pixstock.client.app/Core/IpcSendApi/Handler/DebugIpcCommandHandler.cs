using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.IpcApi.Handler
{
  public class DebugIpcCommandHandler : IResolveDeclare
  {
    public string ResolveName => "DebugIpcCommand";

    public Type ResolveType => typeof(Handler);

    public class Handler : PackageResolveHandler
    {
      public override void Handle(object param)
      {
        Console.WriteLine("[DEBUG][DebugIpcCommandHandler] Handle - " + param);
      }
    }
  }
}
