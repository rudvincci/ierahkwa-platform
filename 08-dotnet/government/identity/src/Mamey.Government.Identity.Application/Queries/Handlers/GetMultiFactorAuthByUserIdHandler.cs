using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetMultiFactorAuthByUserIdHandler : IQueryHandler<GetMultiFactorAuthByUserId, MultiFactorAuthDto>
{
    private readonly IMultiFactorAuthRepository _multiFactorAuthRepository;

    public GetMultiFactorAuthByUserIdHandler(IMultiFactorAuthRepository multiFactorAuthRepository)
    {
        _multiFactorAuthRepository = multiFactorAuthRepository;
    }

    public async Task<MultiFactorAuthDto> HandleAsync(GetMultiFactorAuthByUserId query, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        
        if (multiFactorAuth is null)
        {
            throw new MultiFactorAuthNotFoundException(query.UserId);
        }

        return MapToMultiFactorAuthDto(multiFactorAuth);
    }

    private static MultiFactorAuthDto MapToMultiFactorAuthDto(MultiFactorAuth multiFactorAuth)
    {
        return new MultiFactorAuthDto(
            multiFactorAuth.Id,
            multiFactorAuth.UserId,
            multiFactorAuth.EnabledMethods.Select(c=> c.ToString()),
            multiFactorAuth.RequiredMethods,
            multiFactorAuth.Status.ToString(),
            multiFactorAuth.CreatedAt,
            multiFactorAuth.ActivatedAt
        );
    }
}
