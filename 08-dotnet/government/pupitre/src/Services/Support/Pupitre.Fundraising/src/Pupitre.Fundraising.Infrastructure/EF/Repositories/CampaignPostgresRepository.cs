using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Fundraising.Domain.Entities;
using Pupitre.Fundraising.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Fundraising.Infrastructure.EF.Repositories;

internal class CampaignPostgresRepository : EFRepository<Campaign, CampaignId>, ICampaignRepository, IDisposable
{
    private readonly CampaignDbContext _entityNameDbContext;
    public CampaignPostgresRepository(CampaignDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Campaign>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Campaign> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Campaigns.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Campaign> GetAsync(CampaignId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Campaigns
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Campaign entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Campaigns.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(CampaignId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Campaigns.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Campaign entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Campaigns.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Campaign entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(CampaignId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Campaigns.Single(c => c.Id == id.Value), cancellationToken);
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