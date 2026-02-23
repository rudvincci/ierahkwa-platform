using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mamey.Government.Modules.Documents.Core.EF;

internal class DocumentsContextFactory : IDesignTimeDbContextFactory<DocumentsDbContext>
{
    public DocumentsDbContext CreateDbContext(string[] args)
    {
        // Build the configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Bind PostgresOptions from the configuration
        var postgresOptions = new PostgresOptions();
        configuration.GetSection("postgres").Bind(postgresOptions);

        var optionsBuilder = new DbContextOptionsBuilder<DocumentsDbContext>();
        
        optionsBuilder.UseNpgsql(postgresOptions.ConnectionString, npgsql =>
        {
            npgsql.MigrationsAssembly("Mamey.Government.Modules.Documents.Core");
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Documents", "documents");
        });

        return new DocumentsDbContext(optionsBuilder.Options);
    }
}
