// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class VerifySmsTwoFactorHandler : ICommandHandler<VerifySmsTwoFactor>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public VerifySmsTwoFactorHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(VerifySmsTwoFactor command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new SmsTwoFactorVerificationRejected(command.UserId, "User not found"), cancellationToken);
//             return;
//         }
//         var isValid = await _userManager.VerifyChangePhoneNumberTokenAsync(user, command.Code, command.PhoneNumber);
//         if (!isValid)
//         {
//             await _messageBroker.PublishAsync(new SmsTwoFactorVerificationRejected(command.UserId, "Invalid code"), cancellationToken);
//             return;
//         }
//         // Phone number is now verified, enable two-factor if not already
//         if (!await _userManager.GetTwoFactorEnabledAsync(user))
//         {
//             var twoFactorResult = await _userManager.SetTwoFactorEnabledAsync(user, true);
//             if (!twoFactorResult.Succeeded)
//             {
//                 await _messageBroker.PublishAsync(new SmsTwoFactorVerificationRejected(command.UserId, "Failed to enable two-factor"), cancellationToken);
//                 return;
//             }
//         }
//         await _userManager.UpdateAsync(user);
//         // await _userManager.CacheUserAsync(user, TimeSpan.FromMinutes(30));
//         await _messageBroker.PublishAsync(new SmsTwoFactorVerified(user.Id), cancellationToken);
//     }
// }