using Microsoft.EntityFrameworkCore;

namespace Mamey.Portal.Tenant.Infrastructure.Persistence;

public sealed class TenantDbContext : DbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }

    public DbSet<TenantRow> Tenants => Set<TenantRow>();
    public DbSet<TenantSettingsRow> TenantSettings => Set<TenantSettingsRow>();
    public DbSet<TenantDocumentNamingRow> DocumentNaming => Set<TenantDocumentNamingRow>();
    public DbSet<TenantUserMappingRow> UserMappings => Set<TenantUserMappingRow>();
    public DbSet<TenantUserInviteRow> UserInvites => Set<TenantUserInviteRow>();
    public DbSet<TenantDocumentTemplateRow> DocumentTemplates => Set<TenantDocumentTemplateRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenantRow>(b =>
        {
            b.ToTable("tenants");
            b.HasKey(x => x.TenantId);
            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.DisplayName).HasMaxLength(256).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<TenantSettingsRow>(b =>
        {
            b.ToTable("tenant_settings");
            b.HasKey(x => x.TenantId);
            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.BrandingJson).HasMaxLength(65535).IsRequired();
            b.Property(x => x.ActivationJson).HasMaxLength(65535);
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<TenantDocumentNamingRow>(b =>
        {
            b.ToTable("tenant_document_naming");
            b.HasKey(x => x.TenantId);
            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.PatternJson).HasMaxLength(65535).IsRequired();
            b.Property(x => x.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<TenantUserMappingRow>(b =>
        {
            b.ToTable("tenant_user_mappings");
            b.HasKey(x => new { x.Issuer, x.Subject });
            b.Property(x => x.Issuer).HasMaxLength(256).IsRequired();
            b.Property(x => x.Subject).HasMaxLength(256).IsRequired();
            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.Email).HasMaxLength(256);
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt).IsRequired();
            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.Email);
        });

        modelBuilder.Entity<TenantUserInviteRow>(b =>
        {
            b.ToTable("tenant_user_invites");
            b.HasKey(x => new { x.Issuer, x.Email });
            b.Property(x => x.Issuer).HasMaxLength(256).IsRequired();
            b.Property(x => x.Email).HasMaxLength(256).IsRequired();
            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt).IsRequired();
            b.Property(x => x.UsedAt);
            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.Email);
        });

        modelBuilder.Entity<TenantDocumentTemplateRow>(b =>
        {
            b.ToTable("tenant_document_templates");
            b.HasKey(x => new { x.TenantId, x.Kind });
            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.Kind).HasMaxLength(64).IsRequired();
            b.Property(x => x.TemplateHtml).HasMaxLength(65535).IsRequired();
            b.Property(x => x.UpdatedAt).IsRequired();
            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.Kind);
        });
    }
}

