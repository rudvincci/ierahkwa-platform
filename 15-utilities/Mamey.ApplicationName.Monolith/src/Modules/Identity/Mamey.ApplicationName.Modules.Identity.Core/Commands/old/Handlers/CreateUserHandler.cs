// using Mamey.ApplicationName.Modules.Identity.Core.EF;
// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Exceptions;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Abstractions.Entities;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Mamey.Types;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class CreateUserHandler : ICommandHandler<CreateUser>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly RoleManager<ApplicationRole> _roleManager;
//     private readonly IMessageBroker _messageBroker;
//     private readonly AppOptions _options;
//     private readonly ApplicationIdentityUnitOfWork _unitOfWork;
//
//     public CreateUserHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker,
//         RoleManager<ApplicationRole> roleManager, AppOptions options, ApplicationIdentityUnitOfWork unitOfWork)
//     {
//         _userManager = userManager;
//         _messageBroker = messageBroker;
//         _roleManager = roleManager;
//         _options = options;
//         _unitOfWork = unitOfWork;
//     }
//
//     public async Task HandleAsync(CreateUser command, CancellationToken cancellationToken = default)
//     {
//         var existingUser = await _userManager.FindByEmailAsync(command.Email);
//         if (existingUser != null) return;
//
//         // if any role does not roles exist
//         if (!command.Roles.All(role => ((IQueryable<ApplicationRole>)_roleManager.Roles).Any(ir => ir.Name == role)))
//         {
//             // TODO: Change to custom exemption
//             throw new ArgumentException("Invalid role", nameof(command.Roles));
//         }
//
//         var newUser = new ApplicationUser(command.Id)
//         {
//             UserName = command.UserName,
//             Email = command.Email,
//             EmailConfirmed = !command.ConfirmEmail,
//             PhoneNumberConfirmed = !command.ConfirmPhone,
//             FullName = command.Name.FullName,
//             Name = command.Name
//         };
//         
//         await _unitOfWork.ExecuteAsync(async () =>
//         {
//             var result = await  CreateUserAsync(newUser, command.Password,
//                 command.Roles, command.RegistrationComplete);
//             var confirmUrl =
//                 $"{_options.WebClientUrl}/account/confirm-email?userId={result.Item1.Id}&token={Uri.EscapeDataString(result.Item2)}";
//             await _messageBroker.PublishAsync(new SignedUp(result.Item1.Id, result.Item1.Email, string.Join(",", command.Roles), result.Item1.Name, confirmUrl),
//                 cancellationToken);
//         });
//     }
//
//     private async Task<(ApplicationUser, string)> CreateUserAsync(ApplicationUser user, string password, IEnumerable<string> roles,
//         bool registrationComplete)
//     {
//         IdentityResult identityResult;
//
//         identityResult = await _userManager.CreateAsync(user, password);
//         if (!identityResult.Succeeded)
//         {
//             throw new CreateUserException(string.Join(", ", 
//                 identityResult.Errors.Select(e => e.Description)));
//         }
//         
//         foreach (var role in roles)
//         {
//             await _userManager.AddToRoleAsync(user, role);
//         }
//
//         // Add custom claims for the admin user
//         var claims = new List<System.Security.Claims.Claim>
//         {
//             new("FullName", user.FullName),
//             new("RegistrationComplete", registrationComplete.ToString()),
//         };
//
//         foreach (var claim in claims)
//         {
//             await _userManager.AddClaimAsync(user, claim);
//         }
//         
//         var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//         await _userManager.UpdateAsync(user);
//         // await _userManager.CacheUserAsync(user);
//         return (user, token);
//     }
// }