# Changelog

All notable changes to the Ierahkwa Sovereign Platform will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.4.0] — 2026-02-23

### Added
- **Seguridad Soberana** — Sovereign AppSec platform (replaces Aikido, Snyk, SonarQube)
- **IDE Soberano** — Sovereign agentic development environment (replaces Kiro.dev, VS Code, Cursor)
- **Agente Soberano** — Sovereign AI coding agent with CLI and 8 skills (replaces OpenCode.ai, Qwen Code)
- **SAST AutoFix Engine** (`07-scripts/sast-autofix.js`) — 8 fix patterns + 8 code quality rules
- **Custom SAST Rules** (`07-scripts/custom-rules.json`) — 5 sovereign-specific SAST rules
- **Nube Soberana** — Sovereign cloud & collaboration (replaces Nextcloud, Google Workspace)
- **Repositorio Soberano** — Sovereign artifact repository (replaces Cloudsmith, Nexus, Artifactory)
- **LowCode Soberano** — Sovereign low-code application platform (replaces Budibase, Retool)
- **Automatización Soberana** — Sovereign process automation (replaces Huginn, Zapier, IFTTT)
- **Flujos Soberano** — Sovereign flow-based programming (replaces Node-RED, n8n)
- **ML Soberano** — Sovereign machine learning platform (replaces PyCaret, SageMaker)
- **DevOps Soberano** — Sovereign DevOps automation (replaces StackStorm, Ansible Tower)
- **Plantillas Soberana** — Sovereign template & page builder (replaces GrapeJS, Webflow)
- **Orquestador Soberano** — Sovereign LLM orchestration (replaces Flowise, LangFlow, Dify)
- **Colaboración Soberana** — Sovereign workspace & collaboration (replaces AppFlowy, Notion)
- **Backend Soberano** — Sovereign low-code backend platform (replaces Manifest.Build, Supabase, Firebase)
- AI coding agent CLI with skills system, session management, and MCP protocol support

### Changed
- Platform count updated from 59 to **70 flagship** (332+ total)
- README.md updated with all new platform entries and counts

## [2.3.0] — 2026-02-22

### Added
- **9 GitHub Agentic Workflows** for Continuous AI (triage, docs, testing, security, quality, reporting, translation, performance, supply chain)
- **17 new test suites** with 275+ test cases across 11 backend services
- **ACCESSIBILITY.md** — GAAD Pledge compliance, WCAG 2.2 AA standards
- **Accessibility middleware** (`03-backend/shared/accessibility.js`) with HTML audit, error handling, a11y headers
- **Accessibility audit tool** (`04-infraestructura/accessibility/a11y-audit.js`) scanning 51 platforms
- **Playwright E2E test suite** — platform smoke tests, WCAG checks, keyboard trap detection, API health
- **Supply chain security** — Shai-Hulud defense with lifecycle script auditing, SBOM generation, secret scanning
- **Supply chain audit tool** (`07-scripts/supply-chain-audit.js`) detecting malicious patterns
- **Robótica Soberana** — Sovereign robotics platform (replaces ROS/NVIDIA Isaac)
- **Ecosistema Abierto** — Open source ecosystem showcase (replaces GitHub Collections)
- **Copilot instructions** (`.github/copilot-instructions.md`) with full architecture patterns
- **Dependabot** monitoring npm, NuGet, Cargo, Docker, and GitHub Actions
- **2 new issue templates** — accessibility barriers, agentic workflow requests
- **Accessibility CI workflow** running WCAG checks on HTML platform PRs
- **Supply chain security CI workflow** with dependency audit, artifact validation, SBOM

### Changed
- **14 HTML platforms** updated with skip-nav, `<main>` landmarks, focus-visible, reduced-motion
- **CI/CD pipeline** expanded with accessibility and expanded-tests jobs
- **`.npmrc`** hardened: ignore-scripts, save-exact, audit-level=high, namespace protection
- **`SECURITY.md`** updated with supply chain security control matrix

### Fixed
- Missing semantic landmarks across sovereign HTML platforms
- Missing keyboard focus indicators on interactive elements
- Missing `prefers-reduced-motion` media query support

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

[Unreleased]: https://github.com/rudvincci/ierahkwa-platform/compare/v2.3.0...HEAD
[2.3.0]: https://github.com/rudvincci/ierahkwa-platform/compare/v2.1.0...v2.3.0
[2.1.0]: https://github.com/rudvincci/ierahkwa-platform/compare/v2.0.0...v2.1.0
[2.0.0]: https://github.com/rudvincci/ierahkwa-platform/compare/v1.0.0...v2.0.0
[1.0.0]: https://github.com/rudvincci/ierahkwa-platform/releases/tag/v1.0.0
