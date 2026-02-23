using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Accessibility.Api")]
[assembly: InternalsVisibleTo("Pupitre.Accessibility.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Accessibility.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Accessibility.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Accessibility.Tests.Unit")]
namespace Pupitre.Accessibility.Application
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

