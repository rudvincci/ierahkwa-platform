# Mamey.Ktt

**Library**: `Mamey.Ktt`  
**Location**: `Mamey/src/Mamey.Ktt/`  
**Type**: Banking Library - SWIFT Message Templates  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Ktt`

## Overview

Mamey.Ktt provides SWIFT message template generation and formatting for the Mamey framework. It includes MT (Message Type) message models and template builders for various SWIFT message categories.

### Key Features

- **SWIFT Messages**: MT message models (MT103, MT102, MT101, etc.)
- **Message Templates**: Template-based message generation
- **Message Builder**: Template builder with field replacement
- **Field Tags**: SWIFT field tag attributes
- **Category Support**: Multiple SWIFT message categories

## Installation

```bash
dotnet add package Mamey.Ktt
```

## Quick Start

```csharp
using Mamey.Ktt;
using Mamey.Ktt.Message.Category1;

var mt103 = new MT103
{
    TransactionReference = "ABC1234567890123",
    BankOperationCode = "CRED",
    ValueDateCurrencyAmount = "241117USD10000,50"
};

var message = MessageBuilder.BuildMessage(template, mt103);
```

## Core Components

- **MessageBuilder**: Template message builder
- **MT103**: MT103 message model
- **MT102**: MT102 message model
- **MT101**: MT101 message model
- **FieldTagAttribute**: Field tag attribute
- **FieldNameAttribute**: Field name attribute

## Usage Examples

### Build MT103 Message

```csharp
var mt103 = new MT103
{
    TransactionReference = "ABC1234567890123",
    BankOperationCode = "CRED",
    ValueDateCurrencyAmount = "241117USD10000,50",
    OrderingCustomerBIC = "BANKUS33XXX",
    OrderingCustomerDetails = "John Doe, 123 Main St",
    BeneficiaryCustomer = "Jane Smith",
    RemittanceInformation = "Payment for invoice #12345"
};

var template = @"F20: Transaction Reference Number : {{20}}";
var message = MessageBuilder.BuildMessage(template, mt103);
```

## Related Libraries

- **Mamey.ISO.ISO20022**: ISO 20022 financial messaging

## Tags

#swift #mt-messages #banking #financial-messaging #mamey















