# Mamey.Policies

**Library**: `Mamey.Policies`  
**Location**: `Mamey/src/Mamey.Policies/`  
**Type**: Cross-Cutting Library - Policy Management  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Policies`

## Overview

Mamey.Policies provides policy management and enforcement for the Mamey framework. It enables command-level policy evaluation and enforcement middleware.

### Key Features

- **Policy Handlers**: Command-based policy handlers
- **Policy Dispatcher**: Policy evaluation dispatcher
- **Policy Enforcement Middleware**: Automatic policy enforcement
- **Multi-Policy Support**: Support for multiple policies per command

## Installation

```bash
dotnet add package Mamey.Policies
```

## Quick Start

```csharp
using Mamey.Policies;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddPolicyHandlers()
    .AddInMemoryPolicyDispatcher();

var app = builder.Build();
app.UsePolicies();
app.Run();
```

## Core Components

- **IPolicy**: Policy interface
- **IPolicy<T>**: Command-specific policy interface
- **IPolicyHandler<T>**: Policy handler interface
- **IPolicyDispatcher**: Policy dispatcher interface
- **PolicyEnforcementMiddleware**: Policy enforcement middleware
- **PolicyEvaluatorDispatcher**: In-memory policy dispatcher

## Usage Examples

### Define Policy

```csharp
public class UserCreationPolicy : Policy<CreateUserCommand>
{
    public bool CanCreateUser(CreateUserCommand command)
    {
        // Policy evaluation logic
        return true;
    }
}
```

### Policy Handler

```csharp
public class UserCreationPolicyHandler : IPolicyHandler<UserCreationPolicy>
{
    public async Task<bool> EvaluateAsync(UserCreationPolicy policy, CancellationToken cancellationToken)
    {
        // Policy evaluation logic
        return true;
    }
}
```

## Related Libraries

- **Mamey.CQRS.Commands**: Command pattern

## Tags

#policies #authorization #security #mamey

