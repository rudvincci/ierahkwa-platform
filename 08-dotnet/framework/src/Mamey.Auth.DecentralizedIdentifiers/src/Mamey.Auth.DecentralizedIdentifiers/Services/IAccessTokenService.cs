using System.Threading.Tasks;
using Mamey.Auth.DecentralizedIdentifiers.Handlers;

namespace Mamey.Auth.DecentralizedIdentifiers.Services;

/// <summary>
/// Interface for DID access token service
/// </summary>
public interface IAccessTokenService
{
    /// <summary>
    /// Store access token
    /// </summary>
    /// <param name="token">Token to store</param>
    /// <returns>Task</returns>
    Task StoreTokenAsync(DidToken token);
    
    /// <summary>
    /// Get access token by ID
    /// </summary>
    /// <param name="tokenId">Token ID</param>
    /// <returns>Token if found</returns>
    Task<DidToken> GetTokenAsync(string tokenId);
    
    /// <summary>
    /// Revoke access token
    /// </summary>
    /// <param name="tokenId">Token ID</param>
    /// <returns>Task</returns>
    Task RevokeTokenAsync(string tokenId);
    
    /// <summary>
    /// Check if token is valid
    /// </summary>
    /// <param name="tokenId">Token ID</param>
    /// <returns>True if valid</returns>
    Task<bool> IsTokenValidAsync(string tokenId);
}





