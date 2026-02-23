// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Entities;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class ConfirmEmailHandler : ICommandHandler<ConfirmEmail>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public ConfirmEmailHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(ConfirmEmail command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new UserEmailConfirmationRejected(command.UserId, "User not found"), cancellationToken);
//             return;
//         }
//         
//         var result = await _userManager.ConfirmEmailAsync(user, Uri.UnescapeDataString(command.Token));
//         if (!result.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new UserEmailConfirmationRejected(command.UserId, string.Join(",", result.Errors)), cancellationToken);
//             return;
//         }
//         user.EmailConfirmed = true;
//         await _userManager.UpdateAsync(user);
//         
//         // await _userManager.CacheUserAsync(user);
//         
//         await _messageBroker.PublishAsync(new UserEmailConfirmed(user.Id), cancellationToken);
//     }
// }