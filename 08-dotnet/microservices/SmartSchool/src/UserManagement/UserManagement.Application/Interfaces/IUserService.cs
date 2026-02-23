using Common.Application.DTOs;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto?> GetByUserNameAsync(string userName);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<PagedResult<UserDto>> GetAllAsync(QueryParameters parameters);
    Task<PagedResult<UserDto>> GetByTenantAsync(int tenantId, QueryParameters parameters);
    Task<IEnumerable<UserDto>> GetByRoleAsync(string roleName);
    Task<UserDto> CreateAsync(CreateUserDto createUserDto);
    Task<UserDto> UpdateAsync(UpdateUserDto updateUserDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ActivateAsync(int id);
    Task<bool> DeactivateAsync(int id);
    Task<bool> AssignRolesAsync(int userId, IEnumerable<string> roles);
}
