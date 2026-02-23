using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.AIContent.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIContent.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.AIContent.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.AIContent.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.AIContent.Tests.Unit")]
namespace Pupitre.AIContent.Application
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

