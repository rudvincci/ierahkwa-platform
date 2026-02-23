using System.Text;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.Methods.Key;

namespace Mamey.Identity.Decentralized.Methods.Peer;

public static class DidDocumentFactory
{
    public static DidDocument FromDidKey(string did)
    {
        // Extract method-specific-id (multibase/multicodec)
        var didObj = new Did(did);
        var methodSpecificId = didObj.MethodSpecificId;

        // Use your MulticodecKeyParser and VerificationMethod as in previous implementation
        var (keyType, publicKeyBytes, codec) = MulticodecKeyParser.Parse(methodSpecificId);
        var publicKeyMultibase = "z" + methodSpecificId;
        var vmId = $"{did}#key-1";
        var vmType = "Ed25519VerificationKey2018"; // or from codec/keyType

        var verificationMethod = new VerificationMethod(
            id: vmId,
            type: vmType,
            controller: did,
            publicKeyJwk: null, // can export JWK if needed
            publicKeyBase58: null,
            publicKeyMultibase: publicKeyMultibase);

        return new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: did,
            controller: new[] { did },
            verificationMethods: new[] { verificationMethod },
            authentication: new[] { vmId }
        );
    }

    public static DidDocument FromDidPeer(string did)
    {
        // Your existing did:peer logic, e.g. supporting Numalgo 0/1
        // (see previous peer code)
        var didObj = new Did(did);
        var msid = didObj.MethodSpecificId;
        if (msid.StartsWith("0"))
        {
            var docBytes = Utilities.Base58.Decode(msid.Substring(1));
            var json = Encoding.UTF8.GetString(docBytes);
            return DidDocument.Parse(json);
        }
        else if (msid.StartsWith("1"))
        {
            var inceptionKey = msid.Substring(1);
            var publicKeyBytes = Utilities.Base58.Decode(inceptionKey);
            var vmId = $"{did}#key-1";
            var vm = new VerificationMethod(
                id: vmId,
                type: "Ed25519VerificationKey2018",
                controller: did,
                publicKeyBase58: inceptionKey);
            return new DidDocument(
                context: new[] { "https://www.w3.org/ns/did/v1" },
                id: did,
                controller: new[] { did },
                verificationMethods: new[] { vm },
                authentication: new[] { vmId }
            );
        }
        else
        {
            throw new NotSupportedException($"Unsupported did:peer method-specific-id: {msid}");
        }
    }
}