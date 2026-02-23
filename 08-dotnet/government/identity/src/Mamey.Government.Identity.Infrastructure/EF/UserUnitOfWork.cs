using Mamey.Postgres;

namespace Mamey.Government.Identity.Infrastructure.EF;

internal class UserUnitOfWork : PostgresUnitOfWork<UserDbContext>, IUserUnitOfWork
{
    public UserUnitOfWork(UserDbContext dbContext) : base(dbContext)
    {
    }
}
