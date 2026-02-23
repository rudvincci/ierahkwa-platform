# Quantum Resistance in MameyNode

## Overview

MameyNode has been upgraded with post-quantum cryptography (PQC) to protect long-lived secrets, transactions, and identities against adversaries with large-scale quantum computers. The design follows the NIST PQC standards, combines classical and PQC algorithms during the transition period, and keeps the rest of the Mamey framework consistent with existing DDD, CQRS, and microservice patterns.

This document explains the threat model, high-level architecture, and how the PQC building blocks fit into MameyNode.

## Threat Model

- **Adversary capabilities**:
  - Can record network traffic today and decrypt it later once quantum computers are viable (store-now, decrypt-later).
  - Can eventually break classical public-key algorithms (RSA, ECDSA, Ed25519) using Shor's algorithm.
  - May attempt downgrade attacks to force classical-only crypto when PQC is available.
- **Assets to protect**:
  - Long-term blockchain keys (validators, wallets, bridge keys).
  - Authentication tokens (JWTs, service-to-service identities).
  - Encrypted data at rest (database keys, secrets).
  - Network-level confidentiality of sensitive channels.
- **Non-goals**:
  - We do not attempt to defend against physical compromise of validators or HSMs.
  - Side-channel and fault attacks are handled primarily by the underlying liboqs and .NET crypto primitives, with additional guidance in `SECURITY_AUDIT.md`.

## Architecture Overview

At a high level, PQC support is layered as follows:

- **Crypto layer (`Mamey.Security.PostQuantum`)**
  - Provides ML-DSA (FIPS 204) and ML-KEM (FIPS 203) primitives via `liboqs` P/Invoke.
  - Exposes interfaces `IPQSigner`, `IPQEncryptor`, `IPQKeyGenerator` and a `HybridSigner` for classical+PQC combinations.
- **Core security layer (`Mamey.Security`)**
  - Extends `SecurityOptions`, `Signer`, `SecurityProvider`, and `IEncryptor` to understand PQC algorithms and hybrid signatures.
  - Adds `SignPostQuantum`, `VerifyPostQuantum`, `EncryptPostQuantum`, and `DecryptPostQuantum` APIs.
- **Blockchain services**
  - `Mamey.Blockchain.Crypto`: PQC-aware gRPC `CryptoService` (key generation, sign/verify, algorithm discovery).
  - `Mamey.Blockchain.Wallet`: PQC wallet keys, migration, and transaction signing support.
  - `Mamey.Blockchain.Node`: PQC-aware blocks, transactions, and consensus validator configuration.
- **Application / auth layer**
  - `Mamey.Auth` and `MameyNode.Portals`: PQC and hybrid JWT tokens, PQC-aware multi-auth pipeline.
- **Tooling & migrations**
  - Database migrations for PostgreSQL and MongoDB.
  - Key migration CLI (`Mamey.Tools.QuantumMigration`) with reporting and rollback support.

## Migration Strategy

The migration strategy is built around four phases:

1. **Foundation**
   - Introduce PQC primitives (`Mamey.Security.PostQuantum`) and wire them into `Mamey.Security` without changing external APIs.
2. **Dual support (hybrid mode)**
   - Enable hybrid signatures and PQC-only keys in new services and schemas while supporting existing classical keys.
   - Add wallet and node data structures capable of storing multiple signatures and PQC metadata.
3. **Gradual adoption**
   - Migrate keys and services in stages using the key migration tool and database migrations.
   - Introduce PQC-aware policies in validators and portals (e.g., require PQC after specific dates).
4. **Quantum-resistant only**
   - Once sufficient adoption is reached, configure validators and auth layers to reject classical-only signatures beyond the migration deadline.

Each phase is designed to be reversible (via rollbacks and dual-write strategies) so that issues discovered in staging or testnet can be addressed before continuing.

## Hybrid vs PQC-only Modes

- **Classical-only**
  - Existing algorithms (RSA, ECDSA, Ed25519) continue to work without change.
- **PQC-only**
  - Keys and signatures are purely ML-DSA/ML-KEM; classical signatures are optional.
  - Used for new deployments where quantum resistance is mandatory from day one.
- **Hybrid mode**
  - Produces and validates both classical and PQC signatures.
  - JWTs and blockchain transactions can carry dual signatures.
  - Validators and auth pipelines can require both signatures to prevent downgrade attacks.

Hybrid mode is the recommended configuration during the long transition window while classical clients are still present.

## Components and Responsibilities

- `Mamey.Security.PostQuantum`:
  - Implements ML-DSA and ML-KEM using liboqs.
  - Provides models (`PQKeyPair`, `PQSignature`, `HybridSignature`) and enums (`SignatureAlgorithm`, `KemAlgorithm`).
- `Mamey.Security`:
  - Routes signing and verification to PQC primitives when enabled.
  - Provides configuration knobs (e.g., `EnablePostQuantum`, `UseHybridSignatures`).
- `Mamey.Blockchain.*`:
  - Crypto service: PQC key lifecycle and RPC APIs.
  - Wallet: PQC key storage, migration logs, and hybrid linkage.
  - Node: PQC-aware transactions, blocks, and consensus policies.
- `Mamey.Auth` & `MameyNode.Portals`:
  - PQC/hybrid JWT tokens, PQC-aware authentication policies.

## When to Enable PQC

You should enable PQC when:

- Your threat model includes long-lived secrets or assets (e.g., government identities, CBDC keys, high-value validators).
- You want to prepare for NIST-compliant PQC rollouts ahead of regulatory requirements.
- You are planning a new deployment and can start in hybrid or PQC-only mode from day one.

For existing mainnet deployments, follow the migration approach in `MIGRATION_GUIDE.md`, starting with staging and testnet rollouts before flipping production flags.


