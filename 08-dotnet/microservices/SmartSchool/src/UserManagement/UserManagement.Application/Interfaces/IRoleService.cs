using Common.Application.DTOs;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Interfaces;

public interface IRoleService
{
    Task<RoleDto?> GetByIdAsync(int id);
    Task<RoleDto?> GetByNameAsync(string name);
    Task<PagedResult<RoleDto>> GetAllAsync(QueryParameters parameters);
    Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto);
    Task<RoleDto> UpdateAsync(UpdateRoleDto updateRoleDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> AssignPoliciesToRoleAsync(int roleId, IEnumerable<int> policyIds);
}

public interface IPolicyService
{
    Task<PolicyDto?> GetByIdAsync(int id);
    Task<PagedResult<PolicyDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<PolicyDto>> GetByRoleAsync(int roleId);
    Task<PolicyDto> CreateAsync(PolicyDto policyDto);
    Task<PolicyDto> UpdateAsync(PolicyDto policyDto);
    Task<bool> DeleteAsync(int id);
}
