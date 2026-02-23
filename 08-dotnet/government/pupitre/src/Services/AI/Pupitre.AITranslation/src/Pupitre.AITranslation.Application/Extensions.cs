using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.AITranslation.Api")]
[assembly: InternalsVisibleTo("Pupitre.AITranslation.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.AITranslation.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.AITranslation.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.AITranslation.Tests.Unit")]
namespace Pupitre.AITranslation.Application
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

