# MameyFutureAI

**Advanced AI and Machine Learning Services Platform for the Mamey Ecosystem**

MameyFutureAI is a Python-based AI/ML services platform that provides high-performance machine learning inference, model management, and AI capabilities for the Mamey ecosystem. It integrates seamlessly with MameyNode via gRPC and implements the complete AI/ML service interface.

## Overview

MameyFutureAI provides:
- **High-Performance Inference**: Rust-based inference engine with Python bindings
- **Model Lifecycle Management**: Complete model registry, versioning, and deployment
- **Real-Time AI Services**: Fraud detection, risk scoring, anomaly detection, and more
- **MameyNode Integration**: Native gRPC integration with MameyNode
- **Production-Ready**: Observability, monitoring, and scaling capabilities

## Architecture

```
MameyFutureAI/
├── mamey_future_ai/          # Main package
│   ├── grpc_server/         # gRPC service (Port 50100)
│   ├── inference/           # Inference engine (Port 50101)
│   ├── training/            # Model training (Port 50102)
│   ├── registry/            # Model registry (Port 50103)
│   └── monitoring/          # Performance monitoring (Port 50104)
├── proto/                    # Protocol buffer definitions
├── tests/                    # Test suite
└── docs/                     # Documentation
```

## Services

| Service | Port | Description |
|---------|------|-------------|
| gRPC Server | 50100 | Main gRPC service implementing MameyNode AI/ML interface |
| Inference Engine | 50101 | High-performance Rust-based inference |
| Training Service | 50102 | Model training and experiment tracking |
| Model Registry | 50103 | Model versioning and management |
| Monitoring | 50104 | Performance monitoring and metrics |

## Features

### AI/ML Capabilities

- **Liquidity Forecasting**: Predict liquidity needs and optimize cash flow
- **Risk Scoring**: Evaluate risk profiles for entities and transactions
- **Fraud Detection**: Real-time fraud detection and prevention
- **Anomaly Detection**: Identify unusual patterns and behaviors
- **Credit Scoring**: Calculate credit scores and predict defaults
- **Predictive Analytics**: Forecast demand, churn, and outcomes
- **Market Analysis**: Analyze market trends and sentiment
- **Recommendation Engine**: Personalized recommendations and offers
- **Natural Language Processing**: Sentiment analysis, entity extraction, document classification
- **Model Management**: Complete lifecycle management for ML models

## Technology Stack

- **Python 3.11+**: Primary development language
- **Rust**: High-performance inference engine (via PyO3)
- **gRPC**: Service communication
- **MLflow**: Model registry and tracking
- **ONNX Runtime**: Optimized inference
- **Redis**: Feature store and caching
- **PostgreSQL**: Metadata storage
- **Prometheus**: Metrics collection
- **Jaeger**: Distributed tracing

## Quick Start

### Prerequisites

- Python 3.11+
- Poetry (recommended) or pip
- Docker and Docker Compose
- Rust (for inference engine)

### Installation

```bash
# Clone the repository
git clone <repository-url>
cd MameyFutureAI

# Install dependencies
poetry install
# or
pip install -r requirements.txt

# Set up environment variables
cp .env.example .env
# Edit .env with your configuration
```

### Running Locally

```bash
# Start all services with Docker Compose
docker-compose up

# Or run individual services
poetry run python -m mamey_future_ai.grpc_server
```

### Development

```bash
# Install development dependencies
poetry install --with dev

# Run tests
poetry run pytest

# Format code
poetry run black mamey_future_ai
poetry run ruff check mamey_future_ai

# Type checking
poetry run mypy mamey_future_ai
```

## Integration with MameyNode

MameyFutureAI implements the complete `AiMlService` interface from MameyNode:

```python
# MameyNode connects to MameyFutureAI
from mamey_node.grpc import AiMlServiceClient

client = AiMlServiceClient("http://mamey-future-ai:50100")
response = await client.forecast_liquidity(request)
```

## Documentation

- **BDD**: `.designs/BDD/MameyFutureAI/MameyFutureAI BDD.md`
- **TDD**: `.designs/TDD/MameyFutureAI/MameyFutureAI TDD.md`
- **API Documentation**: `docs/api/`
- **Module Documentation**: `docs/modules/`

## Project Status

🚧 **In Development** - Phase 1: Foundation and Core Services

See `scrum-master-plans/MameyFutureAI/` for detailed task breakdown.

## License

AGPL-3.0 (Same as Mamey Framework)

## Contributing

See CONTRIBUTING.md for guidelines.

---

**Mamey Framework - Building better AI/ML services with Python**
