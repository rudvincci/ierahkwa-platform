using System.Linq;
using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Shared.Abstractions;
using Mamey.Types;
using Microsoft.Extensions.Logging;
using TenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.Certificates.Core.EF;

internal class CertificatesInitializer : IInitializer
{
    private readonly ICertificateRepository _certificateRepository;
    private readonly ILogger<CertificatesInitializer> _logger;
    
    private static readonly TenantId TenantId = new(SeedData.TenantId);

    public CertificatesInitializer(
        ICertificateRepository certificateRepository,
        ILogger<CertificatesInitializer> logger)
    {
        _certificateRepository = certificateRepository;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting certificates database initialization...");

        // Check if data already exists
        var existingCertificates = await _certificateRepository.BrowseAsync(cancellationToken);

        if (existingCertificates.Any())
        {
            _logger.LogInformation("Database already contains {Count} certificates. Skipping seed.", 
                existingCertificates.Count);
            return;
        }

        var random = new Random(42); // Fixed seed for reproducible data
        var certificates = new List<Certificate>();

        for (int i = 1; i <= 100; i++)
        {
            var certificateId = new CertificateId(SeedData.GenerateDeterministicGuid(i, "certificate"));
            var certificateType = (CertificateType)random.Next(0, 4); // 0-3 for valid types
            var certificateNumber = $"CERT-{DateTime.UtcNow.Year}-{i:D6}";
            var issuedDate = DateTime.UtcNow.AddDays(-random.Next(3650)); // Last 10 years
            var documentPath = $"certificates/{SeedData.TenantId:N}/{certificateId.Value:N}/certificate.pdf";
            
            // 80% have citizen IDs that reference actual citizens (1-100), 20% don't
            Guid? citizenId = random.Next(100) < 80 ? SeedData.GetCitizenId(random.Next(1, 101)) : null;
            
            var certificate = new Certificate(
                certificateId,
                TenantId,
                citizenId,
                certificateType,
                certificateNumber,
                issuedDate,
                documentPath);
            
            // Revoke 5% of certificates
            if (random.Next(100) < 5)
            {
                certificate.Revoke("Administrative revocation");
            }

            certificates.Add(certificate);
        }

        _logger.LogInformation("Created {Count} mock certificates", certificates.Count);

        // Add certificates using repository
        foreach (var certificate in certificates)
        {
            await _certificateRepository.AddAsync(certificate, cancellationToken);
        }
        
        _logger.LogInformation("Successfully seeded {Count} certificates", certificates.Count);
    }
}
