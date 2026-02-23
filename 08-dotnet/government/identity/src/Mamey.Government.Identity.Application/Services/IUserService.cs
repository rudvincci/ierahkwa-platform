using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Services;

internal interface IUserService
{
    #region User CRUD Operations
    Task<Mamey.Government.Identity.Contracts.DTO.UserDto?> GetUserAsync(UserId id, CancellationToken cancellationToken = default);
    Task<Mamey.Government.Identity.Contracts.DTO.UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Mamey.Government.Identity.Contracts.DTO.UserDto> CreateUserAsync(CreateUser command, CancellationToken cancellationToken = default);
    Task<Mamey.Government.Identity.Contracts.DTO.UserDto> UpdateUserAsync(UpdateUser command, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(UserId id, CancellationToken cancellationToken = default);
    #endregion

    #region User Status Management
    Task ActivateUserAsync(ActivateUser command, CancellationToken cancellationToken = default);
    Task DeactivateUserAsync(DeactivateUser command, CancellationToken cancellationToken = default);
    Task LockUserAsync(LockUser command, CancellationToken cancellationToken = default);
    Task UnlockUserAsync(UnlockUser command, CancellationToken cancellationToken = default);
    #endregion

    #region Password Management
    Task ChangeUserPasswordAsync(ChangeUserPassword command, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(string email, string newPassword, CancellationToken cancellationToken = default);
    #endregion

    #region User Search and Filtering
    Task<IEnumerable<Mamey.Government.Identity.Contracts.DTO.UserDto>> GetUsersByStatusAsync(bool isActive, CancellationToken cancellationToken = default);
    Task<IEnumerable<Mamey.Government.Identity.Contracts.DTO.UserDto>> GetUsersByRoleAsync(RoleId roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Mamey.Government.Identity.Contracts.DTO.UserDto>> GetRecentlyActiveUsersAsync(TimeSpan timeSpan, CancellationToken cancellationToken = default);
    #endregion

    #region User Statistics
    Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken cancellationToken = default);
    #endregion
}
