using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mamey.ApplicationName.Modules.Notifications.Core.EF
{
    internal sealed class NotificationsDbContext : DbContext
    {
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<User> Users { get; set; }

        public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("notifications");
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}