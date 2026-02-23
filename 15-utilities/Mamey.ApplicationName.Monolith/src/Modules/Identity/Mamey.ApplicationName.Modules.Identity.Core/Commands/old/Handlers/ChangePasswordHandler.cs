// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Entities;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class ChangePasswordHandler : ICommandHandler<ChangePassword>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public ChangePasswordHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(ChangePassword command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new PasswordChangeRejected(command.UserId, "User not found"), cancellationToken);
//             return;
//         }
//         var result = await _userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
//         if (!result.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new PasswordChangeRejected(command.UserId, "Invalid current password or new password"), cancellationToken);
//             return;
//         }
//         await _messageBroker.PublishAsync(new PasswordChanged(user.Id), cancellationToken);
//     }
// }