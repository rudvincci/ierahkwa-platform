using Microsoft.EntityFrameworkCore;
using Ierahkwa.CyberSecService.Domain;

namespace Ierahkwa.CyberSecService.Infrastructure;

public class CyberSecServiceDbContext : DbContext
{
    public CyberSecServiceDbContext(DbContextOptions<CyberSecServiceDbContext> options) : base(options) { }

    public DbSet<ThreatAlert> ThreatAlerts => Set<ThreatAlert>();
    public DbSet<SecurityIncident> SecurityIncidents => Set<SecurityIncident>();
    public DbSet<CryptoKey> CryptoKeys => Set<CryptoKey>();
    public DbSet<Vulnerability> Vulnerabilities => Set<Vulnerability>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
}
