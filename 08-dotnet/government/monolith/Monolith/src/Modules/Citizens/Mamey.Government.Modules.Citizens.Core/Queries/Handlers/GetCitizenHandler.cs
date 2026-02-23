using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.DTO;

namespace Mamey.Government.Modules.Citizens.Core.Queries.Handlers;

internal sealed class GetCitizenHandler : IQueryHandler<GetCitizen, CitizenDto?>
{
    private readonly ICitizenRepository _repository;

    public GetCitizenHandler(ICitizenRepository repository)
    {
        _repository = repository;
    }

    public async Task<CitizenDto?> HandleAsync(GetCitizen query, CancellationToken cancellationToken = default)
    {
        var citizen = await _repository.GetAsync(new CitizenId(query.CitizenId), cancellationToken);
        if (citizen is null)
        {
            return null;
        }

        return new CitizenDto(
            citizen.Id.Value,
            citizen.TenantId.Value,
            citizen.FirstName, // Already a string convenience property
            citizen.LastName,  // Already a string convenience property
            citizen.DateOfBirth ?? DateTime.MinValue,
            citizen.Status.ToString(),
            citizen.Email?.ToString(),
            citizen.Phone?.ToString(),
            citizen.Address?.ToString(),
            citizen.PhotoPath,
            citizen.Status != CitizenshipStatus.Inactive,
            citizen.CreatedAt,
            citizen.UpdatedAt);
    }
}
