# Simplified Multi-Storage Microservice Template

This is a simplified version of the multi-storage microservice template that focuses on core functionality and can build without external Mamey framework dependencies.

## What's Included

### Core Projects
- **Contracts**: Public DTOs, Commands, Queries, and Events
- **Domain**: Business logic and entities
- **Application**: Command/Query handlers and application services
- **Infrastructure**: Data access, external service integrations
- **API**: Web API endpoints

### Storage Support
- **PostgreSQL**: Primary data storage for write operations
- **MongoDB**: Read models and query optimization
- **Redis**: Caching layer
- **MinIO**: File storage

### Key Features
- CQRS pattern implementation
- Multi-storage architecture
- RESTful API endpoints
- File upload/download capabilities
- Caching with Redis
- Event-driven architecture

## Getting Started

1. Build the solution:
   ```bash
   dotnet build
   ```

2. Run the API:
   ```bash
   dotnet run --project src/Pupitre.Fundraising.Api
   ```

3. Test endpoints using the provided `.http` file

## Architecture

The template demonstrates a clean architecture with:
- Domain-driven design principles
- Separation of concerns
- Dependency injection
- Repository pattern
- CQRS with separate read/write models

## Storage Strategy

- **Write Operations**: PostgreSQL (ACID compliance)
- **Read Operations**: MongoDB (query optimization)
- **Caching**: Redis (performance)
- **Files**: MinIO (object storage)

This template provides a solid foundation for building microservices with multi-storage capabilities while remaining self-contained and buildable.






















