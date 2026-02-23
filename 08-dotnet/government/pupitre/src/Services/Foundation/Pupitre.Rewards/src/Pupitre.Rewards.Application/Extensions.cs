using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Rewards.Api")]
[assembly: InternalsVisibleTo("Pupitre.Rewards.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Rewards.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Rewards.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Rewards.Tests.Unit")]
namespace Pupitre.Rewards.Application
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

