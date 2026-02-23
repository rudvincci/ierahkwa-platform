using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class InMemoryCitizenPortalService : ICitizenPortalService
{
    private readonly ITenantContext _tenant;

    public InMemoryCitizenPortalService(ITenantContext tenant)
    {
        _tenant = tenant;
    }

    public Task<CitizenProfileDto?> GetProfileAsync(CancellationToken ct = default)
        => Task.FromResult<CitizenProfileDto?>(
            new(
                CitizenId: $"CIT-{_tenant.TenantId.ToUpperInvariant()}-00000001",
                FullName: $"Demo Citizen ({_tenant.TenantId})",
                DateOfBirth: new DateOnly(1990, 1, 1),
                Email: "demo.citizen@example.com"));

    public Task<IReadOnlyList<CitizenApplicationDto>> GetApplicationsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<CitizenApplicationDto>>(
            new[]
            {
                new CitizenApplicationDto("APP-00000001", "Submitted", DateTimeOffset.UtcNow.AddDays(-5)),
                new CitizenApplicationDto("APP-00000002", "Under Review", DateTimeOffset.UtcNow.AddDays(-2)),
            });

    public Task<IReadOnlyList<CitizenDocumentDto>> GetDocumentsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<CitizenDocumentDto>>(
            new[]
            {
                new CitizenDocumentDto(Guid.NewGuid(), "PASS-00000001", "Passport", "Standard", DateTimeOffset.UtcNow.AddDays(-30), DateTimeOffset.UtcNow.AddYears(5)),
                new CitizenDocumentDto(Guid.NewGuid(), "ID-00000001", "Identification Card", "Medicinal Cannabis", DateTimeOffset.UtcNow.AddDays(-10), DateTimeOffset.UtcNow.AddYears(2)),
                new CitizenDocumentDto(Guid.NewGuid(), "TAG-00000001", "Vehicle Tag", "Standard", DateTimeOffset.UtcNow.AddDays(-3), DateTimeOffset.UtcNow.AddYears(1)),
            });

    public Task<IReadOnlyList<CitizenUploadDto>> GetUploadsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<CitizenUploadDto>>(
            new[]
            {
                new CitizenUploadDto(Guid.NewGuid(), "PersonalDocument", "identity.pdf", "application/pdf", 123_456, DateTimeOffset.UtcNow.AddDays(-5)),
                new CitizenUploadDto(Guid.NewGuid(), "PassportPhoto", "passport.png", "image/png", 45_678, DateTimeOffset.UtcNow.AddDays(-5)),
                new CitizenUploadDto(Guid.NewGuid(), "SignatureImage", "signature.png", "image/png", 12_345, DateTimeOffset.UtcNow.AddDays(-5)),
            });
}


