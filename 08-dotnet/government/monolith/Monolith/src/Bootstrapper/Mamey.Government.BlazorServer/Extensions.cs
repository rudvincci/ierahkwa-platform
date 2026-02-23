using System.Reflection;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Contracts;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;

namespace Mamey.Government.BlazorServer;

public static class Extensions
{
    public static WebApplication ConfigureApplication(this WebApplication app, IList<Assembly> assemblies, IList<IModule> modules)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILogger<Program>>();
        logger.LogInformation($"Modules: {string.Join(", ", modules.Select(x => x.Name))}");
        
        foreach (var module in modules)
        {
            module.Use(app);
        }

        // Console.WriteLine("Skipping restore option disabled");
        app.ValidateContracts(assemblies);
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapModuleInfo();
        });

        // app.UseRabbitMq();

        return app;
    }
}
