using Mamey.Postgres;

namespace Pupitre.Operations.Infrastructure.EF;

internal class OperationMetricUnitOfWork : PostgresUnitOfWork<OperationMetricDbContext>, IOperationMetricUnitOfWork
{
    public OperationMetricUnitOfWork(OperationMetricDbContext dbContext) : base(dbContext)
    {
    }
}
