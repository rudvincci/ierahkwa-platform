using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.Mongo.Documents;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Identity.Core.Mongo.Repositories;

internal class UserProfileMongoRepository : IUserProfileRepository
{
    private readonly IMongoRepository<UserProfileDocument, Guid> _repository;
    private readonly ILogger<UserProfileMongoRepository> _logger;

    public UserProfileMongoRepository(
        IMongoRepository<UserProfileDocument, Guid> repository,
        ILogger<UserProfileMongoRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UserProfile?> GetAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(id.Value);
        return document?.AsEntity();
    }

    public async Task AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        var document = new UserProfileDocument(userProfile);
        await _repository.AddAsync(document);
    }

    public async Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        var document = new UserProfileDocument(userProfile);
        await _repository.UpdateAsync(document);
    }

    public async Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id.Value);
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(d => d.Id == id.Value);
    }

    public async Task<IReadOnlyList<UserProfile>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(_ => true);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<UserProfile?> GetByAuthenticatorAsync(string issuer, string subject, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(
            d => d.AuthenticatorIssuer == issuer && d.AuthenticatorSubject == subject);
        
        return document?.AsEntity();
    }

    public async Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(
            d => d.Email == email);
        
        return document?.AsEntity();
    }
}
