using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;
using Mamey.Government.Modules.CitizenshipApplications.Core.Queries;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Queries.Handlers;

internal sealed class GetApplicationByNumberHandler : IQueryHandler<GetApplicationByNumber, ApplicationDto?>
{
    private readonly IApplicationRepository _repository;

    public GetApplicationByNumberHandler(IApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApplicationDto?> HandleAsync(GetApplicationByNumber query, CancellationToken cancellationToken = default)
    {
        var applicationNumber = new ApplicationNumber(query.ApplicationNumber);
        var application = await _repository.GetByApplicationNumberAsync(applicationNumber, cancellationToken);
        if (application is null)
        {
            return null;
        }

        return GetApplicationHandler.MapToDto(application);
    }
}
