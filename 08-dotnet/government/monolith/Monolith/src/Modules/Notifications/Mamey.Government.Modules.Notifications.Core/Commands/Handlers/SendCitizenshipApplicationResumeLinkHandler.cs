using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Notifications.Core.Services;
using Mamey.Government.Modules.Notifications.Core.Templates.Models;
using Mamey.Government.Modules.Notifications.Core.Templates.Types;
using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Commands.Handlers;

internal sealed class SendCitizenshipApplicationResumeLinkHandler : ICommandHandler<SendCitizenshipApplicationResumeLink>
{
    private readonly INotificationService _notificationService;

    public SendCitizenshipApplicationResumeLinkHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(SendCitizenshipApplicationResumeLink command, CancellationToken cancellationToken = default)
    {
        var recipientName = BuildRecipientName(command.FirstName, command.LastName);
        var companyAddress = new Address
        {
            Line = "47 St. Regis Road",
            City = "Akwesasne",
            State = "NY",
            Zip5 = "13655",
            Country = "US"
        };

        var emailModel = new CitizenshipApplicationResume(
            "Mamey Government",
            companyAddress,
            recipientName,
            "https://mamey.gov/support",
            command.ApplicationUrl);

        await _notificationService.SendEmailUsingTemplate(
            command.Email,
            "Resume your citizenship application",
            EmailTemplateType.CitizenshipApplicationResume,
            emailModel);
    }

    private static Name BuildRecipientName(string? firstName, string? lastName)
    {
        var safeFirstName = string.IsNullOrWhiteSpace(firstName) ? "Applicant" : firstName.Trim();
        var safeLastName = string.IsNullOrWhiteSpace(lastName) ? "User" : lastName.Trim();
        return new Name(safeFirstName, safeLastName);
    }
}
