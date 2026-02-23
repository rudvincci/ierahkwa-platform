using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mamey.MicroMonolith.Abstractions.Modules;
using Mamey.Modules;

namespace Mamey.MicroMonolith.Infrastructure.Modules;

public static class Extensions
{
    public static IServiceCollection AddModuleInfo(this IServiceCollection services, IList<IModule> modules)
    {
        var moduleInfoProvider = new ModuleInfoProvider();
        var moduleInfo =
            modules?.Select(x => new ModuleInfo(x.Name, x.Policies ?? Enumerable.Empty<string>())) ??
            Enumerable.Empty<ModuleInfo>();
        moduleInfoProvider.Modules.AddRange(moduleInfo);
        services.AddSingleton(moduleInfoProvider);

        return services;
    }

    public static void MapModuleInfo(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("modules", context =>
        {
            var moduleInfoProvider = context.RequestServices.GetRequiredService<ModuleInfoProvider>();
            return context.Response.WriteAsJsonAsync(moduleInfoProvider.Modules);
        });
    }

    public static IHostBuilder ConfigureModules(this IHostBuilder builder)
        => builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            foreach (var settings in GetSettings("*"))
            {
                cfg.AddJsonFile(settings);
            }

            foreach (var settings in GetSettings($"*.{ctx.HostingEnvironment.EnvironmentName}"))
            {
                cfg.AddJsonFile(settings);
            }

            IEnumerable<string> GetSettings(string pattern)
                => Directory.EnumerateFiles(ctx.HostingEnvironment.ContentRootPath,
                    $"module.{pattern}.json", SearchOption.AllDirectories);
        });


    public static IServiceCollection AddModuleRequests(this IServiceCollection services, IList<Assembly> assemblies)
    {
        services.AddModuleRegistry(assemblies);
        services.AddSingleton<IModuleClient, ModuleClient>();
        services.AddSingleton<IModuleSubscriber, ModuleSubscriber>();
        services.AddSingleton<IModuleSerializer, JsonModuleSerializer>();
        services.AddSingleton<IModuleSerializer, MessagePackModuleSerializer>();

        return services;
    }

    public static IModuleSubscriber UseModuleRequests(this IApplicationBuilder app)
        => app.ApplicationServices.GetRequiredService<IModuleSubscriber>();

    // private static void AddModuleRegistry(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    // {
    //     var registry = new ModuleRegistry();
    //     var types = assemblies.SelectMany(x => x.GetTypes()).ToArray();
    //         
    //     var commandTypes = types
    //         .Where(t => t.IsClass && typeof(ICommand).IsAssignableFrom(t))
    //         .ToArray();
    //         
    //     var eventTypes = types
    //         .Where(x => x.IsClass && typeof(IEvent).IsAssignableFrom(x))
    //         .ToArray();
    //
    //     services.AddSingleton<IModuleRegistry>(sp =>
    //     {
    //         var commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
    //         var commandDispatcherType = commandDispatcher.GetType();
    //             
    //         var eventDispatcher = sp.GetRequiredService<IEventDispatcher>();
    //         var eventDispatcherType = eventDispatcher.GetType();
    //
    //         foreach (var type in commandTypes)
    //         {
    //             registry.AddBroadcastAction(type, (@event, cancellationToken) =>
    //                 (Task) commandDispatcherType.GetMethod(nameof(commandDispatcher.SendAsync))
    //                     ?.MakeGenericMethod(type)
    //                     .Invoke(commandDispatcher, new[] {@event, cancellationToken}));
    //         }
    //             
    //         foreach (var type in eventTypes)
    //         {
    //             registry.AddBroadcastAction(type, (@event, cancellationToken) =>
    //                 (Task) eventDispatcherType.GetMethod(nameof(eventDispatcher.PublishAsync))
    //                     ?.MakeGenericMethod(type)
    //                     .Invoke(eventDispatcher, new[] {@event, cancellationToken}));
    //         }
    //
    //         return registry;
    //     });
    // }
    private static void AddModuleRegistry(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var registry = new ModuleRegistry();
        var allTypes = new List<Type>();

        foreach (var asm in assemblies)
        {
            // 1) skip known problematic assemblies
            var name = asm.GetName().Name ?? "";
            if (name.StartsWith("Microsoft.JSInterop", StringComparison.OrdinalIgnoreCase))
                continue;

            Type[] exported;
            try
            {
                // only public types — avoids most internal interop bits
                exported = asm.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // swallow the failures and keep whatever did load
                exported = ex.Types
                    .Where(t => t != null)
                    .Cast<Type>()
                    .ToArray();
            }

            allTypes.AddRange(exported);
        }

        // 2) pick out your commands & events
        var commandTypes = allTypes
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ICommand).IsAssignableFrom(t))
            .ToArray();

        var eventTypes = allTypes
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IEvent).IsAssignableFrom(t))
            .ToArray();

        // 3) register the ModuleRegistry
        services.AddSingleton<IModuleRegistry>(sp =>
        {
            var commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
            var eventDispatcher = sp.GetRequiredService<IEventDispatcher>();
            var cdType = commandDispatcher.GetType();
            var edType = eventDispatcher.GetType();

            foreach (var cmd in commandTypes)
            {
                var mi = cdType.GetMethod(nameof(commandDispatcher.SendAsync))!
                    .MakeGenericMethod(cmd);
                registry.AddBroadcastAction(
                    cmd,
                    (@evt, ct) => (Task)mi.Invoke(commandDispatcher, new[] { @evt, ct })!
                );
            }

            foreach (var ev in eventTypes)
            {
                var mi = edType.GetMethod(nameof(eventDispatcher.PublishAsync))!
                    .MakeGenericMethod(ev);
                registry.AddBroadcastAction(
                    ev,
                    (@evt, ct) => (Task)mi.Invoke(eventDispatcher, new[] { @evt, ct })!
                );
            }

            return registry;
        });
    }
}