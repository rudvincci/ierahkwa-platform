using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Government.Modules.Certificates.Core.DTO;

namespace Mamey.Government.Modules.Certificates.Core.Mappings;

internal static class CertificateMappings
{
    public static CertificateDto AsDto(this Certificate certificate)
        => new()
        {
            Id = certificate.Id.Value,
            TenantId = certificate.TenantId.Value,
            CitizenId = certificate.CitizenId,
            CertificateType = certificate.CertificateType.ToString(),
            CertificateNumber = certificate.CertificateNumber,
            IssuedDate = certificate.IssuedDate,
            DocumentPath = certificate.DocumentPath,
            IsActive = certificate.IsActive,
            RevokedAt = certificate.RevokedAt,
            RevocationReason = certificate.RevocationReason,
            CreatedAt = certificate.CreatedAt,
            UpdatedAt = certificate.UpdatedAt
        };

    public static CertificateSummaryDto AsSummaryDto(this Certificate certificate)
        => new()
        {
            Id = certificate.Id.Value,
            CertificateType = certificate.CertificateType.ToString(),
            CertificateNumber = certificate.CertificateNumber,
            CitizenId = certificate.CitizenId,
            IssuedDate = certificate.IssuedDate,
            IsActive = certificate.IsActive
        };
}
