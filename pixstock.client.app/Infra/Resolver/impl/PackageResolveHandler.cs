using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Infra.Resolver.Impl
{
  public abstract class PackageResolveHandler : IResolveHandler
  {
    public abstract void Handle(object param);
  }

}
