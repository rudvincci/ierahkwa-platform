// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class EnableAuthenticatorHandler : ICommandHandler<EnableAuthenticator>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public EnableAuthenticatorHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(EnableAuthenticator command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new EnableAuthenticatorRejected(command.UserId, "User not found"), cancellationToken);
//             return;
//         }
//         await _userManager.ResetAuthenticatorKeyAsync(user);
//         var key = await _userManager.GetAuthenticatorKeyAsync(user);
//         
//         if (string.IsNullOrEmpty(key))
//         {
//             await _messageBroker.PublishAsync(new EnableAuthenticatorRejected(command.UserId, "Failed to generate authenticator key"), cancellationToken);
//             return;
//         }
//         // Enable 2FA if not enabled
//         if (!await _userManager.GetTwoFactorEnabledAsync(user))
//         {
//             var set2faResult = await _userManager.SetTwoFactorEnabledAsync(user, true);
//             if (!set2faResult.Succeeded)
//             {
//                 await _messageBroker.PublishAsync(new EnableAuthenticatorRejected(command.UserId, string.Join(",", set2faResult.Errors)), cancellationToken);
//                 return;
//             }
//         }
//         await _messageBroker.PublishAsync(new AuthenticatorEnabled(user.Id, key), cancellationToken);
//     }
// }