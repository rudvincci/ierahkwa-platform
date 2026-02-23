using Mamey.Government.Identity.Application.Commands.Handlers;
using Mamey.Logging.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Identity.Infrastructure.Logging;

internal static class Extensions
{
    public static IMameyBuilder AddHandlersLogging(this IMameyBuilder builder)
    {
        var assembly = typeof(CreateUserHandler).Assembly;
        
        builder.Services.AddSingleton<IMessageToLogTemplateMapper>(new MessageToLogTemplateMapper());
        
        return builder
            .AddCommandHandlersLogging(assembly)
            .AddEventHandlersLogging(assembly);
    }
}
