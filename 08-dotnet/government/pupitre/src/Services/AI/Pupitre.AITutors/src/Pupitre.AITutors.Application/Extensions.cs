using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.AITutors.Api")]
[assembly: InternalsVisibleTo("Pupitre.AITutors.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.AITutors.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.AITutors.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.AITutors.Tests.Unit")]
namespace Pupitre.AITutors.Application
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

