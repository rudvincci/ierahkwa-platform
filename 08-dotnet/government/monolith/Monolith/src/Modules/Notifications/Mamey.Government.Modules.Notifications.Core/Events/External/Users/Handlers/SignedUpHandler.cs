using Mamey.Government.Modules.Notifications.Core.Services;
using Mamey.Government.Modules.Notifications.Core.Templates.Models;
using Mamey.Government.Modules.Notifications.Core.Templates.Types;
using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Events.External.Users.Handlers;

internal sealed class SignedUpHandler : IEventHandler<SignedUp>
{
    public SignedUpHandler(IUserService userService, INotificationService notificationService, IMessageBroker messageBroker)
    {
        _userService = userService;
        _notificationService = notificationService;
        _messageBroker = messageBroker;
    }

    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly IMessageBroker _messageBroker;

    public async Task HandleAsync(SignedUp @event, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetAsync(@event.UserId);
        // var company
        // TODO: Create settings
        var emailModel = new WelcomeDto("Future BDET Bank", new Address()
        {
            Line = "47 St. Regis Road",
            City = "Akwesasne",
            State = "NY",
            Zip5 = "13655",
            Country = "US",
        }, user.Name, "support@futurebdetbank.com", @event.ConfirmUrl);
        await _notificationService.SendEmailUsingTemplate(user.Email, "Confirm Email", EmailTemplateType.Welcome, emailModel);
        await _messageBroker.PublishAsync(new EmailConfirmationSent(user.Id, user.Email), cancellationToken);
    }
}