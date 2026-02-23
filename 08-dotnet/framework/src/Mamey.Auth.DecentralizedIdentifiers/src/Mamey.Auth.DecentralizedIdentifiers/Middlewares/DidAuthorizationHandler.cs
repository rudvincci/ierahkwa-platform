using Microsoft.AspNetCore.Authorization;

namespace Mamey.Auth.DecentralizedIdentifiers.Middlewares;

/// <summary>
/// Authorization handler for DID/VC claims.
/// </summary>
public class DidAuthorizationHandler : AuthorizationHandler<DidAuthRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        DidAuthRequirement requirement)
    {
        var user = context.User;

        // CredentialType (e.g., "vc:type" = "AlumniCredential")
        if (!string.IsNullOrWhiteSpace(requirement.CredentialType) &&
            !user.HasClaim("vc:type", requirement.CredentialType))
            return Task.CompletedTask;

        // Issuer DID (e.g., "vc:issuer")
        if (!string.IsNullOrWhiteSpace(requirement.IssuerDid) &&
            !user.HasClaim("vc:issuer", requirement.IssuerDid))
            return Task.CompletedTask;

        // Specific claim type/value (e.g., "alumniOf" = "did:example:uni1")
        if (!string.IsNullOrWhiteSpace(requirement.RequiredClaim))
        {
            if (!string.IsNullOrWhiteSpace(requirement.RequiredClaimValue))
            {
                if (!user.HasClaim(requirement.RequiredClaim, requirement.RequiredClaimValue))
                    return Task.CompletedTask;
            }
            else
            {
                if (!user.HasClaim(c => c.Type == requirement.RequiredClaim))
                    return Task.CompletedTask;
            }
        }

        // (Optional) Enforce that the authentication method was a Presentation
        if (requirement.RequirePresentation &&
            !user.HasClaim("vc:presentation", "true"))
            return Task.CompletedTask;

        // All checks passed:
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}



