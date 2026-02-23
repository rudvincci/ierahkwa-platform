// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Chronicle;
// using Mamey.CQRS.Events;
// using Mamey.Government.Modules.Passports.Core.Events;
// using Mamey.Government.Modules.Saga.Api.Messages.Citizenship;
// using Microsoft.Extensions.Logging;
//
// namespace Mamey.Government.Modules.Saga.Api.Integration;
//
// /// <summary>
// /// Handles events from the Passports module and forwards them to the saga.
// /// </summary>
// internal sealed class PassportsEventHandler :
//     IEventHandler<PassportIssuedEvent>
// {
//     private readonly ISagaCoordinator _sagaCoordinator;
//     private readonly ILogger<PassportsEventHandler> _logger;
//
//     public PassportsEventHandler(
//         ISagaCoordinator sagaCoordinator,
//         ILogger<PassportsEventHandler> logger)
//     {
//         _sagaCoordinator = sagaCoordinator;
//         _logger = logger;
//     }
//
//     public async Task HandleAsync(PassportIssuedEvent @event, CancellationToken cancellationToken = default)
//     {
//         // Only forward to saga if this is part of an application workflow
//         if (!@event.ApplicationId.HasValue || @event.ApplicationId == Guid.Empty)
//         {
//             return;
//         }
//         
//         _logger.LogInformation("Forwarding PassportIssued event to saga: {PassportId}, CitizenId: {CitizenId}", 
//             @event.PassportId, @event.CitizenId);
//         
//         var message = new PassportIssued(
//             @event.PassportId,
//             @event.CitizenId,
//             @event.ApplicationId.Value,
//             @event.TenantId,
//             @event.PassportNumber);
//
//         await _sagaCoordinator.ProcessAsync(message, SagaContext.Empty);
//     }
// }
