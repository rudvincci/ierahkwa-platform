using System.Reflection;
using Mamey.Auth.Identity;
using Mamey.MicroMonolith.Abstractions.Auth;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Contracts;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;

namespace Mamey.Casino.BlazorServer;

public static class Extensions
{
    

    public static async Task<WebApplication> ConfigureApplication(this WebApplication app, IList<Assembly> assemblies, IList<IModule> modules)
    {
        var rolesPermissionMapping = Auth.Identity.Extensions.RolePermissionMapper.GenerateRolePermissionMapping<AppRole>();
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILogger<Program>>();
        // logger.LogInformation($"Modules: {string.Join(", ", modules.Select(x => x.Name))}");
        
        
        
        foreach (var module in modules)
        {
            module.Use(app);
        }

        // Console.WriteLine("Skipping restore option disabled");
        app.ValidateContracts(assemblies);
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            //endpoints.MapGet("/", context => context.Response.WriteAsync("Mamey API!"));
            endpoints.MapModuleInfo();
        });

        app.UseMamey();
        // app.UseRabbitMq();

        return app;
    }
    
}