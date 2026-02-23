using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetUserByEmailHandler : IQueryHandler<Mamey.Government.Identity.Contracts.Queries.GetUserByEmail, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> HandleAsync(Mamey.Government.Identity.Contracts.Queries.GetUserByEmail query, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(query.Email, cancellationToken);
        
        if (user is null)
        {
            throw new UserNotFoundException();
        }

        return MapToUserDto(user);
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto(
            user.Id,
            user.SubjectId,
            user.Username,
            user.Email,
            user.IsActive,
            user.IsLocked,
            user.LockedUntil,
            user.LastLoginAt,
            user.EmailConfirmationRequired,
            user.TwoFactorEnabled,
            user.MultiFactorEnabled,
            user.EmailConfirmedAt,
            user.TwoFactorEnabledAt,
            user.MultiFactorEnabledAt,
            user.CreatedAt,
            user.ModifiedAt
        );
    }
}
