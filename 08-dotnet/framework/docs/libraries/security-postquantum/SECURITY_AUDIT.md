# PQC Security Audit Checklist

This document provides a checklist and guidance for auditing the post-quantum cryptography implementation in MameyNode.

## Scope

- `Mamey.Security` and `Mamey.Security.PostQuantum`.
- PQC-aware services: Crypto, Wallet, Node, Auth/Portals.
- Key management, storage, and migration tooling.

## NIST and Standards Compliance

- [ ] ML-DSA implementation conforms to FIPS 204 (parameter sets, encoding, failure behavior).
- [ ] ML-KEM implementation conforms to FIPS 203.
- [ ] Only NIST-approved parameter sets are enabled in production.
- [ ] NIST test vectors are configured and run in a controlled environment (liboqs + official vector sets).

## Cryptographic Correctness

- [ ] All PQC operations use `System.Security.Cryptography.RandomNumberGenerator` (or platform equivalent) for randomness.
- [ ] Key sizes and signature sizes match those documented in `PQC_ALGORITHMS.md`.
- [ ] All error paths for sign/verify/encapsulate/decapsulate are handled without leaking secrets.

## Side-Channel and Constant-Time Behavior

- [ ] Verify that liboqs is built with side-channel countermeasures enabled.
- [ ] Confirm that critical operations (ML-DSA, ML-KEM, AES-GCM) are constant-time with respect to secret data.
- [ ] Perform timing analysis on key operations under varied inputs.

## Key Management and Storage

- [ ] Private keys are always stored encrypted at rest (e.g., AES-256-GCM with strong KDF).
- [ ] PostgreSQL `wallet_keys_v2` schema enforces key size limits and PQC metadata.
- [ ] MongoDB `wallet_keys_v2` collection mirrors relational schema conceptually.
- [ ] Key migration log captures classical→PQC mappings and statuses.
- [ ] Access controls prevent unauthorized key export or modification.

## Hybrid and Downgrade Protection

- [ ] Hybrid signatures require both classical and PQC parts to be valid where configured.
- [ ] Validators and portals reject classical-only signatures after the configured migration deadline.
- [ ] Protocol messages include algorithm identifiers and cannot be silently downgraded.

## Authentication and Authorization

- [ ] PQC JWTs and hybrid JWTs are validated using correct algorithms and keys.
- [ ] Classical 3-part JWTs are still accepted where policy allows, and rejected where `requirePQC=true`.
- [ ] Multi-auth pipeline enforces PQC policies consistently across portals.

## Logging and Monitoring

- [ ] Security-relevant events (key generation, migration, failed verification) are logged with appropriate detail.
- [ ] Logs do not contain sensitive key material or full signatures.
- [ ] Metrics exist for PQC operation counts, latencies, and error rates.

## Penetration Testing

- [ ] External penetration test performed on PQC-enabled endpoints.
- [ ] Attack scenarios: downgrade, replay, MITM, key substitution, token forgery.
- [ ] No critical or high-severity findings remain open; remediation plan exists for medium/low findings.

## Documentation and Training

- [ ] Developers and operators have reviewed `QUANTUM_RESISTANCE.md` and `MIGRATION_GUIDE.md`.
- [ ] Runbooks include PQC-specific steps for deployment, rollback, and incident response.

Audit results should be captured in dedicated reports (for example: `security-audit/PENETRATION_TEST_RESULTS.md`, `security-audit/NIST_COMPLIANCE_CHECKLIST.md`) as referenced in the scrum tasks.


