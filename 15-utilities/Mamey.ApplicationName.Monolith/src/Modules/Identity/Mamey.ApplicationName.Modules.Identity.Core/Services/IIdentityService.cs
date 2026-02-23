using System.Net;
using Mamey.Auth.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Core.Services;

internal interface IIdentityService
{
    Task<(HttpStatusCode, ApplicationUser?, string)> ConfirmEmailAsync(Guid userId, string token);
}

internal class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(HttpStatusCode, ApplicationUser?, string)> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            // await _messageBroker.PublishAsync(new UserEmailConfirmationRejected(userId, "User not found"));
            
            return (HttpStatusCode.NotFound, null, "User not found");
        }
        
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            // await _messageBroker.PublishAsync(new UserEmailConfirmationRejected(userId, string.Join(",", result.Errors)), cancellationToken);
            return (HttpStatusCode.BadRequest, null, string.Join(",", result.Errors.SelectMany(c => c.Code)));
        }
        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);
        // await _userManager.CacheUserAsync(user);
        
        return (HttpStatusCode.Accepted, user, "Email confirmed");
    }
}

internal static class Extensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        
        
        services.AddScoped<IIdentityService, IdentityService>();
        return services;
    }
}