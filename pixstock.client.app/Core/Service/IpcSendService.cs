using pixstock.apl.app.core.Infra;
using pixstock.client.app.Core.Intent;
using pixstock.client.app.Core.IpcApi;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.Service
{
  public class IpcSendService : IResolveDeclare
  {
    public string ResolveName => ServiceType.FrontendIpc.ToString();

    public Type ResolveType => typeof(Handler);

    public class Handler : PackageResolveHandler
    {
      private readonly Container mContainer;

      private IpcSendResolveHandlerFactory mFactory = null;

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
        handler.Handle(new IpcSendServiceParam { Data = intentParam.ExtraData });
      }

      private IpcSendResolveHandlerFactory GetFactory()
      {
        if (mFactory == null)
        {
          mFactory = mContainer.GetInstance<IpcSendResolveHandlerFactory>();
        }

        return mFactory;
      }
    }
  }
}
