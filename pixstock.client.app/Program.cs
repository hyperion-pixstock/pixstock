using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace pixstock.client.app
{
  public class Program
  {
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseElectron(args)
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
              logging.AddDebug();
            })
           .UseStartup<Startup>()
           .Build();
  }
}
