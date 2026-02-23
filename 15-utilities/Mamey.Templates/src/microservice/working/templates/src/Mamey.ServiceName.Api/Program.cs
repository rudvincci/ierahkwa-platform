using Mamey;
using Mamey.WebApi;
using Microsoft.AspNetCore;
using Mamey.ServiceName.Infrastructure;
using Mamey.Logging;
using Mamey.Net;
using Mamey.Secrets.Vault;
using Mamey.ServiceName.Api;

namespace Mamey.ServiceName.Api;

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
                   .AddEntityNameRoutes()
              );
          })
          .UseLogging()
          .UseVault();
}

