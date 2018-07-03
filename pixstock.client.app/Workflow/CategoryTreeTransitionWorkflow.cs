using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using pixstock.apl.app.core;
using pixstock.apl.app.core.Cache;
using pixstock.apl.app.core.Dao;
using pixstock.apl.app.core.Infra;
using pixstock.apl.app.core.IpcApi.Response;
using pixstock.apl.app.json.ServerMessage;
using pixstock.apl.app.Models;
using SimpleInjector;

namespace Pixstock.Applus.Foundations.ContentBrowser.Transitions
{
    public partial class CategoryTreeTransitionWorkflow
    {
        private ILogger mLogger;

        readonly Container mContainer;

        public CategoryTreeTransitionWorkflow(Container container)
        {
            this.mContainer = container;

            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
            this.mLogger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        async Task OnHomePageBase_Entry()
        {

        }

        async Task OnHomePageBase_Exit()
        {

        }

        async Task OnThumbnailListPage_Entry()
        {

        }

        async Task OnThumbnailListPage_Exit()
        {

        }

        async Task OnPreviewPage_Entry()
        {

        }

        async Task OnREQUEST_GetCategory(object param) { }

        async Task OnACT_ContinueCategoryList(object param) { }

        async Task OnRESPONSE_GETCATEGORY(object param)
        {
            this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnRESPONSE_GETCATEGORY]");
            await Task.Delay(1);

            var intentManager = mContainer.GetInstance<IIntentManager>();
            var memCache = mContainer.GetInstance<IMemoryCache>();
            CategoryDetailResponse response;
            if (memCache.TryGetValue("ResponseCategory", out response))
            {
                memCache.Set("CategoryList", new CategoryListParam
                {
                    Category = response.Category,
                    CategoryList = response.SubCategory
                });

                memCache.Set("ContentList", new ContentListParam
                {
                    Category = response.Category,
                    ContentList = response.Content
                });

                intentManager.AddIntent(ServiceType.FrontendIpc, "UpdateProp", "CategoryList");
                intentManager.AddIntent(ServiceType.FrontendIpc, "UpdateProp", "ContentList");
            }
            else
            {
                Console.WriteLine("    MemCache TryGet Failer");
            }
        }

        async Task OnRESPONSE_GETCATEGORYCONTENT(object param)
        {
            this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnRESPONSE_GETCATEGORYCONTENT]");
            var intentManager = mContainer.GetInstance<IIntentManager>();
            var memCache = mContainer.GetInstance<IMemoryCache>();
            CategoryDetailResponse response;
            if (memCache.TryGetValue("ResponseCategoryContent", out response))
            {
                memCache.Set("ContentList", new ContentListParam
                {
                    Category = response.Category,
                    ContentList = response.Content
                });

                intentManager.AddIntent(ServiceType.FrontendIpc, "UpdateProp", "ContentList");
            }
            else
            {
                Console.WriteLine("    MemCache TryGet Failer");
            }
        }

        async Task OnCategorySelectBtnClick(object param)
        {
            this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnCategorySelectBtnClick]");

            var intentManager = mContainer.GetInstance<IIntentManager>();
            var memCache = mContainer.GetInstance<IMemoryCache>();
            await Task.Run(() =>
            {
                CategoryDetailResponse response;
                if (memCache.TryGetValue("ResponseCategoryContent", out response))
                {
                    memCache.Set("ContentList", new ContentListParam
                    {
                        Category = response.Category,
                        ContentList = response.Content
                    });

                    intentManager.AddIntent(ServiceType.FrontendIpc, "UpdateProp", "ContentList");
                }
                else
                {
                    Console.WriteLine("    MemCache TryGet Failer");
                }
            });
        }

        async Task OnACT_DEBUGCOMMAND(object param)
        {
            try
            {
                await Task.Run(()=>
                {
                    this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnACT_DEBUGCOMMAND] param={0}", param);
                    var screenManager = mContainer.GetInstance<IScreenManager>();

                    screenManager.DumpBackStack();
                });
            }
            catch (Exception expr)
            {
                Console.WriteLine(expr.Message);
            }
        }

        async Task OnACT_DISPLAY_PREVIEWCURRENTLIST(object param)
        {
            try
            {
                this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnACT_DISPLAY_PREVIEWCURRENTLIST] param={0}", param);
                var intentManager = mContainer.GetInstance<IIntentManager>();
                var memCache = mContainer.GetInstance<IMemoryCache>();

                ContentListParam objContentList;
                if (memCache.TryGetValue("ContentList", out objContentList))
                {
                    // コンテント一覧の項目位置(param)にあるコンテント情報を読み込む
                    var paramObj = JsonConvert.DeserializeObject<ActDisplayPreviewcurrentlistParam>(param.ToString());
                    var contentPosition = paramObj.ContentListPos;
                    var content = objContentList.ContentList[contentPosition];

                    if (paramObj.UpdateCategoryDisplayInfo)
                    {
                        intentManager.AddIntent(ServiceType.Server, "UpdateReading", content.Id);
                    }

                    if (paramObj.UpdateLastDisplayContent)
                    {
                        this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnACT_DISPLAY_PREVIEWCURRENTLIST] UpdateLastDisplayContent=true, Category={0}", objContentList.Category);
                        if (objContentList.Category != null)
                        {

                            // 更新したいプロパティを設定
                            var updateCategoryProp = new
                            {
                                NextDisplayContentId = content.Id
                            };

                            var updateCategoryPropParam = new UpdateCategoryPropParam
                            {
                                CategoryId = objContentList.Category.Id,
                                Category = updateCategoryProp
                            };

                            intentManager.AddIntent(ServiceType.Server, "UpdateCategoryProp", JsonConvert.SerializeObject(updateCategoryPropParam));
                        }
                    }

                    intentManager.AddIntent(ServiceType.Server, "GETCONTENT", content.Id);
                }
                else
                {
                    throw new ApplicationException("ContentListプロパティを取得できませんでした");
                }
            }
            catch (Exception expr)
            {
                Console.WriteLine(expr.Message);
            }
        }

        async Task OnACT_UpperCategoryList(object param)
        {
            this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnACT_UpperCategoryList]");

            var intentManager = mContainer.GetInstance<IIntentManager>();
            var memCache = mContainer.GetInstance<IMemoryCache>();
            var categoryDao = new CategoryDao();
            try
            {
                CategoryListParam objCategoryList;
                if (memCache.TryGetValue("CategoryList", out objCategoryList))
                {
                    var parentCategory = categoryDao.LoadParentCategory(objCategoryList.Category.Id);
                    if (parentCategory == null)
                    {
                        throw new ApplicationException("Not Get");
                    }

                    var paramJson = JsonConvert.SerializeObject(new GetCategoryParam
                    {
                        CategoryId = parentCategory.Id,
                        OffsetSubCategory = 0,
                        LimitOffsetSubCategory = 10
                    });

                    intentManager.AddIntent(ServiceType.Server, "GETCATEGORY", paramJson);
                }
            }
            catch (Exception expr)
            {
                this.mLogger.LogError(expr, "メッセージの処理に失敗しました");
            }
        }

        async Task OnRESPONSE_GETCONTENT(object param)
        {
            this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnRESPONSE_GETCONTENT]");

            var intentManager = mContainer.GetInstance<IIntentManager>();
            var memCache = mContainer.GetInstance<IMemoryCache>();
            ContentDetailResponse response;
            if (memCache.TryGetValue("ResponsePreviewContent", out response))
            {
                memCache.Set("PreviewContent", new PreviewContentParam()
                {
                    Content = response.Content,
                    Category = response.Category
                });

                intentManager.AddIntent(ServiceType.FrontendIpc, "UpdateProp", "PreviewContent");
            }
            else
            {
                Console.WriteLine("    MemCache TryGet Failer");
            }
        }

        async Task OnACT_CATEGORYTREE_UPDATE(object param)
        {
            try
            {
                this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnACT_CATEGORYTREE_UPDATE] ");

                var intentManager = mContainer.GetInstance<IIntentManager>();
                var memCache = mContainer.GetInstance<IMemoryCache>();

                this.mLogger.LogDebug(LoggingEvents.Undefine, "1" + param.ToString());

                long tgtCategoryId = long.Parse(param.ToString());

                // TODO: サーバに問い合わせるためのIntentメッセージを発行する
                this.mLogger.LogDebug(LoggingEvents.Undefine, "2");

                // TODOの間は、ハードコードされたダミーデータを取得したことにする
                List<Category> subCategoryList = new List<Category>();
                subCategoryList.Add(new Category { Id = 2L, Name = "サブカテゴリA" });
                subCategoryList.Add(new Category { Id = 3L, Name = "サブカテゴリB" });
                subCategoryList.Add(new Category { Id = 4L, Name = "サブカテゴリC" });
                this.mLogger.LogDebug(LoggingEvents.Undefine, "3");
                memCache.Set("CategoryList", new CategoryListParam
                {
                    Category = new Category
                    {
                        Id = 1L,
                        Name = "サンプルカテゴリ"
                    },
                    CategoryList = subCategoryList.ToArray()
                });

                this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnACT_CATEGORYTREE_UPDATE] Execute 'CategoryList' IntentMessage");
                intentManager.AddIntent(ServiceType.FrontendIpc, "UpdateProp", "CategoryList");
            }catch(Exception expr)
            {
                this.mLogger.LogDebug(LoggingEvents.Undefine, expr, "Error");
            }
        }

        async Task OnACT_THUMBNAILLIST_UPDATE(object param)
        {
            this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnACT_THUMBNAILLIST_UPDATE]");

            // TODO: サーバに問い合わせるためのIntentメッセージを発行する
        }

        async Task OnACT_PREVIEW_UPDATE(object param)
        {
            this.mLogger.LogDebug(LoggingEvents.Undefine, "[CategoryTreeTransitionWorkflow][OnACT_PREVIEW_UPDATE]");

            // TODO: サーバに問い合わせるためのIntentメッセージを発行する
        }
    }
}
