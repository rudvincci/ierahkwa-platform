using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Bookstore.Domain.Entities;
using Pupitre.Bookstore.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Bookstore.Infrastructure.EF.Repositories;

internal class BookPostgresRepository : EFRepository<Book, BookId>, IBookRepository, IDisposable
{
    private readonly BookDbContext _entityNameDbContext;
    public BookPostgresRepository(BookDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Book>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Book> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Books.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Book> GetAsync(BookId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Books
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Book entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Books.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(BookId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Books.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Book entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Books.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Book entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(BookId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Books.Single(c => c.Id == id.Value), cancellationToken);
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _entityNameDbContext.Dispose();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}