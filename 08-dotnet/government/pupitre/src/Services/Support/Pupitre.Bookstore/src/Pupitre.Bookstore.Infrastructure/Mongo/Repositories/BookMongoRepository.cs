using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Bookstore.Domain.Repositories;
using Pupitre.Bookstore.Domain.Entities;
using Pupitre.Bookstore.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Bookstore.Infrastructure.Mongo.Repositories;

internal class BookMongoRepository : IBookRepository
{
    private readonly IMongoRepository<BookDocument, Guid> _repository;

    public BookMongoRepository(IMongoRepository<BookDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new BookDocument(book));

    public async Task UpdateAsync(Book book, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new BookDocument(book));
    public async Task DeleteAsync(BookId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Book>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Book> GetAsync(BookId id, CancellationToken cancellationToken = default)
    {
        var book = await _repository.GetAsync(id.Value);
        return book?.AsEntity();
    }
    public async Task<bool> ExistsAsync(BookId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



