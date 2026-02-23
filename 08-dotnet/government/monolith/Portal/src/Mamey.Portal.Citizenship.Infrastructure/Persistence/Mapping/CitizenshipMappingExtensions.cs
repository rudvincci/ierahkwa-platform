using Mamey.Portal.Citizenship.Domain.Entities;
using Mamey.Portal.Citizenship.Domain.ValueObjects;

namespace Mamey.Portal.Citizenship.Infrastructure.Persistence.Mapping;

internal static class CitizenshipMappingExtensions
{
    public static CitizenshipApplication ToDomainEntity(this CitizenshipApplicationRow row)
    {
        var uploads = row.Uploads.Select(ToDomainEntity).ToList();
        var issuedDocuments = row.IssuedDocuments.Select(ToDomainEntity).ToList();

        return CitizenshipApplication.Rehydrate(
            row.Id,
            row.TenantId,
            new ApplicationNumber(row.ApplicationNumber),
            ParseApplicationStatus(row.Status),
            row.FirstName,
            row.LastName,
            row.DateOfBirth,
            row.Email,
            row.MiddleName,
            row.Height,
            row.EyeColor,
            row.HairColor,
            row.Sex,
            row.PhoneNumber,
            row.PlaceOfBirth,
            row.CountryOfOrigin,
            row.MaritalStatus,
            row.PreviousNames,
            row.AddressLine1,
            row.City,
            row.Region,
            row.PostalCode,
            row.AcknowledgeTreaty,
            row.SwearAllegiance,
            row.AffidavitDate,
            row.HasBirthCertificate,
            row.HasPhotoId,
            row.HasProofOfResidence,
            row.HasBackgroundCheck,
            row.AuthorizeBiometricEnrollment,
            row.DeclareUnderstanding,
            row.ConsentToVerification,
            row.ConsentToDataProcessing,
            row.RejectionReason,
            row.ExtendedDataJson,
            row.CreatedAt,
            row.UpdatedAt,
            uploads,
            issuedDocuments);
    }

    public static CitizenshipApplicationRow ToRow(this CitizenshipApplication application)
    {
        return new CitizenshipApplicationRow
        {
            Id = application.Id,
            TenantId = application.TenantId,
            ApplicationNumber = application.ApplicationNumber.Value,
            Status = application.Status.ToString(),
            FirstName = application.FirstName,
            LastName = application.LastName,
            DateOfBirth = application.DateOfBirth,
            Email = application.Email,
            MiddleName = application.MiddleName,
            Height = application.Height,
            EyeColor = application.EyeColor,
            HairColor = application.HairColor,
            Sex = application.Sex,
            PhoneNumber = application.PhoneNumber,
            PlaceOfBirth = application.PlaceOfBirth,
            CountryOfOrigin = application.CountryOfOrigin,
            MaritalStatus = application.MaritalStatus,
            PreviousNames = application.PreviousNames,
            AddressLine1 = application.AddressLine1,
            City = application.City,
            Region = application.Region,
            PostalCode = application.PostalCode,
            AcknowledgeTreaty = application.AcknowledgeTreaty,
            SwearAllegiance = application.SwearAllegiance,
            AffidavitDate = application.AffidavitDate,
            HasBirthCertificate = application.HasBirthCertificate,
            HasPhotoId = application.HasPhotoId,
            HasProofOfResidence = application.HasProofOfResidence,
            HasBackgroundCheck = application.HasBackgroundCheck,
            AuthorizeBiometricEnrollment = application.AuthorizeBiometricEnrollment,
            DeclareUnderstanding = application.DeclareUnderstanding,
            ConsentToVerification = application.ConsentToVerification,
            ConsentToDataProcessing = application.ConsentToDataProcessing,
            RejectionReason = application.RejectionReason,
            ExtendedDataJson = application.ExtendedDataJson,
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };
    }

    public static void UpdateFromDomain(this CitizenshipApplicationRow row, CitizenshipApplication application)
    {
        row.TenantId = application.TenantId;
        row.ApplicationNumber = application.ApplicationNumber.Value;
        row.Status = application.Status.ToString();
        row.FirstName = application.FirstName;
        row.LastName = application.LastName;
        row.DateOfBirth = application.DateOfBirth;
        row.Email = application.Email;
        row.MiddleName = application.MiddleName;
        row.Height = application.Height;
        row.EyeColor = application.EyeColor;
        row.HairColor = application.HairColor;
        row.Sex = application.Sex;
        row.PhoneNumber = application.PhoneNumber;
        row.PlaceOfBirth = application.PlaceOfBirth;
        row.CountryOfOrigin = application.CountryOfOrigin;
        row.MaritalStatus = application.MaritalStatus;
        row.PreviousNames = application.PreviousNames;
        row.AddressLine1 = application.AddressLine1;
        row.City = application.City;
        row.Region = application.Region;
        row.PostalCode = application.PostalCode;
        row.AcknowledgeTreaty = application.AcknowledgeTreaty;
        row.SwearAllegiance = application.SwearAllegiance;
        row.AffidavitDate = application.AffidavitDate;
        row.HasBirthCertificate = application.HasBirthCertificate;
        row.HasPhotoId = application.HasPhotoId;
        row.HasProofOfResidence = application.HasProofOfResidence;
        row.HasBackgroundCheck = application.HasBackgroundCheck;
        row.AuthorizeBiometricEnrollment = application.AuthorizeBiometricEnrollment;
        row.DeclareUnderstanding = application.DeclareUnderstanding;
        row.ConsentToVerification = application.ConsentToVerification;
        row.ConsentToDataProcessing = application.ConsentToDataProcessing;
        row.RejectionReason = application.RejectionReason;
        row.ExtendedDataJson = application.ExtendedDataJson;
        row.CreatedAt = application.CreatedAt;
        row.UpdatedAt = application.UpdatedAt;
    }

    public static CitizenshipUpload ToDomainEntity(this CitizenshipUploadRow row)
    {
        return new CitizenshipUpload(
            row.Id,
            row.ApplicationId,
            new DocumentKind(row.Kind),
            row.FileName,
            row.ContentType,
            row.Size,
            row.StorageBucket,
            row.StorageKey,
            row.UploadedAt);
    }

    public static CitizenshipUploadRow ToRow(this CitizenshipUpload upload)
    {
        return new CitizenshipUploadRow
        {
            Id = upload.Id,
            ApplicationId = upload.ApplicationId,
            Kind = upload.Kind.Value,
            FileName = upload.FileName,
            ContentType = upload.ContentType,
            Size = upload.Size,
            StorageBucket = upload.StorageBucket,
            StorageKey = upload.StorageKey,
            UploadedAt = upload.UploadedAt
        };
    }

    public static void UpdateFromDomain(this CitizenshipUploadRow row, CitizenshipUpload upload)
    {
        row.ApplicationId = upload.ApplicationId;
        row.Kind = upload.Kind.Value;
        row.FileName = upload.FileName;
        row.ContentType = upload.ContentType;
        row.Size = upload.Size;
        row.StorageBucket = upload.StorageBucket;
        row.StorageKey = upload.StorageKey;
        row.UploadedAt = upload.UploadedAt;
    }

    public static IssuedDocument ToDomainEntity(this CitizenshipIssuedDocumentRow row)
    {
        DocumentNumber? documentNumber = string.IsNullOrWhiteSpace(row.DocumentNumber)
            ? null
            : new DocumentNumber(row.DocumentNumber);

        return IssuedDocument.Rehydrate(
            row.Id,
            row.ApplicationId,
            new DocumentKind(row.Kind),
            documentNumber,
            row.ExpiresAt,
            row.FileName,
            row.ContentType,
            row.Size,
            row.StorageBucket,
            row.StorageKey,
            row.CreatedAt);
    }

    public static CitizenshipIssuedDocumentRow ToRow(this IssuedDocument document)
    {
        return new CitizenshipIssuedDocumentRow
        {
            Id = document.Id,
            ApplicationId = document.ApplicationId,
            Kind = document.Kind.Value,
            DocumentNumber = document.DocumentNumber?.Value,
            ExpiresAt = document.ExpiresAt,
            FileName = document.FileName,
            ContentType = document.ContentType,
            Size = document.Size,
            StorageBucket = document.StorageBucket,
            StorageKey = document.StorageKey,
            CreatedAt = document.CreatedAt
        };
    }

    public static void UpdateFromDomain(this CitizenshipIssuedDocumentRow row, IssuedDocument document)
    {
        row.ApplicationId = document.ApplicationId;
        row.Kind = document.Kind.Value;
        row.DocumentNumber = document.DocumentNumber?.Value;
        row.ExpiresAt = document.ExpiresAt;
        row.FileName = document.FileName;
        row.ContentType = document.ContentType;
        row.Size = document.Size;
        row.StorageBucket = document.StorageBucket;
        row.StorageKey = document.StorageKey;
        row.CreatedAt = document.CreatedAt;
    }

    public static CitizenshipStatus ToDomainEntity(this CitizenshipStatusRow row)
    {
        var progressionApps = row.ProgressionApplications.Select(ToDomainEntity).ToList();

        return CitizenshipStatus.Rehydrate(
            row.Id,
            row.TenantId,
            row.Email,
            ParseCitizenshipStatus(row.Status),
            row.ApplicationId,
            row.StatusGrantedAt,
            row.StatusExpiresAt,
            row.YearsCompleted,
            row.CreatedAt,
            row.UpdatedAt,
            progressionApps);
    }

    public static CitizenshipStatusRow ToRow(this CitizenshipStatus status)
    {
        return new CitizenshipStatusRow
        {
            Id = status.Id,
            TenantId = status.TenantId,
            Email = status.Email,
            Status = status.Status.ToString(),
            ApplicationId = status.ApplicationId,
            StatusGrantedAt = status.StatusGrantedAt,
            StatusExpiresAt = status.StatusExpiresAt,
            YearsCompleted = status.YearsCompleted,
            CreatedAt = status.CreatedAt,
            UpdatedAt = status.UpdatedAt
        };
    }

    public static void UpdateFromDomain(this CitizenshipStatusRow row, CitizenshipStatus status)
    {
        row.TenantId = status.TenantId;
        row.Email = status.Email;
        row.Status = status.Status.ToString();
        row.ApplicationId = status.ApplicationId;
        row.StatusGrantedAt = status.StatusGrantedAt;
        row.StatusExpiresAt = status.StatusExpiresAt;
        row.YearsCompleted = status.YearsCompleted;
        row.CreatedAt = status.CreatedAt;
        row.UpdatedAt = status.UpdatedAt;
    }

    public static StatusProgressionApplication ToDomainEntity(this StatusProgressionApplicationRow row)
    {
        return new StatusProgressionApplication(
            row.Id,
            row.TenantId,
            row.CitizenshipStatusId,
            new ApplicationNumber(row.ApplicationNumber),
            ParseCitizenshipStatus(row.TargetStatus),
            ParseApplicationStatus(row.Status),
            row.YearsCompletedAtApplication,
            row.CreatedAt,
            row.UpdatedAt);
    }

    public static StatusProgressionApplicationRow ToRow(this StatusProgressionApplication application)
    {
        return new StatusProgressionApplicationRow
        {
            Id = application.Id,
            TenantId = application.TenantId,
            CitizenshipStatusId = application.CitizenshipStatusId,
            ApplicationNumber = application.ApplicationNumber.Value,
            TargetStatus = application.TargetStatus.ToString(),
            Status = application.Status.ToString(),
            YearsCompletedAtApplication = application.YearsCompletedAtApplication,
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };
    }

    public static void UpdateFromDomain(this StatusProgressionApplicationRow row, StatusProgressionApplication application)
    {
        row.TenantId = application.TenantId;
        row.CitizenshipStatusId = application.CitizenshipStatusId;
        row.ApplicationNumber = application.ApplicationNumber.Value;
        row.TargetStatus = application.TargetStatus.ToString();
        row.Status = application.Status.ToString();
        row.YearsCompletedAtApplication = application.YearsCompletedAtApplication;
        row.CreatedAt = application.CreatedAt;
        row.UpdatedAt = application.UpdatedAt;
    }

    public static IntakeReview ToDomainEntity(this IntakeReviewRow row)
    {
        return new IntakeReview(
            row.Id,
            row.TenantId,
            row.ApplicationId,
            row.ReviewerName,
            row.ReviewDate,
            row.ApplicationComplete,
            row.AllDocumentsReceived,
            row.IdentityVerified,
            row.BackgroundCheckComplete,
            row.BirthCertificateVerified,
            row.PhotoIdVerified,
            row.ProofOfResidenceVerified,
            row.PassportPhotoVerified,
            row.SignatureVerified,
            row.CompletenessNotes,
            row.DocumentNotes,
            row.AdditionalNotes,
            row.Recommendation,
            row.RecommendationReason,
            row.CreatedAt,
            row.UpdatedAt);
    }

    public static IntakeReviewRow ToRow(this IntakeReview review)
    {
        return new IntakeReviewRow
        {
            Id = review.Id,
            TenantId = review.TenantId,
            ApplicationId = review.ApplicationId,
            ReviewerName = review.ReviewerName,
            ReviewDate = review.ReviewDate,
            ApplicationComplete = review.ApplicationComplete,
            AllDocumentsReceived = review.AllDocumentsReceived,
            IdentityVerified = review.IdentityVerified,
            BackgroundCheckComplete = review.BackgroundCheckComplete,
            BirthCertificateVerified = review.BirthCertificateVerified,
            PhotoIdVerified = review.PhotoIdVerified,
            ProofOfResidenceVerified = review.ProofOfResidenceVerified,
            PassportPhotoVerified = review.PassportPhotoVerified,
            SignatureVerified = review.SignatureVerified,
            CompletenessNotes = review.CompletenessNotes,
            DocumentNotes = review.DocumentNotes,
            AdditionalNotes = review.AdditionalNotes,
            Recommendation = review.Recommendation,
            RecommendationReason = review.RecommendationReason,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }

    public static void UpdateFromDomain(this IntakeReviewRow row, IntakeReview review)
    {
        row.TenantId = review.TenantId;
        row.ApplicationId = review.ApplicationId;
        row.ReviewerName = review.ReviewerName;
        row.ReviewDate = review.ReviewDate;
        row.ApplicationComplete = review.ApplicationComplete;
        row.AllDocumentsReceived = review.AllDocumentsReceived;
        row.IdentityVerified = review.IdentityVerified;
        row.BackgroundCheckComplete = review.BackgroundCheckComplete;
        row.BirthCertificateVerified = review.BirthCertificateVerified;
        row.PhotoIdVerified = review.PhotoIdVerified;
        row.ProofOfResidenceVerified = review.ProofOfResidenceVerified;
        row.PassportPhotoVerified = review.PassportPhotoVerified;
        row.SignatureVerified = review.SignatureVerified;
        row.CompletenessNotes = review.CompletenessNotes;
        row.DocumentNotes = review.DocumentNotes;
        row.AdditionalNotes = review.AdditionalNotes;
        row.Recommendation = review.Recommendation;
        row.RecommendationReason = review.RecommendationReason;
        row.CreatedAt = review.CreatedAt;
        row.UpdatedAt = review.UpdatedAt;
    }

    public static PaymentPlan ToDomainEntity(this PaymentPlanRow row)
    {
        return new PaymentPlan(
            row.Id,
            row.TenantId,
            row.ApplicationId,
            row.ApplicationNumber,
            row.Amount,
            row.Currency,
            row.Status,
            row.PaymentReference,
            row.PaymentMethod,
            row.PaymentGateway,
            row.CreatedAt,
            row.UpdatedAt,
            row.PaidAt);
    }

    public static PaymentPlanRow ToRow(this PaymentPlan plan)
    {
        return new PaymentPlanRow
        {
            Id = plan.Id,
            TenantId = plan.TenantId,
            ApplicationId = plan.ApplicationId,
            ApplicationNumber = plan.ApplicationNumber,
            Amount = plan.Amount,
            Currency = plan.Currency,
            Status = plan.Status,
            PaymentReference = plan.PaymentReference,
            PaymentMethod = plan.PaymentMethod,
            PaymentGateway = plan.PaymentGateway,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt,
            PaidAt = plan.PaidAt
        };
    }

    public static void UpdateFromDomain(this PaymentPlanRow row, PaymentPlan plan)
    {
        row.TenantId = plan.TenantId;
        row.ApplicationId = plan.ApplicationId;
        row.ApplicationNumber = plan.ApplicationNumber;
        row.Amount = plan.Amount;
        row.Currency = plan.Currency;
        row.Status = plan.Status;
        row.PaymentReference = plan.PaymentReference;
        row.PaymentMethod = plan.PaymentMethod;
        row.PaymentGateway = plan.PaymentGateway;
        row.CreatedAt = plan.CreatedAt;
        row.UpdatedAt = plan.UpdatedAt;
        row.PaidAt = plan.PaidAt;
    }

    private static ApplicationStatus ParseApplicationStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return ApplicationStatus.Draft;
        }

        return Enum.TryParse(status, true, out ApplicationStatus parsed)
            ? parsed
            : ApplicationStatus.Draft;
    }

    private static CitizenshipStatusType ParseCitizenshipStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return CitizenshipStatusType.Probationary;
        }

        return Enum.TryParse(status, true, out CitizenshipStatusType parsed)
            ? parsed
            : CitizenshipStatusType.Probationary;
    }
}
