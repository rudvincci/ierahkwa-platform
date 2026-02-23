using Mamey.FWID.Operations.Api.DTO;

namespace Mamey.FWID.Operations.Api.Infrastructure;

public class OperationUpdatedEventArgs : EventArgs
{
    public OperationDto Operation { get; }

    public OperationUpdatedEventArgs(OperationDto operation)
    {
        Operation = operation;
    }
}



