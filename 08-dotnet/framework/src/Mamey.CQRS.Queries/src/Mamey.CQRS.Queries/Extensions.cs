using System.Reflection;
using Mamey.CQRS.Queries.Decorators;
using Mamey.CQRS.Queries.Dispatchers;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.CQRS.Queries;

public static class Extensions
{
    public static IMameyBuilder AddQueryHandlers(this IMameyBuilder builder, IEnumerable<Assembly>? assemblies = null)
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
                !t.IsDefined(typeof(DecoratorAttribute), false) &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
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

        // Dump everything at once
        foreach (var diag in handlerDiagnostics)
            Console.WriteLine(diag);

        return builder;
    }

    private static bool ImplementsOpenGenericInterface(Type type, Type openGeneric)
    {
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);
    }
    public static IMameyBuilder AddScopedQueryHandlers(this IMameyBuilder builder, IEnumerable<Assembly>? assemblies = null)
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
                !t.IsDefined(typeof(DecoratorAttribute), false) &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
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

        // Dump everything at once
        foreach (var diag in handlerDiagnostics)
            Console.WriteLine(diag);

        return builder;
    }
    public static IMameyBuilder AddInMemoryQueryDispatcher(this IMameyBuilder builder)
    {
        builder.Services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
        return builder;
    }
    public static IServiceCollection AddPagedQueryDecorator(this IServiceCollection services)
    {
        services.TryDecorate(typeof(IQueryHandler<,>), typeof(PagedQueryHandlerDecorator<,>));

        return services;
    }
}