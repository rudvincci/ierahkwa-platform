namespace MameyNode.UI.Services;

/// <summary>
/// Session-scoped authentication state for the Blazor Server circuit.
/// </summary>
public class AuthSessionService
{
    public bool IsAuthenticated { get; private set; }
    public string? InstitutionDid { get; private set; }
    public string? JwtToken { get; private set; }

    // Wallet key used to sign challenges (dev/onboarding)
    public string? WalletKeyId { get; private set; }
    public string? WalletPublicKey { get; private set; }
    public string? WalletPassword { get; private set; }

    public void SetAuthenticated(
        string institutionDid,
        string jwtToken,
        string walletKeyId,
        string walletPublicKey,
        string? walletPassword)
    {
        IsAuthenticated = true;
        InstitutionDid = institutionDid;
        JwtToken = jwtToken;
        WalletKeyId = walletKeyId;
        WalletPublicKey = walletPublicKey;
        WalletPassword = walletPassword;
    }

    public void Clear()
    {
        IsAuthenticated = false;
        InstitutionDid = null;
        JwtToken = null;
        WalletKeyId = null;
        WalletPublicKey = null;
        WalletPassword = null;
    }
}

