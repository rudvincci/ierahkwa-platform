using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.GLEs.Api")]
[assembly: InternalsVisibleTo("Pupitre.GLEs.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.GLEs.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.GLEs.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.GLEs.Tests.Unit")]
namespace Pupitre.GLEs.Application
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

