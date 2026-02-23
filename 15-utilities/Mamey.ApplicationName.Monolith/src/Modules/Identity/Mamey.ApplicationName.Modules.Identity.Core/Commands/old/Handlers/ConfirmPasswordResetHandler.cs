// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class ConfirmPasswordResetHandler : ICommandHandler<ConfirmPasswordReset>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public ConfirmPasswordResetHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(ConfirmPasswordReset command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new PasswordResetConfirmationRejected(command.UserId, "User not found"), cancellationToken);
//             return;
//         }
//
//         var result = await _userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);
//         if (!result.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new PasswordResetConfirmationRejected(command.UserId, "Invalid token or password"), cancellationToken);
//             return;
//         }
//
//         await _messageBroker.PublishAsync(new PasswordResetConfirmed(user.Id), cancellationToken);
//     }
// }