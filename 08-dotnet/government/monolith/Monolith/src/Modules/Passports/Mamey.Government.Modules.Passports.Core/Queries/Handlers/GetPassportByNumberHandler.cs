using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.DTO;
using Mamey.Government.Modules.Passports.Core.Mappings;

namespace Mamey.Government.Modules.Passports.Core.Queries.Handlers;

internal sealed class GetPassportByNumberHandler : IQueryHandler<GetPassportByNumber, PassportDto?>
{
    private readonly IPassportRepository _repository;

    public GetPassportByNumberHandler(IPassportRepository repository)
    {
        _repository = repository;
    }

    public async Task<PassportDto?> HandleAsync(GetPassportByNumber query, CancellationToken cancellationToken = default)
    {
        var passport = await _repository.GetByPassportNumberAsync(
            new PassportNumber(query.PassportNumber), cancellationToken);
        return passport?.AsDto();
    }
}
