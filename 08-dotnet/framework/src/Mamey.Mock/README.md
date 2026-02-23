# Mamey.Mock

**Library**: `Mamey.Mock`  
**Location**: `Mamey/src/Mamey.Mock/`  
**Type**: Testing Library - Mock Data Generation  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Mock`

## Overview

Mamey.Mock provides mock data generation utilities for testing in the Mamey framework. It includes name generation, email generation, phone numbers, and other common test data.

### Key Features

- **Name Generation**: Generate names with gender support
- **Email Generation**: Generate email addresses from names
- **Phone Number Generation**: Generate phone numbers
- **Date Generation**: Generate dates of birth and timestamps
- **Document URLs**: Generate KYC document and profile image URLs
- **License Numbers**: Generate license and registration numbers

## Installation

```bash
dotnet add package Mamey.Mock
```

## Quick Start

```csharp
using Mamey.Mock;

var name = TypeMockService.GenerateName(Gender.Male);
var email = TypeMockService.GenerateEmail(name);
var phone = TypeMockService.GeneratePhoneNumber();
```

## Core Components

- **TypeMockService**: Static service for mock data generation

## Usage Examples

### Generate Name

```csharp
var name = TypeMockService.GenerateName(Gender.Female);
// Returns Name with FirstName, MiddleName, LastName, Nickname
```

### Generate Email

```csharp
var email = TypeMockService.GenerateEmail(name);
// Returns "firstName.lastName@example.com"
```

### Generate Phone Number

```csharp
var phone = TypeMockService.GeneratePhoneNumber();
// Returns "555-XXX-XXXX"
```

### Generate Date of Birth

```csharp
var dateOfBirth = TypeMockService.GenerateDateOfBirth();
// Returns DateTime between 21-55 years ago
```

## Related Libraries

- **Mamey.Types**: Value objects (Name, etc.)

## Tags

#testing #mocking #test-data #generation #mamey















