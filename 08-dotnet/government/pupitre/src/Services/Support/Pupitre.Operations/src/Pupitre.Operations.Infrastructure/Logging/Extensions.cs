using Mamey;
using Mamey.Logging.CQRS;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Operations.Application.Commands;
using Pupitre.Operations.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal static class Extensions
{
    public static IMameyBuilder AddHandlersLogging(this IMameyBuilder builder)
    {
        var assembly = typeof(AddOperationMetric).Assembly;
        
        builder.Services.AddSingleton<IMessageToLogTemplateMapper>(new MessageToLogTemplateMapper());
        
        return builder
            .AddCommandHandlersLogging(assembly)
            .AddEventHandlersLogging(assembly);
    }
}
