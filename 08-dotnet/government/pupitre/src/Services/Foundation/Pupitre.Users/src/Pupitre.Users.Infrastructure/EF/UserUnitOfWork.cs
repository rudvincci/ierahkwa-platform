using Mamey.Postgres;

namespace Pupitre.Users.Infrastructure.EF;

internal class UserUnitOfWork : PostgresUnitOfWork<UserDbContext>, IUserUnitOfWork
{
    public UserUnitOfWork(UserDbContext dbContext) : base(dbContext)
    {
    }
}
