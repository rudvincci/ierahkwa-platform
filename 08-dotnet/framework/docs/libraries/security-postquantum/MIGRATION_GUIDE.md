# PQC Migration Guide for MameyNode

This guide describes how to migrate an existing MameyNode deployment from classical cryptography to a quantum-resistant posture using ML-DSA, ML-KEM, and hybrid modes.

## Prerequisites

- MameyNode upgraded to the PQC-enabled versions of:
  - `Mamey.Security` and `Mamey.Security.PostQuantum`.
  - `Mamey.Blockchain.Crypto`, `Mamey.Blockchain.Wallet`, `Mamey.Blockchain.Node`.
  - `Mamey.Auth` and `MameyNode.Portals`.
- Database migrations applied:
  - PostgreSQL: `001_add_pqc_support.sql`.
  - MongoDB: `001_add_pqc_support.js`.
- Key migration tool available:
  - `Mamey.Tools.QuantumMigration` CLI built and accessible.
- Staging and testnet environments mirroring production topology.

## High-Level Phases

1. **Prepare**
   - Upgrade code and apply database migrations.
   - Enable PQC in configuration (staging first).
2. **Dual-write (hybrid)**
   - Generate new PQC keys for critical actors (validators, wallets).
   - Start producing hybrid signatures while still accepting classical-only.
3. **Key migration**
   - Use the migration tool to create PQC/hybrid equivalents for existing classical keys.
   - Maintain classical→PQC linkage for compatibility.
4. **Enforce PQC**
   - Configure validators and auth layers to require PQC/hybrid after a defined date or block height.
5. **Decommission classical-only**
   - After sufficient adoption, phase out classical-only keys and signatures where regulations allow.

## Step-by-Step Migration (Staging)

1. **Apply migrations**
   - Run PostgreSQL and MongoDB migrations on staging.
   - Validate using `Mamey/migrations/validate.sh`.

2. **Enable PQC configuration**
   - In `security` section, set:
     - `EnablePostQuantum = true`.
     - `UseHybridSignatures = true` for critical services.
   - In node validator configs:
     - Set `BaseValidationLevel = Hybrid`.
     - Set `RequireQuantumResistantSignatures = false` initially.
   - In portals (`multiAuth.postQuantum`):
     - `enabled = true`, `requirePQC = false`, `acceptHybrid = true`.

3. **Generate new PQC keys**
   - Use `CryptoService` gRPC or CLI tools to generate ML-DSA/ML-KEM keys for:
     - Validator nodes.
     - System wallets and bridges.
   - Store keys in `wallet_keys_v2` with proper PQC metadata.

4. **Run dual-signing in staging**
   - Enable hybrid signatures for JWTs and blockchain transactions.
   - Verify that classical clients still function while PQC-aware components see PQC metadata.

5. **Run key migration tool**
   - For non-ephemeral keys, run:
     - `dotnet run --project Mamey/tools/Mamey.Tools.QuantumMigration migrate-keys --source-wallet <path> --target-algorithm ml-dsa-65 --hybrid-mode true --migrate-transactions false`.
   - Review `migration-report.json` and address failures.

6. **Testing**
   - Execute unit, integration, and performance tests.
   - Confirm that PQC + hybrid paths pass existing test suites.

## Production Rollout (Summary)

1. **Testnet deployment**
   - Deploy PQC-enabled stack to testnet with hybrid mode on.
   - Run community testing and collect metrics.

2. **Mainnet phased rollout**
   - Follow the phased deployment strategy (5% → 25% → 50% → 100%) described in the deployment tasks.
   - Keep `RequirePQC` disabled but hybrid mode enabled during early phases.

3. **Enforce PQC**
   - Once adoption is sufficient, set:
     - Validators: `RequireQuantumResistantSignatures = true`, `QuantumResistantMandatoryDate` to your deadline.
     - Portals: `postQuantum.requirePQC = true`, `postQuantum.mandatoryDate` to the same date.

4. **Post-migration validation**
   - Confirm that classical-only signatures are rejected after the deadline, while hybrid/PQC-only succeed.
   - Validate that historical data (blocks, transactions, keys) remain readable and verifiable.

## Rollback Procedures

If a critical issue is discovered:

- **Database rollback**
  - Use `rollback_001.sql` (PostgreSQL) and `rollback_001.js` (MongoDB) to revert PQC schema changes.
  - Restore from backups taken immediately before migration.
- **Key migration rollback**
  - Use the key migration tool's `rollbackKeyAsync` hooks in your host application to revert per-key changes.
- **Configuration rollback**
  - Set `EnablePostQuantum = false` and `UseHybridSignatures = false` in `SecurityOptions`.
  - Disable `multiAuth.postQuantum` in portal configs.
  - Set validator `RequireQuantumResistantSignatures = false` and revert validation levels to classical.

Document all rollback executions in your operational runbooks and ensure they are rehearsed in staging before use on mainnet.


