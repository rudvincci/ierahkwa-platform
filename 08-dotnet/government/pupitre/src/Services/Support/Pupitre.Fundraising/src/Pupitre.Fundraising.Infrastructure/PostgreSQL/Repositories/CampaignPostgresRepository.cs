using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Fundraising.Domain.Entities;
using Pupitre.Fundraising.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Fundraising.Infrastructure.PostgreSQL.Repositories;

internal class CampaignPostgresRepository : EFRepository<Campaign, CampaignId>, ICampaignRepository, IDisposable
{
    private readonly FundraisingDbContext _serviceNameDbContext;
    public CampaignPostgresRepository(FundraisingDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Campaign>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Campaign> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Campaigns.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Campaign> GetAsync(CampaignId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Campaigns
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Campaign entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Campaigns.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(CampaignId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Campaigns.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Campaign entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Campaigns.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Campaign entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(CampaignId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Campaigns.Single(c => c.Id == id.Value), cancellationToken);
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