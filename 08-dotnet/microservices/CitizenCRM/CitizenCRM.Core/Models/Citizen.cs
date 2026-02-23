namespace CitizenCRM.Core.Models;

public class Citizen
{
    public Guid Id { get; set; }
    public string CitizenId { get; set; } = string.Empty; // National ID
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public Gender Gender { get; set; }
    public MaritalStatus MaritalStatus { get; set; }
    public string? Nationality { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? PhotoUrl { get; set; }
    public CitizenStatus Status { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationMethod { get; set; }
    public string? WalletAddress { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? Occupation { get; set; }
    public string? Employer { get; set; }
    public string? TaxId { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public string? PassportNumber { get; set; }
    public string? DriversLicense { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastInteractionAt { get; set; }

    public List<CitizenCase> Cases { get; set; } = new();
    public List<CitizenDocument> Documents { get; set; } = new();
    public List<CitizenInteraction> Interactions { get; set; } = new();
}

public class CitizenCase
{
    public Guid Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public Guid CitizenId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CaseType Type { get; set; }
    public CaseStatus Status { get; set; }
    public CasePriority Priority { get; set; }
    public string? Department { get; set; }
    public Guid? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? Resolution { get; set; }
    public int? SatisfactionRating { get; set; }
    public string? Feedback { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<CaseNote> Notes { get; set; } = new();
    public List<CaseActivity> Activities { get; set; } = new();
}

public class CitizenInteraction
{
    public Guid Id { get; set; }
    public Guid CitizenId { get; set; }
    public Guid? CaseId { get; set; }
    public InteractionType Type { get; set; }
    public InteractionChannel Channel { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Content { get; set; }
    public InteractionDirection Direction { get; set; }
    public Guid? AgentId { get; set; }
    public string? AgentName { get; set; }
    public int? DurationSeconds { get; set; }
    public DateTime InteractionAt { get; set; }
    public string? Sentiment { get; set; }
}

public class CitizenDocument
{
    public Guid Id { get; set; }
    public Guid CitizenId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public bool IsVerified { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class CaseNote
{
    public Guid Id { get; set; }
    public Guid CaseId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CaseActivity
{
    public Guid Id { get; set; }
    public Guid CaseId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public Guid PerformedBy { get; set; }
    public string PerformedByName { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
}

public class ServiceRequest
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public Guid CitizenId { get; set; }
    public string CitizenName { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public string? Department { get; set; }
    public ServiceRequestStatus Status { get; set; }
    public string? FormData { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? ProcessedBy { get; set; }
    public string? Notes { get; set; }
    public decimal? Fee { get; set; }
    public bool FeePaid { get; set; }
}

public enum Gender { Male, Female, Other, PreferNotToSay }
public enum MaritalStatus { Single, Married, Divorced, Widowed, Separated, Other }
public enum CitizenStatus { Active, Inactive, Deceased, Emigrated, Suspended }
public enum CaseType { Inquiry, Complaint, Request, Suggestion, Emergency, Legal, Financial, Other }
public enum CaseStatus { New, Open, InProgress, Pending, Resolved, Closed, Cancelled }
public enum CasePriority { Low, Medium, High, Urgent, Critical }
public enum InteractionType { Call, Email, Chat, InPerson, Social, SMS, Letter }
public enum InteractionChannel { Phone, Email, WebChat, Mobile, Office, Mail, Portal }
public enum InteractionDirection { Inbound, Outbound }
public enum ServiceRequestStatus { Submitted, UnderReview, Approved, Rejected, InProgress, Completed, Cancelled }
