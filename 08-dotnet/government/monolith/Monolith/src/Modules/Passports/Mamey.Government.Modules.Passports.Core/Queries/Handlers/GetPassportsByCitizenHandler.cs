using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.DTO;
using Mamey.Government.Modules.Passports.Core.Mappings;

namespace Mamey.Government.Modules.Passports.Core.Queries.Handlers;

internal sealed class GetPassportsByCitizenHandler : IQueryHandler<GetPassportsByCitizen, IEnumerable<PassportSummaryDto>>
{
    private readonly IPassportRepository _repository;

    public GetPassportsByCitizenHandler(IPassportRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PassportSummaryDto>> HandleAsync(GetPassportsByCitizen query, CancellationToken cancellationToken = default)
    {
        var passports = await _repository.GetByCitizenAsync(new CitizenId(query.CitizenId), cancellationToken);
        return passports.Select(p => p.AsSummaryDto());
    }
}
