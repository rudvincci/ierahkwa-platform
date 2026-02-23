using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Repositories;

internal class UserMongoRepository : IUserRepository
{
    private readonly IMongoRepository<UserDocument, Guid> _repository;

    public UserMongoRepository(IMongoRepository<UserDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new UserDocument(user));

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new UserDocument(user));

    public async Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);

    public async Task<IReadOnlyList<User>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();

    public async Task<User> GetAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetAsync(id.Value);
        return user?.AsEntity();
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);

    public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindAsync(u => u.Username == username);
        return user.FirstOrDefault()?.AsEntity();
    }

    public async Task<User> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindAsync(u => u.Email == email.Value);
        return user.FirstOrDefault()?.AsEntity();
    }

    public async Task<User> GetBySubjectIdAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindAsync(u => u.SubjectId == subjectId.Value);
        return user.FirstOrDefault()?.AsEntity();
    }

    public async Task<IReadOnlyList<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(u => u.Status == status.ToString());
        return users.Select(u => u.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(u => u.Status == UserStatus.Active.ToString());
        return users.Select(u => u.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<User>> GetRecentlyAuthenticatedAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        var sinceTimestamp = since.ToUnixTimeMilliseconds();
        var users = await _repository.FindAsync(u => u.LastLoginAt.HasValue && u.LastLoginAt.Value >= sinceTimestamp);
        return users.Select(u => u.AsEntity()).ToList();
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(u => u.Username == username);
        return users.Any();
    }

    public async Task<bool> UsernameExistsAsync(string username, UserId excludeId, CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(u => u.Username == username && u.Id != excludeId.Value);
        return users.Any();
    }

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(u => u.Email == email.Value);
        return users.Any();
    }

    public async Task<bool> EmailExistsAsync(Email email, UserId excludeId, CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(u => u.Email == email.Value && u.Id != excludeId.Value);
        return users.Any();
    }

    public async Task<bool> SubjectIdExistsAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(u => u.SubjectId == subjectId.Value);
        return users.Any();
    }

    public async Task<bool> SubjectIdExistsAsync(SubjectId subjectId, UserId excludeId, CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(u => u.SubjectId == subjectId.Value && u.Id != excludeId.Value);
        return users.Any();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(_ => true);
        return users.Count();
    }

    public async Task<int> CountByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(u => u.Status == status.ToString());
        return users.Count();
    }

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.FindAsync(u => u.Status == UserStatus.Active.ToString());
        return users.Count();
    }

    // Additional methods required by IUserRepository interface
    public Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<IReadOnlyList<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<IReadOnlyList<User>> GetUsersWithFailedAttemptsAsync(int minAttempts, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<int> CountLockedAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<int> CountWithTwoFactorAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<int> CountWithMultiFactorAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");

    public Task<IReadOnlyList<User>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("MongoDB repository is for analytics only");
}
