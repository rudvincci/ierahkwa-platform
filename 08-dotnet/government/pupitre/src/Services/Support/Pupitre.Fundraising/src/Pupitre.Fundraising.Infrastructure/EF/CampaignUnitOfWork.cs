using Mamey.Postgres;

namespace Pupitre.Fundraising.Infrastructure.EF;

internal class CampaignUnitOfWork : PostgresUnitOfWork<CampaignDbContext>, ICampaignUnitOfWork
{
    public CampaignUnitOfWork(CampaignDbContext dbContext) : base(dbContext)
    {
    }
}
