// using Mamey.ApplicationName.Modules.Identity.Core.Storage;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Entities;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Mamey.Persistence.Redis;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class SignOutHandler : ICommandHandler<SignOut>
// {
//     public SignOutHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker, ICache cache, IRefreshTokenStore refreshTokenStore)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//         _cache = cache;
//         _refreshTokenStore = refreshTokenStore;
//     }
//
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//     private readonly ICache _cache;
//     private readonly IRefreshTokenStore _refreshTokenStore;
//     public Task HandleAsync(SignOut command, CancellationToken cancellationToken = default)
//     {
//         throw new NotImplementedException();
//     }
// }