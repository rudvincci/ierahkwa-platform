using Mamey;
using Mamey.WebApi;
using Microsoft.AspNetCore;
using Pupitre.AISpeech.Infrastructure;
using Mamey.Logging;
using Mamey.Net;
using Mamey.Secrets.Vault;
using Pupitre.AISpeech.Api;

namespace Pupitre.AISpeech.Api;

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
                   .AddSpeechRequestRoutes()
              );
          })
          .UseLogging()
          .UseVault();
}

