using System.Threading;
using System.Threading.Tasks;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Clients;

internal interface INotificationsClient
{
    Task SendApplicationStartEmailAsync(
        string email,
        string applicationUrl,
        string? firstName = null,
        string? lastName = null,
        CancellationToken cancellationToken = default);

    Task SendApplicationResumeEmailAsync(
        string email,
        string applicationUrl,
        string? firstName = null,
        string? lastName = null,
        CancellationToken cancellationToken = default);
}
