using Mamey.Postgres;

namespace Pupitre.Compliance.Infrastructure.EF;

internal class ComplianceRecordUnitOfWork : PostgresUnitOfWork<ComplianceRecordDbContext>, IComplianceRecordUnitOfWork
{
    public ComplianceRecordUnitOfWork(ComplianceRecordDbContext dbContext) : base(dbContext)
    {
    }
}
