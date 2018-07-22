using pixstock.apl.app.core.Infra;
using pixstock.client.app.Core.Intent;
using pixstock.client.app.Core.ServerMessageApi;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.Service
{
  public class ServerMessageService : IResolveDeclare
  {
    public string ResolveName => ServiceType.Server.ToString();

    public Type ResolveType => typeof(Handler);

    public class Handler : PackageResolveHandler
    {
      private readonly Container mContainer;

      private ServiceMessageResolveHandlerFactory mFactory = null;

      /// <summary>
      /// コンストラクタ
      /// </summary>
      /// <param name="container"></param>
      public Handler(Container container)
      {
        this.mContainer = container;
      }

      public override void Handle(object param)
      {
        var intentParam = (IntentParam)param;

        var handler = GetFactory().CreateNew(intentParam.IntentName); // paramからIntentメッセージ名を取得し、ファクトリー経由でハンドラを取得する
        handler.Handle(new ServerMessageServiceParam { Data = intentParam.ExtraData });
      }

      private ServiceMessageResolveHandlerFactory GetFactory()
      {
        if (mFactory == null)
        {
          mFactory = mContainer.GetInstance<ServiceMessageResolveHandlerFactory>();
        }

        return mFactory;
      }
    }
  }
}
