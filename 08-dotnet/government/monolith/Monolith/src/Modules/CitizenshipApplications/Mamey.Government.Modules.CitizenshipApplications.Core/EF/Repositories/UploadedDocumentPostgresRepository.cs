using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF.Repositories;

internal class UploadedDocumentPostgresRepository : IUploadedDocumentRepository
{
    private readonly CitizenshipApplicationsDbContext _context;
    private readonly ILogger<UploadedDocumentPostgresRepository> _logger;

    public UploadedDocumentPostgresRepository(
        CitizenshipApplicationsDbContext context,
        ILogger<UploadedDocumentPostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UploadedDocument?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.UploadedDocuments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task AddAsync(UploadedDocument document, CancellationToken cancellationToken = default)
    {
        await _context.UploadedDocuments.AddAsync(document, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UploadedDocument document, CancellationToken cancellationToken = default)
    {
        var existing = await _context.UploadedDocuments
            .FirstOrDefaultAsync(d => d.Id == document.Id, cancellationToken);
        
        if (existing == null)
        {
            _logger.LogWarning("Document not found for update: {DocumentId}", document.Id);
            return;
        }

        // Since most properties are read-only, only update ApplicationId if it changed
        var entry = _context.Entry(existing);
        if (entry.Property(d => d.ApplicationId).CurrentValue != document.ApplicationId)
        {
            entry.Property(d => d.ApplicationId).CurrentValue = document.ApplicationId;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var document = await _context.UploadedDocuments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        
        if (document != null)
        {
            _context.UploadedDocuments.Remove(document);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.UploadedDocuments
            .AnyAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<UploadedDocument>> GetByApplicationIdAsync(AppId applicationId, CancellationToken cancellationToken = default)
    {
        return await _context.UploadedDocuments
            .Where(d => d.ApplicationId == applicationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UploadedDocument>> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default)
    {
        return await _context.UploadedDocuments
            .Where(d => d.DocumentType == documentType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UploadedDocument>> GetByApplicationIdAndTypeAsync(AppId applicationId, string documentType, CancellationToken cancellationToken = default)
    {
        return await _context.UploadedDocuments
            .Where(d => d.ApplicationId == applicationId && d.DocumentType == documentType)
            .ToListAsync(cancellationToken);
    }
}
