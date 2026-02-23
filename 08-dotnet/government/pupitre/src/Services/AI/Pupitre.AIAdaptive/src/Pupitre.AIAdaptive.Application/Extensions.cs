using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.AIAdaptive.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIAdaptive.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.AIAdaptive.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.AIAdaptive.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.AIAdaptive.Tests.Unit")]
namespace Pupitre.AIAdaptive.Application
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

