using System.Linq;
using Mamey.Government.Modules.Passports.Core.Domain.Entities;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Shared.Abstractions;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Passports.Core.EF;

internal class PassportsInitializer : IInitializer
{
    private readonly IPassportRepository _passportRepository;
    private readonly ILogger<PassportsInitializer> _logger;
    
    private static readonly TenantId TenantId = new(SeedData.TenantId);

    public PassportsInitializer(
        IPassportRepository passportRepository,
        ILogger<PassportsInitializer> logger)
    {
        _passportRepository = passportRepository;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting passports database initialization...");

        // Check if data already exists
        var existingPassports = await _passportRepository.BrowseAsync(cancellationToken);

        if (existingPassports.Any())
        {
            _logger.LogInformation("Database already contains {Count} passports. Skipping seed.", 
                existingPassports.Count);
            return;
        }

        var random = new Random(42); // Fixed seed for reproducible data
        var passports = new List<Passport>();

        for (int i = 1; i <= 100; i++)
        {
            var passportId = new PassportId(SeedData.GenerateDeterministicGuid(i, "passport"));
            // Reference actual citizen IDs (1-100) - not all citizens have passports
            var citizenId = SeedData.GetCitizenId(random.Next(1, 101));
            var passportNumber = new PassportNumber($"P{DateTime.UtcNow.Year}{i:D6}");
            var issuedDate = DateTime.UtcNow.AddDays(-random.Next(3650)); // Last 10 years
            var expiryDate = issuedDate.AddYears(10); // 10 year validity
            var mrz = $"P<USASMITH<<JOHN<<<<<<<<<<<<<<<<<<<<<<<<<<{passportNumber.Value}";
            
            var passport = new Passport(
                passportId,
                TenantId,
                citizenId,
                passportNumber,
                issuedDate,
                expiryDate,
                mrz);
            
            // Revoke 3% of passports
            if (random.Next(100) < 3)
            {
                passport.Revoke("Administrative revocation");
            }

            passports.Add(passport);
        }

        _logger.LogInformation("Created {Count} mock passports", passports.Count);

        // Add passports using repository
        foreach (var passport in passports)
        {
            await _passportRepository.AddAsync(passport, cancellationToken);
        }
        
        _logger.LogInformation("Successfully seeded {Count} passports", passports.Count);
    }
}
