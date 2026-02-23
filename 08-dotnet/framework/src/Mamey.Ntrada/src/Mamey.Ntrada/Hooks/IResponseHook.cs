using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mamey.Ntrada.Hooks
{
    public interface IResponseHook
    {
        Task InvokeAsync(HttpResponse response, ExecutionData data);
    }
}