using Mamey.Postgres;

namespace Pupitre.Bookstore.Infrastructure.EF;

internal class BookUnitOfWork : PostgresUnitOfWork<BookDbContext>, IBookUnitOfWork
{
    public BookUnitOfWork(BookDbContext dbContext) : base(dbContext)
    {
    }
}
