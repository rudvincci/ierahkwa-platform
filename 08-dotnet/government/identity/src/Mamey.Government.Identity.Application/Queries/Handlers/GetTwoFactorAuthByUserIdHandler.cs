using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetTwoFactorAuthByUserIdHandler : IQueryHandler<GetTwoFactorAuthByUserId, TwoFactorAuthDto>
{
    private readonly ITwoFactorAuthRepository _twoFactorAuthRepository;

    public GetTwoFactorAuthByUserIdHandler(ITwoFactorAuthRepository twoFactorAuthRepository)
    {
        _twoFactorAuthRepository = twoFactorAuthRepository;
    }

    public async Task<TwoFactorAuthDto> HandleAsync(GetTwoFactorAuthByUserId query, CancellationToken cancellationToken = default)
    {
        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        
        if (twoFactorAuth is null)
        {
            throw new TwoFactorAuthNotFoundException(query.UserId);
        }

        return MapToTwoFactorAuthDto(twoFactorAuth);
    }

    private static TwoFactorAuthDto MapToTwoFactorAuthDto(TwoFactorAuth twoFactorAuth)
    {
        return new TwoFactorAuthDto(
            twoFactorAuth.Id,
            twoFactorAuth.UserId,
            twoFactorAuth.SecretKey,
            twoFactorAuth.QrCodeUrl,
            twoFactorAuth.BackupCodes,
            twoFactorAuth.Status.ToString(),
            twoFactorAuth.CreatedAt,
            twoFactorAuth.ActivatedAt
        );
    }
}
