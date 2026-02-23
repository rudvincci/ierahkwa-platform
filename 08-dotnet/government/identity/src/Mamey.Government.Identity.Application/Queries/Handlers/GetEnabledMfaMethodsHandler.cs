using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetEnabledMfaMethodsHandler : IQueryHandler<GetEnabledMfaMethods, IEnumerable<string>>
{
    private readonly IMultiFactorAuthRepository _multiFactorAuthRepository;

    public GetEnabledMfaMethodsHandler(IMultiFactorAuthRepository multiFactorAuthRepository)
    {
        _multiFactorAuthRepository = multiFactorAuthRepository;
    }

    public async Task<IEnumerable<string>> HandleAsync(GetEnabledMfaMethods query, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        
        if (multiFactorAuth is null)
        {
            throw new MultiFactorAuthNotFoundException(query.UserId);
        }

        return multiFactorAuth.EnabledMethods.Select(c=> c.ToString());
    }
}
