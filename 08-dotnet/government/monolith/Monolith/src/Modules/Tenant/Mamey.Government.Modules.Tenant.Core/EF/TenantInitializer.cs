using System.Linq;
using Mamey.Government.Shared.Abstractions;
using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Tenant.Core.EF;

internal class TenantInitializer : IInitializer
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ILogger<TenantInitializer> _logger;

    private static readonly string[] TenantNames = {
        "United States Government", "State of California", "State of New York", "State of Texas",
        "State of Florida", "State of Illinois", "State of Pennsylvania", "State of Ohio",
        "State of Georgia", "State of North Carolina", "State of Michigan", "State of New Jersey",
        "State of Virginia", "State of Washington", "State of Arizona", "State of Massachusetts",
        "State of Tennessee", "State of Indiana", "State of Missouri", "State of Maryland"
    };

    private static readonly string[] Domains = {
        "us.gov", "ca.gov", "ny.gov", "tx.gov", "fl.gov", "il.gov", "pa.gov", "oh.gov",
        "ga.gov", "nc.gov", "mi.gov", "nj.gov", "va.gov", "wa.gov", "az.gov", "ma.gov",
        "tn.gov", "in.gov", "mo.gov", "md.gov"
    };

    public TenantInitializer(
        ITenantRepository tenantRepository,
        ILogger<TenantInitializer> logger)
    {
        _tenantRepository = tenantRepository;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting tenant database initialization...");

        // Check if data already exists
        var existingTenants = await _tenantRepository.BrowseAsync(cancellationToken);

        if (existingTenants.Any())
        {
            _logger.LogInformation("Database already contains {Count} tenants. Skipping seed.", 
                existingTenants.Count);
            return;
        }

        var random = new Random(42); // Fixed seed for reproducible data
        var tenants = new List<TenantEntity>();

        // Create default tenant (the one used by CitizenshipApplications)
        var defaultTenantId = new TenantId(SeedData.TenantId);
        var defaultTenant = new TenantEntity(
            defaultTenantId,
            "Default Government Tenant",
            "default.gov",
            isActive: true);
        tenants.Add(defaultTenant);

        // Create additional tenants with deterministic IDs
        for (int i = 0; i < TenantNames.Length; i++)
        {
            var tenantId = new TenantId(SeedData.GenerateDeterministicGuid(i + 1, "tenant"));
            var tenant = new TenantEntity(
                tenantId,
                TenantNames[i],
                Domains[i],
                isActive: random.Next(100) < 90); // 90% active
            
            tenants.Add(tenant);
        }

        _logger.LogInformation("Created {Count} mock tenants", tenants.Count);

        // Add tenants using repository
        foreach (var tenant in tenants)
        {
            await _tenantRepository.AddAsync(tenant, cancellationToken);
        }
        
        _logger.LogInformation("Successfully seeded {Count} tenants", tenants.Count);
    }
}
