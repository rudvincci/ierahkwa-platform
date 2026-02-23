# API Changes for Post-Quantum Support

This document summarizes the key API changes introduced to support post-quantum cryptography in MameyNode.

## Core Security (`Mamey.Security`)

### New/Extended Types

- `SecurityOptions`
  - Added:
    - `bool EnablePostQuantum` – master switch for PQC features.
    - `bool UseHybridSignatures` – when true, emits and validates hybrid signatures.
    - `SignatureAlgorithm DefaultSignatureAlgorithm` – default PQC signature algorithm (ML-DSA-65).
    - `KemAlgorithm DefaultKemAlgorithm` – default PQC KEM algorithm (ML-KEM-768).
    - `DateTimeOffset? QuantumMigrationDeadline` – optional migration deadline.

- `ISecurityProvider`
  - New methods:
    - `byte[] SignPostQuantum(byte[] data, byte[] privateKey, SignatureAlgorithm algorithm)`.
    - `bool VerifyPostQuantum(byte[] data, byte[] signature, byte[] publicKey, SignatureAlgorithm algorithm)`.
    - `byte[] EncryptPostQuantum(byte[] data, byte[] recipientPublicKey, KemAlgorithm algorithm)`.
    - `byte[] DecryptPostQuantum(byte[] ciphertext, byte[] recipientPrivateKey, KemAlgorithm algorithm)`.

- `Signer`
  - Becomes PQC-aware and hybrid-aware:
    - Classical-only behavior preserved when PQC is disabled.
    - When PQC is enabled and a PQC-capable certificate is used, signatures may be PQC-only or hybrid.

- `IEncryptor` / `Encryptor`
  - Extended with ML-KEM-aware methods used internally by `SecurityProvider` and high-level services.

## PQC Library (`Mamey.Security.PostQuantum`)

New public APIs:

- `IPQSigner`, `IPQEncryptor`, `IPQKeyGenerator`, `IHybridCryptoProvider`.
- `MLDSASigner`, `MLKEMEncryptor`, `HybridSigner` concrete implementations.
- Models:
  - `PQKeyPair`, `PQSignature`, `PQCiphertext`, `HybridSignature`.
  - Enums: `SignatureAlgorithm`, `KemAlgorithm` (classical, PQC, hybrid entries).

These are designed to be used directly in advanced scenarios, while most applications interact only with `ISecurityProvider` and high-level service clients.

## Auth Layer (`Mamey.Auth`)

- `JwtOptions` (Mamey.Auth)
  - Added:
    - `bool UsePostQuantumSigning`.
    - `bool UseHybridSigning`.
    - `SignatureAlgorithm SignatureAlgorithm`.
    - `byte[] PQPrivateKey`, `byte[] PQPublicKey` (optional key material).

- `PostQuantumJwtToken`
  - New type representing 3-part or 4-part tokens:
    - Classical: `header.payload.classicalSig`.
    - PQC/Hybrid: `header.payload.classicalSig.pqSig`.
  - Methods:
    - `string ToTokenString()`.
    - `static PostQuantumJwtToken Parse(string token)`.
    - Base64URL helpers for header/payload/signature.

- `PQCAuthenticationService`
  - New helper service that:
    - Generates classical, PQC-only, or hybrid JWTs based on `JwtOptions` and `IPQSigner`.
    - Validates tokens, enforcing PQC/hybrid requirements when configured.

## Blockchain Services

### Crypto Service (`Mamey.Blockchain.Crypto`)

- `crypto.proto`
  - Extended with PQC concepts:
    - `GenerateKeypairRequest`/`Response` support PQC and hybrid keypairs.
    - `SignRequest`/`SignResponse` and `VerifyResponse` include algorithm names and separate classical/PQC fields.
    - `GetSupportedAlgorithms` RPC lists available algorithms and parameters.

- `PostQuantumCryptoService`
  - New gRPC service implementation using `IPQKeyGenerator`, `IPQSigner`, and `IHybridCryptoProvider`.

### Wallet Service (`Mamey.Blockchain.Wallet`)

- `wallet.proto`
  - New / extended messages:
    - `KeyInfo` includes PQC metadata (quantum_resistant, nist_status, security_level, linked_classical_key_id, linked_pq_key_id).
    - `GenerateKeyRequest` supports migration/hybrid flags.
    - `MigrateKey` RPC and related messages for key migration flows.

- `PostQuantumWalletService`
  - New gRPC service skeleton for PQC-aware wallet operations and key migration orchestration.

### Node Service (`Mamey.Blockchain.Node`)

- `node.proto`
  - `BlockInfo` extended with:
    - `signature_algorithm`, `signature`, `classical_signature`, `quantum_resistant`.
  - New RPC:
    - `VerifyBlock(VerifyBlockRequest) returns (VerifyBlockResponse)`.

- Node client models (`Mamey.Blockchain.Node`)
  - `BlockInfo` mirrors proto PQC fields.
  - New `MameyTransaction` type with PQC/hybrid-aware fields and serialization.

### Consensus (`Mamey.Blockchain.Node.Consensus`)

- `ValidatorNodeConfiguration`
  - New PQC fields:
    - `RequireQuantumResistantSignatures`, `QuantumResistantMandatoryDate`, `QuantumResistantMandatoryBlockHeight`.
    - `AcceptHybridTransactions`, `AcceptClassicalTransactions`.
    - `MaxPQCTransactionsPerBlock`, `MaxBlockSizeBytes`.
    - `BaseValidationLevel` (Hybrid, ClassicalOnly, HybridRequired, QuantumResistantOnly).

- `PQCValidator`
  - New validator for PQC-aware transaction and block validation.

For performance considerations and detailed usage examples see `PERFORMANCE_GUIDE.md` and the code snippets in `QUANTUM_RESISTANCE.md`.


