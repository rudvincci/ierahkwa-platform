using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;

namespace Mamey.Government.Modules.TravelIdentities.Core.Queries;

/// <summary>
/// Public validation query for travel identity verification.
/// </summary>
public class ValidateTravelIdentity : IQuery<TravelIdentityValidationResultDto?>
{
    public string IdNumber { get; set; } = string.Empty;
    public string? DateOfBirth { get; set; } // Format: YYYY-MM-DD
    public string? LastName { get; set; }
}
