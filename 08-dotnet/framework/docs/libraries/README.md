# Mamey Framework Libraries

Complete reference for all 110+ Mamey Framework libraries organized by category.

## Library Categories

### [Core Framework](core/)
Foundation libraries for the Mamey Framework including the core framework, microservice infrastructure, and networking utilities.

**Libraries (7):**
- [Mamey](core/mamey.md) - Core framework with builder pattern and base abstractions
- [Mamey.Net](core/net.md) - Network utilities and HTTP client extensions
- [Mamey.Micro](core/micro.md) - Microservice utilities
- [Mamey.Microservice.Abstractions](core/microservice-abstractions.md) - Microservice abstractions
- [Mamey.Microservice.Infrastructure](core/microservice-infrastructure.md) - Microservice infrastructure
- [Mamey.MicroMonolith.Abstractions](core/micromonolith-abstractions.md) - MicroMonolith abstractions
- [Mamey.MicroMonolith.Infrastructure](core/micromonolith-infrastructure.md) - MicroMonolith infrastructure

### [CQRS](cqrs/)
Command Query Responsibility Segregation pattern implementations for commands, queries, and events.

**Libraries (6):**
- [Mamey.CQRS.Commands](cqrs/cqrs-commands.md) - Command pattern implementation
- [Mamey.CQRS.Events](cqrs/cqrs-events.md) - Event handling and dispatching
- [Mamey.CQRS.Queries](cqrs/cqrs-queries.md) - Query pattern implementation
- [Mamey.Logging.CQRS](cqrs/logging-cqrs.md) - CQRS logging decorators
- [Mamey.MessageBrokers.CQRS](cqrs/messagebrokers-cqrs.md) - Message broker CQRS integration
- [Mamey.WebApi.CQRS](cqrs/webapi-cqrs.md) - Web API CQRS integration

### [Messaging](messaging/)
Message broker abstractions and implementations for asynchronous messaging.

**Libraries (5):**
- [Mamey.MessageBrokers](messaging/messagebrokers.md) - Message broker abstractions
- [Mamey.MessageBrokers.RabbitMQ](messaging/messagebrokers-rabbitmq.md) - RabbitMQ implementation
- [Mamey.MessageBrokers.Outbox](messaging/messagebrokers-outbox.md) - Outbox pattern implementation
- [Mamey.MessageBrokers.Outbox.EntityFramework](messaging/messagebrokers-outbox-entityframework.md) - Entity Framework outbox
- [Mamey.MessageBrokers.Outbox.Mongo](messaging/messagebrokers-outbox-mongo.md) - MongoDB outbox

### [Authentication](auth/)
Authentication and authorization libraries supporting multiple authentication providers.

**Libraries (13):**
- [Mamey.Auth](auth/auth.md) - Core authentication framework
- [Mamey.Auth.Abstractions](auth/auth-abstractions.md) - Authentication abstractions
- [Mamey.Auth.Azure](auth/auth-azure.md) - Azure AD authentication
- [Mamey.Auth.Azure.B2B](auth/auth-azure-b2b.md) - Azure AD B2B authentication
- [Mamey.Auth.Azure.B2C](auth/auth-azure-b2c.md) - Azure AD B2C authentication
- [Mamey.Auth.Azure.B2B.BlazorWasm](auth/auth-azure-b2b-blazorwasm.md) - Azure AD B2B Blazor WebAssembly
- [Mamey.Auth.Jwt](auth/auth-jwt.md) - JWT authentication
- [Mamey.Auth.Jwt.BlazorWasm](auth/auth-jwt-blazorwasm.md) - JWT Blazor WebAssembly
- [Mamey.Auth.Jwt.Server](auth/auth-jwt-server.md) - JWT server-side authentication
- [Mamey.Auth.Identity](auth/auth-identity.md) - ASP.NET Core Identity integration
- [Mamey.Auth.Distributed](auth/auth-distributed.md) - Distributed authentication
- [Mamey.Auth.Decentralized](auth/auth-decentralized.md) - Decentralized authentication
- [Mamey.Auth.DecentralizedIdentifiers](auth/auth-decentralizedidentifiers.md) - DID authentication

### [Identity](identity/)
Identity management libraries for user and identity management.

**Libraries (11):**
- [Mamey.Identity.Core](identity/identity-core.md) - Core identity framework
- [Mamey.Identity.AspNetCore](identity/identity-aspnetcore.md) - ASP.NET Core Identity
- [Mamey.Identity.Azure](identity/identity-azure.md) - Azure Identity integration
- [Mamey.Identity.Blazor](identity/identity-blazor.md) - Blazor Identity
- [Mamey.Identity.Jwt](identity/identity-jwt.md) - JWT Identity
- [Mamey.Identity.EntityFramework](identity/identity-entityframework.md) - Entity Framework Identity
- [Mamey.Identity.Redis](identity/identity-redis.md) - Redis Identity caching
- [Mamey.Identity.Decentralized](identity/identity-decentralized.md) - Decentralized Identity
- [Mamey.Identity.Distributed](identity/identity-distributed.md) - Distributed Identity
- [Mamey.Azure.Identity](identity/azure-identity.md) - Azure Identity
- [Mamey.Azure.Identity.BlazorWasm](identity/azure-identity-blazorwasm.md) - Azure Identity Blazor WebAssembly

### [Persistence](persistence/)
Database and persistence layer integrations for various data stores.

**Libraries (8):**
- [Mamey.Persistence.MongoDB](persistence/persistence-mongodb.md) - MongoDB integration
- [Mamey.Persistence.PostgreSQL](persistence/persistence-postgresql.md) - PostgreSQL integration
- [Mamey.Persistence.Redis](persistence/persistence-redis.md) - Redis caching
- [Mamey.Persistence.SQL](persistence/persistence-sql.md) - SQL Server integration
- [Mamey.Persistence.MySql](persistence/persistence-mysql.md) - MySQL integration
- [Mamey.Persistence.MySQL](persistence/persistence-mysql.md) - MySQL integration (alternative)
- [Mamey.Persistence.Minio](persistence/persistence-minio.md) - MinIO object storage
- [Mamey.Persistence.OpenStack](persistence/persistence-openstack.md) - OpenStack object storage

### [Observability](observability/)
Logging, tracing, and metrics libraries for application observability.

**Libraries (6):**
- [Mamey.Logging](observability/logging.md) - Comprehensive logging framework
- [Mamey.Tracing.Jaeger](observability/tracing-jaeger.md) - Jaeger distributed tracing
- [Mamey.Tracing.Jaeger.RabbitMQ](observability/tracing-jaeger-rabbitmq.md) - Jaeger RabbitMQ integration
- [Mamey.Metrics.Prometheus](observability/metrics-prometheus.md) - Prometheus metrics
- [Mamey.Metrics.AppMetrics](observability/metrics-appmetrics.md) - AppMetrics
- [Mamey.OpenTracingContrib](observability/opentracingcontrib.md) - OpenTracing contributions

### [Infrastructure](infrastructure/)
Infrastructure services including API gateway, service discovery, load balancing, and security.

**Libraries (13):**
- [Mamey.WebApi](infrastructure/webapi.md) - Web API framework
- [Mamey.WebApi.Security](infrastructure/webapi-security.md) - Web API security
- [Mamey.WebApi.Swagger](infrastructure/webapi-swagger.md) - Swagger/OpenAPI integration
- [Mamey.Discovery.Consul](infrastructure/discovery-consul.md) - Consul service discovery
- [Mamey.LoadBalancing.Fabio](infrastructure/loadbalancing-fabio.md) - Fabio load balancing
- [Mamey.Ntrada](infrastructure/ntrada.md) - Ntrada API Gateway
- [Mamey.Secrets.Vault](infrastructure/secrets-vault.md) - HashiCorp Vault integration
- [Mamey.Security](infrastructure/security.md) - **Security utilities (Encryption, Hashing, Certificates)**
  - **Integration Libraries**:
    - [Mamey.Security.EntityFramework](../Mamey/src/Mamey.Security.EntityFramework/README.md) - EF Core value converters
    - [Mamey.Security.MongoDB](../Mamey/src/Mamey.Security.MongoDB/README.md) - MongoDB BSON serializers
    - [Mamey.Security.Redis](../Mamey/src/Mamey.Security.Redis/README.md) - Redis serializers
- [Mamey.Policies](infrastructure/policies.md) - Policy enforcement
- [Mamey.Modules](infrastructure/modules.md) - Modular architecture
- [Mamey.Modules.MessageBrokers](infrastructure/modules-messagebrokers.md) - Modular message brokers
- [Mamey.Modules.MessageBrokers.Outbox](infrastructure/modules-messagebrokers-outbox.md) - Modular outbox
- [Mamey.Modules.MessageBrokers.Outbox.EntityFramework](infrastructure/modules-messagebrokers-outbox-entityframework.md) - Modular EF outbox

### [Integration](integration/)
Third-party service integrations for payments, communications, banking, and more.

**Libraries (11):**
- [Mamey.Stripe](integration/stripe.md) - Stripe payment processing
- [Mamey.Visa](integration/visa.md) - Visa payment integration
- [Mamey.Mifos](integration/mifos.md) - Mifos X financial services
- [Mamey.Emails](integration/emails.md) - Email services
- [Mamey.Twilio](integration/twilio.md) - Twilio SMS and voice
- [Mamey.Graph](integration/graph.md) - Microsoft Graph API
- [Mamey.Blockchain](integration/blockchain.md) - Blockchain integration
- [Mamey.Web3](integration/web3.md) - Web3 integration
- [Mamey.OpenBanking](integration/openbanking.md) - Open Banking APIs
- [Mamey.Ktt](integration/ktt.md) - Key Telex Transfer messaging
- [Mamey.Binimoy](integration/binimoy.md) - Binimoy protocol

### [UI & Client](ui/)
User interface and client-side libraries for Blazor, MAUI, and web applications.

**Libraries (4):**
- [Mamey.Blazor.Abstractions](ui/blazor-abstractions.md) - Blazor abstractions
- [Mamey.Blazor.Identity](ui/blazor-identity.md) - Blazor Identity
- [Mamey.BlazorWasm](ui/blazorwasm.md) - Blazor WebAssembly
- [Mamey.Maui](ui/maui.md) - .NET MAUI integration

### [Utilities](utilities/)
Utility libraries for document processing, algorithms, data generation, and more.

**Libraries (8):**
- [Mamey.Adobe](utilities/adobe.md) - Adobe PDF Services
- [Mamey.Algorithms](utilities/algorithms.md) - Algorithms and data structures
- [Mamey.Barcode](utilities/barcode.md) - Barcode generation
- [Mamey.Excel](utilities/excel.md) - Excel processing
- [Mamey.Image](utilities/image.md) - Image processing
- [Mamey.Word](utilities/word.md) - Word document processing
- [Mamey.Mock](utilities/mock.md) - Mock data generation
- [Mamey.Templates](utilities/templates.md) - Document templates

### [Standards](standards/)
ISO standards, compliance libraries, and identity standards implementations.

**Libraries (14):**
- [Mamey.ISO.Abstractions](standards/iso-abstractions.md) - ISO standard abstractions
- [Mamey.ISO.ISO3166](standards/iso-iso3166.md) - ISO 3166 country codes
- [Mamey.ISO.ISO4217](standards/iso-iso4217.md) - ISO 4217 currency codes
- [Mamey.ISO.ISO639](standards/iso-iso639.md) - ISO 639 language codes
- [Mamey.ISO.ISO13616](standards/iso-iso13616.md) - ISO 13616 IBAN
- [Mamey.ISO.ISO20022](standards/iso-iso20022.md) - ISO 20022 financial messaging
- [Mamey.ISO.ISO22301](standards/iso-iso22301.md) - ISO 22301 business continuity
- [Mamey.ISO.ISO27001](standards/iso-iso27001.md) - ISO 27001 information security
- [Mamey.ISO.ISO8583](standards/iso-iso8583.md) - ISO 8583 financial transaction messages
- [Mamey.ISO.ISO9362](standards/iso-iso9362.md) - ISO 9362 SWIFT/BIC codes
- [Mamey.ISO.PCI_DSS](standards/iso-pci_dss.md) - PCI DSS compliance
- [Mamey.AmvvaStandards](standards/amvvastandards.md) - AAMVA driver license standards
- [Mamey.TravelIdentityStandards](standards/travelidentitystandards.md) - ICAO travel document standards
- [Mamey.Biometrics](standards/biometrics.md) - Biometric processing

### [Azure](azure/)
Azure-specific integrations and services.

**Libraries (2):**
- [Mamey.Azure.Abstractions](azure/azure-abstractions.md) - Azure abstractions
- [Mamey.Azure.Blobs](azure/azure-blobs.md) - Azure Blob Storage

### [HTTP](http/)
HTTP client libraries and REST API integrations.

**Libraries (2):**
- [Mamey.Http](http/http.md) - HTTP client utilities
- [Mamey.Http.RestEase](http/http-restease.md) - RestEase integration

### [Documentation](docs/)
Documentation and API documentation tools.

**Libraries (1):**
- [Mamey.Docs.Swagger](docs/docs-swagger.md) - Swagger/OpenAPI documentation

## Statistics

- **Total Libraries**: 110
- **Core Framework**: 7 libraries
- **CQRS**: 6 libraries
- **Messaging**: 5 libraries
- **Authentication**: 13 libraries
- **Identity**: 11 libraries
- **Persistence**: 8 libraries
- **Observability**: 6 libraries
- **Infrastructure**: 13 libraries
- **Integration**: 11 libraries
- **UI & Client**: 4 libraries
- **Utilities**: 8 libraries
- **Standards**: 14 libraries
- **Azure**: 2 libraries
- **HTTP**: 2 libraries
- **Documentation**: 1 library

## 🔍 Quick Search

Use the search functionality to find specific libraries, features, or topics.

## Contributing

We welcome contributions! Please see our Contributing Guide for details.

---

**Mamey Framework** - Building better microservices with .NET

