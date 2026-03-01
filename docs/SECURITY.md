# Security Policy

## Supported Versions
| Version | Supported |
|---------|-----------|
| 3.2.x | Yes (current) |
| 3.1.x | Security fixes only |
| < 3.0 | No |

## Reporting Vulnerabilities
Report security issues to: security@ierahkwa.org
Do NOT create public GitHub issues for security vulnerabilities.

## Security Architecture

### Encryption
- Transport: TLS 1.3 + HTTP/2
- At-rest: AES-256-GCM
- Post-quantum: ML-DSA-65 (signatures) + ML-KEM-1024 (key exchange)
- Zero-knowledge proofs for identity verification

### Authentication
- JWT RS256 tokens with tier-based access
- FWID biometric identity verification
- 2FA required for all admin access
- Social recovery for DID (did:wampum)

### Infrastructure
- Kubernetes namespace isolation
- NetworkPolicies per NEXUS domain
- Pod security contexts (non-root, read-only filesystem)
- Secret management via K8s Secrets (encrypted at rest)
- cert-manager for automatic TLS certificate rotation

### Compliance
- UNDRIP (UN Declaration on Rights of Indigenous Peoples)
- ILO Convention 169
- WCAG 2.2 AA (100% accessible)
- GAAD (Global Accessibility Awareness Day)
- Censorship-resistant by design

### Monitoring
- Real-time security alerts via Grafana
- Prometheus metrics for anomaly detection
- Distributed tracing via OpenTelemetry + Jaeger
- Centralized logging via Loki
