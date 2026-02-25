using DevToolsService.Domain;
using Microsoft.EntityFrameworkCore;

namespace DevToolsService.Infrastructure;

public class DevToolsServiceDbContext : DbContext
{
    public DevToolsServiceDbContext(DbContextOptions<DevToolsServiceDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Repository> Repositories => Set<Repository>();
    public DbSet<CodeSnippet> CodeSnippets => Set<CodeSnippet>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<BuildPipeline> BuildPipelines => Set<BuildPipeline>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Project>().HasKey(e => e.Id);
        modelBuilder.Entity<Repository>().HasKey(e => e.Id);
        modelBuilder.Entity<CodeSnippet>().HasKey(e => e.Id);
        modelBuilder.Entity<Template>().HasKey(e => e.Id);
        modelBuilder.Entity<BuildPipeline>().HasKey(e => e.Id);
    }
}
