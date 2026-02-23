using System;
using System.Threading.Tasks;

namespace Mamey.MicroMonolith.Infrastructure.UnitOfWork;

public interface IUnitOfWork
{
    Task ExecuteAsync(Func<Task> action);
}