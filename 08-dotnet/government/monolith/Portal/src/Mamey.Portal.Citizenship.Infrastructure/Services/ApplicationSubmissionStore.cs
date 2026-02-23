using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;

namespace Mamey.Portal.Citizenship.Infrastructure.Services;

public sealed class ApplicationSubmissionStore : IApplicationSubmissionStore
{
    private readonly CitizenshipDbContext _db;

    public ApplicationSubmissionStore(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task SaveAsync(ApplicationSubmission submission, IReadOnlyList<ApplicationUploadRecord> uploads, CancellationToken ct = default)
    {
        var row = new CitizenshipApplicationRow
        {
            Id = submission.Id,
            TenantId = submission.TenantId,
            ApplicationNumber = submission.ApplicationNumber,
            Status = submission.Status,
            FirstName = submission.FirstName,
            LastName = submission.LastName,
            DateOfBirth = submission.DateOfBirth,
            Email = submission.Email,
            MiddleName = submission.MiddleName,
            Height = submission.Height,
            EyeColor = submission.EyeColor,
            HairColor = submission.HairColor,
            Sex = submission.Sex,
            PhoneNumber = submission.PhoneNumber,
            PlaceOfBirth = submission.PlaceOfBirth,
            CountryOfOrigin = submission.CountryOfOrigin,
            MaritalStatus = submission.MaritalStatus,
            PreviousNames = submission.PreviousNames,
            AddressLine1 = submission.AddressLine1,
            City = submission.City,
            Region = submission.Region,
            PostalCode = submission.PostalCode,
            AcknowledgeTreaty = submission.AcknowledgeTreaty,
            SwearAllegiance = submission.SwearAllegiance,
            AffidavitDate = submission.AffidavitDate,
            HasBirthCertificate = submission.HasBirthCertificate,
            HasPhotoId = submission.HasPhotoId,
            HasProofOfResidence = submission.HasProofOfResidence,
            HasBackgroundCheck = submission.HasBackgroundCheck,
            AuthorizeBiometricEnrollment = submission.AuthorizeBiometricEnrollment,
            DeclareUnderstanding = submission.DeclareUnderstanding,
            ConsentToVerification = submission.ConsentToVerification,
            ConsentToDataProcessing = submission.ConsentToDataProcessing,
            ExtendedDataJson = submission.ExtendedDataJson,
            CreatedAt = submission.CreatedAt,
            UpdatedAt = submission.UpdatedAt,
        };

        foreach (var upload in uploads)
        {
            row.Uploads.Add(new CitizenshipUploadRow
            {
                Id = upload.Id,
                ApplicationId = upload.ApplicationId,
                Kind = upload.Kind,
                FileName = upload.FileName,
                ContentType = upload.ContentType,
                Size = upload.Size,
                StorageBucket = upload.StorageBucket,
                StorageKey = upload.StorageKey,
                UploadedAt = upload.UploadedAt,
            });
        }

        _db.Applications.Add(row);
        await _db.SaveChangesAsync(ct);
    }
}
