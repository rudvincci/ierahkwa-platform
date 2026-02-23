using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Requests;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;

namespace Mamey.Portal.Citizenship.Infrastructure.Services;

public sealed class CitizenshipBackofficeStore : ICitizenshipBackofficeStore
{
    private readonly CitizenshipDbContext _db;

    public CitizenshipBackofficeStore(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<BackofficeApplicationSummary>> GetRecentApplicationsAsync(string tenantId, int take, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 200);

        return await _db.Applications
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new BackofficeApplicationSummary(
                x.Id,
                x.ApplicationNumber,
                x.Status,
                x.FirstName + " " + x.LastName,
                x.DateOfBirth,
                x.Uploads.Count,
                x.CreatedAt,
                x.UpdatedAt))
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<BackofficeApplicationDetails?> GetApplicationAsync(string tenantId, Guid id, CancellationToken ct = default)
    {
        var row = await _db.Applications
            .AsNoTracking()
            .Include(x => x.Uploads)
            .Where(x => x.TenantId == tenantId && x.Id == id)
            .SingleOrDefaultAsync(ct);

        if (row is null)
        {
            return null;
        }

        var uploads = row.Uploads
            .OrderByDescending(u => u.UploadedAt)
            .Select(u => new BackofficeUploadSummary(
                u.Id,
                u.Kind,
                u.FileName,
                u.ContentType,
                u.Size,
                u.StorageBucket,
                u.StorageKey,
                u.UploadedAt))
            .ToList();

        return new BackofficeApplicationDetails(
            row.Id,
            row.ApplicationNumber,
            row.Status,
            row.TenantId,
            row.FirstName,
            row.LastName,
            row.DateOfBirth,
            row.Email,
            row.AddressLine1,
            row.City,
            row.Region,
            row.PostalCode,
            row.CreatedAt,
            row.UpdatedAt,
            uploads);
    }

    public async Task UpdateApplicationStatusByNumberAsync(string tenantId, string applicationNumber, string status, DateTimeOffset updatedAt, CancellationToken ct = default)
    {
        var row = await _db.Applications
            .Where(x => x.TenantId == tenantId && x.ApplicationNumber == applicationNumber)
            .SingleOrDefaultAsync(ct);

        if (row is null)
        {
            throw new InvalidOperationException("Application not found (or not in this tenant).");
        }

        row.Status = status;
        row.UpdatedAt = updatedAt;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<IssuedDocumentSummary>> GetIssuedDocumentsAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        return await _db.IssuedDocuments
            .AsNoTracking()
            .Where(x => x.ApplicationId == applicationId)
            .Join(
                _db.Applications.AsNoTracking().Where(a => a.TenantId == tenantId),
                doc => doc.ApplicationId,
                app => app.Id,
                (doc, _) => doc)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new IssuedDocumentSummary(
                x.Id,
                x.ApplicationId,
                x.Kind,
                x.DocumentNumber,
                x.ExpiresAt,
                x.FileName,
                x.ContentType,
                x.Size,
                x.StorageBucket,
                x.StorageKey,
                x.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<ApplicationCertificateSnapshot?> GetApplicationForCertificateAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == applicationId)
            .Select(x => new ApplicationCertificateSnapshot(
                x.Id,
                x.ApplicationNumber,
                x.FirstName,
                x.LastName,
                x.DateOfBirth,
                x.CreatedAt))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ApplicationPassportSnapshot?> GetApplicationForPassportAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == applicationId)
            .Select(x => new ApplicationPassportSnapshot(
                x.Id,
                x.ApplicationNumber,
                x.FirstName,
                x.LastName,
                x.DateOfBirth,
                x.Email))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ApplicationIdCardSnapshot?> GetApplicationForIdCardAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == applicationId)
            .Select(x => new ApplicationIdCardSnapshot(
                x.Id,
                x.ApplicationNumber,
                x.FirstName,
                x.LastName,
                x.MiddleName,
                x.DateOfBirth,
                x.AddressLine1,
                x.City,
                x.Region,
                x.PostalCode,
                x.Height,
                x.Sex,
                x.EyeColor,
                x.HairColor))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ApplicationVehicleTagSnapshot?> GetApplicationForVehicleTagAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == applicationId)
            .Select(x => new ApplicationVehicleTagSnapshot(
                x.Id,
                x.ApplicationNumber,
                x.FirstName,
                x.LastName))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ApplicationTravelIdSnapshot?> GetApplicationForTravelIdAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == applicationId)
            .Select(x => new ApplicationTravelIdSnapshot(
                x.Id,
                x.ApplicationNumber,
                x.FirstName,
                x.LastName,
                x.DateOfBirth,
                x.AddressLine1,
                x.City,
                x.Region,
                x.PostalCode))
            .SingleOrDefaultAsync(ct);
    }

    private static string RenderPassportHtml(
        string tenantId,
        string passportNumber,
        string applicationNumber,
        string fullName,
        DateOnly dob,
        DateTimeOffset issuedAt,
        DateTimeOffset expiresAt,
        string? mrzLine1 = null,
        string? mrzLine2 = null,
        string? mrzLine3 = null)
    {
        var mrzSection = !string.IsNullOrEmpty(mrzLine1) && !string.IsNullOrEmpty(mrzLine2) && !string.IsNullOrEmpty(mrzLine3)
            ? $@"
                   <div class=""mrz-section"">
                     <div class=""mrz-label"">Machine Readable Zone (MRZ)</div>
                     <div class=""mrz-line"">{System.Net.WebUtility.HtmlEncode(mrzLine1)}</div>
                     <div class=""mrz-line"">{System.Net.WebUtility.HtmlEncode(mrzLine2)}</div>
                     <div class=""mrz-line"">{System.Net.WebUtility.HtmlEncode(mrzLine3)}</div>
                   </div>"
            : "<div class=\"muted\">MRZ not available</div>";

        return $@"<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  <title>Passport</title>
  <style>
    body {{ font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif; margin: 24px; }}
    .card {{ border: 2px solid #111; border-radius: 14px; padding: 18px; max-width: 860px; }}
    .row {{ display:flex; justify-content:space-between; gap:16px; }}
    .hdr {{ font-weight: 700; font-size: 20px; }}
    .muted {{ color:#444; font-size: 13px; }}
    .grid {{ display:grid; grid-template-columns: 1fr 1fr; gap: 12px 18px; margin-top: 14px; }}
    .kv .k {{ font-size: 12px; color:#555; text-transform: uppercase; letter-spacing: .06em; }}
    .kv .v {{ font-size: 16px; font-weight: 600; }}
    .photo {{ width: 140px; height: 180px; border: 1px solid #333; border-radius: 10px; background: repeating-linear-gradient(45deg,#eee,#eee 10px,#f7f7f7 10px,#f7f7f7 20px); }}
    .footer {{ margin-top: 16px; display:flex; justify-content:space-between; align-items:flex-end; }}
    .sig {{ width: 220px; height: 58px; border-bottom: 2px solid #111; }}
    .mrz-section {{ margin-top: 20px; padding: 12px; background: #f0f0f0; border-radius: 8px; }}
    .mrz-label {{ font-size: 11px; color: #666; text-transform: uppercase; letter-spacing: 0.1em; margin-bottom: 8px; }}
    .mrz-line {{ font-family: 'Courier New', monospace; font-size: 16px; letter-spacing: 2px; margin: 4px 0; color: #000; }}
  </style>
</head>
<body>
  <div class=""card"">
    <div class=""row"">
      <div>
        <div class=""hdr"">Passport</div>
        <div class=""muted"">Tenant: {System.Net.WebUtility.HtmlEncode(tenantId)}</div>
        <div class=""muted"">Application: {System.Net.WebUtility.HtmlEncode(applicationNumber)}</div>
      </div>
      <div style=""text-align:right"">
        <div class=""muted"">Passport No.</div>
        <div style=""font-weight:800; font-size:18px"">{System.Net.WebUtility.HtmlEncode(passportNumber)}</div>
      </div>
    </div>

    <div class=""row"" style=""margin-top:14px"">
      <div class=""photo""></div>
      <div style=""flex:1"">
        <div class=""grid"">
          <div class=""kv""><div class=""k"">Full name</div><div class=""v"">{System.Net.WebUtility.HtmlEncode(fullName)}</div></div>
          <div class=""kv""><div class=""k"">Date of birth</div><div class=""v"">{dob:yyyy-MM-dd}</div></div>
          <div class=""kv""><div class=""k"">Issued</div><div class=""v"">{issuedAt:yyyy-MM-dd}</div></div>
          <div class=""kv""><div class=""k"">Expires</div><div class=""v"">{expiresAt:yyyy-MM-dd}</div></div>
        </div>
      </div>
    </div>

    {mrzSection}

    <div class=""footer"">
      <div class=""muted"">Official Seal (placeholder)</div>
      <div style=""text-align:right"">
        <div class=""sig""></div>
        <div class=""muted"">Authorized Signature</div>
      </div>
    </div>
  </div>
</body>
</html>";
    }

    public async Task<IssuedDocumentSummary?> GetIssuedDocumentAsync(string tenantId, Guid applicationId, string kind, CancellationToken ct = default)
    {
        return await _db.IssuedDocuments
            .AsNoTracking()
            .Where(x => x.ApplicationId == applicationId && x.Kind == kind)
            .Join(
                _db.Applications.AsNoTracking().Where(a => a.TenantId == tenantId),
                doc => doc.ApplicationId,
                app => app.Id,
                (doc, _) => doc)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new IssuedDocumentSummary(
                x.Id,
                x.ApplicationId,
                x.Kind,
                x.DocumentNumber,
                x.ExpiresAt,
                x.FileName,
                x.ContentType,
                x.Size,
                x.StorageBucket,
                x.StorageKey,
                x.CreatedAt))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IssuedDocumentSummary> InsertIssuedDocumentAsync(string tenantId, IssuedDocumentCreate create, CancellationToken ct = default)
    {
        var appExists = await _db.Applications
            .AsNoTracking()
            .AnyAsync(a => a.TenantId == tenantId && a.Id == create.ApplicationId, ct);

        if (!appExists)
        {
            throw new InvalidOperationException("Application not found (or not in this tenant).");
        }

        var row = new CitizenshipIssuedDocumentRow
        {
            Id = Guid.NewGuid(),
            ApplicationId = create.ApplicationId,
            Kind = create.Kind,
            DocumentNumber = create.DocumentNumber,
            ExpiresAt = create.ExpiresAt,
            FileName = create.FileName,
            ContentType = create.ContentType,
            Size = create.Size,
            StorageBucket = create.StorageBucket,
            StorageKey = create.StorageKey,
            CreatedAt = create.CreatedAt
        };

        _db.IssuedDocuments.Add(row);
        await _db.SaveChangesAsync(ct);

        return new IssuedDocumentSummary(
            row.Id,
            row.ApplicationId,
            row.Kind,
            row.DocumentNumber,
            row.ExpiresAt,
            row.FileName,
            row.ContentType,
            row.Size,
            row.StorageBucket,
            row.StorageKey,
            row.CreatedAt);
    }

    public async Task UpdateApplicationUpdatedAtAsync(string tenantId, Guid applicationId, DateTimeOffset updatedAt, CancellationToken ct = default)
    {
        var application = await _db.Applications
            .SingleOrDefaultAsync(a => a.Id == applicationId && a.TenantId == tenantId, ct);

        if (application is null)
        {
            throw new InvalidOperationException("Application not found (or not in this tenant).");
        }

        application.UpdatedAt = updatedAt;
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateIssuedDocumentAsync(Guid issuedDocumentId, string documentNumber, DateTimeOffset? expiresAt, CancellationToken ct = default)
    {
        var row = await _db.IssuedDocuments
            .SingleOrDefaultAsync(d => d.Id == issuedDocumentId, ct);

        if (row is null)
        {
            throw new InvalidOperationException("Issued document not found.");
        }

        row.DocumentNumber = documentNumber;
        row.ExpiresAt = expiresAt;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<string?> GetApplicationEmailAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == applicationId)
            .Select(x => x.Email)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<ApplicationReissueSnapshot>> GetApplicationsForReissueAsync(string tenantId, string email, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId && a.Email == email &&
                        (a.Status == "Approved" || a.Status == "Completed" || a.Status == "PassportIssued"))
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new ApplicationReissueSnapshot(
                a.Id,
                a.Email,
                a.Status,
                a.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<IssuedDocumentSummary>> GetIssuedDocumentsForApplicationAsync(Guid applicationId, CancellationToken ct = default)
    {
        return await _db.IssuedDocuments
            .AsNoTracking()
            .Where(d => d.ApplicationId == applicationId)
            .Select(d => new IssuedDocumentSummary(
                d.Id,
                d.ApplicationId,
                d.Kind,
                d.DocumentNumber,
                d.ExpiresAt,
                d.FileName,
                d.ContentType,
                d.Size,
                d.StorageBucket,
                d.StorageKey,
                d.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task RemoveIssuedDocumentAsync(Guid issuedDocumentId, CancellationToken ct = default)
    {
        var doc = await _db.IssuedDocuments
            .SingleOrDefaultAsync(d => d.Id == issuedDocumentId, ct);

        if (doc is null)
        {
            return;
        }

        _db.IssuedDocuments.Remove(doc);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> ApplicationExistsAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .AnyAsync(a => a.TenantId == tenantId && a.Id == applicationId, ct);
    }

    public async Task UpsertIntakeReviewAsync(string tenantId, SubmitIntakeReviewRequest request, DateTimeOffset now, CancellationToken ct = default)
    {
        var existingReview = await _db.IntakeReviews
            .FirstOrDefaultAsync(r => r.ApplicationId == request.ApplicationId && r.TenantId == tenantId, ct);

        if (existingReview is not null)
        {
            // Update existing review
            existingReview.ReviewerName = request.ReviewerName;
            existingReview.ReviewDate = request.ReviewDate;
            existingReview.ApplicationComplete = request.ApplicationComplete;
            existingReview.AllDocumentsReceived = request.AllDocumentsReceived;
            existingReview.IdentityVerified = request.IdentityVerified;
            existingReview.BackgroundCheckComplete = request.BackgroundCheckComplete;
            existingReview.BirthCertificateVerified = request.BirthCertificateVerified;
            existingReview.PhotoIdVerified = request.PhotoIdVerified;
            existingReview.ProofOfResidenceVerified = request.ProofOfResidenceVerified;
            existingReview.PassportPhotoVerified = request.PassportPhotoVerified;
            existingReview.SignatureVerified = request.SignatureVerified;
            existingReview.CompletenessNotes = request.CompletenessNotes ?? string.Empty;
            existingReview.DocumentNotes = request.DocumentNotes ?? string.Empty;
            existingReview.AdditionalNotes = request.AdditionalNotes ?? string.Empty;
            existingReview.Recommendation = request.Recommendation;
            existingReview.RecommendationReason = request.RecommendationReason ?? string.Empty;
            existingReview.UpdatedAt = now;
        }
        else
        {
            // Create new review
            var review = new IntakeReviewRow
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ApplicationId = request.ApplicationId,
                ReviewerName = request.ReviewerName,
                ReviewDate = request.ReviewDate,
                ApplicationComplete = request.ApplicationComplete,
                AllDocumentsReceived = request.AllDocumentsReceived,
                IdentityVerified = request.IdentityVerified,
                BackgroundCheckComplete = request.BackgroundCheckComplete,
                BirthCertificateVerified = request.BirthCertificateVerified,
                PhotoIdVerified = request.PhotoIdVerified,
                ProofOfResidenceVerified = request.ProofOfResidenceVerified,
                PassportPhotoVerified = request.PassportPhotoVerified,
                SignatureVerified = request.SignatureVerified,
                CompletenessNotes = request.CompletenessNotes ?? string.Empty,
                DocumentNotes = request.DocumentNotes ?? string.Empty,
                AdditionalNotes = request.AdditionalNotes ?? string.Empty,
                Recommendation = request.Recommendation,
                RecommendationReason = request.RecommendationReason ?? string.Empty,
                CreatedAt = now,
                UpdatedAt = now,
            };

            _db.IntakeReviews.Add(review);
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<ApplicationDecisionSnapshot?> GetApplicationForDecisionAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(a => a.Id == applicationId && a.TenantId == tenantId)
            .Select(a => new ApplicationDecisionSnapshot(
                a.Id,
                a.Status,
                a.Email))
            .SingleOrDefaultAsync(ct);
    }

    public async Task UpdateApplicationStatusAsync(
        string tenantId,
        Guid applicationId,
        string status,
        DateTimeOffset updatedAt,
        string? rejectionReason,
        CancellationToken ct = default)
    {
        var application = await _db.Applications
            .SingleOrDefaultAsync(a => a.Id == applicationId && a.TenantId == tenantId, ct);

        if (application is null)
        {
            throw new ArgumentException("Application not found or does not belong to this tenant.");
        }

        application.Status = status;
        if (rejectionReason is not null)
        {
            application.RejectionReason = rejectionReason;
        }
        application.UpdatedAt = updatedAt;

        await _db.SaveChangesAsync(ct);
    }

    public async Task<IssuedDocumentSummary?> GetIssuedDocumentByIdAsync(Guid issuedDocumentId, CancellationToken ct = default)
    {
        return await _db.IssuedDocuments
            .AsNoTracking()
            .Where(d => d.Id == issuedDocumentId)
            .Select(d => new IssuedDocumentSummary(
                d.Id,
                d.ApplicationId,
                d.Kind,
                d.DocumentNumber,
                d.ExpiresAt,
                d.FileName,
                d.ContentType,
                d.Size,
                d.StorageBucket,
                d.StorageKey,
                d.CreatedAt))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ApplicationRenewalSnapshot?> GetApplicationForRenewalAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        return await _db.Applications
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId && a.Id == applicationId)
            .Select(a => new ApplicationRenewalSnapshot(
                a.Id,
                a.Email))
            .SingleOrDefaultAsync(ct);
    }
}

