using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Mamey.MicroMonolith.Infrastructure.UnitOfWork;

public abstract class BaseUnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : DbContext
{
    protected readonly TDbContext DbContext;

    protected BaseUnitOfWork(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task ExecuteAsync(Func<Task> action)
    {
        using var transaction = await DbContext.Database.BeginTransactionAsync();
        try
        {
            await action();
            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}