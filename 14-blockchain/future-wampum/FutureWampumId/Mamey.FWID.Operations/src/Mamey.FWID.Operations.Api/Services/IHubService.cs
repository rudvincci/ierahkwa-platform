using Mamey.FWID.Operations.Api.DTO;

namespace Mamey.FWID.Operations.Api.Services;

public interface IHubService
{
    Task PublishOperationPendingAsync(OperationDto operation);
    Task PublishOperationCompletedAsync(OperationDto operation);
    Task PublishOperationRejectedAsync(OperationDto operation);
}



