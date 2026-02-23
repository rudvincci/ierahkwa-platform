using Mamey.Postgres;

namespace Pupitre.AIContent.Infrastructure.EF;

internal class ContentGenerationUnitOfWork : PostgresUnitOfWork<ContentGenerationDbContext>, IContentGenerationUnitOfWork
{
    public ContentGenerationUnitOfWork(ContentGenerationDbContext dbContext) : base(dbContext)
    {
    }
}
