using Mamey.Postgres;

namespace Pupitre.AITranslation.Infrastructure.EF;

internal class TranslationRequestUnitOfWork : PostgresUnitOfWork<TranslationRequestDbContext>, ITranslationRequestUnitOfWork
{
    public TranslationRequestUnitOfWork(TranslationRequestDbContext dbContext) : base(dbContext)
    {
    }
}
