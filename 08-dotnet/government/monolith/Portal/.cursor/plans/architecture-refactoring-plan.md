# Mamey Government Portal - Architecture Refactoring Plan

## Overview

This plan addresses critical architectural gaps identified in the current Portal implementation. The portal was built as a prototype with business logic in the wrong layers and doesn't follow the Mamey Framework patterns defined in CLAUDE.md.

**Status**: Not Started
**Priority**: CRITICAL - Must be completed before adding new features
**Estimated Effort**: 3-5 days of refactoring work

---

## Critical Issues Identified

### 1. **Empty Domain Layers** (BLOCKER)
- All Domain projects contain only placeholder `Class1.cs` files
- No domain entities, value objects, or repository interfaces defined
- Business logic scattered across Infrastructure and Web projects

### 2. **Missing Extensions.cs Files** (BLOCKER)
- No `Extensions.cs` in any Infrastructure module
- All 350+ lines of service registration hardcoded in Program.cs
- Violates Mamey Framework's modular registration pattern

### 3. **Services in Wrong Layers** (HIGH PRIORITY)
- Application services implemented in Infrastructure projects
- Business logic (PDF generation, validation, workflows) in Infrastructure
- Should follow: Domain → Application → Infrastructure separation

### 4. **Not Using Mamey.Persistence.Postgres** (HIGH PRIORITY)
- Direct EF Core usage instead of Mamey abstractions
- Missing repository pattern implementation
- No use of Mamey's persistence composition patterns

### 5. **Inline SQL Instead of Migrations** (HIGH PRIORITY)
- Schema defined with `ExecuteSqlRaw()` in Program.cs
- No version-controlled migration files
- Doesn't follow EF Core best practices

### 6. **Not Using Mamey.Word** (MEDIUM PRIORITY)
- HTML templates instead of proper DOCX generation
- Missing structured document generation capabilities
- Should leverage Mamey.Word for certificates, passports, ID cards

---

## Refactoring Phases

### Phase 1: Infrastructure Module Extensions (CRITICAL)

**Goal**: Extract all service registrations from Program.cs into module-specific Extensions.cs files

**Tasks**:
1. Create `Mamey.Portal.Citizenship.Infrastructure/Extensions.cs`
2. Create `Mamey.Portal.Cms.Infrastructure/Extensions.cs`
3. Create `Mamey.Portal.Tenant.Infrastructure/Extensions.cs`
4. Create `Mamey.Portal.Library.Infrastructure/Extensions.cs`
5. Create `Mamey.Portal.Auth.Infrastructure/Extensions.cs`

**Pattern to Follow** (from Mamey.Government.Citizens):
```csharp
public static class Extensions
{
    public static IMameyBuilder AddCitizenshipInfrastructure(
        this IMameyBuilder builder)
    {
        // 1. Ensure logging
        if (!builder.Services.Any(s => s.ServiceType == typeof(ILoggerFactory)))
        {
            builder.Services.AddLogging();
        }

        // 2. Register application services FIRST
        builder.Services.AddApplicationServices();

        // 3. Add Mamey framework infrastructure
        return builder
            .AddErrorHandler<ExceptionToResponseMapper>()
            .AddPostgresDb()
            .AddMongoDb()
            .AddRedisRepositories()
            .AddCompositeRepositories()
            .AddHandlersLogging()
            .AddMicroserviceSharedInfrastructure()
            .AddApplication();
    }
}
```

**Files to Create**:
- `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Citizenship.Infrastructure/Extensions.cs`
- `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Cms.Infrastructure/Extensions.cs`
- `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Tenant.Infrastructure/Extensions.cs`
- `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Library.Infrastructure/Extensions.cs`
- `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Auth.Infrastructure/Extensions.cs`

**Program.cs After Refactoring**:
```csharp
var builder = WebApplication.CreateBuilder(args);
var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

// Register domain modules
mameyBuilder
    .AddCitizenshipInfrastructure()
    .AddCmsInfrastructure()
    .AddTenantInfrastructure()
    .AddLibraryInfrastructure()
    .AddAuthInfrastructure();

// Register shared infrastructure (MinIO, Redis, etc.)
mameyBuilder.AddSharedInfrastructure();

// Register Web-specific services (MudBlazor, SignalR, etc.)
builder.Services.AddWebServices();

var app = builder.Build();
// ... middleware configuration
```

---

### Phase 2: Domain Layer Implementation (CRITICAL)

**Goal**: Create proper domain entities separate from persistence Row classes

**Current State**:
- `CitizenshipApplicationRow` in Infrastructure/Persistence
- No domain entity `CitizenshipApplication`
- Business rules embedded in services

**Target Structure**:
```
Mamey.Portal.Citizenship.Domain/
├── Entities/
│   ├── CitizenshipApplication.cs (domain entity)
│   ├── IssuedDocument.cs
│   ├── CitizenshipStatus.cs
│   ├── StatusProgressionApplication.cs
│   ├── IntakeReview.cs
│   └── PaymentPlan.cs
├── ValueObjects/
│   ├── ApplicationNumber.cs
│   ├── ApplicationStatus.cs (enum → value object)
│   ├── DocumentKind.cs
│   ├── CitizenshipStatusType.cs
│   └── DocumentNumber.cs
├── Repositories/
│   ├── IApplicationRepository.cs
│   ├── IIssuedDocumentRepository.cs
│   ├── ICitizenshipStatusRepository.cs
│   └── IPaymentPlanRepository.cs
├── Events/
│   ├── ApplicationSubmitted.cs
│   ├── ApplicationApproved.cs
│   ├── ApplicationRejected.cs
│   ├── DocumentIssued.cs
│   └── StatusProgressed.cs
└── Exceptions/
    ├── ApplicationNotFoundException.cs
    ├── InvalidApplicationStatusException.cs
    └── DocumentGenerationException.cs
```

**Entity Example** (CitizenshipApplication.cs):
```csharp
namespace Mamey.Portal.Citizenship.Domain.Entities;

public class CitizenshipApplication : AggregateRoot<Guid>
{
    public ApplicationNumber ApplicationNumber { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public Name ApplicantName { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public Email? Email { get; private set; }
    public Address Address { get; private set; }

    // Domain behavior methods
    public void Submit()
    {
        if (Status != ApplicationStatus.Draft)
            throw new InvalidApplicationStatusException();

        Status = ApplicationStatus.Submitted;
        AddDomainEvent(new ApplicationSubmitted(Id, ApplicationNumber));
    }

    public void Approve()
    {
        if (Status != ApplicationStatus.ReviewPending)
            throw new InvalidApplicationStatusException();

        Status = ApplicationStatus.Approved;
        AddDomainEvent(new ApplicationApproved(Id, ApplicationNumber));
    }

    // Constructor for creating new applications
    public static CitizenshipApplication Create(
        string tenantId,
        Name applicantName,
        DateOnly dateOfBirth,
        Email? email,
        Address address)
    {
        var app = new CitizenshipApplication
        {
            Id = Guid.NewGuid(),
            ApplicationNumber = ApplicationNumber.Generate(tenantId),
            Status = ApplicationStatus.Draft,
            ApplicantName = applicantName,
            DateOfBirth = dateOfBirth,
            Email = email,
            Address = address,
            CreatedAt = DateTimeOffset.UtcNow
        };

        return app;
    }
}
```

**Mapping Infrastructure → Domain**:
Infrastructure layer will map between Row classes and Domain entities:
```csharp
// In Infrastructure/Persistence/Repositories/PostgresApplicationRepository.cs
public class PostgresApplicationRepository : IApplicationRepository
{
    private readonly CitizenshipDbContext _context;

    public async Task<CitizenshipApplication?> GetByIdAsync(Guid id)
    {
        var row = await _context.Applications.FindAsync(id);
        return row?.ToDomainEntity();
    }

    public async Task SaveAsync(CitizenshipApplication application)
    {
        var row = CitizenshipApplicationRow.FromDomainEntity(application);
        _context.Applications.Update(row);
        await _context.SaveChangesAsync();
    }
}
```

**Files to Create**:
- Domain entities for all 5 modules (Citizenship, CMS, Tenant, Library, Auth)
- Repository interfaces in Domain layer
- Repository implementations in Infrastructure/Persistence/Repositories

---

### Phase 3: Application Layer Separation (HIGH PRIORITY)

**Goal**: Move business logic from Infrastructure to Application layer

**Services to Move**:

#### From `Mamey.Portal.Citizenship.Infrastructure/Services/`:
**Move to `Mamey.Portal.Citizenship.Application/Services/`:**
1. `CitizenshipApplicationService.cs` → Application service
2. `ApplicationFormPdfGenerator.cs` → Application service
3. `ApplicationWorkflowService.cs` → Application orchestration
4. `ApplicationStatusService.cs` → Application service
5. `StandardsComplianceValidator.cs` → Split:
   - Domain validation rules → Domain layer
   - Application validation → Application layer

**Keep in Infrastructure/Services/:**
- `DocumentStorageService.cs` (MinIO integration)
- `CitizenshipDbContext.cs` (persistence)
- Repository implementations

#### Pattern for Application Services:
```csharp
// Mamey.Portal.Citizenship.Application/Services/CitizenshipApplicationService.cs
namespace Mamey.Portal.Citizenship.Application.Services;

public class CitizenshipApplicationService : ICitizenshipApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IDocumentStorageService _storageService;
    private readonly ILogger<CitizenshipApplicationService> _logger;

    public CitizenshipApplicationService(
        IApplicationRepository applicationRepository,
        IDocumentStorageService storageService,
        ILogger<CitizenshipApplicationService> logger)
    {
        _applicationRepository = applicationRepository;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<ApplicationResponse> SubmitApplicationAsync(
        SubmitApplicationRequest request)
    {
        // Application logic here
        // Uses domain entities and repositories
        // No direct DbContext access!
    }
}
```

**Files to Move/Create**:
- Move 15+ service implementations from Infrastructure to Application
- Create new Application service interfaces if missing
- Update Infrastructure Extensions.cs to register Application services correctly

---

### Phase 4: Repository Pattern Implementation (HIGH PRIORITY)

**Goal**: Implement proper repository abstraction between Application and Infrastructure

**Current Issue**:
- Services directly inject `DbContext`
- No repository abstraction
- Can't swap persistence implementations

**Target Pattern**:
```
Domain Layer:
  IApplicationRepository (interface)

Application Layer:
  CitizenshipApplicationService
    ↓ (depends on)
  IApplicationRepository

Infrastructure Layer:
  PostgresApplicationRepository : IApplicationRepository
    ↓ (uses)
  CitizenshipDbContext
```

**Repository Interface** (in Domain):
```csharp
namespace Mamey.Portal.Citizenship.Domain.Repositories;

public interface IApplicationRepository
{
    Task<CitizenshipApplication?> GetByIdAsync(Guid id);
    Task<CitizenshipApplication?> GetByApplicationNumberAsync(string applicationNumber);
    Task<IEnumerable<CitizenshipApplication>> GetByTenantAsync(string tenantId);
    Task<IEnumerable<CitizenshipApplication>> GetByStatusAsync(ApplicationStatus status);
    Task SaveAsync(CitizenshipApplication application);
    Task DeleteAsync(Guid id);
}
```

**Repository Implementation** (in Infrastructure):
```csharp
namespace Mamey.Portal.Citizenship.Infrastructure.Persistence.Repositories;

public class PostgresApplicationRepository : IApplicationRepository
{
    private readonly CitizenshipDbContext _context;

    public PostgresApplicationRepository(CitizenshipDbContext context)
    {
        _context = context;
    }

    public async Task<CitizenshipApplication?> GetByIdAsync(Guid id)
    {
        var row = await _context.Applications
            .Include(a => a.Uploads)
            .Include(a => a.IssuedDocuments)
            .FirstOrDefaultAsync(a => a.Id == id);

        return row?.ToDomainEntity();
    }

    // ... other methods
}
```

**Files to Create**:
- Repository interfaces in Domain/Repositories for all entities
- Repository implementations in Infrastructure/Persistence/Repositories
- Mapping extensions (Row ↔ Domain entity)

---

### Phase 5: EF Core Migrations (HIGH PRIORITY)

**Goal**: Replace inline SQL with proper EF Core migrations

**Current Issue**:
```csharp
// In Program.cs
db.Database.EnsureCreated();
db.Database.ExecuteSqlRaw("""
    CREATE TABLE IF NOT EXISTS citizenship_issued_documents (...)
    ALTER TABLE citizenship_applications ADD COLUMN IF NOT EXISTS "ExtendedDataJson" text NULL;
    ...
""");
```

**Problems**:
- Schema not version-controlled
- Can't track schema evolution
- Difficult to rollback changes
- Not following EF Core best practices

**Target Approach**:
```bash
# Create initial migration
cd src/Mamey.Portal.Citizenship.Infrastructure
dotnet ef migrations add InitialCreate --output-dir Persistence/Migrations

# Create migration for new features
dotnet ef migrations add AddExtendedDataJsonColumn

# Apply migrations
dotnet ef database update

# In production
dotnet ef database update --connection "Production connection string"
```

**Migration Example**:
```csharp
// Persistence/Migrations/20260108_InitialCreate.cs
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "citizenship_applications",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                TenantId = table.Column<string>(maxLength: 128, nullable: false),
                ApplicationNumber = table.Column<string>(maxLength: 64, nullable: false),
                // ... all columns
            });

        migrationBuilder.CreateIndex(
            name: "IX_citizenship_applications_TenantId_ApplicationNumber",
            table: "citizenship_applications",
            columns: new[] { "TenantId", "ApplicationNumber" },
            unique: true);
    }
}
```

**Steps**:
1. Remove inline SQL from Program.cs
2. Create initial migrations for each module
3. Test migrations with fresh database
4. Document migration commands in README

**Files to Create**:
- Migration files in each Infrastructure/Persistence/Migrations directory
- Update Program.cs to call `Database.MigrateAsync()` instead of `EnsureCreated()`

---

### Phase 6: Mamey.Word Integration (MEDIUM PRIORITY)

**Goal**: Replace HTML templates with DOCX generation using Mamey.Word

**Current State**:
- HTML templates stored in `InMemoryDocumentTemplateStore`
- Limited to HTML output
- No structured document generation

**Mamey.Word Capabilities**:
```csharp
// Available in Mamey.Word library:
public interface IWordService
{
    Task<byte[]> GenerateDocumentFromTemplateAsync(
        string templatePath,
        Dictionary<string, string> replacements);

    Task<byte[]> GenerateDocumentWithImagesAsync(
        string templatePath,
        Dictionary<string, string> replacements,
        Dictionary<string, byte[]> images); // {{QR}}, {{Signature}}, etc.
}
```

**Target Architecture**:
```
1. Store DOCX templates in MinIO (or file system)
2. Use Mamey.Word for document generation
3. Replace HTML templates with DOCX templates

Templates:
- CitizenshipCertificate.docx
- Passport.docx
- IdCard.docx
- VehicleTag.docx
```

**Template Example** (CitizenshipCertificate.docx):
```
CITIZENSHIP CERTIFICATE

This certifies that {{FirstName}} {{LastName}}, born on {{DateOfBirth}},
is a citizen of {{TenantName}} with application number {{ApplicationNumber}}.

Issued: {{IssueDate}}

{{Signature}}

{{OfficialSeal}}
```

**Implementation**:
```csharp
// In Application/Services/CertificateGenerationService.cs
public class CertificateGenerationService : ICertificateGenerationService
{
    private readonly IWordService _wordService;
    private readonly IDocumentStorageService _storageService;

    public async Task<byte[]> GenerateCitizenshipCertificateAsync(
        CitizenshipApplication application)
    {
        // Get template from storage
        var templateBytes = await _storageService.GetTemplateAsync(
            "CitizenshipCertificate.docx");

        // Prepare replacements
        var replacements = new Dictionary<string, string>
        {
            ["FirstName"] = application.ApplicantName.FirstName,
            ["LastName"] = application.ApplicantName.LastName,
            ["DateOfBirth"] = application.DateOfBirth.ToString(),
            ["ApplicationNumber"] = application.ApplicationNumber.Value,
            ["IssueDate"] = DateTime.Now.ToString("yyyy-MM-dd")
        };

        // Generate document
        return await _wordService.GenerateDocumentFromTemplateAsync(
            templateBytes,
            replacements);
    }
}
```

**Files to Create/Update**:
- Add Mamey.Word NuGet reference to Citizenship.Infrastructure
- Create DOCX templates for all document types
- Update document generation services to use Mamey.Word
- Migrate existing HTML templates to DOCX format

---

### Phase 7: InternalsVisibleTo Configuration (LOW PRIORITY)

**Goal**: Configure proper visibility for testing

**Pattern** (from Mamey Framework):
```csharp
// In Infrastructure/Visibility.cs
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Portal.Citizenship.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.Portal.Citizenship.Tests.Integration")]
[assembly: InternalsVisibleTo("Mamey.Portal.Citizenship.Tests.Unit")]
```

**Files to Create**:
- `Visibility.cs` in each Infrastructure project
- Add InternalsVisibleTo for corresponding test projects

---

## Updated Implementation Priorities

### Must-Do Before New Features (CRITICAL):
1. ✅ **Phase 1**: Create Extensions.cs files (1 day)
2. ✅ **Phase 2**: Implement Domain entities (2 days)
3. ✅ **Phase 3**: Move Application services to correct layer (1 day)
4. ✅ **Phase 4**: Implement Repository pattern (1 day)

### Should-Do Soon (HIGH PRIORITY):
5. ✅ **Phase 5**: EF Core migrations (1 day)
6. ✅ **Phase 6**: Mamey.Word integration (2 days)

### Can-Do Later (MEDIUM/LOW PRIORITY):
7. ⏳ **Phase 7**: InternalsVisibleTo (0.5 day)
8. ⏳ CQRS pattern implementation (optional)
9. ⏳ Comprehensive testing (ongoing)

---

## Feature Development - New Priority Order

**After architecture refactoring is complete**, implement these features:

### Group 1: Barcode/Standards (Priority)
1. Phase 4.1.14: Extend Mamey.Barcode with GeneratePDF417Async()
2. Phase 4.1.15: Standards Compliance Validation
3. Phase 4.1.11: AAMVA PDF417 Barcode Generation
4. Phase 4.1.12: ICAO MRZ Generation
5. Phase 4.1.13: Document Number Generation

### Group 2: Workflow Enhancement
6. Phase 4.1.8: Application Package Alignment
7. Phase 4.1.9: Citizenship Status Progression
8. Phase 4.1.10: Application Status Page

---

## Testing Strategy

After each refactoring phase:
1. Run existing tests (ensure nothing breaks)
2. Add unit tests for new domain entities
3. Add integration tests for repositories
4. Add end-to-end tests for application services
5. Performance test with realistic data volumes

---

## Risk Mitigation

**Risks**:
1. Breaking changes during refactoring
2. Data migration issues with EF Core migrations
3. Performance regression with repository abstraction
4. Integration issues with Mamey.Word

**Mitigation**:
1. Refactor incrementally (one module at a time)
2. Test each phase before proceeding
3. Keep existing code working during transition
4. Use feature flags for new implementations
5. Comprehensive integration testing

---

## Reference Documentation

**Mamey Framework Docs**:
- `/Volumes/Barracuda/mamey-io/code-final/CLAUDE.md` - Main framework patterns
- `/Volumes/Barracuda/mamey-io/code-final/Mamey/docs/guides/mamey-framework-master-documentation.md` - Comprehensive reference
- `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Mamey.Government.Citizens/` - Reference microservice implementation

**Portal Docs**:
- `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/docs/` - Portal documentation
- `.cursor/plans/modular_monolith_multi-tenant_portal_with_enhanced_features_f6973c5d.plan.md` - Original feature plan

**Key Libraries**:
- `/Volumes/Barracuda/mamey-io/code-final/Mamey/src/Mamey.Persistence.Postgres/` - Postgres persistence patterns
- `/Volumes/Barracuda/mamey-io/code-final/Mamey/src/Mamey.Word/` - Document generation
- `/Volumes/Barracuda/mamey-io/code-final/Mamey/src/Mamey.Barcode/` - Barcode generation

---

## Success Criteria

The refactoring is complete when:
1. ✅ All domain modules have proper Extensions.cs
2. ✅ Domain entities exist separate from Row classes
3. ✅ Application services are in Application layer (not Infrastructure)
4. ✅ Repository pattern implemented for all aggregates
5. ✅ EF Core migrations replace inline SQL
6. ✅ Program.cs is < 100 lines (all registrations in modules)
7. ✅ All tests pass
8. ✅ No business logic in Web project
9. ✅ Mamey.Word integrated for document generation
10. ✅ Architecture matches Mamey Framework patterns

---

**Last Updated**: 2026-01-08
**Status**: Awaiting approval to begin refactoring
**Estimated Completion**: 2026-01-15 (with focused effort)
