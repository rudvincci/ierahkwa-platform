using Microsoft.EntityFrameworkCore;
using Ierahkwa.BrowserService.Domain;

namespace Ierahkwa.BrowserService.Infrastructure;

public class BrowserServiceDbContext : DbContext
{
    public BrowserServiceDbContext(DbContextOptions<BrowserServiceDbContext> options) : base(options) { }

    public DbSet<BrowsingSession> BrowsingSessions => Set<BrowsingSession>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<ProxyRoute> ProxyRoutes => Set<ProxyRoute>();
    public DbSet<ContentFilter> ContentFilters => Set<ContentFilter>();
    public DbSet<BrowserExtension> BrowserExtensions => Set<BrowserExtension>();}
