using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mamey.Ntrada
{
    public interface IPayloadValidator
    {
        Task<bool> TryValidate(ExecutionData executionData, HttpResponse httpResponse);
        Task<IEnumerable<Error>> GetValidationErrorsAsync(PayloadSchema payloadSchema);
    }
}