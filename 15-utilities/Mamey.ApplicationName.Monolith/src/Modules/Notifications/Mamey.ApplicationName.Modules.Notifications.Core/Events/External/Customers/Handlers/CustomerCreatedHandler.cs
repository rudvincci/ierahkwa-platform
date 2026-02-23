// using Mamey.ApplicationName.Modules.Notifications.Core.Services;
// using Mamey.ApplicationName.Modules.Notifications.Core.Templates.Models;
// using Mamey.ApplicationName.Modules.Notifications.Core.Templates.Types;
// using Mamey.CQRS.Commands;
// using Mamey.CQRS.Events;
// using Microsoft.Extensions.Logging;
//
// namespace Mamey.ApplicationName.Modules.Notifications.Core.Events.External.Customers.Handlers;
//
// public class CustomerCreatedHandler : IEventHandler<CustomerCreated>
// {
//     private readonly ILogger<CustomerCreatedHandler> _logger;
//
//     private readonly INotificationService _notificationService;
//     private readonly IUserService _userService;
//     private readonly ICommandDispatcher _commandDispatcher;
//     public async Task HandleAsync(CustomerCreated @event, CancellationToken cancellationToken = default)
//     {
//         try
//         {
//             var user = await _userService.GetAsync(@event.CustomerId);
//             var emailModel = new WelcomeDto(user.Name, @event.VerificationToken);
//             await _notificationService.SendEmailUsingTemplate(user.Email,
//                 "Welcome to Futurehead",
//                 EmailTemplateType.Welcome,
//                 emailModel);
//
//             var notification = new AddNotification(@event.PersonId, "Welcome to FHBDET", "Complete registration",
//                 "Complete the verfication process", "Email");
//             // TODO: send Twilio message
//             await _commandDispatcher.SendAsync(notification);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex.Message);
//         }
//         // TODO: Set user notification
//     }
// }