# PQC Algorithms in MameyNode

This document describes the post-quantum algorithms used by MameyNode, their parameters, and how they are exposed in the Mamey framework.

## Overview

MameyNode uses NIST-selected algorithms from the ML-DSA (FIPS 204) and ML-KEM (FIPS 203) families, plus optional hybrid constructions that combine classical and PQC schemes:

- **Signatures**: ML-DSA-44, ML-DSA-65, ML-DSA-87 (a.k.a. Dilithium 2/3/5) and classical ECDSA/Ed25519.
- **KEM / key exchange**: ML-KEM-512, ML-KEM-768, ML-KEM-1024 (a.k.a. Kyber 512/768/1024) and classical RSA/ECDH.
- **Hybrid schemes**: RSA+ML-DSA and ECDSA+ML-DSA for layered security during migration.

All algorithms are modeled via `SignatureAlgorithm` and `KemAlgorithm` enums in `Mamey.Security.PostQuantum.Models`.

## Signature Algorithms (ML-DSA)

| Algorithm        | Enum value             | Public key size | Private key size | Signature size | Security level     |
|------------------|------------------------|-----------------|------------------|----------------|--------------------|
| ML-DSA-44       | `ML_DSA_44`            | 1312 bytes      | 2528 bytes       | 2420 bytes     | NIST Level 2 (128) |
| ML-DSA-65       | `ML_DSA_65`            | 1952 bytes      | 4000 bytes       | 3293 bytes     | NIST Level 3 (192) |
| ML-DSA-87       | `ML_DSA_87`            | 2592 bytes      | 4864 bytes       | 4595 bytes     | NIST Level 5 (256) |

These sizes are enforced in `MLDSASigner` and surfaced through the `SignatureSize`, `PublicKeySize`, and `PrivateKeySize` properties.

### Usage in Code

- `MLDSASigner` implements `IPQSigner` and wraps liboqs for ML-DSA key generation, signing, and verification.
- `SecurityProvider` and `Signer` route signing/verification to ML-DSA when PQC is enabled.
- Hybrid signatures are represented by `HybridSignature` which stores classical and PQC parts with a compact binary format.

## KEM Algorithms (ML-KEM)

| Algorithm        | Enum value             | Public key size | Private key size | Ciphertext size | Shared secret size |
|------------------|------------------------|-----------------|------------------|-----------------|--------------------|
| ML-KEM-512      | `ML_KEM_512`           | 800 bytes       | 1632 bytes       | 768 bytes       | 32 bytes           |
| ML-KEM-768      | `ML_KEM_768`           | 1184 bytes      | 2400 bytes       | 1088 bytes      | 32 bytes           |
| ML-KEM-1024     | `ML_KEM_1024`          | 1568 bytes      | 3168 bytes       | 1568 bytes      | 32 bytes           |

These parameters are enforced in `MLKEMEncryptor` via internal `GetSizes`.

### Usage in Code

- `MLKEMEncryptor` implements `IPQEncryptor` and wraps liboqs KEM APIs.
- It also exposes `EncryptMLKEM` / `DecryptMLKEM` helpers that:
  - Perform ML-KEM encapsulation/decapsulation.
  - Use the shared secret as an AES-256-GCM key for data encryption.
- `IEncryptor` and `SecurityProvider` add PQC entry points that can be mapped to ML-KEM operations in higher-level services.

## Hybrid Algorithms

Hybrid algorithms are modeled as enum values and rely on composition:

- `SignatureAlgorithm.HYBRID_RSA_MLDSA65`
- `SignatureAlgorithm.HYBRID_ECDSA_MLDSA65`
- `KemAlgorithm.HYBRID_RSA_MLKEM768`

Hybrid signatures:

- Use classical RSA/ECDSA (for continuity with existing PKI) and ML-DSA (for quantum resistance).
- Are serialized into a single byte array via `HybridSignature.CombinedSignature` and parsed back with `HybridSignature.TryParse`.
- Are verified in `Signer` and `SecurityProvider` by checking both classical and PQC portions.

## API Surfaces

Key types and enums are defined in `Mamey.Security.PostQuantum.Models`:

- `PQKeyPair` – raw key pair structure for PQC algorithms.
- `PQSignature`, `PQCiphertext` – typed wrappers around signature/ciphertext byte arrays.
- `HybridSignature` – container for classical + PQC signatures.
- `SignatureAlgorithm`, `KemAlgorithm` – enumerations listing classical, PQC, and hybrid variants.

Interfaces in `Mamey.Security.PostQuantum.Interfaces`:

- `IPQSigner` – key generation (via helper), sign, and verify.
- `IPQKeyGenerator` – algorithm-agnostic key generation.
- `IPQEncryptor` – KEM encapsulation/decapsulation and optional hybrid encryption.
- `IHybridCryptoProvider` – high-level hybrid signing/verification.

## Algorithm Selection

Selection happens at multiple levels:

- **Configuration** (`SecurityOptions`, `JwtOptions`, validator configs):
  - Default signature algorithm is ML-DSA-65.
  - Default KEM algorithm is ML-KEM-768.
- **Runtime**:
  - gRPC `CryptoService` accepts requested algorithms via RPC messages.
  - Wallet and node services record which algorithm was used for each key/block/transaction.
- **Migration tooling**:
  - The key migration CLI accepts `--target-algorithm` (e.g., `ml-dsa-65`) and maps it to `SignatureAlgorithm`.

For detailed migration flows and configuration examples see `MIGRATION_GUIDE.md`.


