using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Operations.Api")]
[assembly: InternalsVisibleTo("Pupitre.Operations.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Operations.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Operations.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Operations.Tests.Unit")]
namespace Pupitre.Operations.Application
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

