using Mamey.ApplicationName.Modules.Identity.Core.EF;
using Mamey.Auth.Identity;
using Mamey.Auth.Identity.Entities;
using Mamey.Auth.Identity.Managers;
using Mamey.CQRS.Commands;
using Mamey.Exceptions;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Persistence.SQL;
using Mamey.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;

internal sealed class CreateUserHandler : ICommandHandler<CreateUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMessageBroker _messageBroker;
    private readonly AppOptions _options;
    private readonly ApplicationIdentityUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateUserHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker,
        RoleManager<ApplicationRole> roleManager, AppOptions options, ApplicationIdentityUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _messageBroker = messageBroker;
        _roleManager = roleManager as MameyRoleManager;
        _options = options;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleAsync(CreateUser command, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(command.Email);
        if (existingUser != null) return;

        // if any role does not roles exist
        if (!command.Roles.All(role => ((IQueryable<ApplicationRole>)_roleManager.Roles).Any(ir => ir.Name == role)))
        {
            // TODO: Change to custom exemption
            throw new ArgumentException("Invalid role", nameof(command.Roles));
        }
       
        var newUser = new ApplicationUser(command.Id, command.UserName, command.Name, command.Email )
        {
            EmailConfirmed = !command.ConfirmEmail,
            PhoneNumberConfirmed = !command.ConfirmPhone,
        };
        
        await _unitOfWork.ExecuteAsync(async () =>
        {
            // var result = await  CreateUserAsync(newUser, command.Password,
            //     command.Roles, command.RegistrationComplete);
            // var confirmUrl =
            //     $"{_options.WebClientUrl}/account/confirm-email?userId={result.Item1.Id}&token={Uri.EscapeDataString(result.Item2)}";
            // Publish immediately if desired
            
            // await _messageBroker.PublishAsync(new UserCreated(
            //     user.Id, user.Email, user.TenantId), ct);
            
            
            // await _messageBroker.PublishAsync(new SignedUp(result.Item1.Id, result.Item1.Email, string.Join(",", command.Roles), result.Item1.Name, confirmUrl),
                // cancellationToken);
        });
    }

    
}