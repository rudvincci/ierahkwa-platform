using Microsoft.EntityFrameworkCore;
using Ierahkwa.GenomicsService.Domain;

namespace Ierahkwa.GenomicsService.Infrastructure;

public class GenomicsServiceDbContext : DbContext
{
    public GenomicsServiceDbContext(DbContextOptions<GenomicsServiceDbContext> options) : base(options) { }

    public DbSet<GenomeSample> GenomeSamples => Set<GenomeSample>();
    public DbSet<SequenceAnalysis> SequenceAnalysiss => Set<SequenceAnalysis>();
    public DbSet<BiobankEntry> BiobankEntrys => Set<BiobankEntry>();
    public DbSet<ResearchProject> ResearchProjects => Set<ResearchProject>();
    public DbSet<Consent> Consents => Set<Consent>();}
