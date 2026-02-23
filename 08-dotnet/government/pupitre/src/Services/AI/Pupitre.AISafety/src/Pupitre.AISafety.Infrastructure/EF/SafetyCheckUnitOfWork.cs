using Mamey.Postgres;

namespace Pupitre.AISafety.Infrastructure.EF;

internal class SafetyCheckUnitOfWork : PostgresUnitOfWork<SafetyCheckDbContext>, ISafetyCheckUnitOfWork
{
    public SafetyCheckUnitOfWork(SafetyCheckDbContext dbContext) : base(dbContext)
    {
    }
}
