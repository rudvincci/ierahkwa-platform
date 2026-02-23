using Mamey;
using Mamey.Logging.CQRS;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AITutors.Application.Commands;
using Pupitre.AITutors.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal static class Extensions
{
    public static IMameyBuilder AddHandlersLogging(this IMameyBuilder builder)
    {
        var assembly = typeof(AddTutor).Assembly;
        
        builder.Services.AddSingleton<IMessageToLogTemplateMapper>(new MessageToLogTemplateMapper());
        
        return builder
            .AddCommandHandlersLogging(assembly)
            .AddEventHandlersLogging(assembly);
    }
}
