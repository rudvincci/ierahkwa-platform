namespace Mamey.Identity.Redis.Configuration;

public class RedisTokenCacheOptions
{
    public string KeyPrefix { get; set; } = "mamey:identity";
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan DefaultBlacklistExpiration { get; set; } = TimeSpan.FromDays(7);
    public string ConnectionString { get; set; } = "localhost:6379";
    public int Database { get; set; } = 0;
    public bool AbortOnConnectFail { get; set; } = false;
    public int ConnectTimeout { get; set; } = 5000;
    public int SyncTimeout { get; set; } = 5000;
    public int AsyncTimeout { get; set; } = 5000;
    public bool AllowAdmin { get; set; } = false;
    public string? Password { get; set; }
    public string? ClientName { get; set; }
    public bool Ssl { get; set; } = false;
    public string? SslHost { get; set; }
    public bool SslCheckCertificateRevocation { get; set; } = true;
}
