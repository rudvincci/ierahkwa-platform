using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Infrastructure.PostgreSQL.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Notifications.Infrastructure.PostgreSQL;

/// <summary>
/// Entity Framework Core database context for Notifications.
/// </summary>
internal class NotificationDbContext : DbContext
{
    public DbSet<Notification> Notifications { get; set; }

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}







