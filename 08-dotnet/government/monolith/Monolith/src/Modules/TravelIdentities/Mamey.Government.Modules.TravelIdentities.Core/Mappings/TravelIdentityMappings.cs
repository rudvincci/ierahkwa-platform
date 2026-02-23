using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;

namespace Mamey.Government.Modules.TravelIdentities.Core.Mappings;

internal static class TravelIdentityMappings
{
    public static TravelIdentityDto AsDto(this TravelIdentity travelIdentity)
        => new()
        {
            Id = travelIdentity.Id.Value,
            TenantId = travelIdentity.TenantId.Value,
            CitizenId = travelIdentity.CitizenId,
            TravelIdentityNumber = travelIdentity.TravelIdentityNumber.Value,
            IssuedDate = travelIdentity.IssuedDate,
            ExpiryDate = travelIdentity.ExpiryDate,
            Pdf417Barcode = travelIdentity.Pdf417Barcode,
            DocumentPath = travelIdentity.DocumentPath,
            IsActive = travelIdentity.IsActive,
            RevokedAt = travelIdentity.RevokedAt,
            RevocationReason = travelIdentity.RevocationReason,
            CreatedAt = travelIdentity.CreatedAt,
            UpdatedAt = travelIdentity.UpdatedAt
        };

    public static TravelIdentitySummaryDto AsSummaryDto(this TravelIdentity travelIdentity)
        => new()
        {
            Id = travelIdentity.Id.Value,
            TravelIdentityNumber = travelIdentity.TravelIdentityNumber.Value,
            CitizenId = travelIdentity.CitizenId,
            ExpiryDate = travelIdentity.ExpiryDate,
            IsActive = travelIdentity.IsActive
        };
}
