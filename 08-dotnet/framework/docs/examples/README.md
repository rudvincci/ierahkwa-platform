# Mamey Framework Examples

This directory contains comprehensive examples demonstrating how to use the Mamey .NET microservices framework in real-world scenarios.

## Table of Contents

- [Overview](#overview)
- [Basic Examples](#basic-examples)
- [Advanced Examples](#advanced-examples)
- [Integration Examples](#integration-examples)
- [Deployment Examples](#deployment-examples)
- [Best Practices Examples](#best-practices-examples)

## Overview

The examples in this directory are designed to help developers understand how to implement various patterns and features using the Mamey framework. Each example includes:

- Complete, runnable code
- Step-by-step instructions
- Configuration examples
- Best practices
- Common pitfalls and solutions

## Basic Examples

### 1. [Simple Microservice](simple-microservice/)
A basic microservice example demonstrating:
- Core framework setup
- Basic CQRS implementation
- Simple API endpoints
- Basic logging and health checks

**Key Features:**
- User management (CRUD operations)
- Command and query handlers
- Event publishing
- Basic validation

### 2. [CQRS Implementation](cqrs-implementation/)
A comprehensive CQRS example showing:
- Command handling
- Query handling
- Event handling
- Validation and error handling

**Key Features:**
- Product catalog management
- Order processing
- Inventory management
- Event-driven architecture

### 3. [Message Broker Integration](message-broker-integration/)
RabbitMQ integration example demonstrating:
- Message publishing
- Message consumption
- Event handling
- Error handling and retries

**Key Features:**
- Order processing workflow
- Payment processing
- Notification system
- Dead letter queue handling

## Advanced Examples

### 4. [Authentication & Authorization](authentication-authorization/)
Complete authentication system example:
- JWT token management
- Role-based authorization
- User management
- Security best practices

**Key Features:**
- User registration and login
- JWT token generation and validation
- Role-based access control
- Password hashing and validation

### 5. [Persistence Layer](persistence-layer/)
Multi-database persistence example:
- MongoDB integration
- Redis caching
- SQL Server with Entity Framework
- Repository pattern implementation

**Key Features:**
- User data management
- Caching strategies
- Transaction management
- Data migration

### 6. [Observability](observability/)
Comprehensive observability example:
- Structured logging
- Distributed tracing
- Application metrics
- Health monitoring

**Key Features:**
- Serilog configuration
- Jaeger tracing
- Prometheus metrics
- Custom health checks

## Integration Examples

### 7. [Microservices Communication](microservices-communication/)
Inter-service communication example:
- HTTP client communication
- Message-based communication
- Service discovery
- Load balancing

**Key Features:**
- User service
- Order service
- Payment service
- Service mesh integration

### 8. [API Gateway](api-gateway/)
Ntrada API Gateway example:
- Request routing
- Authentication
- Rate limiting
- Request/response transformation

**Key Features:**
- YAML configuration
- JWT authentication
- CORS handling
- Error handling

### 9. [Event Sourcing](event-sourcing/)
Event sourcing implementation:
- Event store
- Aggregate roots
- Event replay
- Snapshot management

**Key Features:**
- Bank account management
- Transaction history
- Event replay
- Snapshot optimization

## Deployment Examples

### 10. [Docker Deployment](docker-deployment/)
Containerized deployment example:
- Dockerfile creation
- Docker Compose setup
- Multi-stage builds
- Production optimization

**Key Features:**
- Multi-service setup
- Environment configuration
- Health checks
- Logging configuration

### 11. [Kubernetes Deployment](kubernetes-deployment/)
Kubernetes deployment example:
- Deployment manifests
- Service configuration
- ConfigMaps and Secrets
- Ingress setup

**Key Features:**
- Horizontal scaling
- Service discovery
- Configuration management
- Monitoring setup

### 12. [Azure Deployment](azure-deployment/)
Azure cloud deployment example:
- Azure Container Instances
- Azure Service Bus
- Azure Cosmos DB
- Azure Application Insights

**Key Features:**
- Cloud-native deployment
- Managed services
- Monitoring and logging
- Cost optimization

## Best Practices Examples

### 13. [Error Handling](error-handling/)
Comprehensive error handling example:
- Global exception handling
- Custom exceptions
- Error logging
- Client error responses

**Key Features:**
- Exception middleware
- Custom error types
- Error correlation
- User-friendly messages

### 14. [Validation](validation/)
Input validation example:
- FluentValidation integration
- Command validation
- Query validation
- Custom validators

**Key Features:**
- Input sanitization
- Business rule validation
- Error message customization
- Validation performance

### 15. [Testing](testing/)
Testing strategies example:
- Unit testing
- Integration testing
- End-to-end testing
- Test data management

**Key Features:**
- Test containers
- Mock services
- Test automation
- Coverage reporting

## Getting Started with Examples

### Prerequisites

Before running any example, ensure you have:

- .NET 9.0 SDK or later
- Docker Desktop
- Visual Studio 2022 or VS Code
- Git

### Running an Example

1. **Clone the repository**
   ```bash
   git clone https://github.com/mamey-io/mamey.git
   cd mamey/docs/examples
   ```

2. **Choose an example**
   ```bash
   cd simple-microservice
   ```

3. **Install dependencies**
   ```bash
   dotnet restore
   ```

4. **Start required services**
   ```bash
   docker-compose up -d
   ```

5. **Run the example**
   ```bash
   dotnet run
   ```

### Example Structure

Each example follows a consistent structure:

```
example-name/
├── src/
│   ├── Example.API/          # Web API project
│   ├── Example.Application/  # Application layer
│   ├── Example.Domain/       # Domain layer
│   └── Example.Infrastructure/ # Infrastructure layer
├── tests/
│   ├── Example.UnitTests/    # Unit tests
│   └── Example.IntegrationTests/ # Integration tests
├── docker-compose.yml        # Docker services
├── README.md                 # Example documentation
└── appsettings.json         # Configuration
```

## Contributing Examples

We welcome contributions of new examples! When contributing:

1. **Follow the structure** outlined above
2. **Include comprehensive documentation** in the README.md
3. **Add unit and integration tests**
4. **Use best practices** demonstrated in existing examples
5. **Test thoroughly** before submitting

### Example Guidelines

- **Keep it simple**: Examples should be easy to understand
- **Be complete**: Include all necessary code and configuration
- **Document well**: Provide clear instructions and explanations
- **Test thoroughly**: Ensure examples work as expected
- **Follow patterns**: Use established patterns and conventions

## Support

For questions about examples:

- [GitHub Issues](https://github.com/mamey-io/mamey/issues)
- [Documentation](https://docs.mamey.io)
- [Discord Community](https://discord.gg/mamey)

## License

All examples are licensed under the MIT License - see the LICENSE file for details.

---

**Happy Coding!** These examples should help you get started with the Mamey framework and build amazing microservices applications.
