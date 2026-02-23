using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Users.Application.DTO;
using Pupitre.Users.Application.Queries;
using Pupitre.Users.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Users.Infrastructure.Mongo.Queries;

internal sealed class BrowseUsersHandler : IQueryHandler<BrowseUsers, PagedResult<UserDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseUsersHandler(IMongoDatabase database, IMongoRepository<UserDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<UserDto>?> HandleAsync(BrowseUsers query, CancellationToken cancellationToken = default)
    {
        PagedResult<UserDocument> pagedResult;

        var userDocuments = _database.GetCollection<UserDocument>("users").AsQueryable();
        IEnumerable<UserDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await userDocuments.PaginateAsync(query);
            return PagedResult<UserDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            userDocuments = userDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await userDocuments.PaginateAsync(query);

        return PagedResult<UserDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseUsers query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



