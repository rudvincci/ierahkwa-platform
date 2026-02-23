# Mamey.Emails

**Library**: `Mamey.Emails`  
**Location**: `Mamey/src/Mamey.Emails/`  
**Type**: Communication Library - Email  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Emails`

## Overview

Mamey.Emails provides comprehensive email functionality for the Mamey framework. It supports multiple email providers including SMTP (via MailKit) and Azure Communication Services (ACS) with template support.

### Key Features

- **SMTP Support**: MailKit-based SMTP email sending
- **Azure Communication Services**: ACS email integration
- **Template Support**: Email templates with model binding
- **Embedded Templates**: Support for embedded resource templates
- **FluentEmail Integration**: FluentEmail API for email composition
- **HTML Support**: HTML email support
- **Business Email Templates**: Pre-built business email templates

## Installation

```bash
dotnet add package Mamey.Emails
```

## Quick Start

```csharp
using Mamey.Emails;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddEmail();

var app = builder.Build();
app.Run();
```

## Configuration

```json
{
  "email": {
    "Enabled": true,
    "Type": "smtp",
    "EmailId": "noreply@example.com",
    "Name": "Mamey App",
    "Mailkit": {
      "Host": "smtp.example.com",
      "Port": 587,
      "Username": "user",
      "Password": "password"
    }
  }
}
```

## Core Components

- **IMameyEmailService**: Email service interface
- **MameyEmailService**: Email service implementation
- **IEmailTemplate**: Email template interface
- **IBusinessEmailTemplate**: Business email template interface
- **EmailOptions**: Configuration options

## Usage Examples

### Send Email with Template

```csharp
@inject IMameyEmailService EmailService

var template = new BusinessEmailTemplate(
    title: "Welcome",
    companyName: "Mamey",
    companyAddress: address,
    recipientName: name,
    supportUrl: "https://support.example.com");

await EmailService.SendEmailUsingTemplate(
    to: "user@example.com",
    subject: "Welcome",
    template: "WelcomeTemplate",
    model: template);
```

## Related Libraries

- **Mamey.Types**: Value objects (Address, Name, etc.)

## Tags

#email #smtp #acs #mailkit #templates #mamey

