using Microsoft.AspNetCore.Authorization;

namespace Mamey.Identity.Decentralized.Middlewares;

/// <summary>
/// Attribute for endpoint or controller-level DID/VC-based authorization.
/// Supports filtering by credential type, issuer DID, or custom claims.
/// </summary>
/// <example>
/// [DidAuth(CredentialType = "AlumniCredential", IssuerDid = "did:example:issuer1", RequiredClaim = "alumniOf", RequiredClaimValue = "did:example:uni1")]
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class DidAuthAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Required type of credential (e.g., "AlumniCredential").
    /// </summary>
    public string CredentialType { get; set; }

    /// <summary>
    /// Required issuer DID.
    /// </summary>
    public string IssuerDid { get; set; }

    /// <summary>
    /// Required claim type (if you want to enforce a particular claim exists).
    /// </summary>
    public string RequiredClaim { get; set; }

    /// <summary>
    /// Required claim value (optional; enforces both type and value).
    /// </summary>
    public string RequiredClaimValue { get; set; }

    /// <summary>
    /// Whether to require a Verifiable Presentation (not just a VC).
    /// </summary>
    public bool RequirePresentation { get; set; } = false;

    /// <summary>
    /// Initializes the attribute and sets the policy for ASP.NET Core.
    /// </summary>
    public DidAuthAttribute()
    {
        Policy = "DidAuthPolicy";
    }
}