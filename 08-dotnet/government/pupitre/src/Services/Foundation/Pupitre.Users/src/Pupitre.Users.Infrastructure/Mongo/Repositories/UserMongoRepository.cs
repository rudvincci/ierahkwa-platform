using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Users.Domain.Repositories;
using Pupitre.Users.Domain.Entities;
using Pupitre.Users.Infrastructure.Mongo.Documents;
using DomainUserId = Pupitre.Users.Domain.Entities.UserId;

namespace Pupitre.Users.Infrastructure.Mongo.Repositories;

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
        
    public async Task DeleteAsync(DomainUserId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
        
    public async Task<IReadOnlyList<User>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
        
    public async Task<User> GetAsync(DomainUserId id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetAsync(id.Value);
        return user?.AsEntity()!;
    }
    
    public async Task<bool> ExistsAsync(DomainUserId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}
