using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Modules.Citizens.Core.Queries.Handlers;

internal sealed class CountCitizensByStatusHandler : IQueryHandler<CountCitizensByStatus, Dictionary<CitizenshipStatus, int>>
{
    private readonly ICitizenRepository _repository;

    public CountCitizensByStatusHandler(ICitizenRepository repository)
    {
        _repository = repository;
    }

    public async Task<Dictionary<CitizenshipStatus, int>> HandleAsync(CountCitizensByStatus query, CancellationToken cancellationToken = default)
    {
        var citizens = await _repository.GetByTenantAsync(new TenantId(query.TenantId), cancellationToken);
        
        // Initialize with all status values at 0
        var counts = Enum.GetValues<CitizenshipStatus>()
            .ToDictionary(s => s, s => 0);
        
        // Count citizens by status
        foreach (var citizen in citizens)
        {
            counts[citizen.Status]++;
        }

        return counts;
    }
}
