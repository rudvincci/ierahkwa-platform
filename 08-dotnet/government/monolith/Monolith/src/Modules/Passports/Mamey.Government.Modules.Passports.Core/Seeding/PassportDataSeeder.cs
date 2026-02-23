using Mamey.Government.Modules.Passports.Core.EF;
using Mamey.Government.Shared.Abstractions.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Passports.Core.Seeding;

/// <summary>
/// Seeds passport data for testing/development.
/// </summary>
public class PassportDataSeeder : IModuleSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PassportDataSeeder> _logger;
    private static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public PassportDataSeeder(IServiceProvider serviceProvider, ILogger<PassportDataSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public int Order => 20;
    public string ModuleName => "Passports";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<PassportsDbContext>();
        
        if (await _context.Passports.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Passports already seeded, skipping");
            return;
        }

        _logger.LogInformation("Seeding passports...");

        var passports = new List<PassportRow>
        {
            // Active passports (linked to seeded citizens)
            CreatePassport(1, Guid.Parse("10000000-0000-0000-0000-000000000001"), "P00000001", DateTime.UtcNow.AddYears(-2), DateTime.UtcNow.AddYears(8), true),
            CreatePassport(2, Guid.Parse("10000000-0000-0000-0000-000000000002"), "P00000002", DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(9), true),
            CreatePassport(3, Guid.Parse("10000000-0000-0000-0000-000000000007"), "P00000003", DateTime.UtcNow.AddYears(-5), DateTime.UtcNow.AddYears(5), true),
            CreatePassport(4, Guid.Parse("10000000-0000-0000-0000-000000000008"), "P00000004", DateTime.UtcNow.AddYears(-3), DateTime.UtcNow.AddYears(7), true),
            
            // Expiring soon (within 3 months)
            CreatePassport(5, Guid.Parse("10000000-0000-0000-0000-000000000003"), "P00000005", DateTime.UtcNow.AddYears(-10).AddMonths(2), DateTime.UtcNow.AddMonths(2), true),
            CreatePassport(6, Guid.Parse("10000000-0000-0000-0000-000000000004"), "P00000006", DateTime.UtcNow.AddYears(-10).AddMonths(1), DateTime.UtcNow.AddMonths(1), true),
            
            // Expired passports
            CreatePassport(7, Guid.Parse("10000000-0000-0000-0000-000000000005"), "P00000007", DateTime.UtcNow.AddYears(-11), DateTime.UtcNow.AddYears(-1), true),
            CreatePassport(8, Guid.Parse("10000000-0000-0000-0000-000000000009"), "P00000008", DateTime.UtcNow.AddYears(-12), DateTime.UtcNow.AddYears(-2), true),
            
            // Revoked passport
            CreatePassport(9, Guid.Parse("10000000-0000-0000-0000-000000000006"), "P00000009", DateTime.UtcNow.AddYears(-4), DateTime.UtcNow.AddYears(6), false, "Reported lost/stolen"),
        };

        await _context.Passports.AddRangeAsync(passports, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} passports", passports.Count);
    }

    private static PassportRow CreatePassport(int index, Guid citizenId, string passportNumber, 
        DateTime issuedDate, DateTime expiryDate, bool isActive, string? revocationReason = null)
    {
        var mrz = GenerateMrz(passportNumber, issuedDate, expiryDate);
        
        return new PassportRow
        {
            Id = Guid.Parse($"30000000-0000-0000-0000-{index:D12}"),
            TenantId = DefaultTenantId,
            CitizenId = citizenId,
            PassportNumber = passportNumber,
            IssuedDate = issuedDate,
            ExpiryDate = expiryDate,
            Mrz = mrz,
            DocumentPath = $"/documents/passports/{passportNumber}.pdf",
            IsActive = isActive,
            RevokedAt = isActive ? null : DateTime.UtcNow.AddDays(-30),
            RevocationReason = revocationReason,
            CreatedAt = issuedDate,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };
    }

    private static string GenerateMrz(string passportNumber, DateTime issuedDate, DateTime expiryDate)
    {
        // Generate ICAO MRZ line 2 (simplified)
        var expiryMrz = expiryDate.ToString("yyMMdd");
        return $"P<MAMEY<<CITIZEN<<<<<<<<<<<<<<<<<<<<<<<<<<\n{passportNumber}<<<<<<MAM{expiryMrz}1M<<<<<<<<<<<<<<<0";
    }
}
