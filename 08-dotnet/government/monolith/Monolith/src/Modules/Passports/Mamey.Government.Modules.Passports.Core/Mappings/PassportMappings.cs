using Mamey.Government.Modules.Passports.Core.Domain.Entities;
using Mamey.Government.Modules.Passports.Core.DTO;

namespace Mamey.Government.Modules.Passports.Core.Mappings;

internal static class PassportMappings
{
    public static PassportDto AsDto(this Passport passport)
    {
        var mrz = passport.Mrz?.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        
        return new PassportDto
        {
            Id = passport.Id.Value,
            TenantId = passport.TenantId.Value,
            CitizenId = passport.CitizenId,
            PassportNumber = passport.PassportNumber.Value,
            MrzLine1 = mrz?.Length > 0 ? mrz[0] : null,
            MrzLine2 = mrz?.Length > 1 ? mrz[1] : null,
            IssuedDate = passport.IssuedDate,
            ExpiryDate = passport.ExpiryDate,
            DocumentPath = passport.DocumentPath,
            IsActive = passport.IsActive,
            RevokedAt = passport.RevokedAt,
            RevocationReason = passport.RevocationReason,
            CreatedAt = passport.CreatedAt,
            UpdatedAt = passport.UpdatedAt
        };
    }

    public static PassportSummaryDto AsSummaryDto(this Passport passport)
        => new()
        {
            Id = passport.Id.Value,
            PassportNumber = passport.PassportNumber.Value,
            CitizenId = passport.CitizenId,
            ExpiryDate = passport.ExpiryDate,
            IsActive = passport.IsActive
        };
}
