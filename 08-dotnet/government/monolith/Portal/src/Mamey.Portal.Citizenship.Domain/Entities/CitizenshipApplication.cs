using Mamey.Portal.Citizenship.Domain.Events;
using Mamey.Portal.Citizenship.Domain.Exceptions;
using Mamey.Portal.Citizenship.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Portal.Citizenship.Domain.Entities;

public sealed class CitizenshipApplication : AggregateRoot<Guid>
{
    public string TenantId { get; private set; } = string.Empty;
    public ApplicationNumber ApplicationNumber { get; private set; }
    public ApplicationStatus Status { get; private set; }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public string? Email { get; private set; }

    public string? MiddleName { get; private set; }
    public string? Height { get; private set; }
    public string? EyeColor { get; private set; }
    public string? HairColor { get; private set; }
    public string? Sex { get; private set; }

    public string? PhoneNumber { get; private set; }
    public string? PlaceOfBirth { get; private set; }
    public string? CountryOfOrigin { get; private set; }
    public string? MaritalStatus { get; private set; }
    public string? PreviousNames { get; private set; }

    public string? AddressLine1 { get; private set; }
    public string? City { get; private set; }
    public string? Region { get; private set; }
    public string? PostalCode { get; private set; }

    public bool AcknowledgeTreaty { get; private set; }
    public bool SwearAllegiance { get; private set; }
    public DateTime? AffidavitDate { get; private set; }
    public bool HasBirthCertificate { get; private set; }
    public bool HasPhotoId { get; private set; }
    public bool HasProofOfResidence { get; private set; }
    public bool HasBackgroundCheck { get; private set; }
    public bool AuthorizeBiometricEnrollment { get; private set; }
    public bool DeclareUnderstanding { get; private set; }
    public bool ConsentToVerification { get; private set; }
    public bool ConsentToDataProcessing { get; private set; }

    public string? RejectionReason { get; private set; }
    public string? ExtendedDataJson { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public List<CitizenshipUpload> Uploads { get; private set; } = new();
    public List<IssuedDocument> IssuedDocuments { get; private set; } = new();

    private CitizenshipApplication() { }

    public CitizenshipApplication(
        Guid id,
        string tenantId,
        ApplicationNumber applicationNumber,
        ApplicationStatus status,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string? email,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
        : base(id)
    {
        TenantId = tenantId;
        ApplicationNumber = applicationNumber;
        Status = status;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Email = email;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static CitizenshipApplication Rehydrate(
        Guid id,
        string tenantId,
        ApplicationNumber applicationNumber,
        ApplicationStatus status,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string? email,
        string? middleName,
        string? height,
        string? eyeColor,
        string? hairColor,
        string? sex,
        string? phoneNumber,
        string? placeOfBirth,
        string? countryOfOrigin,
        string? maritalStatus,
        string? previousNames,
        string? addressLine1,
        string? city,
        string? region,
        string? postalCode,
        bool acknowledgeTreaty,
        bool swearAllegiance,
        DateTime? affidavitDate,
        bool hasBirthCertificate,
        bool hasPhotoId,
        bool hasProofOfResidence,
        bool hasBackgroundCheck,
        bool authorizeBiometricEnrollment,
        bool declareUnderstanding,
        bool consentToVerification,
        bool consentToDataProcessing,
        string? rejectionReason,
        string? extendedDataJson,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt,
        IReadOnlyList<CitizenshipUpload> uploads,
        IReadOnlyList<IssuedDocument> issuedDocuments)
    {
        var application = new CitizenshipApplication(
            id,
            tenantId,
            applicationNumber,
            status,
            firstName,
            lastName,
            dateOfBirth,
            email,
            createdAt,
            updatedAt)
        {
            MiddleName = middleName,
            Height = height,
            EyeColor = eyeColor,
            HairColor = hairColor,
            Sex = sex,
            PhoneNumber = phoneNumber,
            PlaceOfBirth = placeOfBirth,
            CountryOfOrigin = countryOfOrigin,
            MaritalStatus = maritalStatus,
            PreviousNames = previousNames,
            AddressLine1 = addressLine1,
            City = city,
            Region = region,
            PostalCode = postalCode,
            AcknowledgeTreaty = acknowledgeTreaty,
            SwearAllegiance = swearAllegiance,
            AffidavitDate = affidavitDate,
            HasBirthCertificate = hasBirthCertificate,
            HasPhotoId = hasPhotoId,
            HasProofOfResidence = hasProofOfResidence,
            HasBackgroundCheck = hasBackgroundCheck,
            AuthorizeBiometricEnrollment = authorizeBiometricEnrollment,
            DeclareUnderstanding = declareUnderstanding,
            ConsentToVerification = consentToVerification,
            ConsentToDataProcessing = consentToDataProcessing,
            RejectionReason = rejectionReason,
            ExtendedDataJson = extendedDataJson,
            Uploads = uploads.ToList(),
            IssuedDocuments = issuedDocuments.ToList()
        };

        return application;
    }

    public void Submit(DateTimeOffset submittedAt)
    {
        if (Status is not ApplicationStatus.Draft and not ApplicationStatus.Rejected)
        {
            throw new InvalidApplicationStatusException(Id, Status, ApplicationStatus.Submitted);
        }

        Status = ApplicationStatus.Submitted;
        UpdatedAt = submittedAt;
        AddEvent(new ApplicationSubmitted(Id, ApplicationNumber.Value, submittedAt));
    }

    public void Approve(DateTimeOffset approvedAt)
    {
        if (Status != ApplicationStatus.ReviewPending)
        {
            throw new InvalidApplicationStatusException(Id, Status, ApplicationStatus.Approved);
        }

        Status = ApplicationStatus.Approved;
        UpdatedAt = approvedAt;
        AddEvent(new ApplicationApproved(Id, ApplicationNumber.Value, approvedAt));
    }

    public void Reject(string reason, DateTimeOffset rejectedAt)
    {
        if (Status == ApplicationStatus.Approved)
        {
            throw new InvalidApplicationStatusException(Id, Status, ApplicationStatus.Rejected);
        }

        Status = ApplicationStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = rejectedAt;
        AddEvent(new ApplicationRejected(Id, ApplicationNumber.Value, reason, rejectedAt));
    }
}
