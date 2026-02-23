namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

internal interface IJwtTokenService
{
    Task<string> GenerateApplicantTokenAsync(string email, Guid applicationId, CancellationToken cancellationToken = default);
}
