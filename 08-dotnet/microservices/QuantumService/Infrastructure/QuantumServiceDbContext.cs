using Microsoft.EntityFrameworkCore;
using Ierahkwa.QuantumService.Domain;

namespace Ierahkwa.QuantumService.Infrastructure;

public class QuantumServiceDbContext : DbContext
{
    public QuantumServiceDbContext(DbContextOptions<QuantumServiceDbContext> options) : base(options) { }

    public DbSet<QuantumCircuit> QuantumCircuits => Set<QuantumCircuit>();
    public DbSet<QubitAllocation> QubitAllocations => Set<QubitAllocation>();
    public DbSet<HomomorphicTask> HomomorphicTasks => Set<HomomorphicTask>();
    public DbSet<KeyDistribution> KeyDistributions => Set<KeyDistribution>();
    public DbSet<QuantumJob> QuantumJobs => Set<QuantumJob>();}
