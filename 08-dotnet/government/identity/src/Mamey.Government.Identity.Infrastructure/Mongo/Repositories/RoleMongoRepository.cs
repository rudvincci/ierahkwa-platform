using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Repositories;

internal class RoleMongoRepository : IRoleRepository
{
    private readonly IMongoRepository<RoleDocument, Guid> _repository;

    public RoleMongoRepository(IMongoRepository<RoleDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new RoleDocument(role));

    public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new RoleDocument(role));

    public async Task DeleteAsync(RoleId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);

    public async Task<IReadOnlyList<Role>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();

    public async Task<Role> GetAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        var role = await _repository.GetAsync(id.Value);
        return role?.AsEntity();
    }

    public async Task<bool> ExistsAsync(RoleId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);

    public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var role = await _repository.FindAsync(r => r.Name == name);
        return role.FirstOrDefault()?.AsEntity();
    }

    public async Task<IReadOnlyList<Role>> GetByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default)
    {
        var roles = await _repository.FindAsync(r => r.Status == status.ToString());
        return roles.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Role>> GetByPermissionIdAsync(PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        var roles = await _repository.FindAsync(r => r.Permissions.Contains(permissionId.Value));
        return roles.Select(r => r.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _repository.FindAsync(r => r.Status == RoleStatus.Active.ToString());
        return roles.Select(r => r.AsEntity()).ToList();
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        var roles = await _repository.FindAsync(r => r.Name == name);
        return roles.Any();
    }

    public async Task<bool> NameExistsAsync(string name, RoleId excludeId, CancellationToken cancellationToken = default)
    {
        var roles = await _repository.FindAsync(r => r.Name == name && r.Id != excludeId.Value);
        return roles.Any();
    }

    public async Task<bool> HasPermissionAsync(RoleId roleId, PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        var role = await _repository.FindAsync(r => r.Id == roleId.Value);
        return role.FirstOrDefault()?.Permissions.Contains(permissionId.Value) ?? false;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _repository.FindAsync(_ => true);
        return roles.Count();
    }

    public async Task<int> CountByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default)
    {
        var roles = await _repository.FindAsync(r => r.Status == status.ToString());
        return roles.Count();
    }

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _repository.FindAsync(r => r.Status == RoleStatus.Active.ToString());
        return roles.Count();
    }

    // Additional methods required by IRoleRepository interface
    public Task<IReadOnlyList<Role>> GetBySubjectIdAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<IReadOnlyList<Role>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");
}
