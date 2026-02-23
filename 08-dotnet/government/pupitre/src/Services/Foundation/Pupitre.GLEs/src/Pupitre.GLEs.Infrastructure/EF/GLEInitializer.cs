using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.GLEs.Infrastructure.EF;

internal sealed class GLEInitializer
{
    private readonly GLEDbContext _dbContext;
    private readonly ILogger<GLEInitializer> _logger;

    public GLEInitializer(GLEDbContext dbContext, ILogger<GLEInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.GLEs.AnyAsync())
        { }

        await AddGLEsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddGLEsAsync()
    {
        throw new NotImplementedException();
    }
}