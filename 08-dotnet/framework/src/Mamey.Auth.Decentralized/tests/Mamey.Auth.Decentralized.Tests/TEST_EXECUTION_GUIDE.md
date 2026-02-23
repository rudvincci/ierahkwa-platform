# Test Execution Guide

## Overview

This guide provides comprehensive instructions for executing the Mamey.Auth.Decentralized library tests, including setup, execution strategies, and troubleshooting.

## Prerequisites

### Required Software

- **.NET 6.0 SDK** or later
- **Docker** (for integration tests)
- **Visual Studio 2022** or **VS Code** (recommended)
- **Git** (for version control)

### Required Tools

- **Testcontainers** (for database testing)
- **BenchmarkDotNet** (for performance testing)
- **Coverlet** (for code coverage)
- **ReportGenerator** (for coverage reports)

## Test Categories

### 1. Unit Tests

**Purpose**: Test individual components in isolation
**Execution Time**: Fast (< 1 minute)
**Dependencies**: None (uses mocks)

#### Running Unit Tests

```bash
# Run all unit tests
dotnet test --filter "Category=Unit"

# Run specific test class
dotnet test --filter "ClassName=Mamey.Auth.Decentralized.Tests.Core.DidTests"

# Run with coverage
dotnet test --filter "Category=Unit" --collect:"XPlat Code Coverage"
```

#### Unit Test Structure

```
tests/Mamey.Auth.Decentralized.Tests/
├── Core/
│   ├── DidTests.cs
│   ├── DidUrlTests.cs
│   ├── VerificationMethodTests.cs
│   ├── ServiceEndpointTests.cs
│   ├── DidDocumentTests.cs
│   ├── DidResolutionResultTests.cs
│   └── DidDereferencingResultTests.cs
├── Validation/
│   ├── DidValidatorTests.cs
│   ├── DidDocumentValidatorTests.cs
│   └── W3cComplianceValidatorTests.cs
├── Handlers/
│   └── DecentralizedHandlerTests.cs
├── Options/
│   ├── DecentralizedOptionsTests.cs
│   └── DecentralizedOptionsBuilderTests.cs
└── Extensions/
    └── DecentralizedExtensionsTests.cs
```

### 2. Integration Tests

**Purpose**: Test component interactions with real databases
**Execution Time**: Medium (2-5 minutes)
**Dependencies**: Docker containers

#### Running Integration Tests

```bash
# Run all integration tests
dotnet test --filter "Category=Integration"

# Run with specific database
dotnet test --filter "Category=Integration&Database=PostgreSQL"

# Run with coverage
dotnet test --filter "Category=Integration" --collect:"XPlat Code Coverage"
```

#### Integration Test Structure

```
tests/Mamey.Auth.Decentralized.Tests/
├── Integration/
│   ├── DatabaseIntegrationTests.cs
│   ├── DidResolutionIntegrationTests.cs
│   ├── VcIntegrationTests.cs
│   └── CacheIntegrationTests.cs
└── TestConfiguration/
    └── TestConfiguration.cs
```

### 3. Performance Tests

**Purpose**: Benchmark critical operations
**Execution Time**: Long (5-15 minutes)
**Dependencies**: Isolated environment

#### Running Performance Tests

```bash
# Run all performance tests
dotnet test --filter "Category=Performance"

# Run specific benchmark
dotnet test --filter "ClassName=Mamey.Auth.Decentralized.Tests.Performance.DidResolutionBenchmark"

# Run with detailed output
dotnet test --filter "Category=Performance" --logger "console;verbosity=detailed"
```

#### Performance Test Structure

```
tests/Mamey.Auth.Decentralized.Tests/
├── Performance/
│   ├── DidResolutionBenchmark.cs
│   ├── VcValidationBenchmark.cs
│   ├── DatabaseOperationsBenchmark.cs
│   └── CryptographicOperationsBenchmark.cs
└── TestData/
    └── TestDataGenerator.cs
```

### 4. Security Tests

**Purpose**: Validate security measures
**Execution Time**: Medium (3-5 minutes)
**Dependencies**: Isolated environment

#### Running Security Tests

```bash
# Run all security tests
dotnet test --filter "Category=Security"

# Run specific security test
dotnet test --filter "ClassName=Mamey.Auth.Decentralized.Tests.Security.SecurityTests"

# Run with security logging
dotnet test --filter "Category=Security" --logger "console;verbosity=detailed"
```

#### Security Test Structure

```
tests/Mamey.Auth.Decentralized.Tests/
├── Security/
│   ├── SecurityTests.cs
│   ├── InputValidationTests.cs
│   ├── CryptographicSecurityTests.cs
│   └── InjectionAttackTests.cs
└── TestData/
    └── SecurityTestData.cs
```

### 5. W3C Compliance Tests

**Purpose**: Ensure W3C DID 1.1 and VC 1.1 compliance
**Execution Time**: Medium (2-3 minutes)
**Dependencies**: W3C test suite

#### Running W3C Compliance Tests

```bash
# Run all W3C compliance tests
dotnet test --filter "Category=W3CCompliance"

# Run specific compliance test
dotnet test --filter "ClassName=Mamey.Auth.Decentralized.Tests.W3cCompliance.W3cComplianceTests"

# Run with compliance reporting
dotnet test --filter "Category=W3CCompliance" --logger "console;verbosity=detailed"
```

#### W3C Compliance Test Structure

```
tests/Mamey.Auth.Decentralized.Tests/
├── W3cCompliance/
│   ├── W3cComplianceTests.cs
│   ├── DidComplianceTests.cs
│   ├── VcComplianceTests.cs
│   └── ResolutionComplianceTests.cs
└── TestData/
    └── W3cTestData.cs
```

## Test Execution Strategies

### 1. Development Workflow

#### Quick Feedback Loop
```bash
# Run unit tests only (fastest)
dotnet test --filter "Category=Unit"

# Run unit + validation tests
dotnet test --filter "Category=Unit|Category=Validation"

# Run with coverage
dotnet test --filter "Category=Unit" --collect:"XPlat Code Coverage"
```

#### Pre-Commit Validation
```bash
# Run all tests except performance
dotnet test --filter "Category!=Performance"

# Run with coverage and generate report
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report"
```

### 2. CI/CD Pipeline

#### Build Stage
```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build --configuration Release --no-restore

# Run unit tests
dotnet test --configuration Release --no-build --filter "Category=Unit" --collect:"XPlat Code Coverage"
```

#### Integration Stage
```bash
# Start test containers
docker-compose -f docker-compose.test.yml up -d

# Wait for containers to be ready
./scripts/wait-for-containers.sh

# Run integration tests
dotnet test --configuration Release --no-build --filter "Category=Integration" --collect:"XPlat Code Coverage"

# Cleanup containers
docker-compose -f docker-compose.test.yml down
```

#### Performance Stage
```bash
# Run performance tests
dotnet test --configuration Release --no-build --filter "Category=Performance" --logger "console;verbosity=detailed"

# Generate performance report
./scripts/generate-performance-report.sh
```

### 3. Release Validation

#### Full Test Suite
```bash
# Run all tests
dotnet test --configuration Release --collect:"XPlat Code Coverage"

# Generate comprehensive report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"Html;JsonSummary"

# Run security scan
dotnet test --filter "Category=Security" --logger "console;verbosity=detailed"

# Run W3C compliance tests
dotnet test --filter "Category=W3CCompliance" --logger "console;verbosity=detailed"
```

## Test Configuration

### 1. Test Settings

#### appsettings.Test.json
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=mamey_did_test;",
    "MongoDB": "mongodb://localhost:27017/mamey_did_test",
    "Redis": "localhost:6379,defaultDatabase=1"
  },
  "Decentralized": {
    "EnableCaching": true,
    "CacheExpiration": "00:05:00",
    "SupportedMethods": ["web", "key"],
    "EnableDetailedLogging": true,
    "EnablePerformanceMetrics": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

#### Test Categories
```csharp
[TestClass]
[TestCategory("Unit")]
[TestCategory("Core")]
public class DidTests : TestBase
{
    // Test methods
}
```

### 2. Test Data Management

#### Test Data Generation
```csharp
// Generate valid test data
var did = TestDataGenerator.GenerateValidDid();
var didDocument = TestDataGenerator.GenerateValidDidDocument();
var vc = TestDataGenerator.GenerateValidVerifiableCredential();

// Generate invalid test data
var invalidDid = TestDataGenerator.Invalid.GenerateInvalidDid();
var invalidDidDocument = TestDataGenerator.Invalid.GenerateInvalidDidDocument();
```

#### Test Data Cleanup
```csharp
[TestCleanup]
public void TestCleanup()
{
    // Cleanup test data
    TestDataCleanup.CleanupTestData();
}
```

## Troubleshooting

### 1. Common Issues

#### Test Container Issues
```bash
# Check Docker status
docker ps

# Restart Docker
sudo systemctl restart docker

# Clean up containers
docker system prune -f
```

#### Database Connection Issues
```bash
# Check database status
docker-compose -f docker-compose.test.yml ps

# View database logs
docker-compose -f docker-compose.test.yml logs postgres
docker-compose -f docker-compose.test.yml logs mongodb
docker-compose -f docker-compose.test.yml logs redis
```

#### Test Execution Issues
```bash
# Run with detailed logging
dotnet test --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName=Mamey.Auth.Decentralized.Tests.Core.DidTests.Parse_ValidDid_ReturnsDidObject"

# Run with debug output
dotnet test --logger "console;verbosity=diagnostic"
```

### 2. Performance Issues

#### Slow Test Execution
```bash
# Run tests in parallel
dotnet test --parallel

# Run specific test categories
dotnet test --filter "Category=Unit"

# Profile test execution
dotnet test --collect:"XPlat Code Coverage" --settings:runsettings.xml
```

#### Memory Issues
```bash
# Run with memory diagnostics
dotnet test --collect:"XPlat Code Coverage" --settings:runsettings.xml

# Check memory usage
dotnet test --logger "console;verbosity=detailed" | grep "Memory"
```

### 3. Coverage Issues

#### Low Coverage
```bash
# Generate detailed coverage report
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"Html;JsonSummary"

# View coverage report
open coverage-report/index.html
```

#### Missing Coverage
```bash
# Run with coverage and exclude patterns
dotnet test --collect:"XPlat Code Coverage" --settings:runsettings.xml

# Check coverage thresholds
dotnet test --collect:"XPlat Code Coverage" --settings:runsettings.xml --logger "console;verbosity=detailed"
```

## Test Reporting

### 1. Coverage Reports

#### Generate Coverage Report
```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"Html"

# Generate JSON summary
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"JsonSummary"
```

#### Coverage Thresholds
```xml
<!-- runsettings.xml -->
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage" uri="datacollector://Microsoft/CodeCoverage/2.0" assemblyQualifiedName="Coverlet.Collector.DataCollection.CoverletCollector, coverlet.collector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
        <Configuration>
          <Threshold>95</Threshold>
          <ThresholdType>Line</ThresholdType>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

### 2. Performance Reports

#### Generate Performance Report
```bash
# Run performance tests
dotnet test --filter "Category=Performance" --logger "console;verbosity=detailed"

# Generate performance report
./scripts/generate-performance-report.sh
```

#### Performance Thresholds
```csharp
[TestMethod]
public void DidResolution_Performance_Benchmark()
{
    var summary = BenchmarkRunner.Run<DidResolutionBenchmark>();
    Assert.IsTrue(summary.Reports.All(r => r.ResultStatistics.Mean < TimeSpan.FromMilliseconds(100).TotalMilliseconds));
}
```

### 3. Test Results

#### Test Results Summary
```bash
# Run tests and save results
dotnet test --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"

# View test results
dotnet test --logger "console;verbosity=detailed" | grep "Test Run Summary"
```

#### Test Results Analysis
```bash
# Analyze test results
./scripts/analyze-test-results.sh

# Generate test report
./scripts/generate-test-report.sh
```

## Best Practices

### 1. Test Organization

- **Group related tests** in the same test class
- **Use descriptive test names** that explain the scenario
- **Follow AAA pattern** (Arrange, Act, Assert)
- **Use test categories** for different test types
- **Keep tests independent** and isolated

### 2. Test Data

- **Use test data generators** for consistent data
- **Clean up test data** after each test
- **Use realistic test data** that reflects real-world scenarios
- **Avoid hardcoded values** in tests

### 3. Test Execution

- **Run unit tests frequently** during development
- **Run integration tests** before committing
- **Run performance tests** before releases
- **Monitor test execution time** and optimize slow tests

### 4. Test Maintenance

- **Update tests** when requirements change
- **Remove obsolete tests** that are no longer relevant
- **Refactor tests** to improve readability and maintainability
- **Document test scenarios** for complex test cases

## Conclusion

This test execution guide provides comprehensive instructions for running the Mamey.Auth.Decentralized library tests. Following these guidelines will ensure reliable test execution, comprehensive coverage, and high-quality validation of the library's functionality and compliance.

For additional support or questions, please refer to the project documentation or contact the development team.
