// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class EnableSmsTwoFactorHandler : ICommandHandler<EnableSmsTwoFactor>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public EnableSmsTwoFactorHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(EnableSmsTwoFactor command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new SmsTwoFactorEnableRejected(command.UserId, "User not found"), cancellationToken);
//             return;
//         }
//         // Set phone number
//         var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, command.PhoneNumber);
//         if (!setPhoneResult.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new SmsTwoFactorEnableRejected(command.UserId, "Failed to set phone number"), cancellationToken);
//             return;
//         }
//         // TODO: Get time from configuration file
//         await _userManager.UpdateAsync(user);
//         // await _userManager.CacheUserAsync(user, TimeSpan.FromMinutes(30));
//         
//         var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, command.PhoneNumber);
//         // TODO: Confirm the phone number by sending a code
//         await _messageBroker.PublishAsync(new SmsTwoFactorEnabled(user.Id, user.PhoneNumber));
//     }
// }