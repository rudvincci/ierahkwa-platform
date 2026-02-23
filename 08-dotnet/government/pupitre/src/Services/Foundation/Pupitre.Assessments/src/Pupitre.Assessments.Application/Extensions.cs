using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.Assessments.Api")]
[assembly: InternalsVisibleTo("Pupitre.Assessments.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.Assessments.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.Assessments.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.Assessments.Tests.Unit")]
namespace Pupitre.Assessments.Application
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

