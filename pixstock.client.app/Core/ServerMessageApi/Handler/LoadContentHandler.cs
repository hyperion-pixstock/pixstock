using System;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using pixstock.apl.app.core.Dao;
using pixstock.apl.app.core.Infra;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using pixstock.client.app.Workflow;

namespace pixstock.client.app.Core.ServerMessageApi.Handler {
  /// <summary>
  /// コンテント情報取得要求メッセージを処理するハンドラ
  /// /// </summary>
  public class LoadContentHandler : IResolveDeclare {
    public string ResolveName => "LoadContent";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      readonly Logger mLogger;

      readonly IMemoryCache mMemoryCache;

      readonly IIntentManager mIntentManager;

      public Handler (IMemoryCache memoryCache, IIntentManager intentManager) {
        this.mLogger = LogManager.GetCurrentClassLogger ();
        this.mMemoryCache = memoryCache;
        this.mIntentManager = intentManager;
      }

      public override void Handle (object param) {
        this.mLogger.Debug ("IN - {@Param}", param);
        var paramObj = (ServerMessageServiceParam) param;
        var paramHandler = paramObj.Data as HandlerParameter;

        // Guard
        if (string.IsNullOrEmpty (paramHandler.RegisterName)) {
          this.mLogger.Warn ("保存先レジスタ名が未設定です。");
          return;
        }

        var dao_content = new ContentDao ();
        var content = dao_content.LoadContent (paramHandler.ContentId);

        // 取得したURLをキャッシュに格納
        var cacheEntryOptions = new MemoryCacheEntryOptions ()
          .SetSlidingExpiration (TimeSpan.FromSeconds (30));
        mMemoryCache.Set (paramHandler.RegisterName,
          content,
          cacheEntryOptions);

        var workflowParam = new ResInvalidateContentParameter () {
          ContentId = paramHandler.ContentId,
          RegisterName = paramHandler.RegisterName
        };
        mIntentManager.AddIntent (ServiceType.Workflow, "ACT_RESINVALIDATE_CONTENT", workflowParam);
        mLogger.Debug ("OUT");
      }
    }

    public class HandlerParameter {
      public long ContentId;
      public string RegisterName;
    }
  }
}
