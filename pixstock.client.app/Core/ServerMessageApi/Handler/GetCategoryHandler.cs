using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using pixstock.apl.app.core.Dao;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.core.IpcApi.Response;
using pixstock.apl.app.json.ServerMessage;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.ServerMessageApi.Handler
{
  public class GetCategoryHandler : IResolveDeclare
  {
    public string ResolveName => "GETCATEGORY";

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

        var handlerParam = JsonConvert.DeserializeObject<GetCategoryParam>(serviceParam.Data.ToString());

        var dao_cat = new CategoryDao();
        var category = dao_cat.LoadCategory(handlerParam.CategoryId, handlerParam.OffsetSubCategory, handlerParam.LimitOffsetSubCategory);

        mMemoryCache.Set("ResponseCategory", new CategoryDetailResponse
        {
          Category = category,
          SubCategory = category.LinkSubCategoryList.ToArray(),
          Content = category.LinkContentList.ToArray()
        });

        //this.mLogger.LogDebug(LoggingEvents.Undefine, "[Execute] Register RESPONSE_GETCATEGORY");
        mIntentManager.AddIntent(ServiceType.Workflow, "RESPONSE_GETCATEGORY", null);
      }
    }
  }
}
