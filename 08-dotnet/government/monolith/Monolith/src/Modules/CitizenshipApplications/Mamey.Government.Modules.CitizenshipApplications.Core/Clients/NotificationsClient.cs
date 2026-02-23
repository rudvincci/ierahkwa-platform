using System.Threading;
using System.Threading.Tasks;
using Mamey.MicroMonolith.Abstractions.Modules;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Clients;

internal class NotificationsClient : INotificationsClient
{
    private readonly IModuleClient _moduleClient;

    public NotificationsClient(IModuleClient moduleClient)
    {
        _moduleClient = moduleClient;
    }

    public Task SendApplicationStartEmailAsync(
        string email,
        string applicationUrl,
        string? firstName = null,
        string? lastName = null,
        CancellationToken cancellationToken = default)
        => _moduleClient.SendAsync(
            "notifications/send-citizenship-application-link",
            new
            {
                Email = email,
                ApplicationUrl = applicationUrl,
                FirstName = firstName,
                LastName = lastName
            },
            cancellationToken);

    public Task SendApplicationResumeEmailAsync(
        string email,
        string applicationUrl,
        string? firstName = null,
        string? lastName = null,
        CancellationToken cancellationToken = default)
        => _moduleClient.SendAsync(
            "notifications/send-citizenship-application-resume-link",
            new
            {
                Email = email,
                ApplicationUrl = applicationUrl,
                FirstName = firstName,
                LastName = lastName
            },
            cancellationToken);
}
