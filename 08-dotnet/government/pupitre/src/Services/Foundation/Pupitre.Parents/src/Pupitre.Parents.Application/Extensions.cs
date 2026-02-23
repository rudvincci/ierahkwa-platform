using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Parents.Api")]
[assembly: InternalsVisibleTo("Pupitre.Parents.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Parents.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Parents.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Parents.Tests.Unit")]
namespace Pupitre.Parents.Application
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

