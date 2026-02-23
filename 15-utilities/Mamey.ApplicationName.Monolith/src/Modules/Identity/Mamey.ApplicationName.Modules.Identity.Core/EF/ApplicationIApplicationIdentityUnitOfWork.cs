using Mamey.Postgres;

namespace Mamey.ApplicationName.Modules.Identity.Core.EF
{
    internal class ApplicationIdentityUnitOfWork : PostgresUnitOfWork<ApplicationIdentityDbContext>, IApplicationIdentityUnitOfWork
    {
        public ApplicationIdentityUnitOfWork(ApplicationIdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}