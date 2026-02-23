using Mamey.Auth.Identity.Data;
using Mamey.Auth.Identity.Providers;
using Mamey.Persistence.SQL;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mamey.ApplicationName.Modules.Identity.Core.EF;

internal class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
{
    private readonly AppOptions _options;

    public ApplicationIdentityDbContextFactory()
    {
        
    }
    public ApplicationIdentityDbContext CreateDbContext(string[] args)
    {
        // Load configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Build options for Npgsql
        var optionsBuilder = new DbContextOptionsBuilder<MameyIdentityDbContext>();
        var connectionString = configuration.GetConnectionString("postgres:connectionString");
        // var tenantId = configuration.GetSection("app:tenantId").Value;
        // ITenantProvider tenantProvider;
        // if (string.IsNullOrWhiteSpace(tenantId))
        // {
        //     tenantProvider = new TenantProvider()
        // }

        optionsBuilder.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsAssembly("Mamey.ApplicationName.Modules.Identity.Core");
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Identity", "identity");
            
        });
      
        
        
        return new ApplicationIdentityDbContext(optionsBuilder.Options);
    }
}
