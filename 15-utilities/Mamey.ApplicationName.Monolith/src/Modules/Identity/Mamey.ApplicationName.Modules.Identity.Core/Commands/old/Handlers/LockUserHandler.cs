// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class LockUserHandler : ICommandHandler<LockUser>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public LockUserHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(LockUser command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new UserLockedRejected(command.UserId, "User not found"), cancellationToken);
//             return;
//         }
//
//         user.LockoutEnd = command.LockoutEnd;
//         var result = await _userManager.UpdateAsync(user);
//         if (!result.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new UserLockedRejected(command.UserId, string.Join(",", result.Errors)), cancellationToken);
//             return;
//         }
//         // TODO: get time from config
//         await _userManager.UpdateAsync(user);
//         // await _userManager.CacheUserAsync(user, TimeSpan.FromMinutes(30));
//         await _messageBroker.PublishAsync(new UserLocked(user.Id, user.LockoutEnd), cancellationToken);
//     }
// }