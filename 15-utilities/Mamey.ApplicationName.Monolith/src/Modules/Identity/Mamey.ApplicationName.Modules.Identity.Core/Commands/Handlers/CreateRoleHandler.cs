using System.Security.Claims;
using Mamey.ApplicationName.Modules.Identity.Core.Events;
using Mamey.Auth.Identity;
using Mamey.Auth.Identity.Entities;
using Mamey.Auth.Identity.Managers;
using Mamey.Auth.Identity.Providers;
using Mamey.CQRS.Commands;
using Mamey.Exceptions;
using Mamey.MicroMonolith.Abstractions.Dispatchers;
using Mamey.Persistence.SQL;
using Mamey.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;

internal sealed class CreateRoleHandler : ICommandHandler<CreateRole>
{
    private readonly ILogger<CreateRoleHandler> _logger;
    private readonly IDispatcher _dispatcher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MameyRoleManager _roleManager;
    private readonly ITenantProvider _tenantProvider;

    public CreateRoleHandler(ILogger<CreateRoleHandler> logger, IDispatcher dispatcher, IHttpContextAccessor httpContextAccessor, IRoleManager roleManager, ITenantProvider tenantProvider)
    {
        _logger = logger;
        _dispatcher = dispatcher;
        _httpContextAccessor = httpContextAccessor;
        _tenantProvider = tenantProvider;
        _roleManager = (MameyRoleManager)roleManager;
    }

    public async Task HandleAsync(CreateRole command, CancellationToken cancellationToken = default)
    {
      
        var idValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? throw new InvalidOperationException("User ID claim not found.");

        if (!Guid.TryParse(idValue, out var userId))
            throw new InvalidOperationException($"Invalid GUID in NameIdentifier claim: {idValue}");
        var role = await _roleManager.CreateAsync(new ApplicationRole(RoleId.New(),  command.Name,  userId, false, command.Description), 
            userId, cancellationToken);
        
        await _dispatcher.PublishAsync(new RoleCreated(
            role.Id, role.Name), cancellationToken);
    }
}
internal sealed class UpdateRoleHandler : ICommandHandler<UpdateRole>
{
    private readonly ILogger<UpdateRoleHandler> _logger;
    private readonly IDispatcher _dispatcher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MameyRoleManager _roleManager;
    private readonly MameyUserManager _userManager;

    public UpdateRoleHandler(ILogger<UpdateRoleHandler> logger, IDispatcher dispatcher, IHttpContextAccessor httpContextAccessor, IRoleManager roleManager, IUserManager userManager)
    {
        _logger = logger;
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _userManager = (MameyUserManager)userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = (MameyRoleManager)roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    public async Task HandleAsync(UpdateRole command, CancellationToken cancellationToken = default)
    {
        Guid.TryParse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var updatedById) ;
    
        var role = await _roleManager.FindByIdAsync(command.Id);
        if (role == null)
        {
            throw new MameyException("Role does not exist.");
        }
        role.Name = command.Name;
        role.UpdateDescription(command.Description, updatedById);
        var idValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? throw new InvalidOperationException("User ID claim not found.");

        if (!Guid.TryParse(idValue, out var userId))
            throw new InvalidOperationException($"Invalid GUID in NameIdentifier claim: {idValue}");
        await _roleManager.UpdateAsync(role,
            userId, cancellationToken);
        
        await _dispatcher.PublishAsync(new RoleUpdated(
            role.Id, role.Name), cancellationToken);
    }
}
internal sealed class DeleteRoleHandler : ICommandHandler<UpdateRole>
{
    private readonly ILogger<UpdateRoleHandler> _logger;
    private readonly IDispatcher _dispatcher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MameyRoleManager _roleManager;

    public DeleteRoleHandler(ILogger<UpdateRoleHandler> logger, IDispatcher dispatcher, IHttpContextAccessor httpContextAccessor, IRoleManager roleManager)
    {
        _logger = logger;
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _roleManager = (MameyRoleManager)roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    public async Task HandleAsync(UpdateRole command, CancellationToken cancellationToken = default)
    {

        var role = await _roleManager.FindByIdAsync(command.Id);
        if (role == null)
        {
            throw new MameyException("Role does not exist.");
        }
        var idValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? throw new InvalidOperationException("User ID claim not found.");

        if (!Guid.TryParse(idValue, out var userId))
            throw new InvalidOperationException($"Invalid GUID in NameIdentifier claim: {idValue}");
        
        await _roleManager.DeleteAsync(role.Id,
            userId, cancellationToken);
        
        await _dispatcher.PublishAsync(new RoleDeleted(
            role.Id), cancellationToken);
    }
}