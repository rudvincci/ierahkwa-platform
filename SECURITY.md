# Security Policy / Política de Seguridad

## Architecture

All services in the Ierahkwa Sovereign Platform follow these security principles:

### Network Isolation
- All services bind to `127.0.0.1` (never `0.0.0.0`)
- Docker sovereign-net bridge network: `172.28.0.0/16`
- External access only through Nginx reverse proxy with TLS 1.3

### Post-Quantum Cryptography
- **Digital Signatures:** ML-DSA-65 (FIPS 204)
- **Key Encapsulation:** ML-KEM-1024 (FIPS 203)
- **Zero-Knowledge Proofs:** Groth16 for identity verification
- **Hashing:** SHA-3-256 for blockchain state

### Authentication
- JWT with ES256 signatures
- API Keys: 256-bit minimum
- Rate limiting on all endpoints
- ZKP-based identity verification for sovereign citizens

### Data Sovereignty
- All data encrypted at rest (AES-256-GCM)
- TLS 1.3 mandatory for all communications
- No data leaves sovereign jurisdiction without explicit consent
- FWID (Future Wampum ID) for decentralized identity

## Reporting Vulnerabilities

If you discover a security vulnerability, please report it responsibly:

1. **DO NOT** open a public GitHub issue
2. Contact: github.com/rudvincci (private message)
3. Include: description, reproduction steps, impact assessment
4. We will respond within 72 hours
5. We will credit you in the security advisory (if desired)

## Security Checklist Status

- [x] Services bound to 127.0.0.1
- [x] TLS certificate generation scripts
- [x] Docker network isolation
- [x] .env.example with no real secrets
- [x] .gitignore excludes sensitive files
- [ ] Automated security scanning (planned)
- [ ] Penetration testing (planned Q2 2026)
- [ ] SOC 2 compliance audit (planned Q3 2026)

---

*Security is sovereignty. Sovereignty is security.*
