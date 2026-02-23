# OpenAPI Client Code Generation Guide

## Overview

The Mamey.Authentik library uses code generation to create API client classes from Authentik's OpenAPI schema. This ensures the client stays in sync with the Authentik API.

## Prerequisites

1. **NSwag CLI** - Code generation tool
2. **Running Authentik Instance** - To fetch the OpenAPI schema
3. **.NET 9 SDK** - For building the generated code

## Installation

### Install NSwag CLI

```bash
dotnet tool install -g NSwag.ConsoleCore
```

Or update if already installed:

```bash
dotnet tool update -g NSwag.ConsoleCore
```

## Generation Methods

### Method 1: Using the Script (Recommended)

#### Bash (macOS/Linux)

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey/src/Mamey.Authentik
./scripts/generate-client.sh https://your-authentik-instance.com
```

#### PowerShell (Windows)

```powershell
cd Mamey\src\Mamey.Authentik
.\scripts\generate-client.ps1 -AuthentikUrl "https://your-authentik-instance.com"
```

### Method 2: Manual Steps

1. **Fetch the OpenAPI Schema**:

```bash
curl -o schema.json "https://your-authentik-instance.com/api/v3/schema/?format=json"
```

2. **Generate the Client**:

```bash
nswag openapi2csclient \
    /input:schema.json \
    /output:src/Mamey.Authentik.Generated/Api/AuthentikClient.cs \
    /namespace:Mamey.Authentik.Generated \
    /className:AuthentikClient \
    /generateClientClasses:true \
    /generateClientInterfaces:true \
    /useHttpClientCreationMethod:true \
    /useHttpRequestMessageCreationMethod:true \
    /useBaseUrl:true \
    /generateExceptionClasses:true \
    /exceptionClass:AuthentikApiException \
    /jsonLibrary:SystemTextJson \
    /dateType:System.DateTimeOffset \
    /generateOptionalParameters:true \
    /generateJsonMethods:false \
    /useTransformOptionsMethod:true \
    /useTransformResultMethod:true \
    /targetFramework:net9.0
```

## Using Local Authentik Instance

If you have a local Authentik instance running:

```bash
# Using the script
./scripts/generate-client.sh http://localhost:9100

# Or manually
curl -o schema.json "http://localhost:9100/api/v3/schema/?format=json"
nswag openapi2csclient /input:schema.json /output:src/Mamey.Authentik.Generated/Api/AuthentikClient.cs ...
```

## Generated Code Structure

After generation, the following structure will be created:

```
src/Mamey.Authentik.Generated/
├── Api/
│   ├── AuthentikClient.cs          # Main API client
│   ├── IAuthentikClient.cs         # Client interface
│   └── [Service]Api.cs             # Service-specific API classes
├── Models/
│   ├── User.cs                     # User DTOs
│   ├── Group.cs                    # Group DTOs
│   └── [Other Models].cs           # Other DTOs
└── Client/
    └── AuthentikApiException.cs    # Exception classes
```

## Integration with Service Layer

After generation, update the service implementations to use the generated client:

```csharp
public class AuthentikCoreService : IAuthentikCoreService
{
    private readonly AuthentikClient _generatedClient;
    
    public AuthentikCoreService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikCoreService> logger,
        IAuthentikCache? cache = null)
    {
        var httpClient = httpClientFactory.CreateClient("Authentik");
        _generatedClient = new AuthentikClient(httpClient)
        {
            BaseUrl = options.Value.BaseUrl
        };
    }
    
    public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _generatedClient.CoreUsersRetrieveAsync(userId, cancellationToken);
    }
}
```

## Regeneration

### When to Regenerate

- After Authentik version updates
- When new API endpoints are added
- When API models change
- Before major releases

### Regeneration Process

1. **Backup current implementation** (if needed)
2. **Fetch latest schema** from your Authentik instance
3. **Generate new client code**
4. **Review breaking changes**
5. **Update service implementations** if needed
6. **Run tests** to verify compatibility

## Troubleshooting

### Schema Fetch Fails

**Error**: `Failed to fetch schema from ...`

**Solutions**:
- Verify Authentik instance is accessible
- Check URL is correct (include protocol: `http://` or `https://`)
- Verify network connectivity
- Check Authentik instance is running

### NSwag Not Found

**Error**: `nswag: command not found`

**Solutions**:
```bash
dotnet tool install -g NSwag.ConsoleCore
```

### Generation Errors

**Error**: `Error generating client code`

**Solutions**:
- Check schema.json is valid JSON
- Verify NSwag version is compatible
- Check target framework matches project (`net9.0`)
- Review NSwag documentation for parameter changes

### Build Errors After Generation

**Error**: Compilation errors in generated code

**Solutions**:
- Verify target framework matches (`net9.0`)
- Check package references are correct
- Review generated code for syntax errors
- Regenerate with updated NSwag version

## Best Practices

1. **Version Control**: Commit generated code to version control
2. **Documentation**: Document any manual modifications
3. **Testing**: Always run tests after regeneration
4. **Review**: Review generated code for breaking changes
5. **Automation**: Consider automating generation in CI/CD

## CI/CD Integration

### GitHub Actions Example

```yaml
- name: Generate Authentik Client
  run: |
    curl -o schema.json "${{ secrets.AUTHENTIK_URL }}/api/v3/schema/?format=json"
    nswag openapi2csclient /input:schema.json /output:src/Mamey.Authentik.Generated/Api/AuthentikClient.cs ...
```

## Next Steps

After code generation:

1. ✅ Review generated code
2. ✅ Update service implementations
3. ✅ Add integration tests for new methods
4. ✅ Update documentation
5. ✅ Run full test suite
6. ✅ Verify >90% code coverage

---

**Note**: Generated code should not be manually edited as it will be overwritten on regeneration.
