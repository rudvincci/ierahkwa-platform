using Mamey.Exceptions;

namespace Mamey.Government.Modules.Citizens.Core.Exceptions;

public sealed class InvalidStatusProgressionException : MameyException
{
    public string CurrentStatus { get; }
    public string TargetStatus { get; }

    public InvalidStatusProgressionException(string currentStatus, string targetStatus)
        : base($"Cannot progress from '{currentStatus}' to '{targetStatus}'. Invalid status progression.")
    {
        CurrentStatus = currentStatus;
        TargetStatus = targetStatus;
    }
}
