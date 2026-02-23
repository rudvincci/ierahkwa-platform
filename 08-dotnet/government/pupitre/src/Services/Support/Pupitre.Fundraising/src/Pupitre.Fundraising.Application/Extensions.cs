using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Fundraising.Api")]
[assembly: InternalsVisibleTo("Pupitre.Fundraising.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Fundraising.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Fundraising.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Fundraising.Tests.Unit")]
namespace Pupitre.Fundraising.Application
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

