using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mamey.Ntrada
{
    internal interface IRequestExecutionValidator
    {
        Task<bool> TryExecuteAsync(HttpContext context, RouteConfig routeConfig);
    }
}