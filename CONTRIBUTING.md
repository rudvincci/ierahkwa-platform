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

## Testing Requirements / Requisitos de Testing

All contributions must include passing tests. Run the appropriate test suite before submitting a PR.

### Node.js Services

```bash
# Run all Node.js tests
npm test

# Run tests for a specific service
npm test -- --testPathPattern=03-node-services/gateway

# Run tests with coverage
npm run test:coverage

# Run integration tests (requires Docker services running)
npm run test:integration
```

### .NET Microservices

```bash
# Run all .NET tests
dotnet test

# Run tests for a specific project
dotnet test 08-dotnet/tests/Sovereign.Identity.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Go SDK

```bash
# Run Go tests
cd 11-sdks/go/mamey
go test ./...

# Run with verbose output
go test -v ./...

# Run with race detection
go test -race ./...
```

### Rust (MameyNode / MameyForge)

```bash
# Run Rust tests
cd 10-core/mameynode
cargo test

# Run with verbose output
cargo test -- --nocapture
```

### Test Coverage Expectations

| Language | Minimum Coverage |
|----------|-----------------|
| Node.js (Jest) | 70% for new code |
| .NET (xUnit) | 70% for new code |
| Go | 60% for new code |
| Rust | 60% for new code |

## Linting / Linting

The project uses automated linting to maintain consistent code style across all languages.

### ESLint (JavaScript/Node.js)

```bash
# Run ESLint across the project
npx eslint .

# Run with auto-fix
npx eslint . --fix

# Lint a specific directory
npx eslint 03-node-services/gateway/
```

Configuration is in `.eslintrc.json` at the project root. The config extends `eslint:recommended` and enforces semicolons, single quotes, and catches unused variables.

### Prettier (JavaScript/TypeScript/JSON)

```bash
# Check formatting
npx prettier --check .

# Auto-format files
npx prettier --write .
```

Configuration is in `.prettierrc.json` at the project root.

### EditorConfig

The `.editorconfig` file ensures consistent formatting across all editors and IDEs. Install the EditorConfig plugin for your editor:

- **VS Code**: EditorConfig for VS Code (editorconfig.editorconfig)
- **JetBrains IDEs**: Built-in support
- **Vim**: editorconfig-vim plugin

Key settings:
- UTF-8 charset, LF line endings for all files
- 2-space indentation for JS/TS/JSON/YAML
- 4-space indentation for C#, Rust, Python, Solidity
- Tab indentation for Go and Makefiles

### Language-Specific Formatters

| Language | Formatter | Command |
|----------|-----------|---------|
| JavaScript/TypeScript | ESLint + Prettier | `npx eslint . --fix && npx prettier --write .` |
| C# (.NET) | dotnet format | `dotnet format` |
| Rust | rustfmt | `cargo fmt` |
| Go | gofmt | `gofmt -w .` |
| Python | Black | `black .` |

## Code Quality Standards / Estándares de Calidad

### General Guidelines

1. **No secrets or credentials** in code -- use environment variables and `.env` files (never committed)
2. **Meaningful variable and function names** -- avoid single-letter names except in loops
3. **Comments for complex logic** -- explain "why", not "what"
4. **Error handling** -- never silently swallow errors; log or propagate them
5. **Small, focused functions** -- each function should do one thing well
6. **DRY principle** -- avoid code duplication; extract shared logic into utilities

### Pre-Commit Checks

Before submitting a PR, ensure:

```bash
# 1. Linting passes
npx eslint .

# 2. Formatting is correct
npx prettier --check .

# 3. Tests pass
npm test

# 4. No secrets in staged files
git diff --cached --name-only | xargs grep -l "API_KEY\|SECRET\|PASSWORD\|PRIVATE_KEY" || echo "No secrets found"
```

### CI/CD Validation

The GitHub Actions CI pipeline automatically runs on every PR:

- ESLint linting
- Prettier format checking
- Jest test suite with coverage
- .NET build and test
- Docker build validation
- Security audit (`npm audit`)

PRs that fail any CI check will not be merged.

### Dependency Management

- Use `npm ci` (not `npm install`) in CI for reproducible builds
- Pin exact versions (configured via `.npmrc` with `save-exact=true`)
- Review dependency updates before merging Dependabot PRs
- Avoid adding unnecessary dependencies -- prefer built-in Node.js/standard library modules

## Code of Conduct

Please read [CODE_OF_CONDUCT.md](./CODE_OF_CONDUCT.md)

---

*Skennen -- Peace*
