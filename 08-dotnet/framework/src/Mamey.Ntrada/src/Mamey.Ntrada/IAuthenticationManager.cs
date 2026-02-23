using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mamey.Ntrada
{
    public interface IAuthenticationManager
    {
        Task<bool> TryAuthenticateAsync(HttpRequest request, RouteConfig routeConfig);
    }
}