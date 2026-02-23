//
// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Mamey.Types;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class InitiatePasswordResetHandler : ICommandHandler<InitiatePasswordReset>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly IMessageBroker _messageBroker;
//     private readonly AppOptions _appOptions;
//     
//
//     public InitiatePasswordResetHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker, AppOptions appOptions)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//         _appOptions = appOptions;
//     }
//
//     public async Task HandleAsync(InitiatePasswordReset command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByEmailAsync(command.Email);
//         if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
//         {
//             await _messageBroker.PublishAsync(new PasswordResetInitiationRejected(command.Email, "Email not found"), cancellationToken);
//             return;
//         }
//         var token = await _userManager.GeneratePasswordResetTokenAsync(user);
//         var resetUrl = $"{_appOptions.WebClientUrl}/account/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";
//         
//         await _messageBroker.PublishAsync(new PasswordResetInitiated(user.Email, resetUrl), cancellationToken);
//     }
// }