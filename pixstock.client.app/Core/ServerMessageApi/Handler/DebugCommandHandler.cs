using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.ServerMessageApi.Handler
{
  public class DebugCommandHandler : IResolveDeclare
  {
    public string ResolveName => "DebugCommand";

    public Type ResolveType => typeof(Handler);

    public class Handler : PackageResolveHandler {
      public override void Handle(object param)
      {
        ServerMessageServiceParam serviceParam = (ServerMessageServiceParam)param;
        Console.WriteLine("[DEBUG][DebugCommandHandler] Handle - " + serviceParam.Data);
      }
    }
  }
}
