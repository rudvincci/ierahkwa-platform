using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Validation;

/// <summary>
/// Validates verification methods for format and cryptographic compliance.
/// </summary>
public static class VerificationMethodValidator
{
    /// <summary>
    /// Validates a verification method (e.g., for did:key, did:web).
    /// </summary>
    public static IList<string> Validate(IDidVerificationMethod vm)
    {
        var warnings = new List<string>();
        if (vm == null) throw new ArgumentNullException(nameof(vm));

        if (string.IsNullOrWhiteSpace(vm.Id))
            throw new ArgumentException("Verification method is missing 'id'.");

        if (string.IsNullOrWhiteSpace(vm.Type))
            warnings.Add($"Verification method '{vm.Id}' is missing 'type'.");

        if (string.IsNullOrWhiteSpace(vm.Controller))
            warnings.Add($"Verification method '{vm.Id}' is missing 'controller'.");

        if (string.IsNullOrWhiteSpace(vm.PublicKeyBase58) && string.IsNullOrWhiteSpace(vm.PublicKeyMultibase) &&
            (vm.PublicKeyJwk == null || vm.PublicKeyJwk.Count == 0))
            warnings.Add($"Verification method '{vm.Id}' does not provide key material (base58, multibase, or JWK).");

        // Key material format checks can be added here, e.g., base58/multibase/JWK parsing
        return warnings;
    }
}