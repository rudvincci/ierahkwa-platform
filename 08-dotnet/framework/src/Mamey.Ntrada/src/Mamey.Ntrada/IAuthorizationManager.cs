using System.Security.Claims;

namespace Mamey.Ntrada
{
    public interface IAuthorizationManager
    {
        bool IsAuthorized(ClaimsPrincipal user, RouteConfig routeConfig);
    }
}