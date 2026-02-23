using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF;

internal class CitizenApplicationsContextFactory: IDesignTimeDbContextFactory<CitizenshipApplicationsDbContext>
{
    public CitizenshipApplicationsDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<CitizenshipApplicationsDbContext>();
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString);
        
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString, npgsql =>
        {
            npgsql.MigrationsAssembly("Mamey.Government.Modules.CitizenshipApplications.Core");
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Identity", "citizenship_applications");
            
        });

        return new CitizenshipApplicationsDbContext(optionsBuilder.Options);
    }
}