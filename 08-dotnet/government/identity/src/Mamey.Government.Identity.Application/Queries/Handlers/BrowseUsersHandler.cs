using System.Linq;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class BrowseUsersHandler : IQueryHandler<BrowseUsers, PagedResult<UserDto>?>
{
    private readonly IUserRepository _userRepository;

    public BrowseUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserDto>?> HandleAsync(BrowseUsers query, CancellationToken cancellationToken = default)
    {
        var allUsers = await _userRepository.BrowseAsync(cancellationToken);
        
        // Filter users based on query parameters
        var filteredUsers = allUsers.AsQueryable();
        
        if (!string.IsNullOrEmpty(query.Username))
        {
            filteredUsers = filteredUsers.Where(u => 
                u.Username.Contains(query.Username.Trim(), StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrEmpty(query.Email))
        {
            filteredUsers = filteredUsers.Where(u => 
                u.Email.Value.Contains(query.Email.Trim(), StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrEmpty(query.Status))
        {
            if (Enum.TryParse<UserStatus>(query.Status, true, out var status))
            {
                filteredUsers = filteredUsers.Where(u => u.Status == status);
            }
        }
        
        var usersList = filteredUsers.ToList();
        
        // Manual pagination
        var page = query.Page > 0 ? query.Page : 1;
        var pageSize = query.ResultsPerPage > 0 ? query.ResultsPerPage : 10;
        var totalResults = usersList.Count;
        var totalPages = (int)Math.Ceiling(totalResults / (double)pageSize);
        var skip = (page - 1) * pageSize;
        var pagedUsers = usersList.Skip(skip).Take(pageSize);
        
        var userDtos = pagedUsers.Select(MapToUserDto).ToList();
        
        return PagedResult<UserDto>.Create(
            userDtos,
            page,
            pageSize,
            totalPages,
            totalResults
        );
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

