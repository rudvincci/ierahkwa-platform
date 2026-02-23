using Mamey.Government.Modules.Tenant.Core.EF;
using Mamey.Government.Shared.Abstractions.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Tenant.Core.Seeding;

/// <summary>
/// Seeds tenant data for testing/development.
/// </summary>
public class TenantDataSeeder : IModuleSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantDataSeeder> _logger;

    public TenantDataSeeder(IServiceProvider serviceProvider, ILogger<TenantDataSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public int Order => 1;
    public string ModuleName => "Tenant";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        
        if (await _context.Tenants.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Tenants already seeded, skipping");
            return;
        }

        _logger.LogInformation("Seeding tenants...");

        var tenants = new List<TenantRow>
        {
            new()
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DisplayName = "Mamey Government",
                Domain = "gov.mamey.io",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = 1
            },
            new()
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                DisplayName = "Test Municipality",
                Domain = "test.gov.mamey.io",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = 1
            }
        };

        await _context.Tenants.AddRangeAsync(tenants, cancellationToken);
        
        // Add default settings for the default tenant
        var settings = new TenantSettingsRow
        {
            TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            BrandingJson = """{"primaryColor": "#1976d2", "logoUrl": "/images/logo.png"}""",
            DocumentNamingConfig = """{"passportPrefix": "P", "idCardPrefix": "ID", "certPrefix": "CERT"}""",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.TenantSettings.AddAsync(settings, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} tenants", tenants.Count);
    }
}
