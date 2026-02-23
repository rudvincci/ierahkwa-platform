# Mamey.Identity.Decentralized

**Library**: `Mamey.Identity.Decentralized`  
**Location**: `Mamey/src/Mamey.Identity.Decentralized/`  
**Type**: Identity Library - Decentralized Identifiers (DIDs)  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Identity.Decentralized`

## Overview

Mamey.Identity.Decentralized provides comprehensive support for Decentralized Identifiers (DIDs) and Verifiable Credentials (VCs) in the Mamey framework. It implements W3C DID standards and supports multiple DID methods including ION, Web, Ethereum, and Peer.

### Key Features

- **DID Resolution**: Resolve DIDs to DID Documents
- **DID Creation**: Create DIDs using multiple methods (ION, Web, Ethereum, Peer)
- **DID Dereferencing**: Dereference DID URLs and fragments
- **Verifiable Credentials**: Support for W3C Verifiable Credentials
- **Verifiable Presentations**: Support for Verifiable Presentations
- **Multiple DID Methods**: ION, Web, Ethereum (ethr), Peer (peer)
- **Key Management**: Cryptographic key generation and management
- **Credential Status**: Status list and revocation support

## Installation

```bash
dotnet add package Mamey.Identity.Decentralized
```

## Quick Start

```csharp
using Mamey.Identity.Decentralized;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddDecentralizedIdentifiers(options =>
    {
        options.Enabled = true;
        options.DefaultDidMethod = "ion";
        options.Resolver.EnabledMethods.Add("ion");
        options.Resolver.EnabledMethods.Add("web");
    });

var app = builder.Build();
app.UseDecentralizedIdentifiers();
app.Run();
```

## Configuration

```json
{
  "dids": {
    "Enabled": true,
    "DefaultDidMethod": "ion",
    "SupportedProofTypes": ["Ed25519Signature2020"],
    "Resolver": {
      "EnabledMethods": ["ion", "web", "ethr", "peer"]
    },
    "Crypto": {
      "AllowedKeyTypes": ["Ed25519", "Secp256k1"]
    }
  }
}
```

## Core Components

- **IDidService**: DID operations interface
- **DidService**: DID service implementation
- **IDidResolver**: DID resolver interface
- **IKeyProvider**: Key management interface
- **ICredentialStatusService**: Credential status service
- **DidDocument**: DID Document model
- **Did**: DID model

## Usage Examples

### Resolve DID

```csharp
@inject IDidService DidService

var didDocument = await DidService.ResolveAsync("did:ion:example");
```

### Create DID

```csharp
var didDocument = await DidService.CreateAsync("ion", new { });
var did = didDocument.Id;
```

### Dereference DID URL

```csharp
var content = await DidService.DereferenceAsync("did:example:123#key-1");
```

## Related Libraries

- **Mamey.Identity.Core**: Core identity abstractions
- **Mamey.Blockchain**: Blockchain integration

## Tags

#identity #did #decentralized #verifiable-credentials #w3c #mamey

