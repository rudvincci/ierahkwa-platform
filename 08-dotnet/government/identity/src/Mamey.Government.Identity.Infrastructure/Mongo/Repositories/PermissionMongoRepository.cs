using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Repositories;

internal class PermissionMongoRepository : IPermissionRepository
{
    private readonly IMongoRepository<PermissionDocument, Guid> _repository;

    public PermissionMongoRepository(IMongoRepository<PermissionDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new PermissionDocument(permission));

    public async Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new PermissionDocument(permission));

    public async Task DeleteAsync(PermissionId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);

    public async Task<IReadOnlyList<Permission>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();

    public async Task<Permission> GetAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        var permission = await _repository.GetAsync(id.Value);
        return permission?.AsEntity();
    }

    public async Task<bool> ExistsAsync(PermissionId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);

    public async Task<Permission> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var permission = await _repository.FindAsync(p => p.Name == name);
        return permission.FirstOrDefault()?.AsEntity();
    }


    public async Task<IReadOnlyList<Permission>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Resource == resource);
        return permissions.Select(p => p.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Permission>> GetByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Action == action);
        return permissions.Select(p => p.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Permission>> GetByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Status == status.ToString());
        return permissions.Select(p => p.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Permission>> GetActivePermissionsAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Status == PermissionStatus.Active.ToString());
        return permissions.Select(p => p.AsEntity()).ToList();
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Name == name);
        return permissions.Any();
    }

    public async Task<bool> NameExistsAsync(string name, PermissionId excludeId, CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Name == name && p.Id != excludeId.Value);
        return permissions.Any();
    }

    public async Task<bool> ResourceActionExistsAsync(string resource, string action, CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Resource == resource && p.Action == action);
        return permissions.Any();
    }

    public async Task<bool> ResourceActionExistsAsync(string resource, string action, PermissionId excludeId, CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Resource == resource && p.Action == action && p.Id != excludeId.Value);
        return permissions.Any();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(_ => true);
        return permissions.Count();
    }

    public async Task<int> CountByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Status == status.ToString());
        return permissions.Count();
    }

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Status == PermissionStatus.Active.ToString());
        return permissions.Count();
    }

    public async Task<int> CountByResourceAsync(string resource, CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.FindAsync(p => p.Resource == resource);
        return permissions.Count();
    }

    // Additional methods required by IPermissionRepository interface
    public Task<IReadOnlyList<Permission>> GetByResourceAndActionAsync(string resource, string action, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<IReadOnlyList<Permission>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<IReadOnlyList<Permission>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<PermissionId> ids, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<IReadOnlyList<string>> GetDistinctResourcesAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<IReadOnlyList<string>> GetDistinctActionsAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");
}
