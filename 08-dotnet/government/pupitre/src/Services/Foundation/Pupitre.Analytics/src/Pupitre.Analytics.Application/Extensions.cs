using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Analytics.Api")]
[assembly: InternalsVisibleTo("Pupitre.Analytics.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Analytics.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Analytics.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Analytics.Tests.Unit")]
namespace Pupitre.Analytics.Application
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

