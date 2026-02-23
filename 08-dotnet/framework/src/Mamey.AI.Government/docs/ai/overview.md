# Mamey.AI.Government Overview

**Mamey.AI.Government** is a shared library providing AI-first capabilities to the Government Services ecosystem.

## Core Services

1.  **Document Verification**: Automated classification, authenticity checks, and OCR for identity documents.
2.  **Fraud Detection**: ML-based scoring of applications and transactions to detect suspicious patterns.
3.  **KYC/AML Automation**: Automated Sanctions screening, PEP checks, and risk scoring.
4.  **Biometric Matching**: Face matching and liveness detection for identity verification.
5.  **Compliance Automation**: Rule-based engine for regulatory compliance (GOV-005, 2025-ID01).
6.  **Anomaly Detection**: Operational monitoring for unusual patterns in system metrics.
7.  **Chatbot / NLP**: Intelligent citizen support agent.
8.  **Predictive Analytics**: Forecasting for application volumes and processing times.

## Architecture

The library is designed as a set of `Microsoft.Extensions.DependencyInjection` compatible services. It includes:
-   **C# Interfaces/Services**: The primary integration point for microservices.
-   **Python Training Pipeline**: Scripts in `ml/training` for model development.
-   **Model Serving**: Docker/K8s configuration in `docker/` for scalable inference (optional).

## Integration

Add the library to your service:

```csharp
// Infrastructure/Extensions.cs
services.AddGovernmentAI();
```

Inject the required interface:

```csharp
public class MyService
{
    private readonly IDocumentVerificationService _docService;
    public MyService(IDocumentVerificationService docService) => _docService = docService;
}
```
