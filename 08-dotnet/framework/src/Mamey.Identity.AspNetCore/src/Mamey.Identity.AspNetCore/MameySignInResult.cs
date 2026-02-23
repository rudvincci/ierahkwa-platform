using Microsoft.AspNetCore.Identity;

namespace Mamey.Identity.AspNetCore;


/// <summary>
/// Represents the result of a sign-in operation.
/// </summary>
public class MameySignInResult : SignInResult
{
    private static readonly MameySignInResult _requiresMFA = new MameySignInResult { RequiresMFA = true };

    
    /// <summary>
    /// Returns a flag indication whether the user attempting to sign-in requires MFA authentication.
    /// </summary>
    /// <value>True if the user attempting to sign-in requires MFA authentication, otherwise false.</value>
    public bool RequiresMFA { get; protected set; }

    
    public override string ToString()
    {
        return IsLockedOut ? "LockedOut" :
            IsNotAllowed ? "NotAllowed" :
            RequiresTwoFactor ? "RequiresTwoFactor" :
            RequiresMFA ? "RequiresMFA" :
            Succeeded ? "Succeeded" : "Failed";
    }
}


