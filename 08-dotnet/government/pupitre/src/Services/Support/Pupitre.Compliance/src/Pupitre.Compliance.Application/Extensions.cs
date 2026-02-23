using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Compliance.Api")]
[assembly: InternalsVisibleTo("Pupitre.Compliance.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Compliance.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Compliance.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Compliance.Tests.Unit")]
namespace Pupitre.Compliance.Application
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

