using Microsoft.Extensions.Logging.Abstractions;
using Mamey.AmvvaStandards;
using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Requests;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Shared.Storage;
using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Shared.Storage.Templates;
using Mamey.Portal.Shared.Tenancy;
using Mamey.Word;

namespace Mamey.Portal.Shared.Tests;

public sealed class CitizenshipWorkflowTests
{
    [Fact]
    public async Task ValidateApplicationAsync_RejectsMissingFieldsConsentsAndDocs()
    {
        var applicationId = Guid.NewGuid();
        var store = new FakeWorkflowStore
        {
            Application = new WorkflowApplicationSnapshot(
                applicationId,
                "tenant",
                "Submitted",
                Email: "",
                DateOfBirth: default,
                FirstName: "",
                LastName: "",
                AcknowledgeTreaty: false,
                SwearAllegiance: false,
                ConsentToVerification: false,
                ConsentToDataProcessing: false,
                UploadKinds: Array.Empty<string>(),
                ApplicationNumber: "APP-0001")
        };

        var svc = new ApplicationWorkflowService(
            new FixedTenantContext("tenant"),
            new StubBackofficeService(),
            NullLogger<ApplicationWorkflowService>.Instance,
            store);

        var result = await svc.ValidateApplicationAsync(applicationId);

        Assert.False(result);
        Assert.Equal("Rejected", store.LastUpdate?.Status);
        Assert.Contains("Missing required fields", store.LastUpdate?.RejectionReason);
        Assert.Contains("Missing required consents", store.LastUpdate?.RejectionReason);
        Assert.Contains("Missing required documents", store.LastUpdate?.RejectionReason);
    }

    [Fact]
    public async Task ProcessKycAsync_RejectsUnderageApplicants()
    {
        var applicationId = Guid.NewGuid();
        var dob = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-17));
        var store = new FakeWorkflowStore
        {
            Application = new WorkflowApplicationSnapshot(
                applicationId,
                "tenant",
                "Validating",
                Email: "jane@example.com",
                DateOfBirth: dob,
                FirstName: "Jane",
                LastName: "Doe",
                AcknowledgeTreaty: true,
                SwearAllegiance: true,
                ConsentToVerification: true,
                ConsentToDataProcessing: true,
                UploadKinds: new[] { "PersonalDocument", "PassportPhoto", "Signature" },
                ApplicationNumber: "APP-0002")
        };

        var svc = new ApplicationWorkflowService(
            new FixedTenantContext("tenant"),
            new StubBackofficeService(),
            NullLogger<ApplicationWorkflowService>.Instance,
            store);

        var result = await svc.ProcessKycAsync(applicationId);

        Assert.False(result);
        Assert.Equal("Rejected", store.LastUpdate?.Status);
        Assert.Contains("at least 18", store.LastUpdate?.RejectionReason);
    }

    [Fact]
    public async Task CreatePaymentPlanAsync_CallsStoreForApprovedStatus()
    {
        var applicationId = Guid.NewGuid();
        var store = new FakeWorkflowStore
        {
            Application = new WorkflowApplicationSnapshot(
                applicationId,
                "tenant",
                "Approved",
                Email: "jane@example.com",
                DateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-30)),
                FirstName: "Jane",
                LastName: "Doe",
                AcknowledgeTreaty: true,
                SwearAllegiance: true,
                ConsentToVerification: true,
                ConsentToDataProcessing: true,
                UploadKinds: new[] { "PersonalDocument", "PassportPhoto", "Signature" },
                ApplicationNumber: "APP-0003")
        };

        var svc = new ApplicationWorkflowService(
            new FixedTenantContext("tenant"),
            new StubBackofficeService(),
            NullLogger<ApplicationWorkflowService>.Instance,
            store);

        var result = await svc.CreatePaymentPlanAsync(applicationId);

        Assert.True(result);
        Assert.True(store.CreatePaymentPlanCalled);
    }

    [Fact]
    public async Task IssueIdCardAsync_BackfillsMissingDocumentNumber()
    {
        var tenant = new FixedTenantContext("tenant");
        var now = new DateTimeOffset(2026, 01, 12, 12, 0, 0, TimeSpan.Zero);
        var applicationId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        var existing = new IssuedDocumentSummary(
            Guid.NewGuid(),
            applicationId,
            "IdCard:MedicinalCannabis",
            DocumentNumber: null,
            ExpiresAt: null,
            FileName: "idcard.html",
            ContentType: "text/html",
            Size: 128,
            StorageBucket: "bucket",
            StorageKey: "key",
            CreatedAt: now);

        var store = new FakeBackofficeStore
        {
            ExistingIssuedDocument = existing,
            IdCardApplication = new ApplicationIdCardSnapshot(
                applicationId,
                "APP-0004",
                "Jane",
                "Doe",
                null,
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-30)),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null)
        };

        var svc = new CitizenshipBackofficeService(
            store,
            tenant,
            new StubCitizenshipApplicationService(),
            new StubObjectStorage(),
            new StubDocumentNamingStore(),
            new StubDocumentNamingService(),
            new StubDocumentTemplateStore(),
            new StubRealtimeNotifier(),
            new StubAamvaBarcodeService(),
            new StubMrzGenerator(),
            new StubDocumentNumberGenerator(),
            new StubDocumentValidityCalculator(),
            new StubCitizenshipStatusService(),
            new StubWordService(),
            NullLogger<CitizenshipBackofficeService>.Instance);

        var result = await svc.IssueIdCardAsync(applicationId, "MedicinalCannabis");

        var expectedNumber = IssuedDocumentNumberGenerator.IdCard(
            tenant.TenantId,
            existing.CreatedAt,
            existing.ApplicationId,
            "MedicinalCannabis");

        Assert.Equal(expectedNumber, result.DocumentNumber);
        Assert.Equal(expectedNumber, store.LastUpdatedDocumentNumber);
        Assert.Equal(existing.CreatedAt.AddYears(5), store.LastUpdatedExpiresAt);
    }

    private sealed class FixedTenantContext : ITenantContext
    {
        public FixedTenantContext(string tenantId) => TenantId = tenantId;
        public string TenantId { get; }
    }

    private sealed class FakeWorkflowStore : IApplicationWorkflowStore
    {
        public WorkflowApplicationSnapshot? Application { get; set; }
        public (string Status, string? RejectionReason)? LastUpdate { get; private set; }
        public bool CreatePaymentPlanCalled { get; private set; }
        public bool CreatePaymentPlanResult { get; set; } = true;

        public Task<WorkflowApplicationSnapshot?> GetApplicationAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => Task.FromResult(Application);

        public Task<IntakeReviewSnapshot?> GetLatestIntakeReviewAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => Task.FromResult<IntakeReviewSnapshot?>(null);

        public Task<bool> UpdateStatusAsync(string tenantId, Guid applicationId, string status, string? rejectionReason, DateTimeOffset updatedAt, CancellationToken ct = default)
        {
            LastUpdate = (status, rejectionReason);
            return Task.FromResult(true);
        }

        public Task<bool> CreatePaymentPlanIfMissingAsync(string tenantId, Guid applicationId, string applicationNumber, decimal amount, string currency, DateTimeOffset now, CancellationToken ct = default)
        {
            CreatePaymentPlanCalled = true;
            return Task.FromResult(CreatePaymentPlanResult);
        }
    }

    private sealed class FakeBackofficeStore : ICitizenshipBackofficeStore
    {
        public IssuedDocumentSummary? ExistingIssuedDocument { get; set; }
        public ApplicationIdCardSnapshot? IdCardApplication { get; set; }
        public string? LastUpdatedDocumentNumber { get; private set; }
        public DateTimeOffset? LastUpdatedExpiresAt { get; private set; }

        public Task<IReadOnlyList<BackofficeApplicationSummary>> GetRecentApplicationsAsync(string tenantId, int take, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<BackofficeApplicationDetails?> GetApplicationAsync(string tenantId, Guid id, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task UpdateApplicationStatusByNumberAsync(string tenantId, string applicationNumber, string status, DateTimeOffset updatedAt, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<IssuedDocumentSummary>> GetIssuedDocumentsAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<ApplicationCertificateSnapshot?> GetApplicationForCertificateAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<ApplicationPassportSnapshot?> GetApplicationForPassportAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<ApplicationIdCardSnapshot?> GetApplicationForIdCardAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => Task.FromResult(IdCardApplication);

        public Task<ApplicationVehicleTagSnapshot?> GetApplicationForVehicleTagAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<ApplicationTravelIdSnapshot?> GetApplicationForTravelIdAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IssuedDocumentSummary?> GetIssuedDocumentAsync(string tenantId, Guid applicationId, string kind, CancellationToken ct = default)
            => Task.FromResult(ExistingIssuedDocument);

        public Task<IssuedDocumentSummary> InsertIssuedDocumentAsync(string tenantId, IssuedDocumentCreate create, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task UpdateApplicationUpdatedAtAsync(string tenantId, Guid applicationId, DateTimeOffset updatedAt, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task UpdateIssuedDocumentAsync(Guid issuedDocumentId, string documentNumber, DateTimeOffset? expiresAt, CancellationToken ct = default)
        {
            LastUpdatedDocumentNumber = documentNumber;
            LastUpdatedExpiresAt = expiresAt;
            return Task.CompletedTask;
        }

        public Task<string?> GetApplicationEmailAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<ApplicationReissueSnapshot>> GetApplicationsForReissueAsync(string tenantId, string email, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<IssuedDocumentSummary>> GetIssuedDocumentsForApplicationAsync(Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task RemoveIssuedDocumentAsync(Guid issuedDocumentId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<bool> ApplicationExistsAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task UpsertIntakeReviewAsync(string tenantId, SubmitIntakeReviewRequest request, DateTimeOffset now, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<ApplicationDecisionSnapshot?> GetApplicationForDecisionAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task UpdateApplicationStatusAsync(string tenantId, Guid applicationId, string status, DateTimeOffset updatedAt, string? rejectionReason, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IssuedDocumentSummary?> GetIssuedDocumentByIdAsync(Guid issuedDocumentId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<ApplicationRenewalSnapshot?> GetApplicationForRenewalAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();
    }

    private sealed class StubBackofficeService : ICitizenshipBackofficeService
    {
        public Task<IReadOnlyList<BackofficeApplicationSummary>> GetRecentApplicationsAsync(int take = 50, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<BackofficeApplicationSummary>>(Array.Empty<BackofficeApplicationSummary>());

        public Task<BackofficeApplicationDetails?> GetApplicationAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<BackofficeApplicationDetails?>(null);

        public Task<string> CreateManualApplicationAsync(SubmitCitizenshipApplicationRequest request, IReadOnlyList<UploadFile> personalDocuments, UploadFile passportPhoto, UploadFile signatureImage, CancellationToken ct = default)
            => Task.FromResult("APP-0000");

        public Task<IReadOnlyList<IssuedDocumentSummary>> GetIssuedDocumentsAsync(Guid applicationId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<IssuedDocumentSummary>>(Array.Empty<IssuedDocumentSummary>());

        public Task<IssuedDocumentSummary> GenerateCitizenshipCertificateAsync(Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IssuedDocumentSummary> IssuePassportAsync(Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IssuedDocumentSummary> IssueIdCardAsync(Guid applicationId, string variant, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IssuedDocumentSummary> IssueVehicleTagAsync(Guid applicationId, string variant, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IssuedDocumentSummary> IssueTravelIdAsync(Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<IssuedDocumentSummary>> ReissueDocumentsForStatusProgressionAsync(string email, CitizenshipStatus newStatus, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task SubmitIntakeReviewAsync(SubmitIntakeReviewRequest request, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task ApproveApplicationAsync(Guid applicationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task RejectApplicationAsync(Guid applicationId, string reason, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IssuedDocumentSummary> RenewDocumentAsync(Guid expiredDocumentId, CancellationToken ct = default)
            => throw new NotImplementedException();
    }

    private sealed class StubCitizenshipApplicationService : ICitizenshipApplicationService
    {
        public Task<string> SubmitAsync(SubmitCitizenshipApplicationRequest request, IReadOnlyList<UploadFile> personalDocuments, UploadFile passportPhoto, UploadFile signatureImage, CancellationToken ct = default)
            => throw new NotImplementedException();
    }

    private sealed class StubObjectStorage : IObjectStorage
    {
        public Task PutAsync(string bucket, string key, Stream content, long size, string contentType, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<ObjectStorageReadResult> GetAsync(string bucket, string key, CancellationToken ct = default)
            => throw new NotImplementedException();
    }

    private sealed class StubDocumentNamingStore : IDocumentNamingStore
    {
        public Task<DocumentNamingPattern> GetAsync(string tenantId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task SetAsync(string tenantId, DocumentNamingPattern pattern, CancellationToken ct = default)
            => throw new NotImplementedException();
    }

    private sealed class StubDocumentNamingService : IDocumentNamingService
    {
        public string GenerateObjectKey(DocumentNamingPattern pattern, DocumentNamingContext context)
            => throw new NotImplementedException();
    }

    private sealed class StubDocumentTemplateStore : IDocumentTemplateStore
    {
        public Task<string?> GetTemplateAsync(string tenantId, string kind, CancellationToken ct = default)
            => Task.FromResult<string?>(null);

        public Task<IReadOnlyList<DocumentTemplateSummary>> ListTemplatesAsync(string tenantId, int take = 200, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<DocumentTemplateSummary>>(Array.Empty<DocumentTemplateSummary>());

        public Task UpsertTemplateAsync(string tenantId, string kind, string templateHtml, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task DeleteTemplateAsync(string tenantId, string kind, CancellationToken ct = default)
            => throw new NotImplementedException();
    }

    private sealed class StubRealtimeNotifier : ICitizenshipRealtimeNotifier
    {
        public Task NotifyApplicationUpdatedAsync(string tenantId, string email, Guid applicationId, CancellationToken ct = default)
            => Task.CompletedTask;

        public Task NotifyIssuedDocumentCreatedAsync(string tenantId, string email, Guid applicationId, Guid issuedDocumentId, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    private sealed class StubAamvaBarcodeService : IAamvaBarcodeService
    {
        public Task<byte[]?> GenerateDriverLicenseBarcodeAsync(DriverLicenseCard card, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<byte[]?> GenerateIdentificationCardBarcodeAsync(IdentificationCard card, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<byte[]?> GenerateBarcodeFromEncodedDataAsync(string aamvaEncodedData, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }

    private sealed class StubMrzGenerator : IMrzGenerator
    {
        public string GenerateTD1Line1(string documentNumber) => string.Empty;
        public string GenerateTD1Line2(DateTime birthDate, DateTime expDate, string optionalData = "") => string.Empty;
        public string GenerateTD1Line3(string surname, string givenNames) => string.Empty;
        public int CalculateIcaoCheckDigit(string mrzString) => 0;
        public string SanitizeMrzFields(string input) => string.Empty;
    }

    private sealed class StubDocumentNumberGenerator : IDocumentNumberGenerator
    {
        public string GeneratePassportNumber(string surname, DateTime birthDate) => "P-000000000";
        public string GenerateMedicalCardNumber() => "M-000000000";
        public string RemoveDocumentPrefixes(string documentNumber) => documentNumber;
        public int CalculateLuhnCheckDigit(string numberString) => 0;
    }

    private sealed class StubDocumentValidityCalculator : IDocumentValidityCalculator
    {
        public DateTimeOffset CalculateExpirationDate(CitizenshipStatus status, string documentKind, DateTimeOffset issueDate)
            => issueDate.AddYears(5);

        public int GetValidityPeriodYears(CitizenshipStatus status, string documentKind)
            => 5;
    }

    private sealed class StubCitizenshipStatusService : ICitizenshipStatusService
    {
        public Task<CitizenshipStatus?> GetStatusByEmailAsync(string email, CancellationToken ct = default)
            => Task.FromResult<CitizenshipStatus?>(null);

        public Task<CitizenshipStatus?> GetStatusByApplicationIdAsync(Guid applicationId, CancellationToken ct = default)
            => Task.FromResult<CitizenshipStatus?>(null);

        public Task CreateOrUpdateStatusAsync(Guid applicationId, string email, CancellationToken ct = default)
            => Task.CompletedTask;

        public Task<bool> IsEligibleForStatusProgressionAsync(string email, CitizenshipStatus targetStatus, CancellationToken ct = default)
            => Task.FromResult(false);

        public Task<CitizenshipStatusDetailsDto?> GetStatusDetailsAsync(string email, CancellationToken ct = default)
            => Task.FromResult<CitizenshipStatusDetailsDto?>(null);
    }

    private sealed class StubWordService : IWordService
    {
        public Task<string> GenerateWordToPathAsync<T>(T templateModel, string assemblyName, string? outputDirectory = null) where T : IWordTemplateModel
            => throw new NotImplementedException();

        public Task<string> GenerateWordToPathAsync(string resource, string assemblyName, IDictionary<string, object> values, WordDocumentProperties props, Dictionary<string, byte[]>? imageSelectors, string outputFilePath)
            => throw new NotImplementedException();

        public Task<string> GenerateWordFromFileAsync(string templateFilePath, IDictionary<string, object> values, WordDocumentProperties props, Dictionary<string, byte[]>? imageSelectors, string outputFilePath)
            => throw new NotImplementedException();
    }
}
