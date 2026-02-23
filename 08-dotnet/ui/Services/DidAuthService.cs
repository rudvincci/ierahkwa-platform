using System.Text;
using System.Security.Cryptography;
using Mamey.Blockchain.DID;
using Mamey.Blockchain.Wallet;
using Mamey.Wallet;

namespace MameyNode.UI.Services;

public class DidAuthService
{
    private readonly IDIDClient _didClient;
    private readonly MameyWalletClient _walletClient;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IMetadataService _metadataService;
    private readonly AuthSessionService _session;

    public DidAuthService(
        IDIDClient didClient,
        MameyWalletClient walletClient,
        JwtTokenService jwtTokenService,
        IMetadataService metadataService,
        AuthSessionService session)
    {
        _didClient = didClient;
        _walletClient = walletClient;
        _jwtTokenService = jwtTokenService;
        _metadataService = metadataService;
        _session = session;
    }

    public async Task<(bool Success, string? ErrorMessage)> LoginWithDidAsync(
        string institutionDid,
        string walletKeyId,
        string walletPublicKey,
        string? walletPassword = null,
        string? verificationMethodId = null,
        CancellationToken cancellationToken = default)
    {
        var challenge = $"{institutionDid}:{Guid.NewGuid()}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var signResp = await _walletClient.Client.SignMessageAsync(
            new SignMessageRequest
            {
                KeyId = walletKeyId,
                Message = Google.Protobuf.ByteString.CopyFrom(Encoding.UTF8.GetBytes(challenge)),
                Password = walletPassword ?? string.Empty
            },
            cancellationToken: cancellationToken);

        if (!signResp.Success)
        {
            return (false, $"Wallet SignMessage failed: {signResp.ErrorMessage}");
        }

        // DID service currently expects signature as hex string (see mamey-rpc did_service.rs validation)
        var signatureBytes = signResp.Signature.ToByteArray();
        if (signatureBytes.Length == 0)
        {
            return (false, "Wallet SignMessage returned an empty signature.");
        }
        var signatureHex = Convert.ToHexString(signatureBytes).ToLowerInvariant();

        var verify = await _didClient.VerifyDIDOwnershipAsync(
            institutionDid,
            challenge,
            signatureHex,
            verificationMethodId,
            cancellationToken);

        if (!verify.Success || !verify.Verified)
        {
            return (false, verify.ErrorMessage ?? "DID ownership verification failed.");
        }

        // Issue short-lived JWT for subsequent SDK calls (Bearer)
        var jwt = _jwtTokenService.CreateJwt(
            subject: institutionDid,
            permissions: new[] { "*" },
            ttl: TimeSpan.FromHours(1));

        _metadataService.SetInstitutionId(institutionDid);
        _metadataService.SetBearerToken(jwt);
        _session.SetAuthenticated(institutionDid, jwt, walletKeyId, walletPublicKey, walletPassword);

        return (true, null);
    }
}

