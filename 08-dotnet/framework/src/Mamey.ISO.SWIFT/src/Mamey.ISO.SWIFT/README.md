# Mamey.ISO.SWIFT

SWIFT FIN message parser and converter library for the Mamey Framework.

## Features

- ✅ Parse SWIFT FIN messages (Blocks 1-5)
- ✅ Convert to JSON format
- ✅ Convert to XML format
- ✅ Validate message structure
- ✅ Parse MT103 (Customer Credit Transfer) messages
- ✅ Support for multiple message types
- ✅ Extract field values by tag

## Usage

### Basic Parsing

```csharp
using Mamey.ISO.SWIFT;

// Parse a SWIFT FIN message
var swiftMessage = SwiftParser.Parse(@"
{1:F01PTSBCHSSAXXX0001000001}
{2:I103PTSBCHSSXXXXN}
{3:{108:10-103-NVR-0033}}
{4:
:20:10-103-NVR-0033
:32A:091120EUR15000,11
:50F:/123456
1/Name of the ordering customer
6/US/Issuer/123456
:59:/456123
BENEFICIARY CUSTOMER NAME
SYDNEY AUSTRALIA
:71A:BEN
-}
");

// Get field values
var reference = swiftMessage.GetFieldValue("20");
var amount = swiftMessage.GetFieldValue("32A");
```

### Convert to JSON

```csharp
// Convert to JSON
var json = swiftMessage.ToJson(prettyPrint: true);
Console.WriteLine(json);

// Or use the static method
var json2 = SwiftParser.ToJson(swiftMessageText);
```

### Convert to XML

```csharp
// Convert to XML
var xml = swiftMessage.ToXml();
Console.WriteLine(xml);

// Or use the static method
var xml2 = SwiftParser.ToXml(swiftMessageText);
```

### Validate Message

```csharp
// Validate message structure
var validation = SwiftParser.Validate(swiftMessageText);

if (validation.IsValid)
{
    Console.WriteLine("Message is valid");
}
else
{
    foreach (var error in validation.Errors)
    {
        Console.WriteLine($"Error: {error}");
    }
}

foreach (var warning in validation.Warnings)
{
    Console.WriteLine($"Warning: {warning}");
}
```

### Parse MT103 Messages

```csharp
// Parse MT103 specifically
var swiftMessage = SwiftParser.Parse(mt103Text);
var mt103 = Mt103Parser.Parse(swiftMessage);

Console.WriteLine($"Sender Reference: {mt103.SenderReference}");
Console.WriteLine($"Amount: {mt103.ValueDateAmount?.Amount} {mt103.ValueDateAmount?.Currency}");
Console.WriteLine($"Beneficiary: {mt103.BeneficiaryCustomer?.Value}");
```

### Parse Multiple Messages

```csharp
// Parse multiple messages from a file or text
var messages = SwiftParser.ParseMultiple(multipleMessagesText);

foreach (var message in messages)
{
    Console.WriteLine($"Message Type: {message.MessageType}");
    Console.WriteLine($"JSON: {message.ToJson()}");
}
```

### Handle Encrypted Containers

```csharp
// Attempt to extract SWIFT messages from encrypted/compressed containers
var result = await EncryptedContainerHandler.ExtractFromFileAsync("message.fin");

if (result.Success)
{
    foreach (var message in result.Messages)
    {
        Console.WriteLine($"Extracted: {message.MessageType}");
    }
}
else
{
    Console.WriteLine($"Extraction failed: {result.Message}");
    Console.WriteLine($"Detected format: {result.DetectedFormat}");
}

// Check if a file is encrypted
var fileData = await File.ReadAllBytesAsync("message.fin");
var isEncrypted = EncryptedContainerHandler.IsEncryptedContainer(fileData);
```

## Message Structure

SWIFT FIN messages consist of 5 blocks:

- **Block 1**: Basic Header (Application ID, Logical Terminal Address, Session/Sequence Numbers)
- **Block 2**: Application Header (Input/Output ID, Message Type, Receiver Address, Priority)
- **Block 3**: User Header (Optional - contains tags like 108, 121)
- **Block 4**: Text Block (Message body with fields like :20:, :32A:, :59:, etc.)
- **Block 5**: Trailer (Optional - contains tags like MAC, CHK)

## Supported Message Types

Currently supports parsing of:
- **MT103**: Customer Credit Transfer
- **Generic MT**: Any SWIFT FIN message (basic parsing)

Additional MT types can be added by implementing specific parsers similar to `Mt103Parser`.

## Integration with Mamey Framework

This library integrates with:
- `Mamey.ISO.ISO9362` - For BIC code validation
- `Mamey.Bank.Messages` - For MT message models
- `Mamey.ISO.ISO20022` - For ISO 20022 message conversion

## References

- [SWIFT Standards](https://www.swift.com/standards)
- [Incentage Labs SWIFT Converter](https://labs.incentage.com/conversion-service/)
- [CoolUtils SWIFT Viewer](https://www.coolutils.com/online/SWIFTViewer)

## License

Proprietary - Copyright (c) 2025 Mamey.io
