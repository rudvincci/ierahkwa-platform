using System.Text;
using Mamey.ApplicationName.Modules.Notifications.Core.Services;
using Mamey.ApplicationName.Modules.Notifications.Core.Templates.Models;
using Mamey.ApplicationName.Modules.Notifications.Core.Templates.Types;
using Mamey.CQRS.Events;
using Mamey.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Events.External.Users.Handlers;

internal sealed class PasswordResetRequestedHandler : IEventHandler<PasswordResetRequested>
{
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly AppOptions _appOptions;
    private readonly IHttpContextAccessor _httpContextHandler;
    public PasswordResetRequestedHandler(IUserService userService, INotificationService notificationService, AppOptions appOptions, IHttpContextAccessor httpContextHandler)
    {
        _userService = userService;
        _notificationService = notificationService;
        _appOptions = appOptions;
        _httpContextHandler = httpContextHandler;
    }

    public async Task HandleAsync(PasswordResetRequested @event, CancellationToken cancellationToken = default)
    {
        var host = _httpContextHandler.HttpContext.Request.Host.Value;
        var user = await _userService.GetAsync(@event.UserId);
        
        var url = $"{_appOptions.WebClientUrl}/password-reset/?token={WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(@event.Token))}";
        // TODO: get company info settings
        var emailModel = new ResetPassword("Future BDET Bank", new Address()
        {
            Line = "47 St. Regis Road",
            City = "Akwesasne",
            State = "NY",
            Zip5 = "13655",
            Country = "US",
        }, user.Name, "support@futurebdetBank.com", url);
        await _notificationService.SendEmailUsingTemplate(user.Email, "Password Reset", EmailTemplateType.ResetPassword, emailModel);
    }
}