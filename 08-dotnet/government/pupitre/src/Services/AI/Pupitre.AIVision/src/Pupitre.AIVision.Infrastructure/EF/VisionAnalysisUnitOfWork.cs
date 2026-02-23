using Mamey.Postgres;

namespace Pupitre.AIVision.Infrastructure.EF;

internal class VisionAnalysisUnitOfWork : PostgresUnitOfWork<VisionAnalysisDbContext>, IVisionAnalysisUnitOfWork
{
    public VisionAnalysisUnitOfWork(VisionAnalysisDbContext dbContext) : base(dbContext)
    {
    }
}
