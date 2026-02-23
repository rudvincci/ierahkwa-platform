using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Aftercare.Api")]
[assembly: InternalsVisibleTo("Pupitre.Aftercare.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Aftercare.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Aftercare.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Aftercare.Tests.Unit")]
namespace Pupitre.Aftercare.Application
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

