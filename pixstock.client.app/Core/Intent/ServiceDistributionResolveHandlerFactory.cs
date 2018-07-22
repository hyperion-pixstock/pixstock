using pixstock.client.app.Infra.Resolver.Impl;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.Intent
{
  public class ServiceDistributionResolveHandlerFactory : PackageResolveHandlerFactory
  {
    public ServiceDistributionResolveHandlerFactory(Container container) :
      base(container, "pixstock.client.app.Core.Service", Lifestyle.Singleton)
    {

    }
  }
}
