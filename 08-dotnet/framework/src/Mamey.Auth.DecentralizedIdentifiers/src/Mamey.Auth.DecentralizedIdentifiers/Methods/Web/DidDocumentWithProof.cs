using System.Text.Json.Serialization;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Web;

/// <summary>
/// A DID Document with an embedded Linked Data Proof (per JSON-LD Proofs spec).
/// </summary>
public class DidDocumentWithProof : DidDocument
{
    /// <summary>
    /// Gets or sets the Linked Data Proof attached to this DID Document (if present).
    /// </summary>
    [JsonPropertyName("proof")]
    public dynamic Proof { get; set; }

    public DidDocumentWithProof() : base(null, null) { }

    public DidDocumentWithProof(DidDocument doc, dynamic proof)
        : base(doc.Context, doc.Id, doc.Controller, doc.VerificationMethods,
            doc.Authentication, doc.AssertionMethod, doc.KeyAgreement,
            doc.CapabilityDelegation, doc.CapabilityInvocation, doc.ServiceEndpoints,
            doc.AdditionalProperties?.ToDictionary(k => k.Key, v => v.Value))
    {
        this.Proof = proof;
    }

    /// <summary>
    /// Converts to a plain DID Document (without the proof property).
    /// </summary>
    public DidDocument ToDidDocument() => new DidDocument(Context, Id, Controller, VerificationMethods,
        Authentication, AssertionMethod, KeyAgreement, CapabilityDelegation,
        CapabilityInvocation, ServiceEndpoints, AdditionalProperties?.ToDictionary(k => k.Key, v => v.Value));
}