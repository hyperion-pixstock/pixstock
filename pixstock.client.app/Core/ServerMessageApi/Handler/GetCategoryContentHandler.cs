using Microsoft.Extensions.Caching.Memory;
using pixstock.apl.app.core.Dao;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.core.IpcApi.Response;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.ServerMessageApi.Handler
{
  public class GetCategoryContentHandler : IResolveDeclare
  {
    public string ResolveName => "GETCATEGORYCONTENT";

    public Type ResolveType => typeof(Handler);

    public class Handler : PackageResolveHandler
    {
      readonly IMemoryCache mMemoryCache;

      readonly IIntentManager mIntentManager;

      public Handler(IMemoryCache memoryCache, IIntentManager intentManager)
      {
        this.mMemoryCache = memoryCache;
        this.mIntentManager = intentManager;
      }

      public override void Handle(object param)
      {
        ServerMessageServiceParam serviceParam = (ServerMessageServiceParam)param;

        var categoryId = long.Parse(serviceParam.Data.ToString());

        var dao_cat = new CategoryDao();
        var category = dao_cat.LoadCategory(categoryId, 0, CategoryDao.MAXLIMIT);

        mMemoryCache.Set("ResponseCategoryContent", new CategoryDetailResponse
        {
          Category = category,
          Content = category.LinkContentList.ToArray()
        });

        mIntentManager.AddIntent(ServiceType.Workflow, "RESPONSE_GETCATEGORYCONTENT", null);
      }
    }
  }
}
