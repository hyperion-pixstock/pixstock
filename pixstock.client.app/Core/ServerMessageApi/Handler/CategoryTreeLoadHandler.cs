using Microsoft.Extensions.Caching.Memory;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.Models;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.ServerMessageApi.Handler
{
  public class CategoryTreeLoadHandler : IResolveDeclare
  {
    public string ResolveName => "CategoryTreeLoad";

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

        string cacheKey = "CategoryTree";

        var categoryId = long.Parse(serviceParam.Data.ToString());
        cacheKey += categoryId;

        if (!mMemoryCache.TryGetValue(cacheKey, out Category[] s))
        {
          // DEBUG: ダミーデータを作成する
          //this.mLogger.LogDebug("[OnCategoryTreeLoad] ダミーデータを作成します");

          if (categoryId == 1L)
          {
            Category[] cs = {
            new Category
            {
              Id = 2,
              Name = "Child Category01",
              HasLinkSubCategoryFlag = true
            },
            new Category
            {
              Id = 3,
              Name = "Child Category02"
            },
            new Category
            {
              Id = 4,
              Name = "Child Category03"
            }
          };
            s = cs;
          }
          else if (categoryId == 2L)
          {
            Category[] cs = {
            new Category
            {
              Id = 20,
              Name = "ダミーデータカテゴリ1",
              HasLinkSubCategoryFlag = true
            },
            new Category
            {
              Id = 30,
              Name = "ダミーデータカテゴリ2"
            },
            new Category
            {
              Id = 40,
              Name = "ダミーデータカテゴリ3",
              HasLinkSubCategoryFlag = true
            }
          };
            s = cs;
          }

          var cacheEntryOptions = new MemoryCacheEntryOptions()
              .SetSlidingExpiration(TimeSpan.FromSeconds(3));

          //this.mLogger.LogDebug(LoggingEvents.Undefine, "[OnCategoryTreeLoad] Push MemCache (CacheKey={CacheKey})", cacheKey);
          mMemoryCache.Set(cacheKey, s, cacheEntryOptions);
        }

        mIntentManager.AddIntent(ServiceType.Workflow, "ACT_RESINVALIDATE_CATEGORYTREE", categoryId);
      }
    }
  }
}
