// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.ApplicationName.Modules.Identity.Core.Storage;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class RequestNewAccessTokenHandler : ICommandHandler<RequestNewAccessToken>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//     private readonly IRefreshTokenStore _refreshTokenStore;
//     private readonly SignInManager<ApplicationUser> _signInManager;
//     public async Task HandleAsync(RequestNewAccessToken command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new RequestNewAccessTokenRejected(command.UserId, "User not found"), cancellationToken);
//             return;
//         }
//         await _signInManager.RefreshSignInAsync(user);
//         throw new NotImplementedException();
//     }
// }