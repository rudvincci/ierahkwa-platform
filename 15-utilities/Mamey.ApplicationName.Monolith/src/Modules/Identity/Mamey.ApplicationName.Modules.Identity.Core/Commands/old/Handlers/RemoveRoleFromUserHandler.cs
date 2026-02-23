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
// internal sealed class RemoveRoleFromUserHandler : ICommandHandler<RemoveRoleFromUser>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public RemoveRoleFromUserHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker, ICache cache)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(RemoveRoleFromUser command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new RoleRemovalRejected(command.UserId, command.RoleName, "User not found"), cancellationToken);
//             return;
//         }
//
//         var result = await _userManager.RemoveFromRoleAsync(user, command.RoleName);
//         if (!result.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new RoleRemovalRejected(command.UserId, command.RoleName, string.Join(",", result.Errors)), cancellationToken);
//             return;
//         }
//
//         await _userManager.UpdateAsync(user);
//         // await _userManager.CacheUserAsync(user, TimeSpan.FromMinutes(30));
//         await _messageBroker.PublishAsync(new RoleRemovedFromUser(user.Id, command.RoleName), cancellationToken);
//     }
// }