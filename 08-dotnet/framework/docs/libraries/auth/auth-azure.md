# Mamey.Auth.Azure

The Mamey.Auth.Azure library provides comprehensive Azure Active Directory (Azure AD) authentication and authorization capabilities for the Mamey framework. It supports both Azure AD B2B (Business-to-Business) and B2C (Business-to-Consumer) authentication scenarios with Microsoft Graph integration.

## Technical Overview

Mamey.Auth.Azure is a specialized authentication library that provides:

- **Azure AD Integration**: Complete Azure Active Directory authentication support
- **B2B Authentication**: Business-to-business authentication with external organizations
- **B2C Authentication**: Business-to-consumer authentication for customer-facing applications
- **Microsoft Graph Integration**: Full Microsoft Graph API integration for user management
- **Token Management**: Advanced token acquisition, validation, and caching
- **Redis Caching**: Distributed token caching with Redis
- **Multi-Tenant Support**: Support for multi-tenant Azure AD scenarios
- **Policy-Based Authentication**: Custom authentication policies and flows

## Architecture

The library provides a layered architecture for Azure authentication:

```mermaid
graph TB
    subgraph "Application Layer"
        A[Controllers] --> B[Authentication Middleware]
        B --> C[Authorization Policies]
    end
    
    subgraph "Service Layer"
        D[IAzureAuthService] --> E[Microsoft Graph Client]
        F[Token Management] --> G[Redis Cache]
    end
    
    subgraph "Azure AD Layer"
        H[Azure AD B2B] --> I[Azure AD B2C]
        J[MSAL Client] --> K[OpenID Connect]
    end
    
    subgraph "Storage Layer"
        L[Redis Cache] --> M[Token Storage]
        N[User Data] --> O[Claims Storage]
    end
    
    A --> D
    D --> F
    E --> H
    F --> L
    G --> M
```

## Core Components

### Azure Authentication Services
- **IAzureAuthService**: Core interface for Azure authentication operations
- **B2BAuthenticationService**: B2B authentication service implementation
- **B2CAuthenticationService**: B2C authentication service implementation
- **Token Management**: Advanced token acquisition and validation

### Microsoft Graph Integration
- **GraphServiceClient**: Microsoft Graph API client
- **User Management**: Complete user lifecycle management
- **Group Management**: Group membership and management
- **Claims Management**: User claims and attributes

### Configuration
- **AzureOptions**: Base Azure configuration options
- **AzureB2BOptions**: B2B-specific configuration
- **AzureB2COptions**: B2C-specific configuration
- **GraphOptions**: Microsoft Graph configuration

### Caching
- **IRedisTokenCache**: Redis token cache interface
- **RedisTokenCache**: Redis token cache implementation
- **Token Caching**: Distributed token caching

## Installation

### NuGet Package
```bash
dotnet add package Mamey.Auth.Azure
```

### Prerequisites
- .NET 9.0 or later
- Mamey (core framework)
- Mamey.Auth (authentication abstractions)
- Mamey.Persistence.Redis (for Redis caching)
- Azure AD tenant
- Microsoft Graph API access

## Key Features

### Azure AD B2B Features

- **External User Authentication**: Authenticate users from external organizations
- **Guest User Management**: Manage guest users and their access
- **Cross-Tenant Authentication**: Seamless authentication across tenants
- **Policy-Based Access**: Custom access policies for external users
- **User Invitation**: Invite external users to your organization
- **Group Management**: Manage external user group memberships

### Azure AD B2C Features

- **Consumer Authentication**: Customer-facing authentication
- **Social Identity Providers**: Support for social login providers
- **Custom Policies**: Custom authentication and authorization policies
- **User Registration**: Self-service user registration
- **Password Reset**: Self-service password reset
- **Profile Management**: User profile management

### Microsoft Graph Features

- **User Management**: Complete user lifecycle management
- **Group Management**: Group creation and management
- **Claims Management**: User claims and attributes
- **Directory Operations**: Directory-level operations
- **Application Management**: Application registration and management

### Token Management Features

- **Token Acquisition**: Acquire access tokens for various scopes
- **Token Validation**: Validate ID tokens and access tokens
- **Token Refresh**: Refresh expired tokens
- **Token Caching**: Distributed token caching with Redis
- **Token Revocation**: Revoke tokens when needed

## Quick Start

### Basic Setup

```csharp
using Mamey.Auth.Azure;

// Register Azure authentication
builder.Services.AddMamey()
    .AddAzureAuthentication();

// Use Azure authentication
app.UseMamey()
    .UseAzureAuthentication();
```

### B2B Authentication Setup

```csharp
using Mamey.Auth.Azure.B2B;

// Register B2B authentication
builder.Services.AddMamey()
    .AddB2BAuth(allowAnonymousAccess: true);

// Use B2B authentication
app.UseMamey()
    .UseB2BAuth();
```

### B2C Authentication Setup

```csharp
using Mamey.Auth.Azure.B2C;

// Register B2C authentication
builder.Services.AddMamey()
    .AddB2CAuth(allowAnonymousAccess: true);

// Use B2C authentication
app.UseMamey()
    .UseB2CAuth();
```

## API Reference

### Core Interfaces

#### IAzureAuthService

Interface for Azure authentication operations.

```csharp
public interface IAzureAuthService
{
    Task<string> AcquireTokenAsync(string[] scopes);
    Task<bool> VerifyUserActionAsync(string userId, string token);
    Task<bool> ValidateIdTokenAsync(string idToken);
    Task<string> RefreshAccessTokenAsync(string refreshToken);
    Task<bool> DeleteUserAsync(string userId);
    Task<User?> UpdateUserAsync(string userId, User updatedUser);
    Task<bool> DisableUserAsync(string userId);
    Task<bool> EnableUserAsync(string userId);
    Task<Dictionary<string, string>> GetUserClaimsAsync(string userId);
    Task<bool> SetUserClaimsAsync(string userId, Dictionary<string, string> claims);
    Task<bool> AddUserToGroupAsync(string userId, string groupId);
    Task<bool> RemoveUserFromGroupAsync(string userId, string groupId);
    Task<bool> LogoutUserAsync(string userId);
    Task<List<string>> ListGroupsForUserAsync(string userId);
    Task<UserCollectionResponse> SearchUsersAsync(string query);
    Task<bool> ForcePasswordChangeAsync(string userId);
    Task<bool> LockUserAccountAsync(string userId);
    Task<bool> InviteUserAsync(string email, string displayName);
    Task<string> GenerateSignInUrlAsync(string redirectUrl, string state, string nonce);
    Task<bool> ValidateConfigurationAsync();
}
```

### Core Classes

#### AzureOptions

Base Azure configuration options.

```csharp
public class AzureOptions
{
    public bool Enabled { get; set; }
    public string? Type { get; set; }
    public string? Auth { get; set; }
    public string? Instance { get; set; }
    public string? TenantId { get; set; }
    public string? Domain { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Authority { get; set; }
    public string? Scopes { get; set; }
    public string? CallbackPath { get; set; }
    public AzureB2BOptions AzureB2BOptions { get; set; }
    public AzureB2COptions AzureB2COptions { get; set; }
    public DownstreamApi DownstreamApi { get; set; }
    public GraphOptions GraphOptions { get; set; }
}
```

#### AzureB2BOptions

B2B-specific configuration options.

```csharp
public class AzureB2BOptions : AzureOptions
{
    public string? SignedOutCallbackPath { get; set; }
    public string? SignInPolicyId { get; set; }
    public string? SignOutPolicyId { get; set; }
}
```

#### AzureB2COptions

B2C-specific configuration options.

```csharp
public class AzureB2COptions : AzureOptions
{
    public string? RedirectUri { get; set; }
    public string SignedOutCallbackPath { get; set; }
    public string SignUpSignInPolicyId { get; set; }
    public string ResetPasswordPolicyId { get; set; }
    public string EditProfilePolicyId { get; set; }
}
```

## Usage Examples

### Example 1: Basic Azure Authentication

```csharp
using Mamey.Auth.Azure;

public class AuthController : ControllerBase
{
    private readonly IAzureAuthService _azureAuthService;

    public AuthController(IAzureAuthService azureAuthService)
    {
        _azureAuthService = azureAuthService;
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            // Get user information from claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Get user details from Microsoft Graph
            var user = await _azureAuthService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.UserPrincipalName,
                Created = user.CreatedDateTime
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
```

### Example 2: B2B User Management

```csharp
using Mamey.Auth.Azure.B2B;

public class B2BUserController : ControllerBase
{
    private readonly IAzureAuthService _azureAuthService;

    public B2BUserController(IAzureAuthService azureAuthService)
    {
        _azureAuthService = azureAuthService;
    }

    [HttpPost("invite")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> InviteUser([FromBody] InviteUserRequest request)
    {
        try
        {
            var success = await _azureAuthService.InviteUserAsync(request.Email, request.DisplayName);
            if (success)
            {
                return Ok(new { Message = "User invitation sent successfully" });
            }
            return BadRequest(new { Error = "Failed to send user invitation" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _azureAuthService.GetUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
```

### Example 3: B2C User Registration

```csharp
using Mamey.Auth.Azure.B2C;

public class B2CUserController : ControllerBase
{
    private readonly IAzureAuthService _azureAuthService;

    public B2CUserController(IAzureAuthService azureAuthService)
    {
        _azureAuthService = azureAuthService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        try
        {
            var user = new User
            {
                DisplayName = request.DisplayName,
                UserPrincipalName = request.Email,
                AccountEnabled = true,
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = true,
                    Password = request.Password
                }
            };

            var createdUser = await _azureAuthService.CreateUserAsync(user);
            return Ok(new
            {
                Id = createdUser.Id,
                DisplayName = createdUser.DisplayName,
                Email = createdUser.UserPrincipalName
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var resetUrl = await _azureAuthService.InitiatePasswordResetAsync(request.UserId);
            return Ok(new { ResetUrl = resetUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
```

### Example 4: Token Management

```csharp
using Mamey.Auth.Azure;

public class TokenController : ControllerBase
{
    private readonly IAzureAuthService _azureAuthService;

    public TokenController(IAzureAuthService azureAuthService)
    {
        _azureAuthService = azureAuthService;
    }

    [HttpPost("acquire-token")]
    public async Task<IActionResult> AcquireToken([FromBody] AcquireTokenRequest request)
    {
        try
        {
            var scopes = request.Scopes ?? new[] { "https://graph.microsoft.com/.default" };
            var token = await _azureAuthService.AcquireTokenAsync(scopes);
            
            return Ok(new { AccessToken = token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequest request)
    {
        try
        {
            var isValid = await _azureAuthService.ValidateIdTokenAsync(request.IdToken);
            return Ok(new { IsValid = isValid });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var newToken = await _azureAuthService.RefreshAccessTokenAsync(request.RefreshToken);
            return Ok(new { AccessToken = newToken });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
```

## Integration Patterns

### Integration with ASP.NET Core

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add Mamey with Azure authentication
builder.Services.AddMamey()
    .AddAzureAuthentication();

var app = builder.Build();

// Use Azure authentication
app.UseMamey()
    .UseAzureAuthentication();

app.Run();
```

### Integration with Microsoft Graph

```csharp
public class GraphService
{
    private readonly GraphServiceClient _graphClient;
    private readonly IAzureAuthService _azureAuthService;

    public GraphService(GraphServiceClient graphClient, IAzureAuthService azureAuthService)
    {
        _graphClient = graphClient;
        _azureAuthService = azureAuthService;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var users = await _graphClient.Users.GetAsync();
        return users?.Value ?? new List<User>();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        return await _graphClient.Users.PostAsync(user);
    }
}
```

## Configuration Reference

### Azure Configuration

```json
{
  "azure": {
    "enabled": true,
    "type": "b2b",
    "instance": "https://login.microsoftonline.com/",
    "tenantId": "your-tenant-id",
    "domain": "your-domain.com",
    "clientId": "your-client-id",
    "clientSecret": "your-client-secret",
    "authority": "https://login.microsoftonline.com/your-tenant-id",
    "scopes": "https://graph.microsoft.com/.default",
    "callbackPath": "/signin-oidc",
    "b2b": {
      "signedOutCallbackPath": "/signout-callback-oidc",
      "signInPolicyId": "B2C_1_signin",
      "signOutPolicyId": "B2C_1_signout"
    },
    "b2c": {
      "redirectUri": "https://your-app.com/signin-oidc",
      "signedOutCallbackPath": "/signout-callback-oidc",
      "signUpSignInPolicyId": "B2C_1_signup_signin",
      "resetPasswordPolicyId": "B2C_1_reset",
      "editProfilePolicyId": "B2C_1_edit_profile"
    },
    "graphOptions": {
      "baseUrl": "https://graph.microsoft.com/v1.0"
    }
  }
}
```

### Redis Configuration

```json
{
  "redis": {
    "connectionString": "localhost:6379",
    "database": 0
  }
}
```

## Best Practices

1. **Secure Configuration**: Store sensitive configuration in Azure Key Vault
2. **Token Caching**: Use Redis for distributed token caching
3. **Error Handling**: Implement comprehensive error handling
4. **Logging**: Add detailed logging for debugging and monitoring
5. **User Management**: Implement proper user lifecycle management
6. **Security**: Follow Azure AD security best practices
7. **Performance**: Use token caching to improve performance
8. **Testing**: Write comprehensive unit and integration tests
9. **Monitoring**: Monitor authentication events and failures
10. **Documentation**: Document custom policies and configurations

## Troubleshooting

### Common Issues

**Authentication Fails**: Check Azure AD configuration and permissions
**Token Validation Fails**: Verify token validation parameters
**Graph API Errors**: Check Microsoft Graph permissions and scopes
**Redis Connection**: Ensure Redis is accessible and configured correctly
**User Management Issues**: Verify user permissions and group memberships

### Debugging

Enable detailed logging to troubleshoot issues:

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

## Related Libraries

- [Mamey.Auth](auth.md) - Core authentication library
- [Mamey.Auth.Abstractions](auth-abstractions.md) - Authentication abstractions
- [Mamey.Persistence.Redis](persistence-redis.md) - Redis persistence library

## Additional Resources

- Azure AD Authentication Guide
- Microsoft Graph Integration Guide
- B2B Authentication Guide
- B2C Authentication Guide
