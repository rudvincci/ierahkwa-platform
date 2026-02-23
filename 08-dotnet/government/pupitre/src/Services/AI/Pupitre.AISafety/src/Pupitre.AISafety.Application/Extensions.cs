using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.AISafety.Api")]
[assembly: InternalsVisibleTo("Pupitre.AISafety.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.AISafety.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.AISafety.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.AISafety.Tests.Unit")]
namespace Pupitre.AISafety.Application
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

