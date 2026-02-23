using Mamey;
using Mamey.Logging.CQRS;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Rewards.Application.Commands;
using Pupitre.Rewards.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal static class Extensions
{
    public static IMameyBuilder AddHandlersLogging(this IMameyBuilder builder)
    {
        var assembly = typeof(AddReward).Assembly;
        
        builder.Services.AddSingleton<IMessageToLogTemplateMapper>(new MessageToLogTemplateMapper());
        
        return builder
            .AddCommandHandlersLogging(assembly)
            .AddEventHandlersLogging(assembly);
    }
}
