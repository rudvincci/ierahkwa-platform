using System.Text.Json;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Types;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;

/// <summary>
/// Citizenship application aggregate root.
/// Handles the application workflow from submission to approval/rejection.
/// </summary>
internal class CitizenshipApplication : AggregateRoot<AppId>
{
    private readonly List<UploadedDocument> _uploads = new();

    private CitizenshipApplication() { }

    // Primary constructor - maintains monolith structure
    public CitizenshipApplication(
        AppId id,
        TenantId tenantId,
        ApplicationNumber applicationNumber,
        Name applicantName,
        DateTime dateOfBirth,
        Email? email = null,
        ApplicationStatus status = ApplicationStatus.Draft,
        int version = 0)
        : base(id, version)
    {
        TenantId = tenantId;
        ApplicationNumber = applicationNumber;
        ApplicantName = applicantName;
        DateOfBirth = dateOfBirth;
        Email = email;
        Status = status;
        Step = CitizenshipApplicationStep.Initial;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Extended constructor with all code9 properties
    public CitizenshipApplication(
        AppId id,
        TenantId tenantId,
        ApplicationNumber applicationNumber,
        Name applicantName,
        Email email,
        PersonalDetails personalDetails,
        ApplicationStatus status,
        CitizenshipApplicationStep step,
        DateTime createdAt,
        ContactInformation? contactInformation = null,
        ForeignIdentification? foreignIdentification = null,
        bool? haveForeignCitizenshipApplication = null,
        bool? haveCriminalRecord = null,
        List<Dependent>? dependents = null,
        List<Residency>? residencyHistory = null,
        List<ImmigrationHistory>? immigrationHistories = null,
        List<EducationQualification>? educationQualifications = null,
        List<EmploymentHistory>? employmentHistory = null,
        List<ForeignCitizenshipApplication>? foreignCitizenshipApplications = null,
        List<CriminalHistory>? criminalHistory = null,
        List<Reference>? references = null,
        Guid? paymentTransactionId = null,
        UserId? reviewedBy = null,
        DateTime? submittedAt = null,
        DateTime? approvedAt = null,
        DateTime? rejectedAt = null,
        bool isPrimaryApplication = true,
        int version = 0)
        : base(id, version)
    {
        TenantId = tenantId;
        ApplicationNumber = applicationNumber;
        ApplicantName = applicantName;
        Email = email;
        PersonalDetails = personalDetails;
        ContactInformation = contactInformation;
        ForeignIdentification = foreignIdentification;
        HaveForeignCitizenshipApplication = haveForeignCitizenshipApplication;
        HaveCriminalRecord = haveCriminalRecord;
        Dependents = dependents;
        ResidencyHistory = residencyHistory;
        ImmigrationHistories = immigrationHistories;
        EducationQualifications = educationQualifications;
        EmploymentHistory = employmentHistory;
        ForeignCitizenshipApplications = foreignCitizenshipApplications;
        CriminalHistory = criminalHistory;
        References = references;
        PaymentTransactionId = paymentTransactionId;
        ReviewedBy = reviewedBy;
        SubmittedAt = submittedAt;
        ApprovedAt = approvedAt;
        RejectedAt = rejectedAt;
        IsPrimaryApplication = isPrimaryApplication;
        Status = status;
        Step = step;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    // Core properties (monolith structure)
    public TenantId TenantId { get; private set; }
    public ApplicationNumber ApplicationNumber { get; private set; } = null!;
    public Name ApplicantName { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }
    public Email? Email { get; private set; }
    public Phone? Phone { get; private set; }
    public Address? Address { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public CitizenshipApplicationStep Step { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? ExtendedDataJson { get; private set; } // For CIT-001-A through H form data
    public string? AccessLogsJson { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ICollection<UploadedDocument> Uploads = new List<UploadedDocument>();

    // Extended properties from code9
    public bool IsPrimaryApplication { get; set; }
    public bool? HaveForeignCitizenshipApplication { get; set; }
    public bool? HaveCriminalRecord { get; set; }
    public PersonalDetails? PersonalDetails { get; set; }
    public ContactInformation? ContactInformation { get; set; }
    public ForeignIdentification? ForeignIdentification { get; set; }
    public List<Dependent>? Dependents { get; set; }
    public List<Residency>? ResidencyHistory { get; set; }
    public List<ImmigrationHistory>? ImmigrationHistories { get; set; }
    public List<EducationQualification>? EducationQualifications { get; set; }
    public List<EmploymentHistory>? EmploymentHistory { get; set; }
    public List<ForeignCitizenshipApplication>? ForeignCitizenshipApplications { get; set; }
    public List<CriminalHistory>? CriminalHistory { get; set; }
    public List<Reference>? References { get; set; }

    // Payment and review tracking
    public Guid? PaymentTransactionId { get; set; }
    public bool IsPaymentProcessed => PaymentTransactionId.HasValue;
    public UserId? ReviewedBy { get; private set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }

    // Approval/Rejection tracking (monolith)
    public string? ApprovedBy { get; private set; }
    public string? RejectedBy { get; private set; }

    // Rush processing flags
    public bool RushToCitizen { get; private set; } = false;
    public bool RushToDiplomacy { get; private set; } = false;

    // Fees
    public virtual decimal Fee { get; private set; } = 250M;
    public decimal IdentificationCardFee { get; set; } = 50M;

    // Convenience properties
    public string FirstName => ApplicantName.FirstName;
    public string LastName => ApplicantName.LastName;

    // Existing methods from monolith
    public void Submit()
    {
        if (Status != ApplicationStatus.Draft && Status != ApplicationStatus.Rejected)
        {
            throw new InvalidOperationException($"Cannot submit application in {Status} status");
        }

        Status = ApplicationStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ApplicationStatus newStatus, string? reason = null)
    {
        if (Status == newStatus) return;

        Status = newStatus;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string approvedBy)
    {
        if (Status != ApplicationStatus.ReviewPending)
        {
            throw new InvalidOperationException($"Cannot approve application in {Status} status");
        }

        Status = ApplicationStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string reason, string rejectedBy)
    {
        if (Status == ApplicationStatus.Approved)
        {
            throw new InvalidOperationException("Cannot reject an approved application");
        }

        Status = ApplicationStatus.Rejected;
        RejectionReason = reason;
        RejectedBy = rejectedBy;
        RejectedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddUpload(UploadedDocument document)
    {
        _uploads.Add(document);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePersonalDetails(Name applicantName, DateTime dateOfBirth, Email? email, Phone? phone, Address? address)
    {
        ApplicantName = applicantName;
        DateOfBirth = dateOfBirth;
        Email = email;
        Phone = phone;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateExtendedData(string extendedDataJson)
    {
        ExtendedDataJson = extendedDataJson;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAccessLog(ApplicationAccessLog log)
    {
        var logs = string.IsNullOrWhiteSpace(AccessLogsJson)
            ? new List<ApplicationAccessLog>()
            : JsonSerializer.Deserialize<List<ApplicationAccessLog>>(AccessLogsJson) ?? new List<ApplicationAccessLog>();

        logs.Add(log);
        AccessLogsJson = JsonSerializer.Serialize(logs);
        UpdatedAt = DateTime.UtcNow;
    }

    // New methods from code9
    public void UpdateContactInfo(ContactInformation contactInformation)
    {
        if (contactInformation is null)
        {
            throw new ArgumentNullException(nameof(contactInformation));
        }
        ContactInformation = contactInformation;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStep(CitizenshipApplicationStep step)
    {
        Step = step switch
        {
            CitizenshipApplicationStep.Initial
                => throw new Exception($"Cannot update step to {step}"),
            CitizenshipApplicationStep.PersonalDetailsComplete
                => Step == CitizenshipApplicationStep.Initial
                    ? CitizenshipApplicationStep.PersonalDetailsComplete
                    : throw new Exception($"Cannot update step to {step}"),
            CitizenshipApplicationStep.ContactInformationComplete
                => Step == CitizenshipApplicationStep.PersonalDetailsComplete
                    ? CitizenshipApplicationStep.ContactInformationComplete
                    : throw new Exception($"Cannot update step to {step}"),
            CitizenshipApplicationStep.PassportAndIdentificationComplete
                => Step == CitizenshipApplicationStep.ContactInformationComplete
                    ? CitizenshipApplicationStep.PassportAndIdentificationComplete
                    : throw new Exception($"Cannot update step to {step}"),
            CitizenshipApplicationStep.ResidencyAndImmigrationComplete
                => Step == CitizenshipApplicationStep.PassportAndIdentificationComplete
                    ? CitizenshipApplicationStep.ResidencyAndImmigrationComplete
                    : throw new Exception($"Cannot update step to {step}"),
            CitizenshipApplicationStep.EmploymentAndEducationComplete
                => Step == CitizenshipApplicationStep.ResidencyAndImmigrationComplete
                    ? CitizenshipApplicationStep.EmploymentAndEducationComplete
                    : throw new Exception($"Cannot update step to {step}"),
            _ => throw new Exception("Invalid citizenship application step.")
        };

        UpdatedAt = DateTime.UtcNow;
    }
}