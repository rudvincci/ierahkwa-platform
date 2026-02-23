using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

public interface IPublicDocumentValidationService
{
    Task<PublicDocumentValidationResult> ValidateAsync(string tenantId, string documentNumber, CancellationToken ct = default);
}




