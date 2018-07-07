using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Pixstock.Service.Web
{
  /// <summary>
  /// Main App entry point
  /// </summary>
  public class Program
  {
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }

    /// <summary>
    /// Builds the web host.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>The web host, ready to be run.</returns>
    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
          .ConfigureAppConfiguration((hostingContext, config) =>
            {
              var env = hostingContext.HostingEnvironment;
              config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
              config.AddEnvironmentVariables();
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
              logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
              logging.AddConsole();
              logging.AddDebug();
            })
            .UseStartup<Startup>()
            .Build();
  }
}
