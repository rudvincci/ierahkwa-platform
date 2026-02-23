using Mamey.FWID.Sagas.Api;
using Microsoft.AspNetCore;
using Mamey;
using Mamey.WebApi;
using Mamey.FWID.Sagas.Core;
using Mamey.Logging;
using Mamey.Secrets.Vault;

public class Program
{
    public static async Task Main(string[] args)
        => await CreateHostBuilder(args)
            .Build()
            .RunAsync();
    private static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args).
              ConfigureWebHostDefaults(webhostBuilder =>
              {
                  webhostBuilder.ConfigureServices(services =>
                    services.AddMamey()
                    .AddWebApi()
                    .AddFWIDSagaCore()

                        .Build()
                    )
                  .Configure(app =>

                    app
                       .AddFWIDSagaRoutes()
                    .UseFWIDSagaCore()
                  );
              })
              .UseLogging()
              .UseVault()
        ;
}



