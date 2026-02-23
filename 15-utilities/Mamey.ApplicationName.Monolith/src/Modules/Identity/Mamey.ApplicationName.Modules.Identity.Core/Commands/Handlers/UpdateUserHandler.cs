using System.Security.Claims;
using Mamey.ApplicationName.Modules.Identity.Core.Events;
using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
using Mamey.Auth.Identity.Entities;
using Mamey.Auth.Identity.Providers;
using Mamey.CQRS.Commands;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Persistence.SQL;
using Mamey.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;

internal sealed class UpdateUserHandler : ICommandHandler<UpdateUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMessageBroker _messageBroker;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateUserHandler(UserManager<ApplicationUser> userManager, IMessageBroker messageBroker, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _messageBroker = messageBroker;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleAsync(UpdateUser command, CancellationToken cancellationToken = default)
    {
        Guid.TryParse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var updatedById) ;
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        // var user = await _userManager.GetUserFromCacheAsync(command.UserId);
        if (user == null)
        {
            await _messageBroker.PublishAsync(new UserUpdateRejected(command.UserId, "User not found"), cancellationToken);
            return;
        }
        user.SetName(command.Name, updatedById);
        var result = await _userManager.UpdateAsync(user);
        if(!result.Succeeded)
        {
            await _messageBroker.PublishAsync(new UserUpdateRejected(command.UserId, string.Join(",", result.Errors)), cancellationToken);
            return;
        }
        await _userManager.UpdateAsync(user);
        // await _userManager.CacheUserAsync(user, TimeSpan.FromMinutes(30));
        // TODO: FIX tenant
        await _messageBroker.PublishAsync(new UserUpdated(user.Id, user.Email), cancellationToken);
    }
}