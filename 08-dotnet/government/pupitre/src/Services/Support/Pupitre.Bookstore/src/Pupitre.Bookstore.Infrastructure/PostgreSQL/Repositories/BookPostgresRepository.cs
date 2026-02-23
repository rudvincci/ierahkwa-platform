using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Bookstore.Domain.Entities;
using Pupitre.Bookstore.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Bookstore.Infrastructure.PostgreSQL.Repositories;

internal class BookPostgresRepository : EFRepository<Book, BookId>, IBookRepository, IDisposable
{
    private readonly BookstoreDbContext _serviceNameDbContext;
    public BookPostgresRepository(BookstoreDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Book>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Book> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Books.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Book> GetAsync(BookId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Books
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Book entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Books.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(BookId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Books.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Book entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Books.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Book entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(BookId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Books.Single(c => c.Id == id.Value), cancellationToken);
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _serviceNameDbContext.Dispose();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}