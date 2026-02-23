using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CitizenshipApplications.Core.Exceptions;
using ApplicationId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands.Handlers;

internal sealed class SubmitApplicationHandler : ICommandHandler<SubmitApplication>
{
    private readonly IApplicationRepository _repository;

    public SubmitApplicationHandler(IApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(SubmitApplication command, CancellationToken cancellationToken = default)
    {
        var application = await _repository.GetAsync(new ApplicationId(command.ApplicationId), cancellationToken);
        if (application is null)
        {
            throw new ApplicationNotFoundException(command.ApplicationId);
        }

        application.Submit();
        application.UpdateStatus(ApplicationStatus.ReviewPending);

        await _repository.UpdateAsync(application, cancellationToken);
    }
}
