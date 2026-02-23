# Mamey.Twilio

**Library**: `Mamey.Twilio`  
**Location**: `Mamey/src/Mamey.Twilio/`  
**Type**: Communication Library - Twilio SMS/Voice  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Twilio`

## Overview

Mamey.Twilio provides Twilio integration for SMS and voice communication in the Mamey framework. It includes SMS sending, voice calls, and Twilio webhook handling.

### Key Features

- **SMS Sending**: Send SMS messages via Twilio
- **Voice Calls**: Make voice calls via Twilio
- **Webhooks**: Twilio webhook handling
- **Twilio ASP.NET Core**: Integration with Twilio.AspNet.Core

## Installation

```bash
dotnet add package Mamey.Twilio
```

## Quick Start

```csharp
using Mamey.Twilio;

var builder = WebApplication.CreateBuilder(args);

// Twilio integration setup
builder.Services.AddMamey();

var app = builder.Build();
app.Run();
```

## Configuration

```json
{
  "Twilio": {
    "AccountSid": "AC...",
    "AuthToken": "your-auth-token",
    "PhoneNumber": "+1234567890"
  }
}
```

## Core Components

- **TwilioOptions**: Configuration options

## Usage Examples

### Send SMS

```csharp
// Implementation pending
```

## Related Libraries

- **Mamey.Emails**: Email communication

## Tags

#twilio #sms #voice #communication #mamey

