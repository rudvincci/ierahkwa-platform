using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;

namespace Mamey.Portal.Citizenship.Infrastructure.Services;

public sealed class ApplicationStatusStore : IApplicationStatusStore
{
    private readonly CitizenshipDbContext _db;

    public ApplicationStatusStore(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<ApplicationStatusDto?> GetByNumberAsync(string tenantId, string applicationNumber, CancellationToken ct = default)
    {
        var row = await _db.Applications
            .AsNoTracking()
            .Include(x => x.Uploads)
            .Include(x => x.IssuedDocuments)
            .Where(x => x.TenantId == tenantId && x.ApplicationNumber == applicationNumber)
            .SingleOrDefaultAsync(ct);

        IntakeReviewRow? intakeReview = null;
        if (row is not null)
        {
            intakeReview = await _db.IntakeReviews
                .AsNoTracking()
                .Where(r => r.ApplicationId == row.Id && r.TenantId == tenantId)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }

        return row is null ? null : MapToDto(row, intakeReview);
    }

    public async Task<ApplicationStatusDto?> GetByIdAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        var row = await _db.Applications
            .AsNoTracking()
            .Include(x => x.Uploads)
            .Include(x => x.IssuedDocuments)
            .Where(x => x.TenantId == tenantId && x.Id == applicationId)
            .SingleOrDefaultAsync(ct);

        IntakeReviewRow? intakeReview = null;
        if (row is not null)
        {
            intakeReview = await _db.IntakeReviews
                .AsNoTracking()
                .Where(r => r.ApplicationId == row.Id && r.TenantId == tenantId)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }

        return row is null ? null : MapToDto(row, intakeReview);
    }

    public async Task<ApplicationStatusDto?> GetByEmailAsync(string tenantId, string email, CancellationToken ct = default)
    {
        var row = await _db.Applications
            .AsNoTracking()
            .Include(x => x.Uploads)
            .Include(x => x.IssuedDocuments)
            .Where(x => x.TenantId == tenantId && x.Email == email)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);

        IntakeReviewRow? intakeReview = null;
        if (row is not null)
        {
            intakeReview = await _db.IntakeReviews
                .AsNoTracking()
                .Where(r => r.ApplicationId == row.Id && r.TenantId == tenantId)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }

        return row is null ? null : MapToDto(row, intakeReview);
    }

    private static ApplicationStatusDto MapToDto(CitizenshipApplicationRow row, IntakeReviewRow? intakeReview = null)
    {
        var uploads = row.Uploads
            .OrderByDescending(u => u.UploadedAt)
            .Select(u => new ApplicationStatusUploadDto(
                u.Id,
                u.Kind,
                u.FileName,
                u.ContentType,
                u.Size,
                u.UploadedAt))
            .ToList();

        var documents = row.IssuedDocuments
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new ApplicationStatusDocumentDto(
                d.Id,
                d.Kind,
                d.DocumentNumber,
                d.FileName,
                d.ExpiresAt,
                d.CreatedAt))
            .ToList();

        var timeline = BuildTimeline(row, intakeReview);

        var intakeReviewDto = intakeReview is null ? null : new ApplicationStatusIntakeReviewDto(
            intakeReview.ReviewerName,
            intakeReview.ReviewDate,
            intakeReview.Recommendation,
            intakeReview.RecommendationReason,
            intakeReview.ApplicationComplete,
            intakeReview.AllDocumentsReceived,
            intakeReview.IdentityVerified,
            intakeReview.CreatedAt);

        return new ApplicationStatusDto(
            row.Id,
            row.ApplicationNumber,
            row.Status,
            row.FirstName,
            row.LastName,
            row.DateOfBirth,
            row.Email,
            row.CreatedAt,
            row.UpdatedAt,
            uploads,
            documents,
            timeline,
            intakeReviewDto,
            row.RejectionReason);
    }

    private static IReadOnlyList<ApplicationStatusTimelineEntryDto> BuildTimeline(CitizenshipApplicationRow row, IntakeReviewRow? intakeReview = null)
    {
        var timeline = new List<ApplicationStatusTimelineEntryDto>
        {
            new(
                row.CreatedAt,
                "Application Created",
                $"Application {row.ApplicationNumber} was submitted.")
        };

        if (row.Status != "Submitted" && row.UpdatedAt > row.CreatedAt)
        {
            timeline.Add(new ApplicationStatusTimelineEntryDto(
                row.UpdatedAt,
                "Status Updated",
                $"Application status changed to: {row.Status}"));
        }

        foreach (var upload in row.Uploads.OrderBy(u => u.UploadedAt))
        {
            timeline.Add(new ApplicationStatusTimelineEntryDto(
                upload.UploadedAt,
                "Document Uploaded",
                $"{upload.Kind}: {upload.FileName}"));
        }

        foreach (var doc in row.IssuedDocuments.OrderBy(d => d.CreatedAt))
        {
            timeline.Add(new ApplicationStatusTimelineEntryDto(
                doc.CreatedAt,
                "Document Issued",
                $"{doc.Kind} issued{(string.IsNullOrWhiteSpace(doc.DocumentNumber) ? "" : $" (Number: {doc.DocumentNumber})")}"));
        }

        if (intakeReview is not null)
        {
            timeline.Add(new ApplicationStatusTimelineEntryDto(
                intakeReview.CreatedAt,
                "Intake Review Completed",
                $"Reviewed by {intakeReview.ReviewerName}. Recommendation: {intakeReview.Recommendation}"));
        }

        return timeline.OrderBy(t => t.Timestamp).ToList();
    }
}
