using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using pixstock.apl.app.core.Dao;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.Models;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;

namespace pixstock.client.app.Core.ServerMessageApi.Handler {
  /// <summary>
  /// CategroyTree更新指示メッセージを処理するハンドラのクラスです
  /// </summary>
  public class CategoryTreeLoadHandler : IResolveDeclare {
    public string ResolveName => "CategoryTreeLoad";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      readonly IMemoryCache mMemoryCache;

      readonly IIntentManager mIntentManager;

      private readonly Logger mLogger;

      public Handler (IMemoryCache memoryCache, IIntentManager intentManager) {
        mLogger = LogManager.GetCurrentClassLogger ();
        this.mMemoryCache = memoryCache;
        this.mIntentManager = intentManager;
      }

      public override void Handle (object param) {
        this.mLogger.Debug ("IN - {@param}", param);

        ServerMessageServiceParam serviceParam = (ServerMessageServiceParam) param;
        string cacheKey = "CategoryTree";

        var categoryId = long.Parse (serviceParam.Data.ToString ());
        cacheKey += categoryId;

        if (!mMemoryCache.TryGetValue (cacheKey, out Category[] s)) {
          var dao_cat = new CategoryDao();
          var category = dao_cat.LoadCategory(categoryId);
          s = category.LinkSubCategoryList.ToArray();

          var cacheEntryOptions = new MemoryCacheEntryOptions ()
            .SetSlidingExpiration (TimeSpan.FromSeconds (3));

          this.mLogger.Debug ("[OnCategoryTreeLoad] Push MemCache (CacheKey={CacheKey})", cacheKey);
          mMemoryCache.Set (cacheKey, s, cacheEntryOptions);
        }

        mIntentManager.AddIntent (ServiceType.Workflow, "ACT_RESINVALIDATE_CATEGORYTREE", categoryId);
      }
    }
  }
}
