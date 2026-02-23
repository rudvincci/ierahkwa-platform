using Microsoft.AspNetCore.Mvc;

namespace NET10.API.Controllers;

/// <summary>
/// Identity Controller - Sovereign Identity & KYC Management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class IdentityController : ControllerBase
{
    private static readonly List<SovereignIdentity> _identities = InitializeIdentities();
    private static readonly List<KYCVerification> _verifications = new();
    private static readonly List<IdentityCredential> _credentials = InitializeCredentials();
    
    /// <summary>
    /// Get identity by ID
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<SovereignIdentity> GetIdentity(Guid id)
    {
        var identity = _identities.FirstOrDefault(i => i.Id == id);
        if (identity == null) return NotFound();
        return Ok(identity);
    }
    
    /// <summary>
    /// Get identity by wallet address
    /// </summary>
    [HttpGet("wallet/{address}")]
    public ActionResult<SovereignIdentity> GetByWallet(string address)
    {
        var identity = _identities.FirstOrDefault(i => 
            i.WalletAddress.Equals(address, StringComparison.OrdinalIgnoreCase));
        if (identity == null) return NotFound();
        return Ok(identity);
    }
    
    /// <summary>
    /// Register new sovereign identity
    /// </summary>
    [HttpPost("register")]
    public ActionResult<SovereignIdentity> Register([FromBody] RegisterIdentityRequest request)
    {
        var identity = new SovereignIdentity
        {
            Id = Guid.NewGuid(),
            DID = $"did:ierahkwa:{Guid.NewGuid():N}",
            WalletAddress = request.WalletAddress,
            DisplayName = request.DisplayName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            NationAffiliation = request.NationAffiliation,
            KYCLevel = KYCLevel.None,
            CreatedAt = DateTime.UtcNow,
            Status = IdentityStatus.Pending
        };
        
        _identities.Add(identity);
        return CreatedAtAction(nameof(GetIdentity), new { id = identity.Id }, identity);
    }
    
    /// <summary>
    /// Submit KYC verification
    /// </summary>
    [HttpPost("{id}/kyc")]
    public ActionResult<KYCVerification> SubmitKYC(Guid id, [FromBody] SubmitKYCRequest request)
    {
        var identity = _identities.FirstOrDefault(i => i.Id == id);
        if (identity == null) return NotFound();
        
        var verification = new KYCVerification
        {
            Id = Guid.NewGuid(),
            IdentityId = id,
            Level = request.Level,
            Documents = request.Documents,
            Status = VerificationStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };
        
        _verifications.Add(verification);
        return Ok(verification);
    }
    
    /// <summary>
    /// Get KYC status
    /// </summary>
    [HttpGet("{id}/kyc")]
    public ActionResult<KYCStatus> GetKYCStatus(Guid id)
    {
        var identity = _identities.FirstOrDefault(i => i.Id == id);
        if (identity == null) return NotFound();
        
        var verifications = _verifications.Where(v => v.IdentityId == id).OrderByDescending(v => v.SubmittedAt).ToList();
        
        return Ok(new KYCStatus
        {
            IdentityId = id,
            CurrentLevel = identity.KYCLevel,
            Verifications = verifications,
            CanUpgrade = identity.KYCLevel < KYCLevel.Full,
            NextLevelRequirements = GetLevelRequirements(identity.KYCLevel + 1)
        });
    }
    
    /// <summary>
    /// Approve KYC (admin)
    /// </summary>
    [HttpPost("kyc/{verificationId}/approve")]
    public ActionResult ApproveKYC(Guid verificationId)
    {
        var verification = _verifications.FirstOrDefault(v => v.Id == verificationId);
        if (verification == null) return NotFound();
        
        verification.Status = VerificationStatus.Approved;
        verification.ReviewedAt = DateTime.UtcNow;
        verification.ReviewedBy = "admin";
        
        var identity = _identities.FirstOrDefault(i => i.Id == verification.IdentityId);
        if (identity != null)
        {
            identity.KYCLevel = verification.Level;
            identity.KYCVerifiedAt = DateTime.UtcNow;
            identity.Status = IdentityStatus.Verified;
        }
        
        return Ok(new { message = "KYC approved successfully" });
    }
    
    /// <summary>
    /// Issue credential
    /// </summary>
    [HttpPost("{id}/credentials")]
    public ActionResult<IdentityCredential> IssueCredential(Guid id, [FromBody] IssueCredentialRequest request)
    {
        var identity = _identities.FirstOrDefault(i => i.Id == id);
        if (identity == null) return NotFound();
        
        var credential = new IdentityCredential
        {
            Id = Guid.NewGuid(),
            IdentityId = id,
            Type = request.Type,
            Issuer = request.Issuer,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = request.ExpiresAt,
            Claims = request.Claims,
            Status = CredentialStatus.Active,
            Hash = GenerateCredentialHash()
        };
        
        _credentials.Add(credential);
        return Ok(credential);
    }
    
    /// <summary>
    /// Get credentials
    /// </summary>
    [HttpGet("{id}/credentials")]
    public ActionResult<List<IdentityCredential>> GetCredentials(Guid id)
    {
        var credentials = _credentials.Where(c => c.IdentityId == id && c.Status == CredentialStatus.Active).ToList();
        return Ok(credentials);
    }
    
    /// <summary>
    /// Verify credential
    /// </summary>
    [HttpGet("credentials/{credentialId}/verify")]
    public ActionResult<CredentialVerificationResult> VerifyCredential(Guid credentialId)
    {
        var credential = _credentials.FirstOrDefault(c => c.Id == credentialId);
        if (credential == null)
            return Ok(new CredentialVerificationResult { IsValid = false, Error = "Credential not found" });
        
        if (credential.Status != CredentialStatus.Active)
            return Ok(new CredentialVerificationResult { IsValid = false, Error = "Credential is not active" });
        
        if (credential.ExpiresAt.HasValue && credential.ExpiresAt < DateTime.UtcNow)
            return Ok(new CredentialVerificationResult { IsValid = false, Error = "Credential has expired" });
        
        return Ok(new CredentialVerificationResult
        {
            IsValid = true,
            Credential = credential,
            VerifiedAt = DateTime.UtcNow
        });
    }
    
    /// <summary>
    /// Get identity statistics
    /// </summary>
    [HttpGet("stats")]
    public ActionResult<IdentityStats> GetStats()
    {
        return Ok(new IdentityStats
        {
            TotalIdentities = _identities.Count,
            VerifiedIdentities = _identities.Count(i => i.Status == IdentityStatus.Verified),
            PendingVerifications = _verifications.Count(v => v.Status == VerificationStatus.Pending),
            CredentialsIssued = _credentials.Count,
            ActiveCredentials = _credentials.Count(c => c.Status == CredentialStatus.Active),
            KYCLevelDistribution = new Dictionary<string, int>
            {
                ["None"] = _identities.Count(i => i.KYCLevel == KYCLevel.None),
                ["Basic"] = _identities.Count(i => i.KYCLevel == KYCLevel.Basic),
                ["Standard"] = _identities.Count(i => i.KYCLevel == KYCLevel.Standard),
                ["Full"] = _identities.Count(i => i.KYCLevel == KYCLevel.Full)
            }
        });
    }
    
    // Helpers
    private static string GenerateCredentialHash() => $"0x{Guid.NewGuid():N}{Guid.NewGuid().ToString("N")[..24]}";
    
    private static List<string> GetLevelRequirements(KYCLevel level)
    {
        return level switch
        {
            KYCLevel.Basic => new List<string> { "Email verification", "Phone verification" },
            KYCLevel.Standard => new List<string> { "Government ID", "Proof of address", "Selfie verification" },
            KYCLevel.Full => new List<string> { "Source of funds", "Enhanced due diligence", "Video verification" },
            _ => new List<string>()
        };
    }
    
    private static List<SovereignIdentity> InitializeIdentities()
    {
        return new List<SovereignIdentity>
        {
            new()
            {
                DID = "did:ierahkwa:a1b2c3d4e5f6789012345678901234567890",
                WalletAddress = "0x1234567890abcdef1234567890abcdef12345678",
                DisplayName = "Ierahkwa Foundation",
                NationAffiliation = "Akwesasne Mohawk",
                KYCLevel = KYCLevel.Full,
                KYCVerifiedAt = DateTime.UtcNow.AddDays(-90),
                Status = IdentityStatus.Verified
            },
            new()
            {
                DID = "did:ierahkwa:b2c3d4e5f67890123456789012345678901a",
                WalletAddress = "0xabcdef1234567890abcdef1234567890abcdef12",
                DisplayName = "Community Member",
                NationAffiliation = "Haudenosaunee Confederacy",
                KYCLevel = KYCLevel.Standard,
                KYCVerifiedAt = DateTime.UtcNow.AddDays(-30),
                Status = IdentityStatus.Verified
            }
        };
    }
    
    private static List<IdentityCredential> InitializeCredentials()
    {
        return new List<IdentityCredential>
        {
            new()
            {
                IdentityId = _identities.First().Id,
                Type = CredentialType.Citizenship,
                Issuer = "Ierahkwa Sovereign Nation",
                Claims = new Dictionary<string, string>
                {
                    ["nation"] = "Akwesasne Mohawk",
                    ["membershipNumber"] = "AKW-001234",
                    ["enrollmentDate"] = "2020-01-15"
                },
                ExpiresAt = DateTime.UtcNow.AddYears(5)
            },
            new()
            {
                IdentityId = _identities.First().Id,
                Type = CredentialType.VotingRights,
                Issuer = "Ierahkwa Governance",
                Claims = new Dictionary<string, string>
                {
                    ["votingPower"] = "50000000",
                    ["delegateStatus"] = "true"
                },
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            }
        };
    }
}

// ═══════════════════════════════════════════════════════════════
// IDENTITY MODELS
// ═══════════════════════════════════════════════════════════════

public class SovereignIdentity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DID { get; set; } = string.Empty; // Decentralized Identifier
    public string WalletAddress { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? NationAffiliation { get; set; }
    public KYCLevel KYCLevel { get; set; } = KYCLevel.None;
    public DateTime? KYCVerifiedAt { get; set; }
    public IdentityStatus Status { get; set; } = IdentityStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastActiveAt { get; set; }
}

public enum KYCLevel
{
    None = 0,
    Basic = 1,
    Standard = 2,
    Full = 3
}

public enum IdentityStatus
{
    Pending,
    Verified,
    Suspended,
    Revoked
}

public class RegisterIdentityRequest
{
    public string WalletAddress { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? NationAffiliation { get; set; }
}

public class KYCVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
    public KYCLevel Level { get; set; }
    public List<KYCDocument> Documents { get; set; } = new();
    public VerificationStatus Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public string? RejectionReason { get; set; }
}

public enum VerificationStatus
{
    Pending,
    InReview,
    Approved,
    Rejected,
    Expired
}

public class KYCDocument
{
    public string Type { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class SubmitKYCRequest
{
    public KYCLevel Level { get; set; }
    public List<KYCDocument> Documents { get; set; } = new();
}

public class KYCStatus
{
    public Guid IdentityId { get; set; }
    public KYCLevel CurrentLevel { get; set; }
    public List<KYCVerification> Verifications { get; set; } = new();
    public bool CanUpgrade { get; set; }
    public List<string> NextLevelRequirements { get; set; } = new();
}

public class IdentityCredential
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
    public CredentialType Type { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public Dictionary<string, string> Claims { get; set; } = new();
    public CredentialStatus Status { get; set; } = CredentialStatus.Active;
    public string Hash { get; set; } = string.Empty;
}

public enum CredentialType
{
    Citizenship,
    VotingRights,
    Membership,
    LandOwnership,
    BusinessLicense,
    ProfessionalCertification,
    EducationDegree,
    HealthRecord,
    TaxCompliance,
    Custom
}

public enum CredentialStatus
{
    Active,
    Suspended,
    Revoked,
    Expired
}

public class IssueCredentialRequest
{
    public CredentialType Type { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public Dictionary<string, string> Claims { get; set; } = new();
}

public class CredentialVerificationResult
{
    public bool IsValid { get; set; }
    public IdentityCredential? Credential { get; set; }
    public DateTime VerifiedAt { get; set; }
    public string? Error { get; set; }
}

public class IdentityStats
{
    public int TotalIdentities { get; set; }
    public int VerifiedIdentities { get; set; }
    public int PendingVerifications { get; set; }
    public int CredentialsIssued { get; set; }
    public int ActiveCredentials { get; set; }
    public Dictionary<string, int> KYCLevelDistribution { get; set; } = new();
}
