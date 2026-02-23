using Mamey;
using Mamey.WebApi;
using Microsoft.AspNetCore;
using Pupitre.Lessons.Infrastructure;
using Mamey.Logging;
using Mamey.Net;
using Mamey.Secrets.Vault;
using Pupitre.Lessons.Api;

namespace Pupitre.Lessons.Api;

public class Program
{
    public static async Task Main(string[] args)
        => await CreateHostBuilder(args)
            .Build()
            .RunAsync();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args).
          ConfigureWebHostDefaults(webhostBuilder =>
          {
              webhostBuilder.ConfigureServices(services =>
                services
                    .AddMamey()
                    .AddWebApi()
                    .AddInfrastructure()
                    .Build()
                )
              .Configure(app =>
                app.UseInfrastructure()
                   .AddLessonRoutes()
              );
          })
          .UseLogging()
          .UseVault();
}

