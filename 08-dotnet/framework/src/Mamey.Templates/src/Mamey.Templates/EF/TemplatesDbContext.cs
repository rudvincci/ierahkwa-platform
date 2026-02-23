using System.Text.Json;
using Mamey.MessageBrokers.Outbox.Messages;
using Mamey.Persistence.SQL;
using Mamey.Templates.Authorization;
using Mamey.Templates.Registries;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Templates.EF;

public class TemplatesDbContext : DbContext
{
    public TemplatesDbContext(DbContextOptions<TemplatesDbContext> options)
        : base(options) { }

    public DbSet<InboxMessage> Inbox { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }
    public DbSet<DocumentTemplate> Templates => Set<DocumentTemplate>();
    public DbSet<TemplatePolicy> TemplatePolicies => Set<TemplatePolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<InboxMessage>().ToTable("inbox");
        modelBuilder.Entity<OutboxMessage>().ToTable("outbox");
        modelBuilder.Entity<OutboxMessage>()
            .Property(b => b.Headers)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonExtensions.SerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonExtensions.SerializerOptions)!
            )
            .Metadata.SetValueComparer(new DictionaryValueComparer());
            
        modelBuilder.Entity<OutboxMessage>()
            .Property(b => b.Message)
            .HasConversion<JsonValueConverter<object>>();
        modelBuilder.Entity<OutboxMessage>()
            .Property(b => b.MessageContext)
            .HasConversion<JsonValueConverter<object>>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemplatesDbContext).Assembly);
        modelBuilder.Ignore<UserId>();
        modelBuilder.UseSnakeCaseNamingConvention();
        modelBuilder.ApplyUtcDateTimeConverter();
    }



    
}
