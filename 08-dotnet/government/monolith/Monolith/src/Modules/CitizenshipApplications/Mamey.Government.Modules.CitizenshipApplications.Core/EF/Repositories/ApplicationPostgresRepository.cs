using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF.Repositories;

internal class ApplicationPostgresRepository : IApplicationRepository
{
    private readonly CitizenshipApplicationsDbContext _context;
    private readonly ILogger<ApplicationPostgresRepository> _logger;

    public ApplicationPostgresRepository(
        CitizenshipApplicationsDbContext context,
        ILogger<ApplicationPostgresRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CitizenshipApplication?> GetAsync(AppId id, CancellationToken cancellationToken = default)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        
        if (application == null)
        {
            return null;
        }

        // Load UploadedDocuments separately since they're in a different table (Uploads is ignored in EF config)
        await LoadUploadedDocumentsAsync(application, cancellationToken);
        
        return application;
    }

    public async Task AddAsync(CitizenshipApplication application, CancellationToken cancellationToken = default)
    {
        await _context.Applications.AddAsync(application, cancellationToken);
        
        // Add UploadedDocuments separately with ApplicationId property
        foreach (var upload in application.Uploads)
        {
            // Set ApplicationId property directly (it's now a real property with conversion)
            upload.ApplicationId = application.Id;
            await _context.UploadedDocuments.AddAsync(upload, cancellationToken);
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CitizenshipApplication application, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == application.Id, cancellationToken);
        
        if (existing == null)
        {
            _logger.LogWarning("Application not found for update: {ApplicationId}", application.Id.Value);
            return;
        }

        // Update properties manually since SetValues doesn't work well with value objects
        var entry = _context.Entry(existing);
        
        // Update simple properties
        entry.Property(nameof(CitizenshipApplication.Status)).CurrentValue = application.Status;
        entry.Property(nameof(CitizenshipApplication.Step)).CurrentValue = application.Step;
        entry.Property(nameof(CitizenshipApplication.DateOfBirth)).CurrentValue = application.DateOfBirth;
        entry.Property(nameof(CitizenshipApplication.RejectionReason)).CurrentValue = application.RejectionReason;
        entry.Property(nameof(CitizenshipApplication.ExtendedDataJson)).CurrentValue = application.ExtendedDataJson;
        entry.Property(nameof(CitizenshipApplication.AccessLogsJson)).CurrentValue = application.AccessLogsJson;
        entry.Property(nameof(CitizenshipApplication.ApprovedBy)).CurrentValue = application.ApprovedBy;
        entry.Property(nameof(CitizenshipApplication.RejectedBy)).CurrentValue = application.RejectedBy;
        entry.Property(nameof(CitizenshipApplication.SubmittedAt)).CurrentValue = application.SubmittedAt;
        entry.Property(nameof(CitizenshipApplication.ApprovedAt)).CurrentValue = application.ApprovedAt;
        entry.Property(nameof(CitizenshipApplication.RejectedAt)).CurrentValue = application.RejectedAt;
        entry.Property(nameof(CitizenshipApplication.UpdatedAt)).CurrentValue = application.UpdatedAt;
        entry.Property(nameof(CitizenshipApplication.Version)).CurrentValue = application.Version;
        
        // Update value objects (these are handled by HasConversion)
        entry.Property(nameof(CitizenshipApplication.Email)).CurrentValue = application.Email;
        entry.Property(nameof(CitizenshipApplication.Phone)).CurrentValue = application.Phone;
        entry.Property(nameof(CitizenshipApplication.ReviewedBy)).CurrentValue = application.ReviewedBy;
        
        // Update owned entity: ApplicantName (EF Core handles owned entities automatically via OwnsOne)
        entry.Property(nameof(CitizenshipApplication.ApplicantName)).CurrentValue = application.ApplicantName;
        
        // Update owned entity: Address (EF Core handles owned entities automatically via OwnsOne)
        entry.Property(nameof(CitizenshipApplication.Address)).CurrentValue = application.Address;
        
        // Update JSON properties
        entry.Property(nameof(CitizenshipApplication.PersonalDetails)).CurrentValue = application.PersonalDetails;
        entry.Property(nameof(CitizenshipApplication.ContactInformation)).CurrentValue = application.ContactInformation;
        entry.Property(nameof(CitizenshipApplication.ForeignIdentification)).CurrentValue = application.ForeignIdentification;
        entry.Property(nameof(CitizenshipApplication.Dependents)).CurrentValue = application.Dependents;
        entry.Property(nameof(CitizenshipApplication.ResidencyHistory)).CurrentValue = application.ResidencyHistory;
        entry.Property(nameof(CitizenshipApplication.ImmigrationHistories)).CurrentValue = application.ImmigrationHistories;
        entry.Property(nameof(CitizenshipApplication.EducationQualifications)).CurrentValue = application.EducationQualifications;
        entry.Property(nameof(CitizenshipApplication.EmploymentHistory)).CurrentValue = application.EmploymentHistory;
        entry.Property(nameof(CitizenshipApplication.ForeignCitizenshipApplications)).CurrentValue = application.ForeignCitizenshipApplications;
        entry.Property(nameof(CitizenshipApplication.CriminalHistory)).CurrentValue = application.CriminalHistory;
        entry.Property(nameof(CitizenshipApplication.References)).CurrentValue = application.References;
        
        // Handle UploadedDocuments - delete removed ones, add new ones
        var existingDocuments = await _context.UploadedDocuments
            .Where(d => d.ApplicationId == application.Id)
            .ToListAsync(cancellationToken);
        
        var existingIds = existingDocuments.Select(d => d.Id).ToHashSet();
        var newIds = application.Uploads.Select(u => u.Id).ToHashSet();
        
        // Remove deleted documents
        var toDelete = existingDocuments.Where(d => !newIds.Contains(d.Id)).ToList();
        _context.UploadedDocuments.RemoveRange(toDelete);
        
        // Add new documents
        var toAdd = application.Uploads.Where(u => !existingIds.Contains(u.Id)).ToList();
        foreach (var upload in toAdd)
        {
            // Set ApplicationId property directly (it's now a real property with conversion)
            upload.ApplicationId = application.Id;
            await _context.UploadedDocuments.AddAsync(upload, cancellationToken);
        }
        
        // Update existing documents
        foreach (var upload in application.Uploads.Where(u => existingIds.Contains(u.Id)))
        {
            var existingDoc = existingDocuments.First(d => d.Id == upload.Id);
            var docEntry = _context.Entry(existingDoc);
            docEntry.Property(nameof(UploadedDocument.FileName)).CurrentValue = upload.FileName;
            docEntry.Property(nameof(UploadedDocument.StoragePath)).CurrentValue = upload.StoragePath;
            docEntry.Property(nameof(UploadedDocument.DocumentType)).CurrentValue = upload.DocumentType;
            docEntry.Property(nameof(UploadedDocument.FileSize)).CurrentValue = upload.FileSize;
            docEntry.Property(nameof(UploadedDocument.UploadedAt)).CurrentValue = upload.UploadedAt;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AppId id, CancellationToken cancellationToken = default)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        
        if (application != null)
        {
            // UploadedDocuments will be deleted automatically due to cascade delete
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(AppId id, CancellationToken cancellationToken = default)
    {
        return await _context.Applications
            .AnyAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CitizenshipApplication>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var applications = await _context.Applications
            .Include(c=> c.Uploads)
            .ToListAsync(cancellationToken);
        
        // Load UploadedDocuments for each application
        // foreach (var application in applications)
        // {
        //     await LoadUploadedDocumentsAsync(application, cancellationToken);
        // }
        
        return applications;
    }

    public async Task<IList<CitizenshipApplication>> GetAllByApplicationEmail(string email, CancellationToken cancellationToken = default)
    {
        var application = await (
            _context.Applications
                .Include(c=> c.Uploads)
            .Where(a => a.Email == email)).ToListAsync(cancellationToken);
        
        if (application == null)
        {
            return null;
        }

        // await LoadUploadedDocumentsAsync(application, cancellationToken);
        
        return application;
    }

    public async Task<CitizenshipApplication?> GetByApplicationEmail(string email, CancellationToken cancellationToken = default)
    {
        var application = await _context.Applications
            .Include(c=> c.Uploads)
            .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
        return application;
    }

    public async Task<CitizenshipApplication?> GetByApplicationNumberAsync(ApplicationNumber applicationNumber, CancellationToken cancellationToken = default)
    {
        var application = await _context.Applications
            .Include(c=> c.Uploads)
            .FirstOrDefaultAsync(a => a.ApplicationNumber == applicationNumber, cancellationToken);
        
        // if (application == null)
        // {
        //     return null;
        // }

        //await LoadUploadedDocumentsAsync(application, cancellationToken);
        
        return application;
    }

    public async Task<IReadOnlyList<CitizenshipApplication>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var applications = await _context.Applications
            .Include(c=> c.Uploads)
            .Where(a => a.TenantId == tenantId)
            .ToListAsync(cancellationToken);
        
        // Load UploadedDocuments for each application
        // foreach (var application in applications)
        // {
        //     await LoadUploadedDocumentsAsync(application, cancellationToken);
        // }
        
        return applications;
    }

    public async Task<IReadOnlyList<CitizenshipApplication>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default)
    {
        var applications = await _context.Applications
            .Where(a => a.Status == status)
            .Include(c=> c.Uploads)
            .ToListAsync(cancellationToken);
        
        // Load UploadedDocuments for each application
        // foreach (var application in applications)
        // {
        //     await LoadUploadedDocumentsAsync(application, cancellationToken);
        // }
        
        return applications;
    }

    public async Task<int> GetCountByYearAsync(TenantId tenantId, int year, CancellationToken cancellationToken = default)
    {
        return await _context.Applications
            .Include(c=> c.Uploads)
            .Where(a => a.TenantId == tenantId && a.CreatedAt.Year == year)
            .CountAsync(cancellationToken);
    }

    /// <summary>
    /// Loads UploadedDocuments for an application using the ApplicationId property
    /// </summary>
    private async Task LoadUploadedDocumentsAsync(CitizenshipApplication application, CancellationToken cancellationToken)
    {
        // Since Uploads is ignored in EF configuration, we need to load them separately
        // and add them to the application's internal collection via reflection
        var documents = await _context.UploadedDocuments
            .Where(d => d.ApplicationId == application.Id)
            .ToListAsync(cancellationToken);
        
        // Use reflection to add documents to the private _uploads field
        var uploadsField = typeof(CitizenshipApplication).GetField("_uploads", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (uploadsField != null)
        {
            var uploadsList = uploadsField.GetValue(application) as List<UploadedDocument>;
            if (uploadsList != null)
            {
                uploadsList.Clear();
                uploadsList.AddRange(documents);
            }
        }
    }
}
