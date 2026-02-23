using Microsoft.Extensions.DependencyInjection;
using Mamey.CQRS.Commands;

namespace Mamey.Policies;

internal sealed class PolicyEvaluatorDispatcher : IPolicyDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public PolicyEvaluatorDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task EvaluateAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(IPolicyHandler<>).MakeGenericType(commandType);
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        
        await handler.HandleAsync((dynamic)command, cancellationToken);
    }

    Task IPolicyDispatcher.EvaluateAsync<T>(T command, CancellationToken cancellationToken)
    {
        var commandType = command.GetType();
        var handlerType = typeof(IPolicyHandler<>).MakeGenericType(commandType);
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);

        return handler.HandleAsync((dynamic)command, cancellationToken);
    }
}
