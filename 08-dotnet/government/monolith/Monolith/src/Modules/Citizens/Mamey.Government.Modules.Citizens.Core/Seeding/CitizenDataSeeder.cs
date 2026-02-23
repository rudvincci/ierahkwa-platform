using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.EF;
using Mamey.Government.Shared.Abstractions.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Citizens.Core.Seeding;

/// <summary>
/// Seeds citizen data for testing/development.
/// </summary>
public class CitizenDataSeeder : IModuleSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CitizenDataSeeder> _logger;
    private static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public CitizenDataSeeder(IServiceProvider serviceProvider, ILogger<CitizenDataSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public int Order => 10;
    public string ModuleName => "Citizens";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<CitizensDbContext>();
        
        if (await _context.Citizens.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Citizens already seeded, skipping");
            return;
        }

        _logger.LogInformation("Seeding citizens...");

        var citizens = new List<CitizenRow>
        {
            CreateCitizen(1, "John", "Smith", "john.smith@example.com", "+1-555-0101", CitizenshipStatus.Citizen, new DateTime(1985, 3, 15)),
            CreateCitizen(2, "Maria", "Garcia", "maria.garcia@example.com", "+1-555-0102", CitizenshipStatus.Citizen, new DateTime(1990, 7, 22)),
            CreateCitizen(3, "James", "Wilson", "james.wilson@example.com", "+1-555-0103", CitizenshipStatus.Resident, new DateTime(1978, 11, 8)),
            CreateCitizen(4, "Emily", "Johnson", "emily.johnson@example.com", "+1-555-0104", CitizenshipStatus.Resident, new DateTime(1995, 5, 30)),
            CreateCitizen(5, "Michael", "Brown", "michael.brown@example.com", "+1-555-0105", CitizenshipStatus.Probationary, new DateTime(1988, 9, 12)),
            CreateCitizen(6, "Sarah", "Davis", "sarah.davis@example.com", "+1-555-0106", CitizenshipStatus.Probationary, new DateTime(1992, 1, 25)),
            CreateCitizen(7, "Robert", "Miller", "robert.miller@example.com", "+1-555-0107", CitizenshipStatus.Citizen, new DateTime(1970, 4, 18)),
            CreateCitizen(8, "Jennifer", "Taylor", "jennifer.taylor@example.com", "+1-555-0108", CitizenshipStatus.Citizen, new DateTime(1983, 8, 5)),
            CreateCitizen(9, "David", "Anderson", "david.anderson@example.com", "+1-555-0109", CitizenshipStatus.Resident, new DateTime(1975, 12, 3)),
            CreateCitizen(10, "Lisa", "Thomas", "lisa.thomas@example.com", "+1-555-0110", CitizenshipStatus.Probationary, new DateTime(1998, 6, 14)),
        };

        await _context.Citizens.AddRangeAsync(citizens, cancellationToken);
        
        // Add status history for each citizen
        foreach (var citizen in citizens)
        {
            var history = new CitizenshipStatusHistoryRow
            {
                CitizenId = citizen.Id,
                Status = citizen.Status,
                ChangedAt = citizen.CreatedAt,
                Reason = "Initial registration"
            };
            await _context.StatusHistory.AddAsync(history, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} citizens", citizens.Count);
    }

    private static CitizenRow CreateCitizen(int index, string firstName, string lastName, string email, string phone, 
        CitizenshipStatus status, DateTime dateOfBirth)
    {
        return new CitizenRow
        {
            Id = Guid.Parse($"10000000-0000-0000-0000-{index:D12}"),
            TenantId = DefaultTenantId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            AddressJson = $$$"""{"street": "{index * 100} Main Street", "city": "Capital City", "state": "CC", "postalCode": "12345", "country": "Mamey"}""",
            DateOfBirth = dateOfBirth,
            Status = status,
            CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(30, 365)),
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };
    }
}
