using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Certificates.Core.EF;
using Mamey.Government.Shared.Abstractions.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Certificates.Core.Seeding;

/// <summary>
/// Seeds certificate data for testing/development.
/// </summary>
public class CertificateDataSeeder : IModuleSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CertificateDataSeeder> _logger;
    private static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public CertificateDataSeeder(IServiceProvider serviceProvider, ILogger<CertificateDataSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public int Order => 22;
    public string ModuleName => "Certificates";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<CertificatesDbContext>();
        
        if (await _context.Certificates.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Certificates already seeded, skipping");
            return;
        }

        _logger.LogInformation("Seeding certificates...");

        var certificates = new List<CertificateRow>
        {
            // Birth certificates (linked to seeded citizens)
            CreateCertificate(1, Guid.Parse("10000000-0000-0000-0000-000000000001"), CertificateType.BirthCertificate, "BC-2025-000001", DateTime.UtcNow.AddYears(-40), true),
            CreateCertificate(2, Guid.Parse("10000000-0000-0000-0000-000000000002"), CertificateType.BirthCertificate, "BC-2025-000002", DateTime.UtcNow.AddYears(-35), true),
            CreateCertificate(3, Guid.Parse("10000000-0000-0000-0000-000000000003"), CertificateType.BirthCertificate, "BC-2025-000003", DateTime.UtcNow.AddYears(-47), true),
            CreateCertificate(4, Guid.Parse("10000000-0000-0000-0000-000000000004"), CertificateType.BirthCertificate, "BC-2025-000004", DateTime.UtcNow.AddYears(-30), true),
            CreateCertificate(5, Guid.Parse("10000000-0000-0000-0000-000000000005"), CertificateType.BirthCertificate, "BC-2025-000005", DateTime.UtcNow.AddYears(-37), true),
            
            // Citizenship certificates
            CreateCertificate(6, Guid.Parse("10000000-0000-0000-0000-000000000001"), CertificateType.CitizenshipCertificate, "CC-2025-000001", DateTime.UtcNow.AddYears(-5), true),
            CreateCertificate(7, Guid.Parse("10000000-0000-0000-0000-000000000002"), CertificateType.CitizenshipCertificate, "CC-2025-000002", DateTime.UtcNow.AddYears(-3), true),
            CreateCertificate(8, Guid.Parse("10000000-0000-0000-0000-000000000007"), CertificateType.CitizenshipCertificate, "CC-2025-000003", DateTime.UtcNow.AddYears(-10), true),
            CreateCertificate(9, Guid.Parse("10000000-0000-0000-0000-000000000008"), CertificateType.CitizenshipCertificate, "CC-2025-000004", DateTime.UtcNow.AddYears(-7), true),
            
            // Marriage certificates
            CreateCertificate(10, Guid.Parse("10000000-0000-0000-0000-000000000001"), CertificateType.MarriageCertificate, "MC-2025-000001", DateTime.UtcNow.AddYears(-15), true),
            CreateCertificate(11, Guid.Parse("10000000-0000-0000-0000-000000000007"), CertificateType.MarriageCertificate, "MC-2025-000002", DateTime.UtcNow.AddYears(-20), true),
            
            // Death certificates (no citizen link - archived)
            CreateCertificate(12, null, CertificateType.DeathCertificate, "DC-2025-000001", DateTime.UtcNow.AddYears(-1), true),
            CreateCertificate(13, null, CertificateType.DeathCertificate, "DC-2025-000002", DateTime.UtcNow.AddMonths(-6), true),
            
            // Archived certificate
            CreateCertificate(14, Guid.Parse("10000000-0000-0000-0000-000000000003"), CertificateType.BirthCertificate, "BC-2025-000006", DateTime.UtcNow.AddYears(-47), false),
            
            // Revoked certificate
            CreateCertificate(15, Guid.Parse("10000000-0000-0000-0000-000000000006"), CertificateType.CitizenshipCertificate, "CC-2025-000005", DateTime.UtcNow.AddYears(-2), false, "Fraudulent application detected"),
        };

        await _context.Certificates.AddRangeAsync(certificates, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} certificates", certificates.Count);
    }

    private static CertificateRow CreateCertificate(int index, Guid? citizenId, CertificateType type, 
        string certificateNumber, DateTime issuedDate, bool isActive, string? revocationReason = null)
    {
        return new CertificateRow
        {
            Id = Guid.Parse($"50000000-0000-0000-0000-{index:D12}"),
            TenantId = DefaultTenantId,
            CitizenId = citizenId,
            CertificateType = type,
            CertificateNumber = certificateNumber,
            IssuedDate = issuedDate,
            DocumentPath = $"/documents/certificates/{certificateNumber}.pdf",
            IsActive = isActive,
            RevokedAt = isActive ? null : DateTime.UtcNow.AddDays(-60),
            RevocationReason = revocationReason,
            CreatedAt = issuedDate,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };
    }
}
