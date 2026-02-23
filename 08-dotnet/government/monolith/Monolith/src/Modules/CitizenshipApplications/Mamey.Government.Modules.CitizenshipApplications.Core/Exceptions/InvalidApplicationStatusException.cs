using Mamey.Exceptions;
using Mamey.MicroMonolith.Abstractions.Exceptions;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Exceptions;

public sealed class InvalidApplicationStatusException : MameyException
{
    public string CurrentStatus { get; }
    public string ExpectedStatus { get; }

    public InvalidApplicationStatusException(string currentStatus, string expectedStatus)
        : base($"Application is in '{currentStatus}' status but expected '{expectedStatus}'.")
    {
        CurrentStatus = currentStatus;
        ExpectedStatus = expectedStatus;
    }
}
