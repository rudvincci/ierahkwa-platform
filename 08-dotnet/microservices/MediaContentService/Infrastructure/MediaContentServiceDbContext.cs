using MediaContentService.Domain;
using Microsoft.EntityFrameworkCore;

namespace MediaContentService.Infrastructure;

public class MediaContentServiceDbContext : DbContext
{
    public MediaContentServiceDbContext(DbContextOptions<MediaContentServiceDbContext> options) : base(options) { }

    public DbSet<Film> Films => Set<Film>();
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
    public DbSet<ShortVideo> ShortVideos => Set<ShortVideo>();
    public DbSet<MediaChannel> MediaChannels => Set<MediaChannel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Film>().HasKey(e => e.Id);
        modelBuilder.Entity<Song>().HasKey(e => e.Id);
        modelBuilder.Entity<Playlist>().HasKey(e => e.Id);
        modelBuilder.Entity<NewsArticle>().HasKey(e => e.Id);
        modelBuilder.Entity<ShortVideo>().HasKey(e => e.Id);
        modelBuilder.Entity<MediaChannel>().HasKey(e => e.Id);
    }
}
