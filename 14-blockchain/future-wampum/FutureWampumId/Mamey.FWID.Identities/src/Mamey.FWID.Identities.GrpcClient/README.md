# Mamey.FWID.Identities.GrpcClient

gRPC client library for calling the Identities API service.

## Purpose

This library provides a client wrapper for making gRPC calls to the `Mamey.FWID.Identities.Api` service. It should be referenced by other microservices or applications that need to interact with the Identities service via gRPC.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  Your Microservice / Application                            │
│  (e.g., another microservice, Blazor app, Maui app)          │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ References GrpcClient library
                     ▼
┌─────────────────────────────────────────────────────────────┐
│  GrpcClient Library (This Project)                           │
│  - BiometricServiceClient                                 │
│  - Creates gRPC channel to API                              │
│  - Handles authentication (JWT/Certificate)                 │
│  - Wraps gRPC calls with logging                             │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ gRPC Protocol (HTTP/2)
                     │ https://localhost:5001
                     ▼
┌─────────────────────────────────────────────────────────────┐
│  Identities API Service (Port 5001)                          │
│  - BiometricGrpcService                                     │
│  - PermissionSyncGrpcService                               │
│  - Authentication interceptors (JWT/Certificate)             │
└─────────────────────────────────────────────────────────────┘
```

## Usage

### 1. Add Project Reference

In your microservice or application project, add a reference to this library:

```xml
<ProjectReference Include="..\..\Mamey.FWID.Identities\src\Mamey.FWID.Identities.GrpcClient\Mamey.FWID.Identities.GrpcClient.csproj" />
```

### 2. Register in DI

In your `Program.cs` or service configuration:

```csharp
using Mamey.FWID.Identities.GrpcClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddIdentitiesGrpcInfrastructure(args); // Registers BiometricServiceClient
```

### 3. Inject and Use

```csharp
using Mamey.FWID.Identities.GrpcClient.Services;
using Mamey.FWID.Identities.Api.Protos;

public class MyService
{
    private readonly BiometricServiceClient _biometricClient;

    public MyService(BiometricServiceClient biometricClient)
    {
        _biometricClient = biometricClient;
    }

    public async Task<bool> VerifyBiometricAsync(string identityId, byte[] biometricData)
    {
        var request = new VerifyBiometricRequest
        {
            IdentityId = identityId,
            ProvidedBiometric = new BiometricDataMessage
            {
                Type = "Fingerprint",
                Data = Google.Protobuf.ByteString.CopyFrom(biometricData)
            },
            Threshold = 0.85
        };

        var response = await _biometricClient.VerifyBiometricAsync(request);
        
        if (response.IsVerified)
        {
            // Handle success
            return true;
        }
        else
        {
            // Handle failure
            Console.WriteLine($"Verification failed: {response.ErrorMessage}");
            return false;
        }
    }
}
```

## Configuration

The client connects to the Identities API service. By default, it connects to `https://localhost:5001`.

### Command-Line Arguments

You can configure the address via command-line arguments:
- `args[0]`: Host (default: "localhost")
- `args[1]`: Port (default: 5001)

```csharp
// Example: Connect to a different host/port
builder.Services
    .AddMamey()
    .AddIdentitiesGrpcInfrastructure(new[] { "identities-service", "5001" });
```

### Environment Variables

You can also configure via environment variables:

```bash
export IDENTITIES_SERVICE_HOST=identities-service
export IDENTITIES_SERVICE_PORT=5001
```

Or:

```bash
export IDENTITIES_HOST=identities-service
export IDENTITIES_PORT=5001
```

### Configuration File

You can also configure via `appsettings.json` (requires additional implementation):

```json
{
  "GrpcClient": {
    "Address": "https://identities-service:5001"
  }
}
```

## Authentication

The client supports both JWT and Certificate authentication:

### JWT Authentication

For external clients (e.g., Blazor, Maui apps):

```csharp
var jwtToken = "your-jwt-token-here";
builder.Services
    .AddMamey()
    .AddIdentitiesGrpcInfrastructure(args, jwtToken: jwtToken);
```

### Certificate Authentication

For service-to-service communication:

```csharp
var certificate = new X509Certificate2("path/to/certificate.pfx", "password");
builder.Services
    .AddMamey()
    .AddIdentitiesGrpcInfrastructure(args, clientCertificate: certificate);
```

## Available Services

### BiometricServiceClient

Provides methods for biometric verification:

- `VerifyBiometricAsync(VerifyBiometricRequest, CallOptions?, CancellationToken)`: Verifies biometric data for an identity
- `VerifyBiometricStreamAsync(IAsyncEnumerable<VerifyBiometricRequest>, CallOptions?, CancellationToken)`: Verifies biometric data using streaming

## Error Handling

The client throws `RpcException` for gRPC errors. Handle them appropriately:

```csharp
try
{
    var response = await _biometricClient.VerifyBiometricAsync(request);
    // Handle success
}
catch (RpcException ex)
{
    switch (ex.StatusCode)
    {
        case StatusCode.Unauthenticated:
            // Handle authentication error
            break;
        case StatusCode.NotFound:
            // Handle not found error
            break;
        case StatusCode.Internal:
            // Handle internal server error
            break;
        default:
            // Handle other errors
            break;
    }
}
```

## Testing

For testing purposes, you can run the `Program.cs` file (in Debug mode only). This provides a simple web application on port 5101 for testing the gRPC client.

**Note**: In Release builds, `Program.cs` is excluded from compilation to ensure this remains a library project.

## Important Notes

1. **This is a library project**, not a standalone application. Do not run it directly in production.
2. **Reference it from other projects** that need to call the Identities API service.
3. **The API service must be running** before making gRPC calls.
4. **Authentication is required** - ensure you provide either JWT token or certificate.
5. **Connection is lazy** - the gRPC channel is created on first use, not at startup.

## Troubleshooting

### Connection Refused

If you get a connection refused error:
- Ensure the Identities API service is running on the configured port
- Check that the host and port are correct
- Verify network connectivity between services

### Authentication Errors

If you get authentication errors:
- Ensure you're providing a valid JWT token or certificate
- Check that the certificate is valid and not expired
- Verify that the API service is configured to accept your authentication method

### Timeout Errors

If you get timeout errors:
- The default timeout is 30 seconds
- Check that the API service is responding
- Verify that there are no network issues

## See Also

- [Mamey.FWID.Identities.Api](../Mamey.FWID.Identities.Api/README.md) - The API service that this client calls
- [gRPC Documentation](https://grpc.io/docs/) - General gRPC documentation

