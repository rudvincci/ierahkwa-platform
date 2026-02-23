using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Route = Mamey.Ntrada.Configuration.Route;

namespace Mamey.Ntrada
{
    public interface IHandler
    {
        string GetInfo(Route route);
        Task HandleAsync(HttpContext context, RouteConfig config);
    }
}