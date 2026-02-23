# Mamey.Persistence.OpenStack

**Library**: `Mamey.Persistence.OpenStack`  
**Location**: `Mamey/src/Mamey.Persistence.OpenStack/`  
**Type**: Persistence Library - OpenStack Object Storage  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Persistence.OpenStack`

## Overview

Mamey.Persistence.OpenStack provides OpenStack Object Storage (OCS) integration for the Mamey framework. It includes authentication, file storage, and object management.

### Key Features

- **OpenStack Object Storage**: OCS client integration
- **Authentication**: OpenStack authentication support
- **File Operations**: Upload, download, delete operations
- **Request Handling**: HTTP request handling with authentication

## Installation

```bash
dotnet add package Mamey.Persistence.OpenStack
```

## Quick Start

```csharp
using Mamey.Persistence.OpenStack;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddOcsClient(options =>
    {
        options.StorageUrl = "https://storage.example.com";
        options.UserId = "user";
        options.Password = "password";
        options.ProjectId = "project-id";
    });

var app = builder.Build();
app.Run();
```

## Configuration

```json
{
  "OcsClient": {
    "StorageUrl": "https://storage.example.com",
    "AuthRelativeUrl": "/v3/auth/tokens",
    "UserId": "user",
    "Password": "password",
    "AuthMethod": "password",
    "ProjectId": "project-id",
    "RootDirectory": "/"
  }
}
```

## Core Components

- **IOcsClient**: OCS client interface
- **OcsClient**: OCS client implementation
- **IAuthManager**: Authentication manager interface
- **AuthManager**: Authentication manager implementation
- **IRequestHandler**: Request handler interface
- **OcsOptions**: Configuration options

## Related Libraries

- **Mamey.Net**: HTTP client infrastructure

## Tags

#openstack #object-storage #ocs #persistence #mamey

