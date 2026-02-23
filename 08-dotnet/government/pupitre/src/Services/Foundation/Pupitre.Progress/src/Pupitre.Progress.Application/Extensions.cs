using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Progress.Api")]
[assembly: InternalsVisibleTo("Pupitre.Progress.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Progress.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Progress.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Progress.Tests.Unit")]
namespace Pupitre.Progress.Application
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

