using Mamey.Government.Modules.TravelIdentities.Core.EF;
using Mamey.Government.Shared.Abstractions.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.TravelIdentities.Core.Seeding;

/// <summary>
/// Seeds travel identity data for testing/development.
/// </summary>
public class TravelIdentityDataSeeder : IModuleSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TravelIdentityDataSeeder> _logger;
    private static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public TravelIdentityDataSeeder(IServiceProvider serviceProvider, ILogger<TravelIdentityDataSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public int Order => 21;
    public string ModuleName => "Travel Identities";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<TravelIdentitiesDbContext>();
        
        if (await _context.TravelIdentities.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Travel identities already seeded, skipping");
            return;
        }

        _logger.LogInformation("Seeding travel identities...");

        var travelIdentities = new List<TravelIdentityRow>
        {
            // Active travel IDs (linked to seeded citizens)
            CreateTravelIdentity(1, Guid.Parse("10000000-0000-0000-0000-000000000001"), "TI00000001", DateTime.UtcNow.AddYears(-2), DateTime.UtcNow.AddYears(6), true),
            CreateTravelIdentity(2, Guid.Parse("10000000-0000-0000-0000-000000000002"), "TI00000002", DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(7), true),
            CreateTravelIdentity(3, Guid.Parse("10000000-0000-0000-0000-000000000003"), "TI00000003", DateTime.UtcNow.AddYears(-3), DateTime.UtcNow.AddYears(5), true),
            CreateTravelIdentity(4, Guid.Parse("10000000-0000-0000-0000-000000000004"), "TI00000004", DateTime.UtcNow.AddYears(-4), DateTime.UtcNow.AddYears(4), true),
            CreateTravelIdentity(5, Guid.Parse("10000000-0000-0000-0000-000000000007"), "TI00000005", DateTime.UtcNow.AddYears(-2), DateTime.UtcNow.AddYears(6), true),
            CreateTravelIdentity(6, Guid.Parse("10000000-0000-0000-0000-000000000008"), "TI00000006", DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(7), true),
            
            // Expiring soon (within 3 months)
            CreateTravelIdentity(7, Guid.Parse("10000000-0000-0000-0000-000000000005"), "TI00000007", DateTime.UtcNow.AddYears(-8).AddMonths(2), DateTime.UtcNow.AddMonths(2), true),
            CreateTravelIdentity(8, Guid.Parse("10000000-0000-0000-0000-000000000009"), "TI00000008", DateTime.UtcNow.AddYears(-8).AddMonths(1), DateTime.UtcNow.AddMonths(1), true),
            
            // Expired travel IDs
            CreateTravelIdentity(9, Guid.Parse("10000000-0000-0000-0000-000000000006"), "TI00000009", DateTime.UtcNow.AddYears(-9), DateTime.UtcNow.AddMonths(-6), true),
            CreateTravelIdentity(10, Guid.Parse("10000000-0000-0000-0000-000000000010"), "TI00000010", DateTime.UtcNow.AddYears(-10), DateTime.UtcNow.AddYears(-2), true),
            
            // Revoked travel ID
            CreateTravelIdentity(11, Guid.Parse("10000000-0000-0000-0000-000000000006"), "TI00000011", DateTime.UtcNow.AddYears(-5), DateTime.UtcNow.AddYears(3), false, "Damaged beyond recognition"),
        };

        await _context.TravelIdentities.AddRangeAsync(travelIdentities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} travel identities", travelIdentities.Count);
    }

    private static TravelIdentityRow CreateTravelIdentity(int index, Guid citizenId, string idNumber, 
        DateTime issuedDate, DateTime expiryDate, bool isActive, string? revocationReason = null)
    {
        var barcode = GeneratePdf417Barcode(idNumber, issuedDate, expiryDate);
        
        return new TravelIdentityRow
        {
            Id = Guid.Parse($"40000000-0000-0000-0000-{index:D12}"),
            TenantId = DefaultTenantId,
            CitizenId = citizenId,
            TravelIdentityNumber = idNumber,
            IssuedDate = issuedDate,
            ExpiryDate = expiryDate,
            Pdf417Barcode = barcode,
            DocumentPath = $"/documents/travel-ids/{idNumber}.pdf",
            IsActive = isActive,
            RevokedAt = isActive ? null : DateTime.UtcNow.AddDays(-15),
            RevocationReason = revocationReason,
            CreatedAt = issuedDate,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };
    }

    private static string GeneratePdf417Barcode(string idNumber, DateTime issuedDate, DateTime expiryDate)
    {
        // Generate AAMVA PDF417 barcode data (simplified)
        return $@"@
ANSI 123456789
DAQ{idNumber}
DCS<CITIZEN>
DDE<FIRST>
DAG123 MAIN ST
DAICAPITAL CITY
DAJCC
DAK12345
DBB19850101
DBA{expiryDate:yyyyMMdd}
DBD{issuedDate:yyyyMMdd}";
    }
}
