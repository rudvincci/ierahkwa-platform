# Mamey.Stripe

**Library**: `Mamey.Stripe`  
**Location**: `Mamey/src/Mamey.Stripe/`  
**Type**: Payment Library - Stripe Integration  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Stripe`

## Overview

Mamey.Stripe provides comprehensive Stripe payment processing integration for the Mamey framework. It includes payment intents, subscriptions, customers, charges, and webhook handling.

### Key Features

- **Payment Intents**: Create and manage payment intents
- **Subscriptions**: Create and manage subscriptions
- **Customers**: Customer management
- **Charges**: Process payments
- **Webhooks**: Stripe webhook handling
- **Stripe Connect**: Multi-party payment support
- **Stripe Radar**: Fraud detection integration

## Installation

```bash
dotnet add package Mamey.Stripe
```

## Quick Start

```csharp
using Mamey.Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStripe(new StripeOptions
{
    ApiKey = "sk_test_...",
    WebhookSecret = "whsec_...",
    DefaultCurrency = "usd"
});

var app = builder.Build();
app.Run();
```

## Configuration

```json
{
  "Stripe": {
    "ApiKey": "sk_test_...",
    "WebhookSecret": "whsec_...",
    "DefaultCurrency": "usd",
    "SuccessUrl": "https://example.com/success",
    "CancelUrl": "https://example.com/cancel"
  }
}
```

## Core Components

- **IStripeService**: Stripe service interface
- **StripeServiceBase**: Stripe service implementation
- **StripeOptions**: Configuration options
- **PaymentIntentRequest**: Payment intent model
- **SubscriptionRequest**: Subscription model

## Usage Examples

### Create Payment Intent

```csharp
@inject IStripeService StripeService

var request = new PaymentIntentRequest
{
    Amount = 10000, // $100.00
    Currency = "usd",
    Description = "Product purchase"
};

var paymentIntentId = await StripeService.CreatePaymentIntentAsync(request);
```

### Create Subscription

```csharp
var subscriptionRequest = new SubscriptionRequest
{
    CustomerId = "cus_...",
    PlanId = "plan_...",
    Quantity = 1
};

var subscriptionId = await StripeService.CreateSubscriptionAsync(subscriptionRequest);
```

## Related Libraries

- **Mamey.ISO.ISO4217**: Currency codes

## Tags

#stripe #payments #subscriptions #payment-processing #mamey

