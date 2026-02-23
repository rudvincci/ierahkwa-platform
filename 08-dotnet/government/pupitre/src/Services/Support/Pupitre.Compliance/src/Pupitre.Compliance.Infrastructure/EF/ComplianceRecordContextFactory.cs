using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pupitre.Compliance.Infrastructure.EF;

internal class ComplianceRecordContextFactory : IDesignTimeDbContextFactory<ComplianceRecordDbContext>
{
    public ComplianceRecordDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<ComplianceRecordDbContext>();
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString);

        return new ComplianceRecordDbContext(optionsBuilder.Options);
    }
}