// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Chronicle;
// using Mamey.CQRS.Events;
// using Mamey.Government.Modules.Saga.Api.Messages.Citizenship;
// using Mamey.Government.Modules.TravelIdentities.Core.Events;
// using Microsoft.Extensions.Logging;
//
// namespace Mamey.Government.Modules.Saga.Api.Integration;
//
// /// <summary>
// /// Handles events from the TravelIdentities module and forwards them to the saga.
// /// </summary>
// internal sealed class TravelIdentitiesEventHandler :
//     IEventHandler<TravelIdentityIssuedEvent>
// {
//     private readonly ISagaCoordinator _sagaCoordinator;
//     private readonly ILogger<TravelIdentitiesEventHandler> _logger;
//
//     public TravelIdentitiesEventHandler(
//         ISagaCoordinator sagaCoordinator,
//         ILogger<TravelIdentitiesEventHandler> logger)
//     {
//         _sagaCoordinator = sagaCoordinator;
//         _logger = logger;
//     }
//
//     public async Task HandleAsync(TravelIdentityIssuedEvent @event, CancellationToken cancellationToken = default)
//     {
//         // Only forward to saga if this is part of an application workflow
//         if (!@event.ApplicationId.HasValue || @event.ApplicationId == Guid.Empty)
//         {
//             return;
//         }
//         
//         _logger.LogInformation("Forwarding TravelIdentityIssued event to saga: {TravelIdentityId}, CitizenId: {CitizenId}", 
//             @event.TravelIdentityId, @event.CitizenId);
//         
//         var message = new TravelIdentityIssued(
//             @event.TravelIdentityId,
//             @event.CitizenId,
//             @event.ApplicationId.Value,
//             @event.TenantId,
//             @event.TravelIdentityNumber);
//
//         await _sagaCoordinator.ProcessAsync(message, SagaContext.Empty);
//     }
// }
