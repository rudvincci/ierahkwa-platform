using Mamey.FWID.Operations.Api.DTO;
using Mamey.FWID.Operations.Api.Types;

namespace Mamey.FWID.Operations.Api.Services;

public interface IOperationsService
{
    event EventHandler<OperationUpdatedEventArgs> OperationUpdated;
    Task<OperationDto> GetAsync(string id);

    Task<(bool updated, OperationDto operation)> TrySetAsync(string id, string userId, string name,
        OperationState state, string code = null, string reason = null);
}



