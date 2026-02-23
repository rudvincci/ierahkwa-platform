using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pupitre.AIAssessments.Infrastructure.EF;

internal class AIAssessmentContextFactory : IDesignTimeDbContextFactory<AIAssessmentDbContext>
{
    public AIAssessmentDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<AIAssessmentDbContext>();
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString);

        return new AIAssessmentDbContext(optionsBuilder.Options);
    }
}