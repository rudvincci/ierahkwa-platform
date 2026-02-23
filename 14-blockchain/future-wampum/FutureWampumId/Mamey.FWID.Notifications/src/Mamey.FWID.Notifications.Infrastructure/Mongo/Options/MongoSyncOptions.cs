namespace Mamey.FWID.Notifications.Infrastructure.Mongo.Options;

public class MongoSyncOptions
{
    public bool Enabled { get; set; } = true;
    public TimeSpan SyncInterval { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(1);
}







