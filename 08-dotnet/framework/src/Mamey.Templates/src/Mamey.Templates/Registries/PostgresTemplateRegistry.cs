using Mamey.Templates.EF;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Templates.Registries;

public sealed class PostgresTemplateRegistry : ITemplateRegistry
{
    private readonly TemplatesDbContext _db;
    
    public PostgresTemplateRegistry(TemplatesDbContext db) 
        => _db = db;

    public async Task<DocumentTemplate?> FindAsync(TemplateId id, int version, CancellationToken ct = default)
        => await _db.Templates.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.Version == version, ct);

    public async Task<int> LatestApprovedVersionAsync(TemplateId id, CancellationToken ct = default)
    {
        var q = _db.Templates.AsNoTracking()
            .Where(t => t.Id == id && t.Status == DocumentTemplateStatus.Approved);
        if (!await q.AnyAsync(ct)) return 0;
        return await q.MaxAsync(t => t.Version, ct);
    }
}