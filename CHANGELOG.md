# Changelog

All notable changes to the Ierahkwa Sovereign Platform will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.1.0] - 2026-02-22

### Added
- Dockerfiles for 16 services (gateway, identity, treasury, mameynode, voz-soberana, bdet-bank, atabey, zkp-service, and more)
- Test infrastructure with Jest configuration and test suites for Node.js services
- ESLint configuration with project-wide linting rules
- EditorConfig for consistent formatting across all languages
- Prettier configuration for JavaScript/TypeScript formatting
- Go SDK expansion with wallet, token, transaction, and error modules
- CI/CD pipeline improvements with GitHub Actions workflows
- CHANGELOG following Keep a Changelog format
- DEPLOYMENT documentation with full environment setup guides
- Pull request and issue templates for GitHub
- `.npmrc` configuration for strict engine and dependency management

## [2.0.0] - 2026-02-22

### Added
- All 17,463 files unified into the Soberano-Organizado monorepo structure
- 41 .NET microservices across the sovereign platform ecosystem
- 194 .NET framework projects covering identity, treasury, governance, and more
- 16 Node.js services including gateway, BDET bank, Voz Soberana, and Atabey translator
- 39 HTML interactive platforms for citizen-facing applications
- 5 interactive presentations (pitch decks) for sovereign platform stakeholders
- 474 sovereign tokens representing indigenous nations and territories
- 368 government documents including constitutions, treaties, and legal frameworks
- Post-quantum encryption layer (Kyber/Dilithium) for future-proof security
- MameyNode blockchain with Rust-based core and Go bridge services
- MameyForge smart contract engine
- ZKP (Zero-Knowledge Proof) identity verification service
- CI/CD pipeline with GitHub Actions for automated testing and deployment
- Docker Compose configurations for development and staging environments
- Kubernetes manifests and Terraform infrastructure-as-code
- Nginx reverse proxy configuration with SSL termination
- Prometheus and Grafana monitoring stack

## [1.0.0] - 2026-02-22

### Added
- Initial Soberano-Organizado repository structure
- Directory organization with 17 top-level categories
- Base configuration files (package.json, docker-compose, Makefile)
- README with project overview and directory map
- CONTRIBUTING guidelines with branch naming and commit conventions
- CODE_OF_CONDUCT for community participation
- LICENSE (MIT) for open-source distribution
- Security policy (SECURITY.md) with vulnerability reporting process

[Unreleased]: https://github.com/rudvincci/ierahkwa-platform/compare/v2.1.0...HEAD
[2.1.0]: https://github.com/rudvincci/ierahkwa-platform/compare/v2.0.0...v2.1.0
[2.0.0]: https://github.com/rudvincci/ierahkwa-platform/compare/v1.0.0...v2.0.0
[1.0.0]: https://github.com/rudvincci/ierahkwa-platform/releases/tag/v1.0.0
