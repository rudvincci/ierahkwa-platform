using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mamey.MicroMonolith.Abstractions.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Contracts;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace Mamey.ApplicationName.Bootstrapper;

public class Startup
{
    private readonly IList<Assembly> _assemblies;
    private readonly IList<IModule> _modules;
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
         _configuration = configuration;
         _assemblies = ModuleLoader.LoadAssemblies(_configuration, "Mamey.ApplicationName.Modules.");
         _modules = ModuleLoader.LoadModules(_assemblies);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMamey();
        services.AddModularInfrastructure(_assemblies, _modules);
        services.AddSingleton<RolePermissions>(new RolePermissions(RolePermissionMapper.GenerateRolePermissionMapping()));
        foreach (var module in _modules)
        {
            module.Register(services);
        }
    }

    public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        var rolesPermissionMapping = RolePermissionMapper.GenerateRolePermissionMapping();
        // Configure the HTTP request pipeline.
         if (env.IsDevelopment() || env.EnvironmentName == "development")
         {
             app.UseDeveloperExceptionPage();
         }
         else
         {
             // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
             app.UseHsts();
         }

        logger.LogInformation($"Modules: {string.Join(", ", _modules.Select(x => x.Name))}");
        app.UseModularInfrastructure().GetAwaiter().GetResult();
        foreach (var module in _modules)
        {
            module.Use(app);
        }

        // Console.WriteLine("Skipping restore option disabled");
        app.ValidateContracts(_assemblies);
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", context => context.Response.WriteAsync("Mamey API!"));
            endpoints.MapGet("/debug/routes", async context =>
            {
                var endpointDataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();
                await context.Response.WriteAsync(string.Join("\n", endpointDataSource.Endpoints.Select(e => e.DisplayName)));
            });
            endpoints.MapModuleInfo();
        });

        app.UseMamey();
        // app.UseRabbitMq();

        _assemblies.Clear();
        _modules.Clear();
    }
    
}

public static class RolePermissionMapper
{
    public static Dictionary<Type, Dictionary<string, long>> GenerateRolePermissionMapping()
    {
        var mappings = new Dictionary<Type, Dictionary<string, long>>();

        // Dynamically map roles to permissions for Bank and Member roles
        mappings[typeof(FHGPermission)] = MapRoleToPermission(typeof(FhgRole));

        return mappings;
    }

    private static Dictionary<string, long> MapRoleToPermission(Type roleType)
    {
        var rolePermissions = new Dictionary<string, long>();

        // Iterate over all public constant fields in the roleType (BankRole or MemberRole)
        foreach (var field in roleType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var roleName = field.Name; 
            var roleValue = field.GetValue(null); // Corresponding permission value

            if (roleValue is Enum enumValue)
            {
                rolePermissions[roleName] = Convert.ToInt64(enumValue);
            }
        }

        return rolePermissions;
    }
}