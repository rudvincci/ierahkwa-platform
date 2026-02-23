using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Bookstore.Api")]
[assembly: InternalsVisibleTo("Pupitre.Bookstore.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Bookstore.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Bookstore.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Bookstore.Tests.Unit")]
namespace Pupitre.Bookstore.Application
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

