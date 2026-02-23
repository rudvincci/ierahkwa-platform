using System;
using System.Runtime.CompilerServices;
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Application.Services;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Api")]
[assembly: InternalsVisibleTo("Mamey.Government.Identity.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.EndToEnd.Sync")]
[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration")]
[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Unit")]
namespace Mamey.Government.Identity.Application
{
    internal static class Extensions
    {
        internal static IMameyBuilder AddApplication(this IMameyBuilder builder)
        {
            // Services are already registered in Infrastructure.Extensions.AddInfrastructure()
            // before AddMicroserviceSharedInfrastructure() calls AddQueryHandlers()
            // Note: AddQueryHandlers() and AddInMemoryQueryDispatcher() are called by AddMicroserviceSharedInfrastructure()
            return builder
                .AddCommandHandlers()
                .AddEventHandlers()
                .AddInMemoryCommandDispatcher()
                .AddInMemoryEventDispatcher();
        }
    }
}

