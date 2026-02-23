# Mamey.Web3

**Library**: `Mamey.Web3`  
**Location**: `Mamey/src/Mamey.Web3/`  
**Type**: Blockchain Library - Web3 Integration  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Web3`

## Overview

Mamey.Web3 provides Web3 blockchain integration for the Mamey framework. It includes Ethereum blockchain interaction, token balance checking, account management, and block monitoring.

### Key Features

- **Ethereum Integration**: Nethereum-based Ethereum blockchain integration
- **Token Balance Service**: Token balance checking with price data
- **Account Management**: Account creation and management
- **Block Monitoring**: New block processing and monitoring
- **Web3 Provider Service**: Web3 provider management
- **CoinGecko Integration**: Token price data from CoinGecko

## Installation

```bash
dotnet add package Mamey.Web3
```

## Quick Start

```csharp
using Mamey.Web3;
using Mamey.Web3.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IWeb3ProviderService, Web3ProviderService>();
builder.Services.AddScoped<TokenBalanceService>();

var app = builder.Build();
app.Run();
```

## Core Components

- **IWeb3ProviderService**: Web3 provider service interface
- **Web3ProviderService**: Web3 provider service implementation
- **TokenBalanceService**: Token balance service
- **AccountsService**: Account management service
- **NewBlockProcessingService**: Block monitoring service

## Usage Examples

### Get Token Balance

```csharp
@inject TokenBalanceService TokenBalanceService

var balance = await TokenBalanceService.GetEtherBalance("0x...");
var tokenBalances = await TokenBalanceService.GetBalances(0, 10, "0x...");
```

### Get Web3 Instance

```csharp
@inject IWeb3ProviderService Web3Provider

var web3 = Web3Provider.GetWeb3();
```

## Related Libraries

- **Mamey.Blockchain**: Blockchain abstractions

## Tags

#web3 #ethereum #blockchain #crypto #mamey















