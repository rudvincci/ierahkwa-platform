using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mamey.Casino.Infrastructure.EF;

internal class CasinoDbContextFactory : IDesignTimeDbContextFactory<CasinoDbContext>
{
    public CasinoDbContext CreateDbContext(string[] args)
    {
        // Load configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Build options for Npgsql
        var optionsBuilder = new DbContextOptionsBuilder<CasinoDbContext>();
        var connectionString = configuration.GetConnectionString("postgres:connectionString");

        optionsBuilder.UseNpgsql(connectionString, npgsql =>
            npgsql.MigrationsAssembly(typeof(CasinoDbContext).Assembly.FullName));
            
        
        return new CasinoDbContext(optionsBuilder.Options);
    }
}