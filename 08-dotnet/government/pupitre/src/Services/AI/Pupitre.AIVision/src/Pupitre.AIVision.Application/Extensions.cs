using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.AIVision.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIVision.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.AIVision.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.AIVision.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.AIVision.Tests.Unit")]
namespace Pupitre.AIVision.Application
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

