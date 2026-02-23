using Mamey;
using Mamey.WebApi;
using Microsoft.AspNetCore;
using Mamey.FWID.Notifications.Infrastructure;
using Mamey.Logging;
using Mamey.Net;
using Mamey.Secrets.Vault;
using Mamey.FWID.Notifications.Api;

namespace Mamey.FWID.Notifications.Api;

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
                   .AddNotificationRoutes()
                   .UseEndpoints(endpoints =>
                   {
                       endpoints.MapGrpcService<Mamey.FWID.Notifications.Api.Infrastructure.Grpc.NotificationGrpcService>();
                       endpoints.MapHub<Mamey.FWID.Notifications.Application.Hubs.NotificationHub>("/notifications/hub");
                   })
              );
          })
          .UseLogging()
          .UseVault();
}

