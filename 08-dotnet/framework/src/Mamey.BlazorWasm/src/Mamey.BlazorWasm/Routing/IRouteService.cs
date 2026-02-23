namespace Mamey.BlazorWasm.Routing;
public interface IRouteService
{
    List<Route> Routes { get; }
    Task InitializeAsync(bool menu = false);
    event Action<List<Route>> RoutesChanged;
}