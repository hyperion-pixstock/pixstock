using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Infra.Resolver
{
  public interface IResolveHandlerFactory
  {
    IResolveHandler CreateNew(string name);
  }
}
