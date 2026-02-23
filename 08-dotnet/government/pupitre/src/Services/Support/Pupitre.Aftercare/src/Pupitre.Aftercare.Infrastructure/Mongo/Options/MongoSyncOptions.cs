namespace Pupitre.Aftercare.Infrastructure.Mongo.Options;

public class MongoSyncOptions
{
    public const string SectionName = "Mongo:Sync";
    public bool Enabled { get; set; } = true;
    public int SyncIntervalMs { get; set; } = 60000;
    public int BatchSize { get; set; } = 100;
}
