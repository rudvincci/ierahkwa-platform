using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Queries.Handlers;

internal sealed class CountApplicationsByStatusHandler : IQueryHandler<CountApplicationsByStatus, Dictionary<ApplicationStatus, int>>
{
    private readonly IApplicationRepository _repository;

    public CountApplicationsByStatusHandler(IApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Dictionary<ApplicationStatus, int>> HandleAsync(CountApplicationsByStatus query, CancellationToken cancellationToken = default)
    {
        var applications = await _repository.GetByTenantAsync(new TenantId(query.TenantId), cancellationToken);
        
        // Initialize with all status values at 0
        var counts = Enum.GetValues<ApplicationStatus>()
            .ToDictionary(s => s, s => 0);
        
        // Count applications by status
        foreach (var app in applications)
        {
            counts[app.Status]++;
        }

        return counts;
    }
}
