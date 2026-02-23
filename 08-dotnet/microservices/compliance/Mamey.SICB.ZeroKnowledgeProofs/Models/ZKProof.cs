namespace Mamey.SICB.ZeroKnowledgeProofs.Models;

/// <summary>
/// Zero Knowledge Proof for privacy-preserving verification
/// Supports: Identity, Balance, Treaty Compliance, Age, Membership
/// </summary>
public class ZKProof
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ProofId { get; set; } = string.Empty;
    public ZKProofType Type { get; set; }
    public ZKCircuit Circuit { get; set; }
    
    // Proof data
    public string ProofData { get; set; } = string.Empty;
    public string PublicInputs { get; set; } = string.Empty;
    public string VerificationKey { get; set; } = string.Empty;
    
    // Metadata
    public string ProverAddress { get; set; } = string.Empty;
    public string? Statement { get; set; }
    
    // Verification
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifierAddress { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    
    // On-chain reference
    public string? TransactionHash { get; set; }
    public ulong? BlockNumber { get; set; }
}

/// <summary>
/// ZK Proof verification request
/// </summary>
public class ZKVerificationRequest
{
    public Guid ProofId { get; set; }
    public string VerifierAddress { get; set; } = string.Empty;
    public string? Challenge { get; set; }
}

/// <summary>
/// ZK Proof generation request
/// </summary>
public class ZKProofRequest
{
    public ZKProofType Type { get; set; }
    public ZKCircuit Circuit { get; set; } = ZKCircuit.Groth16;
    public string ProverAddress { get; set; } = string.Empty;
    public Dictionary<string, string> PrivateInputs { get; set; } = new();
    public Dictionary<string, string> PublicInputs { get; set; } = new();
    public string? Statement { get; set; }
}

/// <summary>
/// Identity proof - proves identity without revealing personal data
/// </summary>
public class IdentityProof : ZKProof
{
    public string FutureWampumIdHash { get; set; } = string.Empty;
    public bool ProvesAge { get; set; }
    public bool ProvesNationality { get; set; }
    public bool ProvesMembership { get; set; }
}

/// <summary>
/// Balance proof - proves sufficient balance without revealing exact amount
/// </summary>
public class BalanceProof : ZKProof
{
    public string AccountHash { get; set; } = string.Empty;
    public string TokenSymbol { get; set; } = string.Empty;
    public string MinimumBalance { get; set; } = string.Empty; // Proves balance >= this
}

/// <summary>
/// Treaty compliance proof
/// </summary>
public class TreatyComplianceProof : ZKProof
{
    public string TreatyId { get; set; } = string.Empty;
    public string TreatyHash { get; set; } = string.Empty;
    public List<string> ComplianceClaimHashes { get; set; } = new();
}

/// <summary>
/// Age proof - proves age >= threshold without revealing exact date
/// </summary>
public class AgeProof : ZKProof
{
    public int AgeThreshold { get; set; }
    public string DateOfBirthHash { get; set; } = string.Empty;
}

/// <summary>
/// Membership tier proof
/// </summary>
public class MembershipProof : ZKProof
{
    public string MembershipTierHash { get; set; } = string.Empty;
    public int MinimumTier { get; set; } // 0=None, 1=Bronze, 2=Silver, 3=Gold, 4=Platinum
}

public enum ZKProofType
{
    Identity,
    Balance,
    TreatyCompliance,
    Age,
    Membership,
    Transaction,
    Ownership,
    Custom
}

public enum ZKCircuit
{
    Groth16,    // Most common, small proofs
    Plonk,      // Universal setup
    Stark,      // Transparent, quantum-resistant
    Bulletproofs // Range proofs
}
