using System;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using pixstock.apl.app.core.Dao;
using pixstock.apl.app.core.Infra;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;

namespace pixstock.client.app.Core.ServerMessageApi.Handler {
  /// <summary>
  /// プレビュー取得要求メッセージを処理するハンドラ
  /// </summary>
  public class ContentPreviewHandler : IResolveDeclare {
    public string ResolveName => "ContentPreview";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      const string CACHE_KEY = "PreviewUrl";

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
        var dao_content = new ContentDao ();
        var content = dao_content.LoadContent (paramHandler.ContentId);

        // 取得したURLをキャッシュに格納
        var cacheEntryOptions = new MemoryCacheEntryOptions ()
          .SetSlidingExpiration (TimeSpan.FromSeconds (30));
        mMemoryCache.Set (CACHE_KEY,
          content.PreviewFileUrl,
          cacheEntryOptions);

        mIntentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", CACHE_KEY);
        mLogger.Debug ("OUT");
      }
    }

    public class HandlerParameter {
      public long ContentId;
    }
  }
}
