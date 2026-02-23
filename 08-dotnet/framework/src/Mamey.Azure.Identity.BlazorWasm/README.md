# Mamey.Azure.Identity.BlazorWasm

**Library**: `Mamey.Azure.Identity.BlazorWasm`  
**Location**: `Mamey/src/Mamey.Azure.Identity.BlazorWasm/`  
**Type**: Identity Library - Azure AD for Blazor WebAssembly  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Azure.Identity.BlazorWasm`

## Overview

Mamey.Azure.Identity.BlazorWasm provides Azure Active Directory authentication for Blazor WebAssembly applications in the Mamey framework. It includes MSAL (Microsoft Authentication Library) integration and token management.

### Key Features

- **Azure AD Integration**: MSAL-based Azure AD authentication
- **Blazor WebAssembly Support**: Client-side authentication
- **B2C Support**: Azure AD B2C integration
- **B2B Support**: Azure AD B2B integration
- **Token Management**: Access token storage and retrieval
- **Authentication State Provider**: Custom authentication state provider

## Installation

```bash
dotnet add package Mamey.Azure.Identity.BlazorWasm
```

## Quick Start

```csharp
using Mamey.Azure.Identity.BlazorWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
    .AddMamey()
    .AddAzureIdentityBlazorWasm("api://your-api-id/API.Access");

await builder.Build().RunAsync();
```

## Configuration

```json
{
  "Azure": {
    "ClientId": "your-client-id",
    "Authority": "https://login.microsoftonline.com/your-tenant-id",
    "RedirectUri": "https://yourapp.com/authentication/login-callback",
    "Scopes": "api://your-api-id/API.Access",
    "Auth": "b2c"
  }
}
```

## Core Components

- **AzureBlazorWasmOptions**: Configuration options
- **TokenAuthenticationStateProvider**: Authentication state provider
- **BlazorWasmAuthenticationStateProvider**: Blazor WASM auth state provider
- **AuthenticationService**: Authentication service
- **IBlazorAuthenticationService**: Authentication service interface

## Usage Examples

### Get Access Token

```csharp
@inject TokenAuthenticationStateProvider AuthProvider

var token = await AuthProvider.GetTokenAsync();
```

## Related Libraries

- **Mamey.Identity.Blazor**: Blazor identity support
- **Mamey.Azure.Abstractions**: Azure abstractions

## Tags

#azure #identity #blazor #webassembly #msal #b2c #b2b #mamey















