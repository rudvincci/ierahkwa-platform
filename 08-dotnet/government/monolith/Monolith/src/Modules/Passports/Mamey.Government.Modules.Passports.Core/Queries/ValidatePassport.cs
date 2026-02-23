using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core.DTO;

namespace Mamey.Government.Modules.Passports.Core.Queries;

/// <summary>
/// Public validation query for passport verification.
/// </summary>
public class ValidatePassport : IQuery<PassportValidationResultDto?>
{
    public string PassportNumber { get; set; } = string.Empty;
    public string? DateOfBirth { get; set; } // Format: YYYY-MM-DD
    public string? LastName { get; set; }
}
