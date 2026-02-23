# Mamey.TravelIdentityStandards

**Library**: `Mamey.TravelIdentityStandards`  
**Location**: `Mamey/src/Mamey.TravelIdentityStandards/`  
**Type**: Identity Library - Travel Document Standards  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.TravelIdentityStandards`

## Overview

Mamey.TravelIdentityStandards provides ICAO Doc 9303 compliant machine-readable travel document (MRTD) support for the Mamey framework. It includes passport, ID card, and visa document models with MRZ generation.

### Key Features

- **ICAO Doc 9303 Compliance**: Full compliance with ICAO standards
- **Passport Documents**: Passport document model (TD3)
- **ID Card Documents**: ID card document model (TD1)
- **Visa Documents**: Visa document model
- **MRZ Generation**: Machine-readable zone generation
- **Document Validation**: Comprehensive document validation
- **Photo Specifications**: Photo dimension and format validation

## Installation

```bash
dotnet add package Mamey.TravelIdentityStandards
```

## Quick Start

```csharp
using Mamey.TravelIdentityStandards;

var personData = new PersonData
{
    Surname = "DOE",
    GivenNames = "JOHN",
    DateOfBirth = new DateTime(1980, 1, 1),
    Nationality = "USA",
    Gender = "M"
};

var passport = new PassportDocument(
    personData: personData,
    documentNumber: "123456789",
    issuingCountry: "USA",
    expiryDate: DateTime.Now.AddYears(10),
    optionalData: "",
    photoFormat: "JPEG",
    issuerName: "United States Department of State",
    issuerCode: "USA",
    documentTypeCode: "P",
    securityFeatures: new[] { "Hologram", "UV" },
    issueDate: DateTime.Now
);
```

## Core Components

- **TravelDocument**: Base travel document class
- **PassportDocument**: Passport document model
- **IdCardTravelDocument**: ID card document model
- **VisaDocument**: Visa document model
- **PersonData**: Person data model
- **MrzGenerator**: MRZ generator
- **IdCardMrz**: ID card MRZ model
- **PassportMrz**: Passport MRZ model

## Usage Examples

### Create Passport

```csharp
var passport = new PassportDocument(
    personData: personData,
    documentNumber: "123456789",
    issuingCountry: "USA",
    expiryDate: DateTime.Now.AddYears(10),
    optionalData: "",
    photoFormat: "JPEG",
    issuerName: "United States Department of State",
    issuerCode: "USA",
    documentTypeCode: "P",
    securityFeatures: new[] { "Hologram", "UV" },
    issueDate: DateTime.Now
);

var mrz = passport.Mrz; // Generated MRZ
```

## Related Libraries

- **Mamey.AmvvaStandards**: AAMVA standards
- **Mamey.Biometrics**: Biometric support

## Tags

#icao #travel-documents #mrtd #passport #mrz #mamey

