# Contributing to Mamey.Authentik

Thank you for your interest in contributing to Mamey.Authentik!

## Getting Started

1. Fork the repository
2. Clone your fork
3. Create a feature branch
4. Make your changes
5. Add tests
6. Ensure all tests pass
7. Submit a pull request

## Development Setup

1. Install .NET 9.0 SDK
2. Install NSwag CLI: `dotnet tool install -g NSwag.ConsoleCore`
3. Restore dependencies: `dotnet restore`
4. Build: `dotnet build`
5. Run tests: `dotnet test`

## Code Style

- Follow C# coding conventions
- Use XML documentation comments for public APIs
- Ensure code passes all linters
- Maintain 90%+ test coverage

## Pull Request Process

1. Update CHANGELOG.md with your changes
2. Update documentation if needed
3. Ensure all CI checks pass
4. Request review from maintainers

## Code Generation

When Authentik API changes:

1. Update schema: `./scripts/update-schema.sh <authentik-url>`
2. Regenerate client: `./scripts/generate-client.sh <authentik-url>`
3. Review generated code
4. Update service implementations
5. Update tests
6. Update documentation

## Questions?

Open an issue for questions or discussions.
