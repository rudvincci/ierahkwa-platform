# Mamey.Mifos

**Library**: `Mamey.Mifos`  
**Location**: `Mamey/src/Mamey.Mifos/`  
**Type**: Financial Library - Mifos Integration  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Mifos`

## Overview

Mamey.Mifos provides integration with Mifos (now Fineract) core banking platform for the Mamey framework. It includes client management, loan management, savings accounts, and office management.

### Key Features

- **Client Management**: Create and manage clients
- **Loan Management**: Create and manage loans
- **Savings Accounts**: Manage savings accounts
- **Office Management**: Office operations
- **Authentication**: Basic and OAuth authentication support
- **Tenant Support**: Multi-tenant support

## Installation

```bash
dotnet add package Mamey.Mifos
```

## Quick Start

```csharp
using Mamey.Mifos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMifos(options =>
{
    options.Enabled = true;
    options.HostUrl = "https://mifos.example.com";
    options.AuthType = "basic";
    options.Username = "mifos";
    options.Password = "password";
    options.TenantId = "default";
});

var app = builder.Build();
app.Run();
```

## Configuration

```json
{
  "mifos": {
    "Enabled": true,
    "HostUrl": "https://mifos.example.com",
    "AuthType": "basic",
    "Username": "mifos",
    "Password": "password",
    "TenantId": "default"
  }
}
```

## Core Components

- **IMifosApiClient**: Mifos API client interface
- **MifosApiClient**: Mifos API client implementation
- **IMifosClientService**: Client management service
- **IMifosLoansService**: Loan management service
- **IMifosSavingsAccountService**: Savings account service
- **IMifosOfficesService**: Office management service
- **MifosOptions**: Configuration options

## Usage Examples

### Create Client

```csharp
@inject IMifosClientService ClientService

var client = await ClientService.CreateClientAsync(clientRequest);
```

### Create Loan

```csharp
@inject IMifosLoansService LoansService

var loan = await LoansService.CreateLoanAsync(loanRequest);
```

## Related Libraries

- **Mamey.Http**: HTTP client infrastructure

## Tags

#mifos #fineract #core-banking #financial #mamey















