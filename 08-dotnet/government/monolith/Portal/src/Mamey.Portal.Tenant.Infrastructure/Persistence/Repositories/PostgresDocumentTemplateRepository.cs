using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Tenant.Domain.Entities;
using Mamey.Portal.Tenant.Domain.Repositories;
using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Infrastructure.Persistence.Repositories;

public sealed class PostgresDocumentTemplateRepository : IDocumentTemplateRepository
{
    private readonly TenantDbContext _db;

    public PostgresDocumentTemplateRepository(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<TenantDocumentTemplate?> GetAsync(string tenantId, string kind, CancellationToken ct = default)
    {
        var row = await _db.DocumentTemplates.AsNoTracking()
            .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Kind == kind, ct);

        return row is null
            ? null
            : new TenantDocumentTemplate(new TenantId(row.TenantId), row.Kind, row.TemplateHtml, row.UpdatedAt);
    }

    public async Task<IReadOnlyList<TenantDocumentTemplate>> GetByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        var rows = await _db.DocumentTemplates.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Kind)
            .ToListAsync(ct);

        return rows.Select(x => new TenantDocumentTemplate(new TenantId(x.TenantId), x.Kind, x.TemplateHtml, x.UpdatedAt)).ToList();
    }

    public async Task SaveAsync(TenantDocumentTemplate template, CancellationToken ct = default)
    {
        var row = await _db.DocumentTemplates
            .SingleOrDefaultAsync(x => x.TenantId == template.TenantId.Value && x.Kind == template.Kind, ct);

        if (row is null)
        {
            _db.DocumentTemplates.Add(new TenantDocumentTemplateRow
            {
                TenantId = template.TenantId.Value,
                Kind = template.Kind,
                TemplateHtml = template.TemplateHtml,
                UpdatedAt = template.UpdatedAt
            });
        }
        else
        {
            row.TemplateHtml = template.TemplateHtml;
            row.UpdatedAt = template.UpdatedAt;
        }

        await _db.SaveChangesAsync(ct);
    }
}
