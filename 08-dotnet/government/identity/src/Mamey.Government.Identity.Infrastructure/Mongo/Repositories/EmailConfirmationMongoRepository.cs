using System;
using Mamey.Persistence.MongoDB;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Types;
using Mamey;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Repositories;

internal class EmailConfirmationMongoRepository : IEmailConfirmationRepository
{
    private readonly IMongoRepository<EmailConfirmationDocument, Guid> _repository;

    public EmailConfirmationMongoRepository(IMongoRepository<EmailConfirmationDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(EmailConfirmation emailConfirmation, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new EmailConfirmationDocument(emailConfirmation));

    public async Task UpdateAsync(EmailConfirmation emailConfirmation, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new EmailConfirmationDocument(emailConfirmation));

    public async Task DeleteAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);

    public async Task<IReadOnlyList<EmailConfirmation>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
            .Select(c => c.AsEntity())
            .ToList();

    public async Task<EmailConfirmation> GetAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        var confirmation = await _repository.GetAsync(id.Value);
        return confirmation?.AsEntity();
    }

    public async Task<bool> ExistsAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);

    public async Task<EmailConfirmation> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var confirmations = await _repository.FindAsync(c => c.UserId == userId.Value);
        return confirmations.FirstOrDefault()?.AsEntity();
    }

    public async Task<EmailConfirmation> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var confirmations = await _repository.FindAsync(c => c.Email == email);
        return confirmations.FirstOrDefault()?.AsEntity();
    }

    public async Task<EmailConfirmation> GetByConfirmationCodeAsync(string confirmationCode, CancellationToken cancellationToken = default)
    {
        var confirmations = await _repository.FindAsync(c => c.ConfirmationCode == confirmationCode);
        return confirmations.FirstOrDefault()?.AsEntity();
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetByStatusAsync(EmailConfirmationStatus status, CancellationToken cancellationToken = default)
    {
        var confirmations = await _repository.FindAsync(c => c.Status == status.ToString());
        return confirmations.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetExpiredConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var confirmations = await _repository.FindAsync(c => c.ExpiresAt < now || c.Status == EmailConfirmationStatus.Expired.ToString());
        return confirmations.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetPendingConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        var confirmations = await _repository.FindAsync(c => c.Status == EmailConfirmationStatus.Pending.ToString());
        return confirmations.Select(c => c.AsEntity()).ToList();
    }

    public async Task<bool> HasPendingConfirmationAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var confirmations = await _repository.FindAsync(c => c.UserId == userId.Value && c.Status == EmailConfirmationStatus.Pending.ToString());
        return confirmations.Any();
    }

    public async Task<bool> HasPendingConfirmationAsync(string email, CancellationToken cancellationToken = default)
    {
        var confirmations = await _repository.FindAsync(c => c.Email == email && c.Status == EmailConfirmationStatus.Pending.ToString());
        return confirmations.Any();
    }

    public async Task DeleteExpiredConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var expired = await _repository.FindAsync(c => c.ExpiresAt < now || c.Status == EmailConfirmationStatus.Expired.ToString());
        foreach (var confirmation in expired)
        {
            await _repository.DeleteAsync(confirmation.Id);
        }
    }

    public async Task DeleteConfirmationsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var confirmations = await _repository.FindAsync(c => c.UserId == userId.Value);
        foreach (var confirmation in confirmations)
        {
            await _repository.DeleteAsync(confirmation.Id);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var confirmations = await _repository.FindAsync(_ => true);
        return confirmations.Count();
    }

    public async Task<int> CountByStatusAsync(EmailConfirmationStatus status, CancellationToken cancellationToken = default)
    {
        var confirmations = await _repository.FindAsync(c => c.Status == status.ToString());
        return confirmations.Count();
    }

    public async Task<int> CountExpiredAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var confirmations = await _repository.FindAsync(c => c.ExpiresAt < now || c.Status == EmailConfirmationStatus.Expired.ToString());
        return confirmations.Count();
    }

    public async Task<int> CountExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        var beforeTimestamp = before.ToUnixTimeMilliseconds();
        var confirmations = await _repository.FindAsync(c => c.ExpiresAt < beforeTimestamp || c.Status == EmailConfirmationStatus.Expired.ToString());
        return confirmations.Count();
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetPendingAsync(CancellationToken cancellationToken = default)
        => await GetPendingConfirmationsAsync(cancellationToken);

    public async Task<IReadOnlyList<EmailConfirmation>> GetExpiredAsync(CancellationToken cancellationToken = default)
        => await GetExpiredConfirmationsAsync(cancellationToken);

    public async Task<IReadOnlyList<EmailConfirmation>> GetExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        var beforeTimestamp = before.ToUnixTimeMilliseconds();
        var confirmations = await _repository.FindAsync(c => c.ExpiresAt < beforeTimestamp || c.Status == EmailConfirmationStatus.Expired.ToString());
        return confirmations.Select(c => c.AsEntity()).ToList();
    }
}

