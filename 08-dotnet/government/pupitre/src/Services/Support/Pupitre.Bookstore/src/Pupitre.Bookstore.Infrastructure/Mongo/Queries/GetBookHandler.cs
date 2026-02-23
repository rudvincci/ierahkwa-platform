using System;
using Mamey.CQRS.Queries;
using Pupitre.Bookstore.Application.DTO;
using Pupitre.Bookstore.Application.Queries;
using Pupitre.Bookstore.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Bookstore.Infrastructure.Mongo.Queries;

internal sealed class GetBookHandler : IQueryHandler<GetBook, BookDetailsDto>
{
    private const string collectionName = "bookstore";
    private readonly IMongoDatabase _database;

    public GetBookHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<BookDetailsDto> HandleAsync(GetBook query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<BookDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


