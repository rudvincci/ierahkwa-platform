using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Factory for creating proof services based on proof type
/// </summary>
public interface IProofServiceFactory
{
    IProofService GetProofService(ProofType proofType);
}

/// <summary>
/// Default implementation of proof service factory
/// </summary>
public class ProofServiceFactory : IProofServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ProofServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IProofService GetProofService(ProofType proofType)
    {
        return proofType switch
        {
            ProofType.Ed25519Signature2020 => new Ed25519ProofService(_serviceProvider),
            ProofType.RsaSignature2018 => new RsaProofService(_serviceProvider),
            ProofType.EcdsaSecp256k1Signature2019 => new EcdsaProofService(_serviceProvider),
            ProofType.BbsBlsSignature2020 => new BbsProofService(_serviceProvider),
            ProofType.JsonWebSignature2020 => new JwsProofService(_serviceProvider),
            _ => throw new NotSupportedException($"Proof type {proofType} is not supported")
        };
    }
}

/// <summary>
/// Ed25519 signature proof service.
/// </summary>
public class Ed25519ProofService : IProofService
{
    private readonly IKeyProvider _keyProvider;
    private readonly ILogger<Ed25519ProofService> _logger;

    public Ed25519ProofService(IServiceProvider serviceProvider)
    {
        _keyProvider = serviceProvider.GetRequiredService<IKeyProvider>();
        _logger = serviceProvider.GetRequiredService<ILogger<Ed25519ProofService>>();
    }

    public async Task<object> CreateProofAsync(
        string jsonLd,
        string verificationMethodId,
        byte[] privateKey,
        string proofPurpose,
        string type,
        string created = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating Ed25519 signature proof");

            // Create the proof
            var proof = new Proof
            {
                Type = type ?? "Ed25519Signature2020",
                Created = created != null ? DateTimeOffset.Parse(created) : DateTimeOffset.UtcNow,
                VerificationMethod = verificationMethodId,
                ProofPurpose = proofPurpose ?? "assertionMethod"
            };

            // Sign the document
            var signature = await _keyProvider.SignAsync(Encoding.UTF8.GetBytes(jsonLd), verificationMethodId);
            proof.Jws = Convert.ToBase64String(signature);

            return proof;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Ed25519 signature proof");
            throw;
        }
    }

    public async Task<bool> VerifyProofAsync(
        string jsonLd,
        object proof,
        byte[] publicKey,
        string type,
        string proofPurpose,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Verifying Ed25519 signature proof");

            // Verify the signature
            var isValid = await _keyProvider.VerifyAsync(Encoding.UTF8.GetBytes(jsonLd), Convert.FromBase64String(((Proof)proof).Jws), ((Proof)proof).VerificationMethod);

            _logger.LogDebug("Ed25519 signature verification result: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Ed25519 signature proof");
            return false;
        }
    }

    public async Task<bool> IsRevokedAsync(string statusListCredentialUrl, int statusListIndex,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking revocation status for Ed25519 proof");
            // Implementation would check the status list
            // For now, return false (not revoked)
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check revocation status for Ed25519 proof");
            return false;
        }
    }

    public Task<bool> ValidateProofAsync(string jsonLd, object proof, IDidVerificationMethod verificationMethod, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Ed25519 proof service not yet implemented");
    }
}

/// <summary>
/// RSA signature proof service.
/// </summary>
public class RsaProofService : IProofService
{
    private readonly IKeyProvider _keyProvider;
    private readonly ILogger<RsaProofService> _logger;

    public RsaProofService(IServiceProvider serviceProvider)
    {
        _keyProvider = serviceProvider.GetRequiredService<IKeyProvider>();
        _logger = serviceProvider.GetRequiredService<ILogger<RsaProofService>>();
    }

    public async Task<object> CreateProofAsync(
        string jsonLd,
        string verificationMethodId,
        byte[] privateKey,
        string proofPurpose,
        string type,
        string created = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating RSA signature proof");

            // Create the proof
            var proof = new Proof
            {
                Type = type ?? "RsaSignature2018",
                Created = created != null ? DateTimeOffset.Parse(created) : DateTimeOffset.UtcNow,
                VerificationMethod = verificationMethodId,
                ProofPurpose = proofPurpose ?? "assertionMethod"
            };

            // Sign the document
            var signature = await _keyProvider.SignAsync(Encoding.UTF8.GetBytes(jsonLd), verificationMethodId);
            proof.Jws = Convert.ToBase64String(signature);

            return proof;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create RSA signature proof");
            throw;
        }
    }

    public async Task<bool> VerifyProofAsync(
        string jsonLd,
        object proof,
        byte[] publicKey,
        string type,
        string proofPurpose,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Verifying RSA signature proof");

            // Verify the signature
            var isValid = await _keyProvider.VerifyAsync(Encoding.UTF8.GetBytes(jsonLd), Convert.FromBase64String(((Proof)proof).Jws), ((Proof)proof).VerificationMethod);

            _logger.LogDebug("RSA signature verification result: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify RSA signature proof");
            return false;
        }
    }

    public async Task<bool> IsRevokedAsync(string statusListCredentialUrl, int statusListIndex,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking revocation status for RSA proof");
            // Implementation would check the status list
            // For now, return false (not revoked)
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check revocation status for RSA proof");
            return false;
        }
    }

    public Task<bool> ValidateProofAsync(string jsonLd, object proof, IDidVerificationMethod verificationMethod, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("RSA proof service not yet implemented");
    }
}

/// <summary>
/// ECDSA signature proof service.
/// </summary>
public class EcdsaProofService : IProofService
{
    private readonly IKeyProvider _keyProvider;
    private readonly ILogger<EcdsaProofService> _logger;

    public EcdsaProofService(IServiceProvider serviceProvider)
    {
        _keyProvider = serviceProvider.GetRequiredService<IKeyProvider>();
        _logger = serviceProvider.GetRequiredService<ILogger<EcdsaProofService>>();
    }

    public async Task<object> CreateProofAsync(
        string jsonLd,
        string verificationMethodId,
        byte[] privateKey,
        string proofPurpose,
        string type,
        string created = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("ECDSA proof service not yet implemented");
    }

    public async Task<bool> VerifyProofAsync(
        string jsonLd,
        object proof,
        byte[] publicKey,
        string type,
        string proofPurpose,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("ECDSA proof service not yet implemented");
    }

    public async Task<bool> IsRevokedAsync(string statusListCredentialUrl, int statusListIndex,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("ECDSA proof service not yet implemented");
    }

    public Task<bool> ValidateProofAsync(string jsonLd, object proof, IDidVerificationMethod verificationMethod, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("ECDSA proof service not yet implemented");
    }
}

/// <summary>
/// BBS+ signature proof service.
/// </summary>
public class BbsProofService : IProofService
{
    private readonly IKeyProvider _keyProvider;
    private readonly ILogger<BbsProofService> _logger;

    public BbsProofService(IServiceProvider serviceProvider)
    {
        _keyProvider = serviceProvider.GetRequiredService<IKeyProvider>();
        _logger = serviceProvider.GetRequiredService<ILogger<BbsProofService>>();
    }

    public async Task<object> CreateProofAsync(
        string jsonLd,
        string verificationMethodId,
        byte[] privateKey,
        string proofPurpose,
        string type,
        string created = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("BBS+ proof service not yet implemented");
    }

    public async Task<bool> VerifyProofAsync(
        string jsonLd,
        object proof,
        byte[] publicKey,
        string type,
        string proofPurpose,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("BBS+ proof service not yet implemented");
    }

    public async Task<bool> IsRevokedAsync(string statusListCredentialUrl, int statusListIndex,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("BBS+ proof service not yet implemented");
    }

    public Task<bool> ValidateProofAsync(string jsonLd, object proof, IDidVerificationMethod verificationMethod, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("BBS+ proof service not yet implemented");
    }
}

/// <summary>
/// JWS signature proof service.
/// </summary>
public class JwsProofService : IProofService
{
    private readonly IKeyProvider _keyProvider;
    private readonly ILogger<JwsProofService> _logger;

    public JwsProofService(IServiceProvider serviceProvider)
    {
        _keyProvider = serviceProvider.GetRequiredService<IKeyProvider>();
        _logger = serviceProvider.GetRequiredService<ILogger<JwsProofService>>();
    }

    public async Task<object> CreateProofAsync(
        string jsonLd,
        string verificationMethodId,
        byte[] privateKey,
        string proofPurpose,
        string type,
        string created = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("JWS proof service not yet implemented");
    }

    public async Task<bool> VerifyProofAsync(
        string jsonLd,
        object proof,
        byte[] publicKey,
        string type,
        string proofPurpose,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("JWS proof service not yet implemented");
    }

    public async Task<bool> IsRevokedAsync(string statusListCredentialUrl, int statusListIndex,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("JWS proof service not yet implemented");
    }

    public Task<bool> ValidateProofAsync(string jsonLd, object proof, IDidVerificationMethod verificationMethod, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("JWS proof service not yet implemented");
    }
}