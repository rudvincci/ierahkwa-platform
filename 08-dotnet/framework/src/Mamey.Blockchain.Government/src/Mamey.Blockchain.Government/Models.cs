namespace Mamey.Blockchain.Government;

/// <summary>
/// Create identity request
/// </summary>
public class CreateIdentityRequest
{
    public string CitizenId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Create identity result
/// </summary>
public class CreateIdentityResult
{
    public string IdentityId { get; set; } = string.Empty;
    public string BlockchainAccount { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Identity information
/// </summary>
public class IdentityInfo
{
    public string IdentityId { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public IdentityStatus Status { get; set; }
}

/// <summary>
/// Identity status
/// </summary>
public enum IdentityStatus
{
    Pending = 0,
    Active = 1,
    Suspended = 2,
    Revoked = 3
}

/// <summary>
/// Upload document request
/// </summary>
public class UploadDocumentRequest
{
    public string IdentityId { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public byte[] DocumentData { get; set; } = Array.Empty<byte>();
    public string MimeType { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Upload document result
/// </summary>
public class UploadDocumentResult
{
    public string DocumentId { get; set; } = string.Empty;
    public string DocumentHash { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Create vote request
/// </summary>
public class CreateVoteRequest
{
    public string VoteId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public ulong StartTime { get; set; }
    public ulong EndTime { get; set; }
    public string EligibilityCriteria { get; set; } = string.Empty;
}

/// <summary>
/// Create vote result
/// </summary>
public class CreateVoteResult
{
    public string VoteId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Vote results
/// </summary>
public class VoteResults
{
    public string VoteId { get; set; } = string.Empty;
    public int TotalVotes { get; set; }
    public List<VoteResult> Results { get; set; } = new();
}

/// <summary>
/// Vote result
/// </summary>
public class VoteResult
{
    public string Option { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// Compliance check result
/// </summary>
public class ComplianceCheckResult
{
    public bool Compliant { get; set; }
    public List<string> Violations { get; set; } = new();
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}




