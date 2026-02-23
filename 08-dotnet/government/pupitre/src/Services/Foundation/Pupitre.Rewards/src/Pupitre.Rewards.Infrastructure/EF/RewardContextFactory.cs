using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pupitre.Rewards.Infrastructure.EF;

internal class RewardContextFactory : IDesignTimeDbContextFactory<RewardDbContext>
{
    public RewardDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<RewardDbContext>();
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString);

        return new RewardDbContext(optionsBuilder.Options);
    }
}