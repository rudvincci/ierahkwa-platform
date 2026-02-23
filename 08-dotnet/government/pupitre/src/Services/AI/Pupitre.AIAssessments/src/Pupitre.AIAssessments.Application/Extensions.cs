using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;

[assembly: InternalsVisibleTo("Pupitre.AIAssessments.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIAssessments.Infrastructure")]
[assembly: InternalsVisibleTo("Pupitre.AIAssessments.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Pupitre.AIAssessments.Tests.Integration")]
[assembly: InternalsVisibleTo("Pupitre.AIAssessments.Tests.Unit")]
namespace Pupitre.AIAssessments.Application
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

