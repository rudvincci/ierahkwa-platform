using LowCodeDesignService.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowCodeDesignService.Infrastructure;

public class LowCodeDesignServiceDbContext : DbContext
{
    public LowCodeDesignServiceDbContext(DbContextOptions<LowCodeDesignServiceDbContext> options) : base(options) { }

    public DbSet<AppDefinition> AppDefinitions => Set<AppDefinition>();
    public DbSet<UIComponent> UIComponents => Set<UIComponent>();
    public DbSet<DesignAsset> DesignAssets => Set<DesignAsset>();
    public DbSet<FormSchema> FormSchemas => Set<FormSchema>();
    public DbSet<PageLayout> PageLayouts => Set<PageLayout>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AppDefinition>().HasKey(e => e.Id);
        modelBuilder.Entity<UIComponent>().HasKey(e => e.Id);
        modelBuilder.Entity<DesignAsset>().HasKey(e => e.Id);
        modelBuilder.Entity<FormSchema>().HasKey(e => e.Id);
        modelBuilder.Entity<PageLayout>().HasKey(e => e.Id);
    }
}
