// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Entities;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class AddRoleToUserHandler : ICommandHandler<AddRoleToUser>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//
//     public AddRoleToUserHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//     }
//
//     public async Task HandleAsync(AddRoleToUser command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new RoleAdditionRejected(command.UserId, command.RoleName, "User not found"), cancellationToken);
//             return;
//         }
//
//         var result = await _userManager.AddToRoleAsync(user, command.RoleName);
//         if (!result.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new RoleAdditionRejected(command.UserId, command.RoleName, string.Join(",", result.Errors)), cancellationToken);
//             return;
//         }
//         await _userManager.UpdateAsync(user);
//         // await _userManager.CacheUserAsync(user);
//         
//         await _messageBroker.PublishAsync(new RoleAddedToUser(user.Id, command.RoleName), cancellationToken);
//     }
// }
//
