using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Bookstore.Infrastructure.EF;

internal sealed class BookInitializer
{
    private readonly BookDbContext _dbContext;
    private readonly ILogger<BookInitializer> _logger;

    public BookInitializer(BookDbContext dbContext, ILogger<BookInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Books.AnyAsync())
        { }

        await AddBooksAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddBooksAsync()
    {
        throw new NotImplementedException();
    }
}