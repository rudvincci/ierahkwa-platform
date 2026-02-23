using Mamey.Identity.Decentralized.Core;

namespace Mamey.Identity.Decentralized.Validation;

/// <summary>
/// Validates the structural and contextual integrity of a DID Document per W3C DID Core.
/// </summary>
public static class DidDocumentValidator
{
    /// <summary>
    /// Validates all required and optional fields of a DID Document.
    /// Throws on any fatal error, returns warnings as a list.
    /// </summary>
    public static IList<string> Validate(DidDocument doc)
    {
        var warnings = new List<string>();

        if (doc == null) throw new ArgumentNullException(nameof(doc));
        if (doc.Context == null || !doc.Context.Contains(DidContextConstants.W3cDidContext))
            throw new ArgumentException("DID Document must contain the W3C DID context URI.");

        if (string.IsNullOrWhiteSpace(doc.Id))
            throw new ArgumentException("DID Document is missing an 'id'.");

        if (!Uri.IsWellFormedUriString(doc.Id, UriKind.RelativeOrAbsolute))
            warnings.Add("DID Document 'id' is not a well-formed URI.");

        if (doc.VerificationMethods != null)
        {
            foreach (var vm in doc.VerificationMethods)
                warnings.AddRange(VerificationMethodValidator.Validate(vm));
        }

        if (doc.ServiceEndpoints != null)
        {
            foreach (var service in doc.ServiceEndpoints)
                warnings.AddRange(ServiceEndpointValidator.Validate(service));
        }

        // Relationships (authentication, assertionMethod, etc) must reference known keys or be embedded
        var allKeyIds = doc.VerificationMethods?.Select(k => k.Id).ToHashSet() ?? new HashSet<string>();

        ValidateRelationship(doc.Authentication, allKeyIds, "authentication", warnings);
        ValidateRelationship(doc.AssertionMethod, allKeyIds, "assertionMethod", warnings);
        ValidateRelationship(doc.KeyAgreement, allKeyIds, "keyAgreement", warnings);
        ValidateRelationship(doc.CapabilityDelegation, allKeyIds, "capabilityDelegation", warnings);
        ValidateRelationship(doc.CapabilityInvocation, allKeyIds, "capabilityInvocation", warnings);

        return warnings;
    }

    private static void ValidateRelationship(IEnumerable<object> rels, HashSet<string> knownKeyIds, string relName,
        IList<string> warnings)
    {
        if (rels == null) return;
        foreach (var rel in rels)
        {
            switch (rel)
            {
                case string keyRef:
                    if (!knownKeyIds.Contains(keyRef))
                        warnings.Add($"Relationship '{relName}' references unknown key '{keyRef}'.");
                    break;
                // For embedded verification methods, assume they are valid objects; real usage should also deep-validate.
            }
        }
    }
}