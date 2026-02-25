using Microsoft.EntityFrameworkCore;
using Ierahkwa.AIEngineService.Domain;

namespace Ierahkwa.AIEngineService.Infrastructure;

public class AIEngineServiceDbContext : DbContext
{
    public AIEngineServiceDbContext(DbContextOptions<AIEngineServiceDbContext> options) : base(options) { }

    public DbSet<AIModel> AIModels => Set<AIModel>();
    public DbSet<TrainingJob> TrainingJobs => Set<TrainingJob>();
    public DbSet<InferenceRequest> InferenceRequests => Set<InferenceRequest>();
    public DbSet<Dataset> Datasets => Set<Dataset>();
    public DbSet<ModelVersion> ModelVersions => Set<ModelVersion>();}
