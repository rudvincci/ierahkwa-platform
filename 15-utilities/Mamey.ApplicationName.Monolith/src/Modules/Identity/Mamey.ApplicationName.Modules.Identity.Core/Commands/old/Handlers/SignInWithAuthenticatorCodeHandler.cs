// using Mamey.Auth.Jwt;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.ApplicationName.Modules.Identity.Core.Storage;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Mamey.Persistence.Redis;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class SignInWithAuthenticatorCodeHandler : ICommandHandler<SignInWithAuthenticatorCode>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly SignInManager<ApplicationUser> _signInManager;
//     private readonly IMessageBroker _messageBroker;
//     private readonly ICache _cache;
//     private readonly IRefreshTokenStore _refreshTokenStore;
//     private readonly JwtOptions _jwtOptions;
//     public async Task HandleAsync(SignInWithAuthenticatorCode command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(command.UserId.ToString());
//         // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new SignInWithAuthenticatorCodeRejected(command.UserId, "User not found"), cancellationToken);
//             return;
//         }
//         var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Authenticator", command.Code);
//         if (!isValid)
//         {
//             await _messageBroker.PublishAsync(new SignInWithAuthenticatorCodeRejected(command.UserId, "Invalid code"), cancellationToken);
//             return;
//         }
//         var result = await  _signInManager.TwoFactorAuthenticatorSignInAsync(command.Code, true, true);
//         if (!result.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new SignInWithAuthenticatorCodeRejected(command.UserId, "Could not authenticate user."));
//         }
//         if (result.IsLockedOut)
//         {
//             await _messageBroker.PublishAsync(new SignInUserRejected(command.UserId, "User is locked out"), cancellationToken);
//             return;
//         }
//
//         if (!result.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new SignInUserRejected(command.UserId, "Invalid credentials"), cancellationToken);
//             return;
//         }
//
//         // Once verified, you may consider the authenticator fully set up.
//         
//     }
// }