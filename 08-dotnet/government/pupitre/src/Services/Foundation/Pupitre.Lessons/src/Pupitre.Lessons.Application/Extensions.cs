using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Lessons.Api")]
[assembly: InternalsVisibleTo("Pupitre.Lessons.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Lessons.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Lessons.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Lessons.Tests.Unit")]
namespace Pupitre.Lessons.Application
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

