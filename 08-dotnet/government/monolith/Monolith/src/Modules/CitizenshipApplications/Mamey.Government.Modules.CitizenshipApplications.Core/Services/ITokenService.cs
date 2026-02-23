namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

internal interface ITokenService
{
    Task<string> GenerateTokenAsync(CancellationToken cancellationToken = default);
    string HashToken(string token);
    bool ValidateToken(string token, string hash);
}
