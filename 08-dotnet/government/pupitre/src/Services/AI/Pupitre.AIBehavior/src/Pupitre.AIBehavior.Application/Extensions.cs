using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.AIBehavior.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIBehavior.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.AIBehavior.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.AIBehavior.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.AIBehavior.Tests.Unit")]
namespace Pupitre.AIBehavior.Application
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

