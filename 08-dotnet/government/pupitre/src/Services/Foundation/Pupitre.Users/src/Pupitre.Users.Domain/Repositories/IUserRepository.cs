using System;
using Pupitre.Users.Domain.Entities;

namespace Pupitre.Users.Domain.Repositories;

internal interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Entities.UserId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<User> GetAsync(Entities.UserId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Entities.UserId id, CancellationToken cancellationToken = default);
}
