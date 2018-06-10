using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Pixstock.Nc.Common;
using Pixstock.Nc.Srv.Ext;
using Pixstock.Service.App;
using Pixstock.Service.App.Builder;
using Pixstock.Service.Core;
using Pixstock.Service.Core.Vfs;
using Pixstock.Service.Gateway;
using Pixstock.Service.Gateway.Repository;
using Pixstock.Service.Infra;
using Pixstock.Service.Infra.Core;
using Pixstock.Service.Infra.Repository;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace pixstock.service.web
{
    public class Startup
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private Container container = new Container();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<AppSettings>("AppSettings", Configuration);
            IntegrateSimpleInjector(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeContainer(app);

            var appConfig = new AppSettings();
            Configuration.Bind("AppSettings", appConfig);

            container.Register<ErrorExceptionResolver>();
            var assemblyParameter = new BuildAssemblyParameter(appConfig);
            var context = new ApplicationContextImpl(assemblyParameter); // アプリケーションコンテキスト
            context.SetDiContainer(container);
            container.RegisterInstance<IApplicationContext>(context);
            container.RegisterInstance<IBuildAssemblyParameter>(assemblyParameter);
            container.Register<ICategoryRepository, CategoryRepository>();
            container.Register<IContentRepository, ContentRepository>();
            container.Register<IEventLogRepository, EventLogRepository>();
            container.Register<IFileMappingInfoRepository, FileMappingInfoRepository>();
            container.Register<ILabelRepository, LabelRepository>();
            container.Register<IWorkspaceRepository, WorkspaceRepository>();
            container.Register<IAppAppMetaInfoRepository, AppAppMetaInfoRepository>();
            container.Register<IThumbnailAppMetaInfoRepository, ThumbnailAppMetaInfoRepository>();
            container.Register<IThumbnailRepository, ThumbnailRepository>();
            container.Register<ApiResponseBuilder>();
            container.Register<IAppDbContext, AppDbContext>(Lifestyle.Scoped);
            container.Register<IThumbnailDbContext, ThumbnailDbContext>(Lifestyle.Scoped);
            container.Register<IThumbnailBuilder, ThumbnailBuilder>();
            container.Register<IFileUpdateRunner, FileUpdateRunner>();

            var messagingManager = new MessagingManager(container);
            container.RegisterInstance<MessagingManager>(messagingManager);
            container.RegisterInstance<IMessagingManager>(messagingManager);

            var extentionManager = new ExtentionManager(container);
            //extentionManager.AddPlugin(typeof(FullBuildExtention)); // 開発中は常に拡張機能を読み込む
            //extentionManager.AddPlugin(typeof(WebScribeExtention)); // 開発中は常に拡張機能を読み込む
            container.RegisterInstance<ExtentionManager>(extentionManager);
            extentionManager.InitializePlugin(context.ExtentionDirectoryPath);
            extentionManager.CompletePlugin();

            var vspFileUpdateWatchManager = new VspFileUpdateWatchManager(container);
            container.RegisterInstance<VspFileUpdateWatchManager>(vspFileUpdateWatchManager);

            container.Verify();

            using (AsyncScopedLifestyle.BeginScope(container))
            {
                var appCtx = (ApplicationContextImpl)container.GetInstance<IApplicationContext>();
                appCtx.Initialize();

                // デフォルトワークスペースが登録済みかチェックする
                var workspaceRepository = container.GetInstance<IWorkspaceRepository>();
                var workspace = workspaceRepository.Load(1L);
                if (workspace == null)
                {
                    if (appConfig.Workspace == null)
                        throw new ApplicationException("デフォルトワークスペースの設定が必要です。");

                    // デフォルトワークスペースを新規登録する
                    workspace = workspaceRepository.New();
                    workspace.Name = appConfig.Workspace.Name;
                    if (appConfig.Workspace.RelativeApplicationDirectoryBasePath)
                    {
                        workspace.PhysicalPath = Path.Combine(appCtx.ApplicationDirectoryPath, appConfig.Workspace.PhysicalPath);
                        workspace.VirtualPath = Path.Combine(appCtx.ApplicationDirectoryPath, appConfig.Workspace.VirtualPath);
                    }
                    else
                    {
                        workspace.PhysicalPath = appConfig.Workspace.PhysicalPath;
                        workspace.VirtualPath = appConfig.Workspace.VirtualPath;
                    }
                    workspaceRepository.Save();

                    _logger.Info($"デフォルトワークスペースを作成しました。 Name:{workspace.Name}");
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            using (AsyncScopedLifestyle.BeginScope(container))
            {
                // カットポイント「INIT」を呼び出す
                extentionManager.Execute(ExtentionCutpointType.INIT);
            }

            using (AsyncScopedLifestyle.BeginScope(container))
            {
                // カットポイント「START」を呼び出す
                extentionManager.Execute(ExtentionCutpointType.START, new CutpointStartParameter { WorkspaceId = 1L });
            }

            // 監視開始
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                var workspaceRepository = container.GetInstance<IWorkspaceRepository>();
                var workspace = workspaceRepository.Load(1L);
                vspFileUpdateWatchManager.StartWatch(workspace);
            }

            app.Use((c, next) => container.GetInstance<ErrorExceptionResolver>().Invoke(c, next));
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(container));

            services.EnableSimpleInjectorCrossWiring(container);
            services.UseSimpleInjectorAspNetRequestScoping(container);
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            // Add application presentation components:
            container.RegisterMvcControllers(app);
            container.RegisterMvcViewComponents(app);

            // Add application services. For instance:
            //container.Register<IUserService, UserService>(Lifestyle.Scoped);

            // Cross-wire ASP.NET services (if any). For instance:
            //container.CrossWire<ILoggerFactory>(app);

            // NOTE: Do prevent cross-wired instances as much as possible.
            // See: https://simpleinjector.org/blog/2016/07/
        }
    }
}
