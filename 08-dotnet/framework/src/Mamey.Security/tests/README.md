# Mamey.Security Test Suite

Comprehensive test suite for the Mamey.Security library ecosystem, including unit tests, integration tests, performance benchmarks, and security validation tests.

## Test Structure

```
tests/
├── Mamey.Security.Tests.Unit/          # Unit tests (fast, no external dependencies)
├── Mamey.Security.Tests.Integration/    # Integration tests (requires Docker)
├── Mamey.Security.Tests.Performance/   # Performance benchmarks (BenchmarkDotNet)
└── Mamey.Security.Tests.Shared/        # Shared test utilities and fixtures
```

## Running Tests

### Quick Start

```bash
# Run all tests
./run-tests.sh

# Run specific category
./run-tests.sh CATEGORY=Unit
./run-tests.sh CATEGORY=Integration
./run-tests.sh CATEGORY=Performance
./run-tests.sh CATEGORY=Security
```

### Using dotnet CLI

```bash
# Unit tests
dotnet test --filter "Category=Unit"

# Integration tests (requires Docker)
dotnet test --filter "Category=Integration"

# Performance tests
dotnet test --filter "Category=Performance"

# Security tests
dotnet test --filter "Category=Security"

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Environment Variables

- `CONFIGURATION`: Build configuration (Debug/Release, default: Release)
- `COVERAGE`: Enable code coverage (true/false, default: false)
- `CATEGORY`: Test category (Unit/Integration/Performance/Security/All, default: All)
- `VERBOSITY`: Test verbosity (quiet/minimal/normal/detailed/diagnostic, default: normal)

## Test Categories

### Unit Tests
- Fast execution (< 1 minute)
- No external dependencies
- Uses mocks and in-memory implementations
- Run on every commit

### Integration Tests
- Medium execution (2-5 minutes)
- Requires Docker (PostgreSQL, MongoDB, Redis)
- Uses Testcontainers for isolated test environments
- Run on PR and nightly

### Performance Tests
- Long execution (5-15 minutes)
- Uses BenchmarkDotNet
- Benchmarks critical operations
- Run on release builds

### Security Tests
- Medium execution (3-5 minutes)
- Validates security measures
- Tests input validation and injection prevention
- Run on every build

## Coverage Goals

- **Unit Tests**: 95%+ code coverage
- **Integration Tests**: 90%+ coverage of integration scenarios
- **All Happy Paths**: 100% coverage
- **All Sad Paths**: 100% coverage
- **Edge Cases**: 100% coverage

## CI/CD Integration

Tests are automatically run in GitHub Actions on:
- Push to main/develop branches
- Pull requests
- Nightly schedule (2 AM UTC)

See `.github/workflows/tests.yml` for CI/CD configuration.

## Test Execution Script

The `run-tests.sh` script provides a convenient way to run tests:

```bash
# Run all tests with coverage
COVERAGE=true ./run-tests.sh

# Run only unit tests
CATEGORY=Unit ./run-tests.sh

# Run with detailed output
VERBOSITY=detailed ./run-tests.sh
```

## Troubleshooting

### Docker Not Running
Integration tests require Docker. Ensure Docker is running before executing integration tests.

### Test Failures
1. Check test output for specific error messages
2. Verify all dependencies are installed
3. Ensure Docker containers are accessible for integration tests
4. Check test configuration files

### Performance Test Failures
Performance tests may fail if the system is under heavy load. Run them in an isolated environment for accurate results.

## Contributing

When adding new tests:
1. Follow the existing test structure
2. Use appropriate test categories
3. Include both happy and sad path tests
4. Add performance benchmarks for critical operations
5. Update this README if adding new test categories



