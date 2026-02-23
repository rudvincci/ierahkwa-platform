using System.Linq;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Government.Shared.Abstractions;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.TravelIdentities.Core.EF;

internal class TravelIdentitiesInitializer : IInitializer
{
    private readonly ITravelIdentityRepository _travelIdentityRepository;
    private readonly ILogger<TravelIdentitiesInitializer> _logger;
    
    private static readonly TenantId TenantId = new(SeedData.TenantId);

    public TravelIdentitiesInitializer(
        ITravelIdentityRepository travelIdentityRepository,
        ILogger<TravelIdentitiesInitializer> logger)
    {
        _travelIdentityRepository = travelIdentityRepository;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting travel identities database initialization...");

        // Check if data already exists
        var existingTravelIdentities = await _travelIdentityRepository.BrowseAsync(cancellationToken);

        if (existingTravelIdentities.Any())
        {
            _logger.LogInformation("Database already contains {Count} travel identities. Skipping seed.", 
                existingTravelIdentities.Count);
            return;
        }

        var random = new Random(42); // Fixed seed for reproducible data
        var travelIdentities = new List<TravelIdentity>();

        for (int i = 1; i <= 100; i++)
        {
            var travelIdentityId = new TravelIdentityId(SeedData.GenerateDeterministicGuid(i, "travelidentity"));
            // Reference actual citizen IDs (1-100) - not all citizens have travel identities
            var citizenId = SeedData.GetCitizenId(random.Next(1, 101));
            var travelIdentityNumber = new TravelIdentityNumber($"TID{DateTime.UtcNow.Year}{i:D7}");
            var issuedDate = DateTime.UtcNow.AddDays(-random.Next(3650)); // Last 10 years
            var expiryDate = issuedDate.AddYears(8); // 8 year validity for ID cards
            var pdf417Barcode = $"@TID{travelIdentityNumber.Value}@";
            
            var travelIdentity = new TravelIdentity(
                travelIdentityId,
                TenantId,
                citizenId,
                travelIdentityNumber,
                issuedDate,
                expiryDate,
                pdf417Barcode);
            
            // Revoke 2% of travel identities
            if (random.Next(100) < 2)
            {
                travelIdentity.Revoke("Administrative revocation");
            }

            travelIdentities.Add(travelIdentity);
        }

        _logger.LogInformation("Created {Count} mock travel identities", travelIdentities.Count);

        // Add travel identities using repository
        foreach (var travelIdentity in travelIdentities)
        {
            await _travelIdentityRepository.AddAsync(travelIdentity, cancellationToken);
        }
        
        _logger.LogInformation("Successfully seeded {Count} travel identities", travelIdentities.Count);
    }
}
