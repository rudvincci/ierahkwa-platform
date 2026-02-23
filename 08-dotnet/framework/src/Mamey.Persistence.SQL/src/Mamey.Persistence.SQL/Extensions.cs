using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Persistence.SQL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Persistence.SQL;

public static class Extensions
{
    public static IMameyBuilder AddMameyPersistence(this IMameyBuilder builder)
    {
        builder.Services.AddScoped(typeof(IEFRepository<,>), typeof(EFRepository<,>));
        return builder;
    }
    public static IServiceCollection AddTransactionalDecorators(this IServiceCollection services)
    {
        services.TryDecorate(typeof(ICommandHandler<>), typeof(TransactionalCommandHandlerDecorator<>));
        services.TryDecorate(typeof(IEventHandler<>), typeof(TransactionalEventHandlerDecorator<>));
        
        return services;
    }
}