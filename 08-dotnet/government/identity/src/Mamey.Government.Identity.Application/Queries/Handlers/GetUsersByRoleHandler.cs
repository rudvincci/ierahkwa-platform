using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetUsersByRoleHandler : IQueryHandler<GetUsersByRole, IEnumerable<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersByRoleHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> HandleAsync(GetUsersByRole query, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByRoleIdAsync(query.RoleId, cancellationToken);
        return users.Select(MapToUserDto);
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
