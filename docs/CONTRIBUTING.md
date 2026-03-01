# Contributing to Ierahkwa

## Code of Conduct
We are committed to providing a welcoming and inclusive environment for indigenous communities worldwide.

## Getting Started

### Prerequisites
- Docker 27+
- .NET SDK 8.0
- Node.js 20.11+
- Go 1.22 (for WAMPUM)
- kubectl configured

### Setup
1. Clone the repository
2. Install prerequisites above
3. Read docs/Ierahkwa-Operations-Manual-v3.3.0.pdf

## Development Workflow

### Branch Naming
- `feature/nexus-name-description` for new features
- `fix/description` for bug fixes
- `docs/description` for documentation

### Commit Messages
Use conventional commits:
- `feat: add payment processing to Tesoro`
- `fix: correct aria-label in dashboard`
- `docs: update OpenAPI spec for Orbital`
- `chore: update Docker base image`

### Pull Requests
1. Create a branch from `main`
2. Make changes with tests
3. Submit PR with clear description
4. Wait for at least 1 code review
5. CI must pass (lint, test, build, security scan)

## Code Standards

### HTML Platforms (02-plataformas-html/)
- Use shared/ierahkwa.css design system
- Include skip-nav, aria-label, aria-hidden on emojis
- Support prefers-reduced-motion
- Mobile-first responsive (600px, 900px breakpoints)
- Vanilla JS only (no frameworks)

### .NET Microservices (08-dotnet/microservices/)
- .NET 8 with Alpine Docker images
- Health endpoint at /health
- Stats endpoint at /stats
- Internal port 8080
- Multi-stage Dockerfile required

### Dockerfiles
- Base: mcr.microsoft.com/dotnet/sdk:8.0-alpine
- Runtime: mcr.microsoft.com/dotnet/aspnet:8.0-alpine
- HEALTHCHECK required
- NEVER hardcode passwords

### Kubernetes (k8s/services/)
- Namespace: ierahkwa
- 2 replicas minimum
- HPA: 2-10 replicas, 70% CPU target
- Health probes: liveness + readiness
- Resource limits required

## AI Models and ONNX Contributions

### Adding New ONNX Models to the AI Model Server
1. Place your ONNX model file in the `ai-model-server/models/` directory
2. Register the model in the server configuration with its input/output tensor specifications
3. Add a health check endpoint that verifies the model loads correctly
4. Include a test suite that validates inference with sample inputs
5. Update the model registry documentation with the model's purpose and expected accuracy

### Client-Side AI (ierahkwa-ai.js)
- Use `shared/ierahkwa-ai.js` for all client-side AI inference
- The library wraps ONNX Runtime Web for browser-based model execution
- Ensure models used client-side are optimized for size and latency
- Test across supported browsers (Chrome, Firefox, Safari, Edge)

### Model Format Requirements
- All models must be in ONNX format (.onnx)
- Recommended maximum model size: 50MB (to keep container images and client downloads manageable)
- Models exceeding 50MB require approval and must document the justification
- Include a model card (Markdown file) describing training data, intended use, and limitations
- Quantized models (INT8/FP16) are preferred for production deployment

## Security
- No secrets in code (use env vars or secret managers)
- TLS 1.3 required in production
- JWT RS256 for authentication
- Post-quantum crypto (ML-DSA-65, ML-KEM-1024)
- Report security issues to security team immediately

## License
AGPL-3.0
