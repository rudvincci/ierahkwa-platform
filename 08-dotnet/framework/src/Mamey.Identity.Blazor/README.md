# Mamey.Identity.Blazor

**Library**: `Mamey.Identity.Blazor`  
**Location**: `Mamey/src/Mamey.Identity.Blazor/`  
**Type**: Identity Library - Blazor WebAssembly  
**Version**: 2.0.*  
**Files**: 9 C# files  
**Namespace**: `Mamey.Identity.Blazor`

## Overview

Mamey.Identity.Blazor provides comprehensive identity and authentication support for Blazor WebAssembly applications in the Mamey framework. It includes client-side authentication state management, token storage, and seamless integration with backend authentication APIs.

### Key Features

- **Blazor WebAssembly Authentication**: Full authentication support for Blazor WASM
- **Token Management**: Automatic token storage and refresh
- **Authentication State Provider**: Custom `AuthenticationStateProvider` implementation
- **API Integration**: HttpClient-based authentication with backend APIs
- **LocalStorage Integration**: Token persistence using browser localStorage
- **Auto Token Refresh**: Automatic token refresh before expiration

## Installation

```bash
dotnet add package Mamey.Identity.Blazor
```

## Quick Start

```csharp
using Mamey.Identity.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMameyIdentityBlazor(options =>
{
    options.ApiBaseUrl = "https://api.example.com";
    options.LoginEndpoint = "/api/auth/login";
    options.AutoRefreshTokens = true;
});

await builder.Build().RunAsync();
```

## Configuration

```json
{
  "Identity:Blazor": {
    "Enabled": true,
    "ApiBaseUrl": "https://api.example.com",
    "LoginEndpoint": "/api/auth/login",
    "LogoutEndpoint": "/api/auth/logout",
    "RefreshTokenEndpoint": "/api/auth/refresh",
    "UserInfoEndpoint": "/api/auth/user",
    "AutoRefreshTokens": true,
    "TokenRefreshIntervalMinutes": 5
  }
}
```

## Core Components

- **IBlazorAuthenticationService**: Authentication service interface
- **BlazorAuthenticationService**: Authentication service implementation
- **IBlazorTokenService**: Token management service
- **BlazorTokenService**: Token service implementation
- **IBlazorUserService**: User information service
- **BlazorUserService**: User service implementation
- **MameyAuthenticationStateProvider**: Custom authentication state provider

## Usage Examples

### Login

```csharp
@inject IBlazorAuthenticationService AuthService

@code {
    private async Task HandleLogin()
    {
        var result = await AuthService.LoginAsync("user@example.com", "password");
        if (result.Success)
        {
            // Navigate to protected page
        }
    }
}
```

### Get Current User

```csharp
@inject IBlazorUserService UserService

@code {
    private AuthenticatedUser? currentUser;

    protected override async Task OnInitializedAsync()
    {
        currentUser = await UserService.GetCurrentUserAsync();
    }
}
```

## Related Libraries

- **Mamey.Identity.Core**: Core identity abstractions
- **Mamey.Identity.AspNetCore**: Backend authentication API

## Tags

#identity #blazor #webassembly #authentication #client-side #mamey















