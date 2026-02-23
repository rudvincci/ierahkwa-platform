# Contributing to Ierahkwa Sovereign Platform

# Contribuir a la Plataforma Soberana Ierahkwa

Niawenhko:wa — Thank you for your interest in contributing!

## Development Setup / Configuración

### Prerequisites / Requisitos

- Docker 27+ & Docker Compose
- Node.js 22 LTS
- .NET 10 SDK
- Rust 1.80+ (for MameyNode/MameyForge)
- Go 1.22+ (for bridge services)
- PostgreSQL 16 (or use Docker)
- Redis 7 (or use Docker)

### Quick Start / Inicio Rápido

```bash
git clone https://github.com/rudvincci/ierahkwa-platform.git
cd ierahkwa-platform
cp .env.example .env
docker compose -f docker-compose.sovereign.yml up -d
```

## Branch Naming / Nombres de Ramas

| Prefix | Purpose / Propósito |
|--------|-------------------|
| `feature/` | New functionality / Nueva funcionalidad |
| `fix/` | Bug fixes / Corrección de errores |
| `docs/` | Documentation / Documentación |
| `infra/` | Infrastructure changes / Cambios de infraestructura |
| `lang/` | Indigenous language additions / Idiomas indígenas |

Example: `feature/atabey-quechua-support`

## Commit Messages / Mensajes de Commit

```
<type>(<scope>): <description>

Types: feat, fix, docs, style, refactor, test, infra, lang
Scope: gateway, identity, mameynode, voz-soberana, atabey, etc.
```

Example: `feat(atabey): add Quechua language support`

## Pull Request Process / Proceso de PR

1. Fork the repository / Hacer fork del repositorio
2. Create a feature branch / Crear rama de feature
3. Make changes with tests / Hacer cambios con tests
4. Submit PR with description / Enviar PR con descripción
5. Wait for review / Esperar revisión

## Code Style / Estilo de Código

| Language | Standard |
|----------|----------|
| C# (.NET) | Microsoft conventions, nullable enabled |
| JavaScript | ESLint with Prettier |
| Rust | `rustfmt` default config |
| Go | `gofmt` |
| Python | Black formatter |

## Indigenous Language Guidelines

When adding indigenous language support:

1. Work with native speakers for translations
2. Respect cultural context — some terms may not have direct translations
3. Use Unicode correctly for special characters
4. Add the language to Atabey Translator config
5. Test with right-to-left (RTL) if applicable

## Code of Conduct

Please read [CODE_OF_CONDUCT.md](./CODE_OF_CONDUCT.md)

---

*Skennen — Peace*
