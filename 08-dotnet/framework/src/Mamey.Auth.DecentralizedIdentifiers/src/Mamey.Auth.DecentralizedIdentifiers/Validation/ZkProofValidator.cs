using Microsoft.Extensions.Logging;

namespace Mamey.Auth.DecentralizedIdentifiers.Validation;

/// <summary>
/// Production ZK Proof validator supporting BBS+, Groth16, and more (pluggable backends).
/// </summary>
public class ZkProofValidator : IZkProofValidator
{
    private readonly ILogger<ZkProofValidator> _logger;
    private readonly IDictionary<string, IZkProofBackend> _backends;

    public ZkProofValidator(ILogger<ZkProofValidator> logger, IEnumerable<IZkProofBackend> customBackends = null)
    {
        _logger = logger;
        _backends = new Dictionary<string, IZkProofBackend>(StringComparer.OrdinalIgnoreCase);

        // Register built-in backends
        RegisterBackend(new BbsBlsZkProofBackend());
        // Add other built-in or injected custom backends
        if (customBackends != null)
        {
            foreach (var backend in customBackends)
            {
                RegisterBackend(backend);
            }
        }
    }

    /// <summary>
    /// Registers a backend for a specific proof type.
    /// </summary>
    public void RegisterBackend(IZkProofBackend backend)
    {
        if (backend == null) throw new ArgumentNullException(nameof(backend));
        _backends[backend.ProofType] = backend;
    }

    /// <inheritdoc/>
    public async Task<bool> ValidateAsync(
        string credentialOrPresentationJson,
        object proofJson,
        string verificationMethod,
        string proofType,
        string protocol = null,
        CancellationToken cancellationToken = default)
    {
        if (!_backends.TryGetValue(proofType, out var backend))
        {
            _logger.LogError("No ZKP backend found for proof type '{ProofType}'", proofType);
            throw new NotSupportedException($"Unsupported proof type '{proofType}'");
        }

        return await backend.ValidateAsync(
            credentialOrPresentationJson,
            proofJson,
            verificationMethod,
            protocol,
            cancellationToken
        );
    }
}