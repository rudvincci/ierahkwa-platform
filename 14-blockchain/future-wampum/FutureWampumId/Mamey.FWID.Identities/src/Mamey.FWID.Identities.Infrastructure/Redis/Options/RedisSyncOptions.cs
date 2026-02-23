namespace Mamey.FWID.Identities.Infrastructure.Redis.Options;

public class RedisSyncOptions
{
    public bool Enabled { get; set; } = true;
    public TimeSpan SyncInterval { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan CacheTimeToLive { get; set; } = TimeSpan.FromHours(24);
}



