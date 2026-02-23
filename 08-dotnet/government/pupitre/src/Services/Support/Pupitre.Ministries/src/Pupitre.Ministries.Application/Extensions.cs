using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Ministries.Api")]
[assembly: InternalsVisibleTo("Pupitre.Ministries.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Ministries.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Ministries.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Ministries.Tests.Unit")]
namespace Pupitre.Ministries.Application
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

