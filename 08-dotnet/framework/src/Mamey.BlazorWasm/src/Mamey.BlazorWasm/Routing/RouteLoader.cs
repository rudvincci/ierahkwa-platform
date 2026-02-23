using System.Reflection;

namespace Mamey.BlazorWasm.Routing;

public class RouteLoader
{
    public static async Task<List<Route>> GetRoutesAsync(bool menu = false)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var locations = assemblies.Where(x => !x.IsDynamic).Select(x => x.Location).ToArray();
        var routeServices = LoadRoutesServices(assemblies);

        var routes = new List<Route>();
        foreach (var routeService in routeServices)
        {
            await routeService.InitializeAsync(menu);
            routes.AddRange(routeService.Routes);
        }
        return await Task.FromResult(routes);
    }
    public static async Task<List<Route>> GetRoutesAsync<T>(bool menu = false)
        where T : class, IRouteService
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(c => c.FullName == typeof(T).Assembly.FullName).ToList();

        var type = typeof(IRouteService);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p));
        
        var locations = assemblies.Where(x => !x.IsDynamic).Select(x => x.Location).ToArray();
        var routeServices = LoadRoutesServices<T>(assemblies);
        var routes = new List<Route>();

        foreach (var routeService in routeServices)
        {
            await routeService.InitializeAsync(menu);
            routes.AddRange(routeService.Routes);
        }

        return await Task.FromResult(routes);
    }

    private static IEnumerable<IRouteService> LoadRoutesServices(IList<Assembly> assemblies)
        => GetRoutesFromForTypes(AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IRouteService).IsAssignableFrom(p))
            );
    private static IEnumerable<IRouteService> LoadRoutesServices<T>(IEnumerable<Assembly> assemblies)
        where T : class, IRouteService
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .Where(c => c.FullName == typeof(T).Assembly.FullName)
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IRouteService).IsAssignableFrom(p))
            .Where(p => p.Name != "RouteService");
        return GetRoutesFromForTypes(types);
    }

    private static IEnumerable<IRouteService> GetRoutesFromForTypes(IEnumerable<Type> types)
        => types
            .Where(x => typeof(IRouteService).IsAssignableFrom(x) && !x.IsInterface)
            .Where(p => p.Name != "RouteService")
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IRouteService>().ToList();
}

