namespace Mamey.Graph.Services;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync();
    Task<string> GetRefreshTokenAsync();
    Task<string> RefreshAccessTokenAsync(string refreshToken);
    bool NeedsRefresh(string refreshToken);
}
