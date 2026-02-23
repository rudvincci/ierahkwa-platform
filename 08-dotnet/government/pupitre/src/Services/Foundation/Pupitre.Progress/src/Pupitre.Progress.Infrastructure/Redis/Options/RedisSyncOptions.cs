namespace Pupitre.Progress.Infrastructure.Redis.Options;

public class RedisSyncOptions
{
    public const string SectionName = "Redis:Sync";
    public bool Enabled { get; set; } = true;
    public int SyncIntervalMs { get; set; } = 60000;
    public int BatchSize { get; set; } = 100;
}
