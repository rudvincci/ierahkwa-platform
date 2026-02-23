namespace Mamey.Persistence.SQL;

public interface IUnitOfWork
{
    Task ExecuteAsync(Func<Task> action);
}
    