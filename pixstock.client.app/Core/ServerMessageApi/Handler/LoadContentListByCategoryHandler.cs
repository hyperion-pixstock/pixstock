using System;
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
  /// コンテント一覧更新要求（カテゴリ）メッセージの処理するハンドラ
  /// </summary>
  public class LoadContentListByCategoryHandler : IResolveDeclare {
    public string ResolveName => "LoadContentListByCategory";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      const string cacheKey = "ContentList";

      readonly Logger mLogger;

      readonly IMemoryCache mMemoryCache;

      readonly IIntentManager mIntentManager;

      public Handler (IMemoryCache memoryCache, IIntentManager intentManager) {
        this.mLogger = LogManager.GetCurrentClassLogger ();
        this.mMemoryCache = memoryCache;
        this.mIntentManager = intentManager;
      }

      public override void Handle (object param) {
        mLogger.Debug ("IN - {@Param}", param);
        ServerMessageServiceParam paramObj = (ServerMessageServiceParam) param;
        var paramHandler = paramObj.Data as HandlerParameter;
        var dao_cat = new CategoryDao ();
        var category = dao_cat.LoadCategory (categoryId: paramHandler.CategoryId, offsetContent: paramHandler.PageNo);

        var cacheEntryOptions = new MemoryCacheEntryOptions ()
          .SetSlidingExpiration (TimeSpan.FromSeconds (3));
        mMemoryCache.Set (cacheKey, category.LinkContentList, cacheEntryOptions);

        mIntentManager.AddIntent (ServiceType.Workflow, "ACT_RESINVALIDATE_CONTENTLIST");
      }
    }

    public class HandlerParameter {
      public long CategoryId;
      public int PageNo;
    }
  }
}
