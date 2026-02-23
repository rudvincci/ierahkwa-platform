using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Ministries.Infrastructure.EF;

internal sealed class MinistryDataInitializer
{
    private readonly MinistryDataDbContext _dbContext;
    private readonly ILogger<MinistryDataInitializer> _logger;

    public MinistryDataInitializer(MinistryDataDbContext dbContext, ILogger<MinistryDataInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.MinistryDatas.AnyAsync())
        { }

        await AddMinistryDatasAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddMinistryDatasAsync()
    {
        throw new NotImplementedException();
    }
}