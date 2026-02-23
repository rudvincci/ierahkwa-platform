using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Mamey.AmvvaStandards;
using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Requests;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Services;
using Mamey.Portal.Citizenship.Infrastructure.Storage;
using Mamey.Portal.Integration.Tests.Fixtures;
using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Shared.Storage.Templates;
using Mamey.Portal.Shared.Tenancy;
using Mamey.Portal.Tenant.Infrastructure.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Mamey.Word;

namespace Mamey.Portal.Integration.Tests;

[Collection("PortalIntegration")]
public sealed class CitizenshipWorkflowIntegrationTests
{
    private readonly PortalIntegrationFixture _fixture;

    public CitizenshipWorkflowIntegrationTests(PortalIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Submit_IssuePassport_ValidateAndDownload()
    {
        const string tenantId = "integration";

        await using var tenantDb = _fixture.CreateTenantDbContext();
        await using var citizenshipDb = _fixture.CreateCitizenshipDbContext();

        var tenantContext = new FixedTenantContext(tenantId);
        var namingStore = new DbDocumentNamingStore(tenantDb);
        var namingService = new DefaultDocumentNamingService();
        var templateStore = new DbDocumentTemplateStore(tenantDb);
        await templateStore.UpsertTemplateAsync(tenantId, "Passport", "<html>{{DocumentNumber}}</html>");

        var storage = new MinioObjectStorage(_fixture.Buckets, _fixture.Objects);

        var appSubmissionStore = new ApplicationSubmissionStore(citizenshipDb);
        var pdfGenerator = new StubApplicationFormPdfGenerator();

        var applicationService = new CitizenshipApplicationService(
            tenantContext,
            storage,
            namingStore,
            namingService,
            pdfGenerator,
            appSubmissionStore);

        var request = new SubmitCitizenshipApplicationRequest(
            FirstName: "Jane",
            LastName: "Doe",
            DateOfBirth: new DateOnly(1990, 1, 5),
            Email: "jane@example.com",
            AddressLine1: "123 Main St",
            City: "Toronto",
            Region: "ON",
            PostalCode: "A1A1A1",
            AcknowledgeTreaty: true,
            SwearAllegiance: true,
            ConsentToVerification: true,
            ConsentToDataProcessing: true,
            DeclareUnderstanding: true);

        var personalDoc = new UploadFile(
            "personal.pdf",
            "application/pdf",
            1,
            new MemoryStream(new byte[] { 0x1 }));

        var passportPhoto = CreateImageUpload("passport.png", 800, 800);
        var signature = CreateImageUpload("signature.png", 600, 100);

        var appNumber = await applicationService.SubmitAsync(
            request,
            new[] { personalDoc },
            passportPhoto,
            signature);

        var backofficeStore = new CitizenshipBackofficeStore(citizenshipDb);
        var statusStore = new CitizenshipStatusStore(citizenshipDb);
        var statusService = new CitizenshipStatusService(tenantContext, statusStore);
        var documentNumberGenerator = new DocumentNumberGenerator();
        var standardsValidator = new StandardsComplianceValidator();
        var mrzGenerator = new MrzGenerator(documentNumberGenerator, standardsValidator);

        var backoffice = new CitizenshipBackofficeService(
            backofficeStore,
            tenantContext,
            applicationService,
            storage,
            namingStore,
            namingService,
            templateStore,
            new NullRealtimeNotifier(),
            new StubAamvaBarcodeService(),
            mrzGenerator,
            documentNumberGenerator,
            new DocumentValidityCalculator(),
            statusService,
            new ThrowingWordService(),
            NullLogger<CitizenshipBackofficeService>.Instance);

        var appId = await citizenshipDb.Applications
            .Where(x => x.TenantId == tenantId && x.ApplicationNumber == appNumber)
            .Select(x => x.Id)
            .SingleAsync();

        var issued = await backoffice.IssuePassportAsync(appId);

        var validator = new PublicDocumentValidationService(new PublicDocumentValidationStore(citizenshipDb));
        var validation = await validator.ValidateAsync(tenantId, issued.DocumentNumber ?? string.Empty);

        Assert.True(validation.Found);
        Assert.True(validation.IsValid);
        Assert.Equal("Passport", validation.Kind);

        var stored = await storage.GetAsync(issued.StorageBucket, issued.StorageKey);
        Assert.NotNull(stored.Content);
        Assert.True(stored.Size > 0);
    }

    private static UploadFile CreateImageUpload(string fileName, int width, int height)
    {
        using var image = new Image<Rgba32>(width, height);
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        ms.Position = 0;
        return new UploadFile(fileName, "image/png", ms.Length, new MemoryStream(ms.ToArray()));
    }

    private sealed class FixedTenantContext : ITenantContext
    {
        public FixedTenantContext(string tenantId) => TenantId = tenantId;
        public string TenantId { get; }
    }

    private sealed class NullRealtimeNotifier : ICitizenshipRealtimeNotifier
    {
        public Task NotifyApplicationUpdatedAsync(string tenantId, string email, Guid applicationId, CancellationToken ct = default)
            => Task.CompletedTask;

        public Task NotifyIssuedDocumentCreatedAsync(string tenantId, string email, Guid applicationId, Guid issuedDocumentId, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    private sealed class StubAamvaBarcodeService : IAamvaBarcodeService
    {
        public Task<byte[]?> GenerateDriverLicenseBarcodeAsync(DriverLicenseCard card, CancellationToken cancellationToken = default)
            => Task.FromResult<byte[]?>(null);

        public Task<byte[]?> GenerateIdentificationCardBarcodeAsync(IdentificationCard card, CancellationToken cancellationToken = default)
            => Task.FromResult<byte[]?>(null);

        public Task<byte[]?> GenerateBarcodeFromEncodedDataAsync(string aamvaEncodedData, CancellationToken cancellationToken = default)
            => Task.FromResult<byte[]?>(null);
    }

    private sealed class StubApplicationFormPdfGenerator : IApplicationFormPdfGenerator
    {
        public Task<byte[]> GenerateCit001AAsync(ApplicationFormData data, CancellationToken ct = default)
            => Task.FromResult(Array.Empty<byte>());

        public Task<byte[]> GenerateCit001BAsync(ApplicationFormData data, CancellationToken ct = default)
            => Task.FromResult(Array.Empty<byte>());

        public Task<byte[]> GenerateCit001CAsync(ApplicationFormData data, CancellationToken ct = default)
            => Task.FromResult(Array.Empty<byte>());

        public Task<byte[]> GenerateCit001DAsync(ApplicationFormData data, CancellationToken ct = default)
            => Task.FromResult(Array.Empty<byte>());

        public Task<byte[]> GenerateCit001EAsync(ApplicationFormData data, CancellationToken ct = default)
            => Task.FromResult(Array.Empty<byte>());

        public Task<byte[]> GenerateCit001GAsync(ApplicationFormData data, CancellationToken ct = default)
            => Task.FromResult(Array.Empty<byte>());

        public Task<byte[]> GenerateCit001HAsync(ApplicationFormData data, CancellationToken ct = default)
            => Task.FromResult(Array.Empty<byte>());

        public Task<Dictionary<string, byte[]>> GenerateAllFormsAsync(ApplicationFormData data, CancellationToken ct = default)
            => Task.FromResult(new Dictionary<string, byte[]>());
    }

    private sealed class ThrowingWordService : IWordService
    {
        public Task<string> GenerateWordToPathAsync<T>(T templateModel, string assemblyName, string? outputDirectory = null) where T : IWordTemplateModel
            => throw new NotSupportedException();

        public Task<string> GenerateWordToPathAsync(string resource, string assemblyName, IDictionary<string, object> values, WordDocumentProperties props, Dictionary<string, byte[]>? imageSelectors, string outputFilePath)
            => throw new NotSupportedException();

        public Task<string> GenerateWordFromFileAsync(string templateFilePath, IDictionary<string, object> values, WordDocumentProperties props, Dictionary<string, byte[]>? imageSelectors, string outputFilePath)
            => throw new NotSupportedException();
    }
}
