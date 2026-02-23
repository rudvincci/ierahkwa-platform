using System.Text;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Methods.Key;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Peer;

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
        var didObj = new Did(did);
        var msid = didObj.MethodSpecificId;
        
        if (msid.StartsWith("0"))
        {
            // Numalgo 0: Base58-encoded DID document
            var docBytes = Utilities.Base58.Decode(msid.Substring(1));
            var json = Encoding.UTF8.GetString(docBytes);
            return DidDocument.Parse(json);
        }
        else if (msid.StartsWith("1"))
        {
            // Numalgo 1: Single key, self-certified
            return CreateNumalgo1Document(did, msid);
        }
        else if (msid.StartsWith("2"))
        {
            // Numalgo 2: Multiple keys, self-certified
            return CreateNumalgo2Document(did, msid);
        }
        else
        {
            throw new NotSupportedException($"Unsupported did:peer numalgo: {msid[0]}");
        }
    }

    /// <summary>
    /// Creates a Numalgo 1 document from a DID and method-specific ID.
    /// </summary>
    private static DidDocument CreateNumalgo1Document(string did, string msid)
    {
        var inceptionKey = msid.Substring(1); // remove '1'
        var publicKeyBytes = Utilities.Base58.Decode(inceptionKey);
        var publicKeyBase58 = inceptionKey;
        
        var vmId = $"{did}#key-1";
        var vm = new VerificationMethod(
            id: vmId,
            type: "Ed25519VerificationKey2018",
            controller: did,
            publicKeyBase58: publicKeyBase58);
            
        return new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: did,
            controller: new[] { did },
            verificationMethods: new[] { vm },
            authentication: new[] { vmId }
        );
    }

    /// <summary>
    /// Creates a Numalgo 2 document from a DID and method-specific ID.
    /// </summary>
    private static DidDocument CreateNumalgo2Document(string did, string msid)
    {
        var parts = msid.Substring(2).Split('.'); // remove '2.'
        var verificationMethods = new List<VerificationMethod>();
        var authentication = new List<string>();
            var services = new List<ServiceEndpoint>();

        // Parse keys and services
        var keyIndex = 1;
        foreach (var part in parts)
        {
            if (part.StartsWith("z")) // Base58 encoded key
            {
                var vmId = $"{did}#key-{keyIndex}";
                var vm = new VerificationMethod(
                    id: vmId,
                    type: "Ed25519VerificationKey2018",
                    controller: did,
                    publicKeyBase58: part);
                
                verificationMethods.Add(vm);
                authentication.Add(vmId);
                keyIndex++;
            }
            else // Service
            {
                try
                {
                    var serviceBytes = Utilities.Base58.Decode(part);
                    var serviceJson = Encoding.UTF8.GetString(serviceBytes);
                    var service = System.Text.Json.JsonSerializer.Deserialize<PeerService>(serviceJson);
                    
                    if (service != null)
                    {
                        services.Add(new ServiceEndpoint(
                            $"{did}#{service.Id}",
                            service.Type,
                            new[] { service.ServiceEndpoint }));
                    }
                }
                catch
                {
                    // Skip invalid service entries
                }
            }
        }

        return new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: did,
            controller: new[] { did },
            verificationMethods: verificationMethods.ToArray(),
            authentication: authentication.ToArray(),
            serviceEndpoints: services.ToArray()
        );
    }
}