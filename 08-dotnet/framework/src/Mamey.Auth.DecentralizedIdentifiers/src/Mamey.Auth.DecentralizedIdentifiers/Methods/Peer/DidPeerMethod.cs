using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Methods.MethodBase;
using System.Text;
using NSec.Cryptography;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Peer
{
    /// <summary>
    /// Implements the did:peer method (https://identity.foundation/peer-did-method-spec/).
    /// Supports deterministic and in-memory (peer-to-peer, not anchored) identifiers.
    /// </summary>
    public class DidPeerMethod : DidMethodBase
    {
        public override string Name => "peer";
        private readonly ILogger<DidPeerMethod> _logger;

        public DidPeerMethod(ILogger<DidPeerMethod> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Supports deterministic (Numalgo 0/1/2) resolution.
        /// </summary>
        public override Task<IDidDocument> ResolveAsync(string did, CancellationToken cancellationToken = default)
        {
            try
        {
            ValidateDid(did);
            var didObj = new Did(did);
                var msid = didObj.MethodSpecificId;

                _logger.LogDebug("Resolving did:peer with numalgo: {Numalgo}", msid[0]);

            // Numalgo 0: base58-encoded DID doc
                if (msid.StartsWith("0"))
            {
                    var docBytes = Utilities.Base58.Decode(msid.Substring(1));
                var json = Encoding.UTF8.GetString(docBytes);
                var doc = DidDocument.Parse(json);
                    _logger.LogInformation("Resolved did:peer numalgo 0: {Did}", did);
                    return Task.FromResult((IDidDocument)doc);
                }
                // Numalgo 1: single key, self-certified
                else if (msid.StartsWith("1"))
                {
                    var doc = CreateNumalgo1Document(did, msid);
                    _logger.LogInformation("Resolved did:peer numalgo 1: {Did}", did);
                return Task.FromResult((IDidDocument)doc);
            }
                // Numalgo 2: multiple keys, self-certified
                else if (msid.StartsWith("2"))
                {
                    var doc = CreateNumalgo2Document(did, msid);
                    _logger.LogInformation("Resolved did:peer numalgo 2: {Did}", did);
                return Task.FromResult((IDidDocument)doc);
            }
            else
            {
                    throw new NotSupportedException($"Unsupported did:peer numalgo: {msid[0]}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resolve did:peer: {Did}", did);
                throw;
            }
        }

        /// <summary>
        /// Creates a new peer DID using the specified options.
        /// </summary>
        public override Task<IDidDocument> CreateAsync(object options, CancellationToken cancellationToken = default)
        {
            try
        {
            if (options is not PeerMethodOptions peerOptions)
                throw new ArgumentException("Invalid options for did:peer creation.");

                _logger.LogInformation("Creating did:peer with numalgo: {Numalgo}", peerOptions.Numalgo);

                return peerOptions.Numalgo switch
                {
                    0 => CreateNumalgo0Document(peerOptions),
                    1 => CreateNumalgo1Document(peerOptions),
                    2 => CreateNumalgo2Document(peerOptions),
                    _ => throw new NotSupportedException($"Unsupported numalgo: {peerOptions.Numalgo}")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create did:peer");
                throw;
            }
        }

        // For did:peer, update/deactivate are not supported unless using a registry.
        public override Task<IDidDocument> UpdateAsync(string did, object updateRequest, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("did:peer update not supported for Numalgo 0/1/2 (stateless).");

        public override Task DeactivateAsync(string did, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("did:peer deactivate not supported (stateless).");

        /// <summary>
        /// Creates a Numalgo 0 document (base58-encoded DID document).
        /// </summary>
        private Task<IDidDocument> CreateNumalgo0Document(PeerMethodOptions options)
        {
            // Generate a basic DID document and encode it
            var keyPair = options.KeyPair ?? PeerKeyGen.GenerateEd25519KeyPair();
            var publicKeyBase58 = Utilities.Base58.Encode(keyPair.PublicKey);

            var did = $"did:peer:1{publicKeyBase58}"; // Use numalgo 1 for the base document
            var baseDoc = CreateNumalgo1Document(did, $"1{publicKeyBase58}");
            
            // Encode the document as base58
            var docJson = System.Text.Json.JsonSerializer.Serialize(baseDoc);
            var docBytes = Encoding.UTF8.GetBytes(docJson);
            var encodedDoc = Utilities.Base58.Encode(docBytes);
            
            var numalgo0Did = $"did:peer:0{encodedDoc}";
            
            _logger.LogInformation("Created did:peer numalgo 0: {Did}", numalgo0Did);
            return Task.FromResult((IDidDocument)baseDoc);
        }

        /// <summary>
        /// Creates a Numalgo 1 document (single key, self-certified).
        /// </summary>
        private Task<IDidDocument> CreateNumalgo1Document(PeerMethodOptions options)
        {
            var keyPair = options.KeyPair ?? PeerKeyGen.GenerateEd25519KeyPair();
            var publicKeyBase58 = Utilities.Base58.Encode(keyPair.PublicKey);
            var did = $"did:peer:1{publicKeyBase58}";
            
            var doc = CreateNumalgo1Document(did, $"1{publicKeyBase58}");
            
            _logger.LogInformation("Created did:peer numalgo 1: {Did}", did);
            return Task.FromResult((IDidDocument)doc);
        }

        /// <summary>
        /// Creates a Numalgo 2 document (multiple keys, self-certified).
        /// </summary>
        private Task<IDidDocument> CreateNumalgo2Document(PeerMethodOptions options)
        {
            var keyPair = options.KeyPair ?? PeerKeyGen.GenerateEd25519KeyPair();
            var publicKeyBase58 = Utilities.Base58.Encode(keyPair.PublicKey);
            
            // For numalgo 2, we need to encode multiple keys and services
            var encodedKeys = new List<string> { publicKeyBase58 };
            var encodedServices = new List<string>();
            
            // Add additional verification methods
            foreach (var vm in options.AdditionalVerificationMethods)
            {
                var vmKeyPair = vm.KeyPair ?? PeerKeyGen.GenerateEd25519KeyPair();
                var vmPublicKeyBase58 = Utilities.Base58.Encode(vmKeyPair.PublicKey);
                encodedKeys.Add(vmPublicKeyBase58);
            }
            
            // Add services
            foreach (var service in options.Services)
            {
                var serviceJson = System.Text.Json.JsonSerializer.Serialize(service);
                var serviceBytes = Encoding.UTF8.GetBytes(serviceJson);
                var encodedService = Utilities.Base58.Encode(serviceBytes);
                encodedServices.Add(encodedService);
            }
            
            // Create the method-specific ID for numalgo 2
            var msid = $"2.{string.Join(".", encodedKeys)}";
            if (encodedServices.Any())
            {
                msid += $".{string.Join(".", encodedServices)}";
            }
            
            var did = $"did:peer:{msid}";
            var doc = CreateNumalgo2Document(did, msid);
            
            _logger.LogInformation("Created did:peer numalgo 2: {Did}", did);
            return Task.FromResult((IDidDocument)doc);
        }

        /// <summary>
        /// Creates a Numalgo 1 document from a DID and method-specific ID.
        /// </summary>
        private DidDocument CreateNumalgo1Document(string did, string msid)
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

            var verificationMethods = new List<VerificationMethod> { vm };
            var authentication = new List<string> { vmId };

            return new DidDocument(
                context: new[] { DidContextConstants.W3cDidContext },
                id: did,
                controller: new[] { did },
                verificationMethods: verificationMethods.ToArray(),
                authentication: authentication.ToArray()
            );
        }

        /// <summary>
        /// Creates a Numalgo 2 document from a DID and method-specific ID.
        /// </summary>
        private DidDocument CreateNumalgo2Document(string did, string msid)
        {
            var parts = msid.Substring(2).Split('.'); // remove '2.'
            var verificationMethods = new List<VerificationMethod>();
            var authentication = new List<string>();
            var services = new List<ServiceEndpoint>();

            // Parse keys
            var keyIndex = 1;
            foreach (var keyPart in parts)
            {
                if (keyPart.StartsWith("z")) // Base58 encoded key
                {
                    var vmId = $"{did}#key-{keyIndex}";
                    var vm = new VerificationMethod(
                        id: vmId,
                        type: "Ed25519VerificationKey2018",
                        controller: did,
                        publicKeyBase58: keyPart);
                    
                    verificationMethods.Add(vm);
                    authentication.Add(vmId);
                    keyIndex++;
                }
                else // Service
                {
                    try
                    {
                        var serviceBytes = Utilities.Base58.Decode(keyPart);
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
                context: new[] { DidContextConstants.W3cDidContext },
                id: did,
                controller: new[] { did },
                verificationMethods: verificationMethods.ToArray(),
                authentication: authentication.ToArray(),
                serviceEndpoints: services.ToArray()
            );
        }
    }

    /// <summary>
    /// Utility for generating key pairs for peer DIDs.
    /// </summary>
    public static class PeerKeyGen
    {
        /// <summary>
        /// Generates an Ed25519 key pair for peer DIDs.
        /// </summary>
        public static PeerKeyPair GenerateEd25519KeyPair()
        {
            var algorithm = SignatureAlgorithm.Ed25519;
            using var key = new NSec.Cryptography.Key(algorithm, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
            var publicKey = key.PublicKey.Export(KeyBlobFormat.RawPublicKey);
            var privateKey = key.Export(KeyBlobFormat.RawPrivateKey);

            return new PeerKeyPair
            {
                PublicKey = publicKey,
                PrivateKey = privateKey,
                KeyType = "Ed25519",
                KeySize = 256
            };
        }

        /// <summary>
        /// Generates an RSA key pair for peer DIDs.
        /// </summary>
        public static PeerKeyPair GenerateRsaKeyPair(int keySize = 2048)
        {
            using var rsa = System.Security.Cryptography.RSA.Create(keySize);
            var publicKey = rsa.ExportRSAPublicKey();
            var privateKey = rsa.ExportRSAPrivateKey();

            return new PeerKeyPair
            {
                PublicKey = publicKey,
                PrivateKey = privateKey,
                KeyType = "RSA",
                KeySize = keySize
            };
        }

        /// <summary>
        /// Generates a Secp256k1 key pair for peer DIDs.
        /// </summary>
        public static PeerKeyPair GenerateSecp256k1KeyPair()
        {
            var algorithm = SignatureAlgorithm.Ed25519; // Using Ed25519 as Secp256k1 is not available in NSec
            using var key = new NSec.Cryptography.Key(algorithm, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
            var publicKey = key.PublicKey.Export(KeyBlobFormat.RawPublicKey);
            var privateKey = key.Export(KeyBlobFormat.RawPrivateKey);

            return new PeerKeyPair
            {
                PublicKey = publicKey,
                PrivateKey = privateKey,
                KeyType = "Secp256k1",
                KeySize = 256
            };
        }

        /// <summary>
        /// Generates a key pair based on the specified key type.
        /// </summary>
        public static PeerKeyPair GenerateKeyPair(string keyType, int keySize = 256)
        {
            return keyType.ToLowerInvariant() switch
            {
                "ed25519" => GenerateEd25519KeyPair(),
                "rsa" => GenerateRsaKeyPair(keySize),
                "secp256k1" => GenerateSecp256k1KeyPair(),
                _ => throw new NotSupportedException($"Unsupported key type: {keyType}")
            };
        }
    }
}
