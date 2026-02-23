# 🎓 Pupitre - AI-First Educational Platform

[![CI](https://github.com/pupitre/pupitre/actions/workflows/ci.yml/badge.svg)](https://github.com/pupitre/pupitre/actions/workflows/ci.yml)
[![CD](https://github.com/pupitre/pupitre/actions/workflows/cd.yml/badge.svg)](https://github.com/pupitre/pupitre/actions/workflows/cd.yml)
[![License: AGPL-3.0](https://img.shields.io/badge/License-AGPL%203.0-blue.svg)](https://opensource.org/licenses/AGPL-3.0)

Pupitre is a comprehensive AI-powered educational platform built with .NET 9 and microservices architecture. It provides personalized learning experiences through AI tutoring, adaptive assessments, and intelligent content delivery.

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Gateway (YARP)                        │
│                         Port: 60000                              │
└─────────────────────────────────────────────────────────────────┘
                                 │
        ┌────────────────────────┼────────────────────────┐
        │                        │                        │
        ▼                        ▼                        ▼
┌───────────────┐      ┌───────────────┐      ┌───────────────┐
│  Foundation   │      │  AI Services  │      │   Support     │
│   Services    │      │   (60101+)    │      │   Services    │
│  (60001-60010)│      │               │      │  (60022-60029)│
└───────────────┘      └───────────────┘      └───────────────┘
        │                        │                        │
        └────────────────────────┼────────────────────────┘
                                 │
        ┌────────────────────────┼────────────────────────┐
        │                        │                        │
        ▼                        ▼                        ▼
┌───────────────┐      ┌───────────────┐      ┌───────────────┐
│  PostgreSQL   │      │   MongoDB     │      │    Redis      │
│  (Write Model)│      │ (Read Model)  │      │   (Cache)     │
└───────────────┘      └───────────────┘      └───────────────┘
```

## 📦 Services

### Foundation Services (10)
| Service | Port | Description |
|---------|------|-------------|
| Users | 60001 | User management and authentication |
| GLEs | 60002 | Grade Level Expectations |
| Curricula | 60003 | Curriculum management |
| Lessons | 60004 | Lesson content and delivery |
| Assessments | 60005 | Assessment management |
| Progress | 60006 | Learning progress tracking |
| Rewards | 60007 | Gamification and rewards |
| Notifications | 60008 | Push notifications |
| Parents | 60009 | Parent portal integration |
| Analytics | 60010 | Learning analytics |

### AI Services (10)
| Service | Port | Description |
|---------|------|-------------|
| AITutors | 60101 | Conversational AI tutoring |
| AIAssessments | 60102 | AI-powered assessment grading |
| AIContent | 60103 | Content generation |
| AISpeech | 60104 | Speech-to-text, text-to-speech |
| AIAdaptive | 60105 | Adaptive learning paths |
| AIBehavior | 60106 | Behavior analysis |
| AISafety | 60107 | Content safety filtering |
| AIRecommendations | 60108 | Learning recommendations |
| AITranslation | 60109 | Multi-language support |
| AIVision | 60110 | Image/document analysis |

### Support Services (8)
| Service | Port | Description |
|---------|------|-------------|
| Educators | 60022 | Educator management |
| Fundraising | 60023 | Campaign management |
| Bookstore | 60024 | Educational materials |
| Aftercare | 60025 | After-school programs |
| Accessibility | 60026 | Accessibility features |
| Compliance | 60027 | Regulatory compliance |
| Ministries | 60028 | Ministry integrations |
| Operations | 60029 | Operations metrics |

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Docker & Docker Compose
- Node.js 20+ (for frontend tools)

### Development Setup

```bash
# Clone repository
git clone https://github.com/pupitre/pupitre.git
cd pupitre

# Start infrastructure
docker-compose -f deploy/docker/docker-compose.yml up -d

# Build solution
dotnet build Pupitre.sln

# Run a specific service
dotnet run --project src/Services/Foundation/Pupitre.Users/src/Pupitre.Users.Api

# Run all services (development)
docker-compose -f deploy/docker/docker-compose.services.yml up -d

# Seed canonical demo data (optional)
dotnet run --project tools/Pupitre.DevSeeder/Pupitre.DevSeeder.csproj
```

See [`docs/dev-seeding.md`](docs/dev-seeding.md) for more options (custom API base URL, dry-run mode, etc.).
```

### Running Tests

```bash
# Run all unit tests
dotnet test tests/**/*.Tests.Unit.csproj

# Run integration tests
dotnet test tests/Pupitre.Tests.Integration

# Run E2E tests
dotnet test tests/Pupitre.Tests.E2E

# Run performance tests (requires k6)
k6 run tests/Pupitre.Tests.Performance/k6/load-test-all-services.js
```

## 🔧 Configuration

Each service uses the standard ASP.NET Core configuration system:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=pupitre;Username=pupitre;Password=secret",
    "MongoDB": "mongodb://localhost:27017",
    "Redis": "localhost:6379"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "pupitre",
    "Password": "secret"
  },
  "Vault": {
    "Address": "http://localhost:8200",
    "Token": "dev-token"
  }
}
```

## 📊 Observability

- **Metrics**: Prometheus (port 9090)
- **Dashboards**: Grafana (port 3000)
- **Tracing**: Jaeger (port 16686)
- **Logging**: Seq (port 5341)

## 🔐 Security

- JWT authentication for API access
- Role-based access control (RBAC)
- HashiCorp Vault for secrets management
- AI Safety filtering for all content
- COPPA/FERPA compliance features

## 📁 Project Structure

```
Pupitre/
├── src/
│   ├── Services/
│   │   ├── Foundation/     # Core educational services
│   │   ├── AI/             # AI-powered services
│   │   └── Support/        # Administrative services
│   ├── Clients/
│   │   ├── BlazorWasm/     # Micro-frontends
│   │   └── Portals/        # Aggregated portals
│   ├── Shared/             # Shared libraries
│   └── Infrastructure/     # API Gateway
├── tests/                  # Test projects
├── deploy/
│   ├── docker/            # Docker Compose files
│   ├── k8s/               # Kubernetes manifests
│   └── helm/              # Helm charts
├── tools/
│   └── Pupitre.DevSeeder/ # Development data seeding console
└── docs/                  # Documentation
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the AGPL-3.0 License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [Mamey Framework](https://mamey.io)
- AI powered by Azure OpenAI and Ollama
- Vector search by Qdrant

---

**Pupitre** - Empowering Education Through AI 🎓✨
