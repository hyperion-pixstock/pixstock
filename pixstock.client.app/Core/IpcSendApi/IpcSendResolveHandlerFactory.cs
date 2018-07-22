using pixstock.client.app.Infra.Resolver.Impl;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.IpcApi
{
  public class IpcSendResolveHandlerFactory : PackageResolveHandlerFactory
  {
    public IpcSendResolveHandlerFactory(Container container) :
      base(container, "pixstock.client.app.Core.IpcApi.Handler")
    {

    }
  }
}
