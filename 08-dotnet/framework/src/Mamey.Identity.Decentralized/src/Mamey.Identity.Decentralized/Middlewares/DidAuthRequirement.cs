using Microsoft.AspNetCore.Authorization;

namespace Mamey.Identity.Decentralized.Middlewares;

public abstract class DidAuthRequirement : IAuthorizationRequirement
{
    public string CredentialType { get; }
    public string IssuerDid { get; }
    public string RequiredClaim { get; }
    public string RequiredClaimValue { get; }
    public bool RequirePresentation { get; }

    public DidAuthRequirement(string credentialType, string issuerDid, string requiredClaim, string requiredClaimValue,
        bool requirePresentation)
    {
        CredentialType = credentialType;
        IssuerDid = issuerDid;
        RequiredClaim = requiredClaim;
        RequiredClaimValue = requiredClaimValue;
        RequirePresentation = requirePresentation;
    }
}