using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.CQRS.Queries;

[assembly: InternalsVisibleTo("Mamey.FWID.Notifications.Api")]
[assembly: InternalsVisibleTo("Mamey.FWID.Notifications.Infrastructure")]
namespace Mamey.FWID.Notifications.Application;

internal static class Extensions
{
    internal static IMameyBuilder AddApplication(this IMameyBuilder builder)
    {
        // Services are already registered in Infrastructure.Extensions.AddInfrastructure()
        // before AddMicroserviceSharedInfrastructure() calls AddQueryHandlers()
        return builder
            .AddCommandHandlers()
            .AddEventHandlers()
            .AddQueryHandlers()
            .AddInMemoryCommandDispatcher()
            .AddInMemoryEventDispatcher()
            .AddInMemoryQueryDispatcher();
    }
}







