using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.AIRecommendations.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIRecommendations.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.AIRecommendations.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.AIRecommendations.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.AIRecommendations.Tests.Unit")]
namespace Pupitre.AIRecommendations.Application
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

