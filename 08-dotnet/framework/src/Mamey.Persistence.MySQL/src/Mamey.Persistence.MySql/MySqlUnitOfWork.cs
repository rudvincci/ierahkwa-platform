using Mamey.Persistence.SQL;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Persistence.MySql;

public abstract class MySqlUnitOfWork<T> : IUnitOfWork where T : DbContext
{
    private readonly T _dbContext;

    protected MySqlUnitOfWork(T dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

