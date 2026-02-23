using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Users.Api")]
[assembly: InternalsVisibleTo("Pupitre.Users.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Users.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Users.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Users.Tests.Unit")]
namespace Pupitre.Users.Application
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

