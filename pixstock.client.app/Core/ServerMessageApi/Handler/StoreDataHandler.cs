using System;
using NLog;
using pixstock.apl.app.core.Dao;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.Models;
using pixstock.client.app.Core.Service;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using pixstock.client.app.Workflow;
using Pixstock.Common.Model;

namespace pixstock.client.app.Core.ServerMessageApi.Handler {
  /// <summary>
  /// データの永続化を実行する
  /// </summary>
  public class StoreDataHandler : IResolveDeclare {
    public string ResolveName => "StoreData";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      readonly Logger mLogger;

      readonly IIntentManager mIntentManager;

      public Handler (IIntentManager intentManager) {
        this.mLogger = LogManager.GetCurrentClassLogger ();
        this.mIntentManager = intentManager;
      }

      public override void Handle (object param) {
        this.mLogger.Debug ("IN - {@Param}", param);
        var paramObj = (ServerMessageServiceParam) param;
        var paramHandler = paramObj.Data as HandlerParameter;

        switch (paramHandler.ModelType) {
          case "Content":
            this.mLogger.Debug ("データモデルタイプをContentとして処理を実行します");
            var dao_content = new ContentDao ();
            var content = (Content) paramHandler.Value;
            dao_content.Update (content);
            if (paramHandler.UpdateNotificationFlag) {
              mIntentManager.AddIntent (ServiceType.Workflow, "ACT_RESINVALIDATE_CONTENT", new ResInvalidateContentParameter { ContentId = content.Id });
            }
            break;
          default:
            this.mLogger.Warn ($"不明なオペレーション({@paramHandler.ModelType})のため実行しませんでした。");
            break;
        }
        this.mLogger.Debug ("OUT");
      }
    }

    public class HandlerParameter {
      public object Value; //< 更新対象のオブジェクト

      public string ModelType; //< 更新対象オブジェクトのデータモデルタイプ

      public bool UpdateNotificationFlag = false;
    }
  }
}
