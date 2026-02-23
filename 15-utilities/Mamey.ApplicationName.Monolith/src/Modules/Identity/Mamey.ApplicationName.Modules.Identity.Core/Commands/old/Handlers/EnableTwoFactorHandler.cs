// // using Mamey.ApplicationName.Modules.Identity.Core.EF.Managers;
// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class EnableTwoFactorHandler : ICommandHandler<EnableTwoFactor>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public EnableTwoFactorHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(EnableTwoFactor command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new EnableTwoFactorRejected(command.UserId, "User not found"), cancellationToken);
//             return;
//         }
//
//         await _userManager.SetTwoFactorEnabledAsync(user, true);
//         var result = await _userManager.UpdateAsync(user);
//         if(!result.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new EnableTwoFactorRejected(command.UserId, string.Join(",", result.Errors)), cancellationToken);
//             return;
//         }
//         await _userManager.UpdateAsync(user);
//         // await _userManager.CacheUserAsync(user, TimeSpan.FromMinutes(30));
//         await _messageBroker.PublishAsync(new TwoFactorEnabled(command.UserId), cancellationToken);
//     }
// }