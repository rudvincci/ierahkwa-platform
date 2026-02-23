using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Curricula.Api")]
[assembly: InternalsVisibleTo("Pupitre.Curricula.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Curricula.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Curricula.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Curricula.Tests.Unit")]
namespace Pupitre.Curricula.Application
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

