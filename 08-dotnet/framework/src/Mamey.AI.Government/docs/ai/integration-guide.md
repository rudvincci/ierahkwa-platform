# AI Integration Guide

## Prerequisites

-   .NET 9.0
-   Mamey Framework
-   Reference to `Mamey.AI.Government` project/package.

## Step-by-Step Integration

### 1. Add Reference

In your Infrastructure or Application csproj:

```xml
<ProjectReference Include="..\..\..\Mamey\src\Mamey.AI.Government\Mamey.AI.Government.csproj" />
```

### 2. Register Services

In your `Infrastructure/Extensions.cs` (or startup):

```csharp
using Mamey.AI.Government;

public static IServiceCollection AddInfrastructure(this IServiceCollection services)
{
    // ... other registrations
    services.AddGovernmentAI();
    return services;
}
```

### 3. Implement Integration Wrapper (Recommended)

Create a service in your Application layer that wraps the generic AI service with domain-specific logic (logging, mapping, event triggering).

**Example: Document Verification**

```csharp
public class DocumentVerificationIntegration : IDocumentVerificationIntegration
{
    private readonly IDocumentVerificationService _aiService;
    
    public async Task VerifyAsync(Guid appId, Stream doc)
    {
        var result = await _aiService.VerifyDocumentAsync(doc, "Passport");
        if (!result.IsVerified) throw new ValidationException(...);
    }
}
```

### 4. Use in Handlers

Inject your integration wrapper (or the raw service) into Command/Query Handlers.

## Configuration

Configure thresholds and feature flags in `appsettings.json`:

```json
"AI": {
  "FeatureFlags": {
    "EnableBiometrics": true
  },
  "Thresholds": {
    "FraudRiskHigh": 80
  }
}
```
