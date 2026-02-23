namespace MameyNode.Portals.Mocks.Models;

// Models based on government.proto and compliance.proto

public enum IdentityStatus
{
    Unspecified = 0,
    Pending = 1,
    Verified = 2,
    Suspended = 3,
    Revoked = 4
}

public enum DocumentStatus
{
    Unspecified = 0,
    Pending = 1,
    Verified = 2,
    Rejected = 3,
    Expired = 4
}

public enum VoteStatus
{
    Unspecified = 0,
    Open = 1,
    Closed = 2,
    Counting = 3,
    Completed = 4
}

public enum ComplianceStatus
{
    Unspecified = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    RequiresReview = 4,
    Flagged = 5
}

public class IdentityInfo
{
    public string IdentityId { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public IdentityStatus Status { get; set; }
    public string BlockchainAccount { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
}

public class DIDInfo
{
    public string DID { get; set; } = string.Empty;
    public DIDStatus Status { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public ulong Version { get; set; }
    public string Controller { get; set; } = string.Empty;
}

public enum DIDStatus
{
    Unspecified = 0,
    Active = 1,
    Deactivated = 2,
    Suspended = 3
}

public class DocumentInfo
{
    public string DocumentId { get; set; } = string.Empty;
    public string IdentityId { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty; // Passport, ID, etc.
    public string DocumentNumber { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string IssuingAuthority { get; set; } = string.Empty;
}

public class VoteInfo
{
    public string VoteId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public VoteStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalVotes { get; set; }
    public Dictionary<string, int> OptionVotes { get; set; } = new();
}

public class VoteResult
{
    public string VoteId { get; set; } = string.Empty;
    public Dictionary<string, int> Results { get; set; } = new();
    public int TotalVotes { get; set; }
    public string Winner { get; set; } = string.Empty;
    public DateTime CalculatedAt { get; set; }
}

public class ComplianceCheckInfo
{
    public string CheckId { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string CheckType { get; set; } = string.Empty; // AML, KYC, etc.
    public ComplianceStatus Status { get; set; }
    public bool Passed { get; set; }
    public string Result { get; set; } = string.Empty;
    public DateTime CheckedAt { get; set; }
    public string CheckedBy { get; set; } = string.Empty;
}

public class CitizenshipApplicationInfo
{
    public string ApplicationId { get; set; } = string.Empty;
    public string ApplicantId { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string ReviewerId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class CitizenInfo
{
    public string CitizenId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime RegisteredAt { get; set; }
    public string IdentityId { get; set; } = string.Empty;
}

public class PassportInfo
{
    public string PassportId { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string IssuingCountry { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}

public class TravelIdentityInfo
{
    public string TravelId { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    public string IdentityType { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public List<string> AuthorizedCountries { get; set; } = new();
}

public class DiplomatInfo
{
    public string DiplomatId { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    public string DiplomaticRank { get; set; } = string.Empty;
    public string AssignmentCountry { get; set; } = string.Empty;
    public DateTime AssignmentStart { get; set; }
    public DateTime? AssignmentEnd { get; set; }
    public string Status { get; set; } = "Active";
}

public class PaymentPlanInfo
{
    public string PlanId { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Frequency { get; set; } = "Monthly";
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active";
}

// Compliance Models (from compliance.proto)
public class AMLFlagInfo
{
    public string FlagId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "Low";
    public DateTime CreatedAt { get; set; }
    public FlagStatus Status { get; set; }
}

public enum FlagStatus
{
    Unspecified = 0,
    Active = 1,
    Resolved = 2,
    Dismissed = 3
}

public class KYCStatusInfo
{
    public string AccountId { get; set; } = string.Empty;
    public string KYCLevel { get; set; } = "basic"; // basic, standard, enhanced
    public string Status { get; set; } = "Pending";
    public DateTime VerifiedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public List<string> VerifiedAttributes { get; set; } = new();
}

public class FraudReportInfo
{
    public string FraudReportId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string FraudType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ReportedAt { get; set; }
    public FraudReportStatus Status { get; set; }
}

public enum FraudReportStatus
{
    Unspecified = 0,
    Pending = 1,
    UnderReview = 2,
    Confirmed = 3,
    Dismissed = 4
}

public class AuditEntryInfo
{
    public string AuditEntryId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Details { get; set; } = new();
}

public class RedFlagInfo
{
    public string RedFlagId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string FlagType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low";
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public RedFlagStatus Status { get; set; }
}

public enum RedFlagStatus
{
    Unspecified = 0,
    Active = 1,
    Resolved = 2,
    Dismissed = 3
}

// RBAC Models
public class RoleInfo
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PermissionInfo
{
    public string PermissionId { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class RoleHierarchyInfo
{
    public string HierarchyId { get; set; } = string.Empty;
    public string ParentRoleId { get; set; } = string.Empty;
    public string ChildRoleId { get; set; } = string.Empty;
    public int Level { get; set; }
}

