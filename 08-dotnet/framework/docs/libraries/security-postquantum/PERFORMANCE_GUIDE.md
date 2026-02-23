# PQC Performance Guide

This guide summarizes performance expectations and tuning options for post-quantum operations in MameyNode.

## Summary

- ML-DSA and ML-KEM are designed to be competitive with, or faster than, classical RSA at comparable security levels.
- Hybrid signatures incur additional overhead but remain acceptable for most transaction and authentication workloads.
- Performance characteristics depend heavily on hardware, liboqs build options, and .NET runtime configuration.

## Key Operations

Typical performance trends (qualitative, not exact numbers):

- **ML-DSA-65**
  - Sign: typically faster than RSA-2048 at similar security levels.
  - Verify: comparable to or slightly slower than ECDSA/Ed25519.
- **ML-KEM-768**
  - Encapsulate/decapsulate: low single-digit microseconds on modern CPUs.
- **Hybrid signatures**
  - Cost ≈ classical + PQC; roughly 1.5× ML-DSA-65 alone in many workloads.

For quantitative data, run the BenchmarkDotNet performance tests in `Mamey.Security.PostQuantum.Tests.Performance` once the test environment has liboqs and CPU isolation.

## Configuration Considerations

### Algorithm Choices

- Use **ML-DSA-65** and **ML-KEM-768** for the default balance between security and performance.
- Use **ML-DSA-44 / ML-KEM-512** in low-risk, latency-sensitive environments.
- Use **ML-DSA-87 / ML-KEM-1024** for maximum security where higher CPU and memory cost is acceptable.

### Hybrid Mode

- Enable hybrid mode only where downgrade resistance is required (validators, high-value wallets, external-facing APIs).
- For internal microservice communication, PQC-only may be sufficient.

### Batching and Reuse

- Reuse signer/encryptor instances instead of constructing them per call.
- Batch PQC operations where possible (e.g., verifying multiple signatures in a single worker).
- Prefer async patterns when integrating with high-throughput services to keep thread pools healthy.

## Benchmarking

To run benchmarks:

1. Ensure `liboqs` is installed and accessible to the test runner.
2. Navigate to `Mamey/src/Mamey.Security.PostQuantum/tests/Mamey.Security.PostQuantum.Tests.Performance`.
3. Run the BenchmarkDotNet project (for example, `dotnet run -c Release`).
4. Review the generated reports (CSV/markdown) for algorithm comparisons.

Benchmarks are organized into:

- `SignatureBenchmarks` – RSA vs ECDSA vs ML-DSA vs hybrid.
- `EncryptionBenchmarks` – RSA vs AES vs ML-KEM-based hybrid encryption.
- `HybridBenchmarks` – cost of dual signatures and verification.

## Operational Tips

- Use CPU pinning and dedicated validator nodes for high-throughput PQC workloads.
- Monitor:
  - Latency (p95, p99) for cryptographic endpoints.
  - CPU utilization on validators and auth gateways.
  - GC activity when running heavy PQC loads.
- For cloud deployments, prefer instances with strong single-core performance and AVX2/AVX-512 support when available.

For security-focused performance trade-offs and audit requirements see `SECURITY_AUDIT.md`.


