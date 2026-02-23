using Mamey.CQRS.Commands;

namespace Mamey.Policies;

public interface IPolicyDispatcher
{
    Task EvaluateAsync<T>(T command, CancellationToken cancellationToken = default) where T : class, ICommand;
}
