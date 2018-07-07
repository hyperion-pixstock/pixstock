using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pixstock.Nc.Common;
using Pixstock.Nc.Srv.Ext;
using Pixstock.Service.Core;
using Pixstock.Service.Infra;
using Pixstock.Service.Infra.Repository;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace Pixstock.Service.Web
{
  /// <summary>
  /// Startup class
  /// </summary>
  public class Startup
  {
    private readonly Container container = new Container();

    private ILogger logger;

    private IConfiguration Configuration { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    /// <summary>
    /// Configures app the services.
    /// </summary>
    /// <param name="services">The services.</param>
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
      //services.AddSpaStaticFiles(c =>
      //{
      //  c.RootPath = "wwwroot";
      //});

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info
        {
          Version = "v1",
          Title = "pixstock.service.web API",
          Description = "A simple example ASP.NET Core Web API",
          Contact = new Contact { Name = "Juan García Carmona", Email = "d.jgc.it@gmail.com", Url = "https://wisegeckos.com" },
        });
        // Set the comments path for the Swagger JSON and UI.
        var basePath = AppContext.BaseDirectory;
        var xmlPath = Path.Combine(basePath, "pixstock.service.web.xml");
        c.IncludeXmlComments(xmlPath);
      });

      IntegrateSimpleInjector(services);
    }

    /// <summary>
    /// Configures the application.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="env">The hosting environment</param>
    /// <param name="loggerFactory">ログ</param>
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      logger = loggerFactory.CreateLogger<Program>();
      container.RegisterInstance<ILoggerFactory>(loggerFactory);

      InitializeContainer(app);
      SetupApplication(loggerFactory);

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseDefaultFiles();
      app.UseStaticFiles();

      // Enable middleware to serve generated Swagger as a JSON endpoint.
      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
      });
      app.UseStaticFiles();
      //app.UseSpaStaticFiles(); // services.AddSpaStaticFilesを使って、ASPに静的なファイルを組み込む場合は併用する

      using (AsyncScopedLifestyle.BeginScope(container))
      {
        // カットポイント「INIT」を呼び出す
        var extentionManager = container.GetInstance<ExtentionManager>();
        extentionManager.Execute(ExtentionCutpointType.INIT);
      }

      using (AsyncScopedLifestyle.BeginScope(container))
      {
        // カットポイント「START」を呼び出す
        var extentionManager = container.GetInstance<ExtentionManager>();
        extentionManager.Execute(ExtentionCutpointType.START, new CutpointStartParameter { WorkspaceId = 1L });
      }

      // 監視開始
      using (AsyncScopedLifestyle.BeginScope(container))
      {
        var vspFileUpdateWatchManager = container.GetInstance<VspFileUpdateWatchManager>();

        var workspaceRepository = container.GetInstance<IWorkspaceRepository>();
        var workspace = workspaceRepository.Load(1L);
        vspFileUpdateWatchManager.StartWatch(workspace);
      }

      app.UseMvc(routes =>
      {
        routes.MapRoute(name: "default", template: "{controller}/{action=index}/{id}");
      });

      app.UseSpa(spa =>
      {
        // To learn more about options for serving an Angular SPA from ASP.NET Core,
        // see https://go.microsoft.com/fwlink/?linkid=864501

        //spa.Options.SourcePath = "wwwroot";
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          //spa.UseAngularCliServer(npmScript: "start");
          spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"); // 外部起動しているHTTPサーバにアクセスする
        }
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

    private void SetupApplication(ILoggerFactory loggerFactory)
    {
      var appConfig = new AppSettings();
      Configuration.Bind("AppSettings", appConfig);

      var assemblyParameter = new BuildAssemblyParameter(appConfig);
      container.RegisterInstance<IBuildAssemblyParameter>(assemblyParameter);

      var context = new ApplicationContextImpl(assemblyParameter, loggerFactory);
      context.SetDiContainer(container);

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

          logger.LogInformation($"デフォルトワークスペースを作成しました。 Name:{workspace.Name}");
        }
      }
    }
  }
}
