using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Fundraising.Infrastructure.EF;

internal sealed class CampaignInitializer
{
    private readonly CampaignDbContext _dbContext;
    private readonly ILogger<CampaignInitializer> _logger;

    public CampaignInitializer(CampaignDbContext dbContext, ILogger<CampaignInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if(await _dbContext.Campaigns.AnyAsync())
        { }

        await AddCampaignsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private Task AddCampaignsAsync()
    {
        throw new NotImplementedException();
    }
}