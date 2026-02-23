using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Exceptions;
using Mamey.Government.Modules.CitizenshipApplications.Core.Services;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands.Handlers;

internal sealed class RejectApplicationHandler : ICommandHandler<RejectApplication>
{
    private readonly IApplicationRepository _repository;
    private readonly IContext _context;

    public RejectApplicationHandler(
        IApplicationRepository repository,
        IContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task HandleAsync(RejectApplication command, CancellationToken cancellationToken = default)
    {
        var application = await _repository.GetAsync(new AppId(command.ApplicationId), cancellationToken);
        if (application is null)
        {
            throw new ApplicationNotFoundException(command.ApplicationId);
        }

        // Get the rejector from the authenticated user context
        var rejectedBy = _context.UserId;
        application.Reject(command.Reason, rejectedBy);

        await _repository.UpdateAsync(application, cancellationToken);
    }
}
