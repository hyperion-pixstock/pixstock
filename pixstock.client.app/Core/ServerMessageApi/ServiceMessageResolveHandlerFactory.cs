using pixstock.client.app.Infra.Resolver.Impl;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.ServerMessageApi
{
  public class ServiceMessageResolveHandlerFactory : PackageResolveHandlerFactory
  {
    public ServiceMessageResolveHandlerFactory(Container container) :
      base(container, "pixstock.client.app.Core.ServerMessageApi.Handler")
    {

    }
  }
}
