using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.DTO;
using Mamey.Government.Modules.Passports.Core.Mappings;

namespace Mamey.Government.Modules.Passports.Core.Queries.Handlers;

internal sealed class GetPassportHandler : IQueryHandler<GetPassport, PassportDto?>
{
    private readonly IPassportRepository _repository;

    public GetPassportHandler(IPassportRepository repository)
    {
        _repository = repository;
    }

    public async Task<PassportDto?> HandleAsync(GetPassport query, CancellationToken cancellationToken = default)
    {
        var passport = await _repository.GetAsync(new PassportId(query.PassportId), cancellationToken);
        return passport?.AsDto();
    }
}
