using Mamey;
using Mamey.Logging.CQRS;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Lessons.Application.Commands;
using Pupitre.Lessons.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal static class Extensions
{
    public static IMameyBuilder AddHandlersLogging(this IMameyBuilder builder)
    {
        var assembly = typeof(AddLesson).Assembly;
        
        builder.Services.AddSingleton<IMessageToLogTemplateMapper>(new MessageToLogTemplateMapper());
        
        return builder
            .AddCommandHandlersLogging(assembly)
            .AddEventHandlersLogging(assembly);
    }
}
