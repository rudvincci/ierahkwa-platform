using Microsoft.EntityFrameworkCore;
using Ierahkwa.ProductivityService.Domain;

namespace Ierahkwa.ProductivityService.Infrastructure;

public class ProductivityServiceDbContext : DbContext
{
    public ProductivityServiceDbContext(DbContextOptions<ProductivityServiceDbContext> options) : base(options) { }

    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<SharedDocument> SharedDocuments => Set<SharedDocument>();
    public DbSet<ProductivityTask> ProductivityTasks => Set<ProductivityTask>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();}
