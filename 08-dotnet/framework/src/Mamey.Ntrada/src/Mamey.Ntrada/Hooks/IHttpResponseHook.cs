using System.Net.Http;
using System.Threading.Tasks;

namespace Mamey.Ntrada.Hooks
{
    public interface IHttpResponseHook
    {
        Task InvokeAsync(HttpResponseMessage response, ExecutionData data);
    }
}