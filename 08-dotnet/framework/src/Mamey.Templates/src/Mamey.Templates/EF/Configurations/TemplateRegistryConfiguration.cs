using Mamey.Templates.Authorization;
using Mamey.Templates.Registries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Templates.EF.Configurations;

internal sealed class TemplateRegistryConfigurationTemplateRegistryCfg : IEntityTypeConfiguration<DocumentTemplate>
{
    public void Configure(EntityTypeBuilder<DocumentTemplate> e)
    {
        e.ToTable("template_registry");
        e.HasKey(x => new { x.Id, x.Version });

        e.Property(x => x.Id)
            .HasConversion(c=> c.Value, c=> new TemplateId(c))
            .IsRequired();
        e.Property(x => x.Version).IsRequired();
        e.Property(x => x.Status).IsRequired();
        e.Property(x => x.StorageRef).IsRequired();
        e.Property(x => x.ContentType).IsRequired();
        e.Property(x => x.Sha256).IsRequired().HasMaxLength(64);
        e.Property(x => x.Size).IsRequired();

        e.HasIndex(x => new { x.Id, x.Status });
    }
}
internal sealed class TemplatePolicyConfiguration : IEntityTypeConfiguration<TemplatePolicy>
{
    public void Configure(EntityTypeBuilder<TemplatePolicy> e)
    {
        e.ToTable("template_policies");
        e.HasKey(x => new { x.TemplateId, x.Version, x.PrincipalType, x.Principal, x.ClaimValue });

        e.Property(x => x.Effect).IsRequired();
        e.Property(x => x.PrincipalType).IsRequired();
        e.Property(x => x.Principal).IsRequired();

        e.HasIndex(x => new { x.TemplateId, x.Version });
    }
}