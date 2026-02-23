using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Mamey.ServiceName.Api")]
[assembly: InternalsVisibleTo("Mamey.ServiceName.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.ServiceName.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Mamey.ServiceName.Tests.Integration")]
[assembly: InternalsVisibleTo("Mamey.ServiceName.Tests.Unit")]
namespace Mamey.ServiceName.Application
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

