using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.AISpeech.Api")]
[assembly: InternalsVisibleTo("Pupitre.AISpeech.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.AISpeech.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.AISpeech.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.AISpeech.Tests.Unit")]
namespace Pupitre.AISpeech.Application
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

