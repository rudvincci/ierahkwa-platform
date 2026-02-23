using System;
using System.Collections.Generic;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class SubjectRedisRepository : ISubjectRepository
{
    private readonly ICache _cache;
    private const string SubjectPrefix = "subject:";
    private const string EmailPrefix = "subject:email:";

    public SubjectRedisRepository(ICache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        var ttl = TimeSpan.FromHours(24);
        await _cache.SetAsync($"{SubjectPrefix}{subject.Id.Value}", subject, ttl);
        await _cache.SetAsync($"{EmailPrefix}{subject.Email.Value}", subject.Id.Value, ttl);
    }

    public async Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        await AddAsync(subject, cancellationToken);
    }

    public async Task DeleteAsync(SubjectId id, CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = await GetAsync(id, cancellationToken);
            if (subject != null)
            {
                await _cache.DeleteAsync<Subject>($"{SubjectPrefix}{id.Value}");
                await _cache.DeleteAsync<Guid>($"{EmailPrefix}{subject.Email.Value}");
            }
            else
            {
                // If subject is null (not found or deserialization error), just delete the subject key
                // The email index will be cleaned up on next sync
                await _cache.DeleteAsync<Subject>($"{SubjectPrefix}{id.Value}");
            }
        }
        catch
        {
            // If GetAsync fails (e.g., deserialization error), just delete the subject key directly
            // The email index will be cleaned up on next sync when it tries to re-sync
            await _cache.DeleteAsync<Subject>($"{SubjectPrefix}{id.Value}");
        }
    }

    public Task<IReadOnlyList<Subject>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Subject>>(new List<Subject>());

    public Task<PagedResult<Subject>> BrowseAsync(Func<Subject, bool> predicate, PagedQueryBase query, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Paged queries not supported in Redis");

    public Task<Subject> GetAsync(SubjectId id, CancellationToken cancellationToken = default)
        => _cache.GetAsync<Subject>($"{SubjectPrefix}{id.Value}");

    public Task<bool> ExistsAsync(SubjectId id, CancellationToken cancellationToken = default)
        => _cache.KeyExistsAsync($"{SubjectPrefix}{id.Value}");

    public async Task<Subject> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var subjectId = await _cache.GetAsync<Guid>($"{EmailPrefix}{email}");
        return subjectId != Guid.Empty ? await GetAsync(new SubjectId(subjectId), cancellationToken) : null;
    }

    public Task<IReadOnlyList<Subject>> GetByStatusAsync(SubjectStatus status, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Subject>>(new List<Subject>());

    public Task<IReadOnlyList<Subject>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Subject>>(new List<Subject>());

    public Task<IReadOnlyList<Subject>> GetByTagAsync(string tag, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Subject>>(new List<Subject>());

    public Task<IReadOnlyList<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Subject>>(new List<Subject>());

    public Task<IReadOnlyList<Subject>> GetRecentlyAuthenticatedAsync(DateTime since, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Subject>>(new List<Subject>());

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => _cache.KeyExistsAsync($"{EmailPrefix}{email}");

    public async Task<bool> EmailExistsAsync(string email, SubjectId excludeId, CancellationToken cancellationToken = default)
    {
        var subject = await GetByEmailAsync(email, cancellationToken);
        return subject != null && subject.Id != excludeId;
    }

    public async Task<bool> HasRoleAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default)
    {
        var subject = await GetAsync(subjectId, cancellationToken);
        return subject?.HasRole(roleId) ?? false;
    }

    public async Task<bool> HasTagAsync(SubjectId subjectId, string tag, CancellationToken cancellationToken = default)
    {
        var subject = await GetAsync(subjectId, cancellationToken);
        return subject != null && subject.Tags.Contains(tag);
    }

    public Task<Subject> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => GetByEmailAsync(email.Value, cancellationToken);
}
