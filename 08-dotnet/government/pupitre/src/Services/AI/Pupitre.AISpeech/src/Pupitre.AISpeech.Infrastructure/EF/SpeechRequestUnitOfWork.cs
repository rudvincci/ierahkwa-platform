using Mamey.Postgres;

namespace Pupitre.AISpeech.Infrastructure.EF;

internal class SpeechRequestUnitOfWork : PostgresUnitOfWork<SpeechRequestDbContext>, ISpeechRequestUnitOfWork
{
    public SpeechRequestUnitOfWork(SpeechRequestDbContext dbContext) : base(dbContext)
    {
    }
}
