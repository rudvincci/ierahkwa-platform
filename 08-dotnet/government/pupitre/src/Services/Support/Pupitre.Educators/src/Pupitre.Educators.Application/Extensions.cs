using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Educators.Api")]
[assembly: InternalsVisibleTo("Pupitre.Educators.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Educators.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Educators.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Educators.Tests.Unit")]
namespace Pupitre.Educators.Application
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

