using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.Methods.MethodBase;
using System.Text;
using NSec.Cryptography;

namespace Mamey.Identity.Decentralized.Methods.Peer
{
    /// <summary>
    /// Implements the did:peer method (https://identity.foundation/peer-did-method-spec/).
    /// Supports deterministic and in-memory (peer-to-peer, not anchored) identifiers.
    /// </summary>
    public class DidPeerMethod : DidMethodBase
    {
        public override string Name => "peer";

        /// <summary>
        /// Only supports deterministic (Numalgo 0/1) resolution by default. For Numalgo 2, add a registry backend.
        /// </summary>
        public override Task<IDidDocument> ResolveAsync(string did, CancellationToken cancellationToken = default)
        {
            ValidateDid(did);
            var didObj = new Did(did);

            // Numalgo 0: base58-encoded DID doc
            if (didObj.MethodSpecificId.StartsWith("0"))
            {
                var docBytes = Utilities.Base58.Decode(didObj.MethodSpecificId.Substring(1));
                var json = Encoding.UTF8.GetString(docBytes);
                var doc = DidDocument.Parse(json);
                return Task.FromResult((IDidDocument)doc);
            }
            // Numalgo 1: inception key, construct DID Doc
            else if (didObj.MethodSpecificId.StartsWith("1"))
            {
                var inceptionKey = didObj.MethodSpecificId.Substring(1); // remove '1'
                var publicKeyBytes = Utilities.Base58.Decode(inceptionKey);

                var peerDid = $"did:peer:1{inceptionKey}";
                var vmId = $"{peerDid}#key-1";
                var vm = new VerificationMethod(
                    id: vmId,
                    type: "Ed25519VerificationKey2018",
                    controller: peerDid,
                    publicKeyBase58: inceptionKey);

                var doc = new DidDocument(
                    context: new[] { DidContextConstants.W3cDidContext },
                    id: peerDid,
                    controller: new[] { peerDid },
                    verificationMethods: new[] { vm },
                    authentication: new[] { vmId }
                );
                return Task.FromResult((IDidDocument)doc);
            }
            else
            {
                throw new NotSupportedException($"Unsupported did:peer method-specific-id: {didObj.MethodSpecificId}");
            }
        }

        /// <summary>
        /// Creates a new peer DID using the specified options.
        /// </summary>
        public override Task<IDidDocument> CreateAsync(object options, CancellationToken cancellationToken = default)
        {
            if (options is not PeerMethodOptions peerOptions)
                throw new ArgumentException("Invalid options for did:peer creation.");

            // For demo: Use Numalgo 1 (single key, self-certified, no external state)
            var keyPair = peerOptions.KeyPair ?? PeerKeyGen.GenerateEd25519KeyPair();

            var publicKeyBase58 = Utilities.Base58.Encode(keyPair.PublicKey);

            var did = $"did:peer:1{publicKeyBase58}";
            var vmId = $"{did}#key-1";
            var vm = new VerificationMethod(
                id: vmId,
                type: "Ed25519VerificationKey2018",
                controller: did,
                publicKeyBase58: publicKeyBase58);

            var doc = new DidDocument(
                context: new[] { DidContextConstants.W3cDidContext },
                id: did,
                controller: new[] { did },
                verificationMethods: new[] { vm },
                authentication: new[] { vmId }
            );
            return Task.FromResult((IDidDocument)doc);
        }

        // For did:peer, update/deactivate are not supported unless using a registry.
        public override Task<IDidDocument> UpdateAsync(string did, object updateRequest, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("did:peer update not supported for Numalgo 0/1 (stateless).");

        public override Task DeactivateAsync(string did, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("did:peer deactivate not supported (stateless).");
    }

    /// <summary>
    /// Utility for generating Ed25519 key pairs for peer DIDs.
    /// </summary>
    public static class PeerKeyGen
    {
        public static PeerKeyPair GenerateEd25519KeyPair()
        {
            var algorithm = SignatureAlgorithm.Ed25519;
            using var key = new NSec.Cryptography.Key(algorithm, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
            var publicKey = key.PublicKey.Export(KeyBlobFormat.RawPublicKey);
            var privateKey = key.Export(KeyBlobFormat.RawPrivateKey);

            return new PeerKeyPair
            {
                PublicKey = publicKey,
                PrivateKey = privateKey
            };
        }
    }
}
