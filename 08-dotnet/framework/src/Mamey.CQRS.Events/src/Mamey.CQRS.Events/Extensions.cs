using System.Reflection;
using Mamey.CQRS.Events.Dispatchers;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mamey.CQRS.Events;

public static class Extensions
{
    public static IMameyBuilder AddEventHandlers(this IMameyBuilder builder, IEnumerable<Assembly>? assemblies = null)
    {
        assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

        var services = builder.Services;
        var serviceTypes = services.Select(s => s.ServiceType).ToHashSet();
        var handlerDiagnostics = new List<string>();

        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericTypeDefinition &&
                !t.IsDefined(typeof(DecoratorAttribute), inherit: false) &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .ToList();

            var ctor = handlerType.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (ctor == null)
            {
                handlerDiagnostics.Add($"[⚠️ Skipped] {handlerType.FullName} has no public constructor.");
                continue;
            }

            var missingDeps = new List<string>();
            var foundDeps = new List<string>();

            foreach (var param in ctor.GetParameters())
            {
                var paramType = param.ParameterType;
                var match = services.FirstOrDefault(s => s.ServiceType == paramType);

                // Special handling for ILogger<T> - it's resolved through ILoggerFactory at runtime
                if (match == null && paramType.IsGenericType && 
                    paramType.GetGenericTypeDefinition() == typeof(ILogger<>))
                {
                    // Check if ILoggerFactory is registered (which enables ILogger<T> resolution)
                    var loggerFactoryMatch = services.FirstOrDefault(s => s.ServiceType == typeof(ILoggerFactory));
                    if (loggerFactoryMatch != null)
                    {
                        foundDeps.Add(
                            $"{paramType.FullName} => resolved via ILoggerFactory ({loggerFactoryMatch.Lifetime})");
                        continue; // Skip adding to missing deps
                    }
                }

                if (match == null)
                    missingDeps.Add(paramType.FullName ?? paramType.Name);
                else
                    foundDeps.Add(
                        $"{paramType.FullName} => {match.ImplementationType?.FullName ?? "factory/instance/unknown"} ({match.Lifetime})");
            }

            if (missingDeps.Any())
            {
                handlerDiagnostics.Add($"""
                                        🔍 Handler: {handlerType.FullName}
                                        Constructor: ({string.Join(", ", ctor.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name))})
                                        ❌ Missing Dependencies:
                                        {string.Join("\n", missingDeps.Select(dep => $"   🔴 {dep}"))}
                                        [⚠️ Skipped] Not registered.
                                        """);
                continue;
            }

            foreach (var iface in interfaces)
            {
                services.AddTransient(iface, handlerType);
                handlerDiagnostics.Add($"[✅ Registered] {iface.FullName} -> {handlerType.FullName}");
            }
        }

        foreach (var diag in handlerDiagnostics)
            Console.WriteLine(diag);

        return builder;
    }
    public static IMameyBuilder AddScopedEventHandlers(this IMameyBuilder builder, IEnumerable<Assembly>? assemblies = null)
    {
        assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

        var services = builder.Services;
        var serviceTypes = services.Select(s => s.ServiceType).ToHashSet();
        var handlerDiagnostics = new List<string>();

        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericTypeDefinition &&
                !t.IsDefined(typeof(DecoratorAttribute), inherit: false) &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .ToList();

            var ctor = handlerType.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (ctor == null)
            {
                handlerDiagnostics.Add($"[⚠️ Skipped] {handlerType.FullName} has no public constructor.");
                continue;
            }

            var missingDeps = new List<string>();
            var foundDeps = new List<string>();

            foreach (var param in ctor.GetParameters())
            {
                var paramType = param.ParameterType;
                var match = services.FirstOrDefault(s => s.ServiceType == paramType);

                // Special handling for ILogger<T> - it's resolved through ILoggerFactory at runtime
                if (match == null && paramType.IsGenericType && 
                    paramType.GetGenericTypeDefinition() == typeof(ILogger<>))
                {
                    // Check if ILoggerFactory is registered (which enables ILogger<T> resolution)
                    var loggerFactoryMatch = services.FirstOrDefault(s => s.ServiceType == typeof(ILoggerFactory));
                    if (loggerFactoryMatch != null)
                    {
                        foundDeps.Add(
                            $"{paramType.FullName} => resolved via ILoggerFactory ({loggerFactoryMatch.Lifetime})");
                        continue; // Skip adding to missing deps
                    }
                }

                if (match == null)
                    missingDeps.Add(paramType.FullName ?? paramType.Name);
                else
                    foundDeps.Add(
                        $"{paramType.FullName} => {match.ImplementationType?.FullName ?? "factory/instance/unknown"} ({match.Lifetime})");
            }

            if (missingDeps.Any())
            {
                handlerDiagnostics.Add($"""
                                        🔍 Handler: {handlerType.FullName}
                                        Constructor: ({string.Join(", ", ctor.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name))})
                                        ❌ Missing Dependencies:
                                        {string.Join("\n", missingDeps.Select(dep => $"   🔴 {dep}"))}
                                        [⚠️ Skipped] Not registered.
                                        """);
                continue;
            }

            foreach (var iface in interfaces)
            {
                services.AddScoped(iface, handlerType);
                handlerDiagnostics.Add($"[✅ Registered] {iface.FullName} -> {handlerType.FullName}");
            }
        }

        foreach (var diag in handlerDiagnostics)
            Console.WriteLine(diag);

        return builder;
    }

    public static IMameyBuilder AddInMemoryEventDispatcher(this IMameyBuilder builder)
    {
        builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
        return builder;
    }
}