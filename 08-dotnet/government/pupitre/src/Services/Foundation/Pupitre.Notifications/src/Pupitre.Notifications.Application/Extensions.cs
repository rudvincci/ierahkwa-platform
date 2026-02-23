using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Notifications.Api")]
[assembly: InternalsVisibleTo("Pupitre.Notifications.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Notifications.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Notifications.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Notifications.Tests.Unit")]
namespace Pupitre.Notifications.Application
{
    internal static class Extensions
    {
        internal static IMameyBuilder AddApplication(this IMameyBuilder builder)
           => builder
               .AddCommandHandlers()
               .AddEventHandlers()
               .AddInMemoryCommandDispatcher()
               .AddInMemoryEventDispatcher();
    }
}

