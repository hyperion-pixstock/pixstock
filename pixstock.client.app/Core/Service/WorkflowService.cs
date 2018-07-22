using Hyperion.Pf.Workflow;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.Workflow;
using pixstock.client.app.Core.Intent;
using pixstock.client.app.Infra.Resolver;
using pixstock.client.app.Infra.Resolver.Impl;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Core.Service
{
  public class WorkflowService : IResolveDeclare
  {
    public const string COMMONMESSAGE_BACKTRANSITION = "ACT_BACKSCREEN";

    public string ResolveName => ServiceType.Workflow.ToString();

    public Type ResolveType => typeof(Handler);

    public class Handler : PackageResolveHandler
    {
      private readonly Container mContainer;

      private readonly HarmonicManager mHarmonic;

      private readonly Perspective mPixstockPerspective;

      /// <summary>
      /// コンストラクタ
      /// </summary>
      /// <param name="container"></param>
      public Handler(Container container)
      {
        this.mContainer = container;

        this.mHarmonic = new HarmonicManager();

        var dictPerspective = new Dictionary<string, string>();
        dictPerspective.Add("MainFrame", "PixstockMainContent");
        this.mPixstockPerspective = new Perspective("PIXSTOCK", ArbitrationMode.AWAB, dictPerspective, mHarmonic);
      }

      public override void Handle(object param)
      {
        var intentParam = (IntentParam)param;
        var screenManager = mContainer.GetInstance<IScreenManager>();

        if (mPixstockPerspective.Status == PerspectiveStatus.Active)
        {
          if (intentParam.IntentName == COMMONMESSAGE_BACKTRANSITION)
          {
            screenManager.BackScreen();
          }
          else
          {
            foreach (var content in mPixstockPerspective.Contents)
            {
              if (content is IPixstockContent)
              {
                ((IPixstockContent)content).FireWorkflowEvent(mContainer, intentParam.IntentName, intentParam.ExtraData);
                screenManager.UpdateScreenTransitionView(intentParam.ExtraData);
              }
            }
          }
        }
      }

      public void Initialize()
      {
        List<IContentBuilder> contentBuilders = new List<IContentBuilder>();
        contentBuilders.Add(new MyContentBuilder() { Container = this.mContainer });

        // Harmonicマネージャの初期化
        //    ・Contentの登録も行う
        //    ・Perspectiveの登録も行う
        mHarmonic.Verify(contentBuilders);
        mHarmonic.RegisterPerspective(mPixstockPerspective);
        mHarmonic.StartPerspective("PIXSTOCK"); // 開発中のみ、ここでPerspectiveを開始する
      }
    }
  }

  class MyContentBuilder : IContentBuilder
  {
    public Container Container { get; set; }

    public Content Build()
    {
      return new PixstockMainContent(Container);
    }
  }
}
