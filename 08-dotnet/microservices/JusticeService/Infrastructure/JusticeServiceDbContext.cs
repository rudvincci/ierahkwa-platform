using Microsoft.EntityFrameworkCore;
using Ierahkwa.JusticeService.Domain;

namespace Ierahkwa.JusticeService.Infrastructure;

public class JusticeServiceDbContext : DbContext
{
    public JusticeServiceDbContext(DbContextOptions<JusticeServiceDbContext> options) : base(options) { }

    public DbSet<Case> Cases => Set<Case>();
    public DbSet<Hearing> Hearings => Set<Hearing>();
    public DbSet<Sentence> Sentences => Set<Sentence>();
    public DbSet<Inmate> Inmates => Set<Inmate>();
    public DbSet<LegalDocument> LegalDocuments => Set<LegalDocument>();
    public DbSet<Judge> Judges => Set<Judge>();
}
