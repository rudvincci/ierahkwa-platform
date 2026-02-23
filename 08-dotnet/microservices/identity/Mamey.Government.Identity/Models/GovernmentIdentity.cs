namespace Mamey.Government.Identity.Models;

/// <summary>
/// Government citizen identity with FutureWampumID integration
/// </summary>
public class GovernmentIdentity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FutureWampumId { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    
    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Nationality { get; set; } = "Ierahkwa";
    
    // Contact
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    
    // Address
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    
    // Verification
    public VerificationLevel VerificationLevel { get; set; } = VerificationLevel.None;
    public bool IsVerified => VerificationLevel >= VerificationLevel.Basic;
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationMethod { get; set; }
    
    // Biometric (hash references to FWID.Identities)
    public string? BiometricHash { get; set; }
    public string? FaceTemplateId { get; set; }
    public string? FingerprintTemplateId { get; set; }
    
    // Blockchain
    public string? WalletAddress { get; set; }
    public string? PublicKey { get; set; }
    
    // KYC/AML
    public KycStatus KycStatus { get; set; } = KycStatus.NotStarted;
    public string? KycProofHash { get; set; } // ZKP reference
    public DateTime? KycCompletedAt { get; set; }
    public AmlRiskLevel AmlRiskLevel { get; set; } = AmlRiskLevel.Unknown;
    
    // Membership
    public MembershipTier MembershipTier { get; set; } = MembershipTier.None;
    public decimal ProfitSharePercent { get; set; }
    public string? ReferralCode { get; set; }
    public Guid? ReferredById { get; set; }
    
    // Documents
    public List<IdentityDocument> Documents { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeactivatedAt { get; set; }
    
    // Methods
    public string GetFullName() => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    
    public void SetVerificationLevel(VerificationLevel level, string method)
    {
        VerificationLevel = level;
        VerificationMethod = method;
        VerifiedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void LinkWallet(string walletAddress, string publicKey)
    {
        WalletAddress = walletAddress;
        PublicKey = publicKey;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetMembership(MembershipTier tier)
    {
        MembershipTier = tier;
        ProfitSharePercent = tier switch
        {
            MembershipTier.Bronze => 10,
            MembershipTier.Silver => 15,
            MembershipTier.Gold => 25,
            MembershipTier.Platinum => 35,
            _ => 0
        };
        UpdatedAt = DateTime.UtcNow;
    }
}

public class IdentityDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
    public DocumentType Type { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string? IssuingAuthority { get; set; }
    public string? IssuingCountry { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? DocumentHash { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum Gender { Male, Female, Other, PreferNotToSay }
public enum VerificationLevel { None, Email, Phone, Basic, Enhanced, Biometric, Full }
public enum KycStatus { NotStarted, Pending, InReview, Approved, Rejected, Expired }
public enum AmlRiskLevel { Unknown, Low, Medium, High, Prohibited }
public enum MembershipTier { None, Bronze, Silver, Gold, Platinum }
public enum DocumentType { Passport, NationalId, DriversLicense, BirthCertificate, TribalId, SovereignId, Other }
