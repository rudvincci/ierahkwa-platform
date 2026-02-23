// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Mamey.Persistence.Redis;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class VerifyAuthenicatorCodeHandler : ICommandHandler<VerifyAutheticatorCode>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public VerifyAuthenicatorCodeHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker, ICache cache)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(VerifyAutheticatorCode command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         if (user == null)
//         {
//             user = await _userManager.FindByIdAsync(command.UserId.ToString());
//             if (user == null)
//             {
//                 await _messageBroker.PublishAsync(new AuthenticatorCodeVerificationRejected(command.UserId, "User not found"), cancellationToken);
//                 return;
//             }
//         }
//         // The 'Authenticator' provider is the default for TOTP authenticator keys
//         var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Authenticator", command.Code);
//         if (!isValid)
//         {
//             await _messageBroker.PublishAsync(new AuthenticatorCodeVerificationRejected(command.UserId, "Invalid code"), cancellationToken);
//             return;
//         }
//         
//         // Once verified, you may consider the authenticator fully set up.
//         await _messageBroker.PublishAsync(new AuthenticatorCodeVerified(user.Id), cancellationToken);
//     }
// }