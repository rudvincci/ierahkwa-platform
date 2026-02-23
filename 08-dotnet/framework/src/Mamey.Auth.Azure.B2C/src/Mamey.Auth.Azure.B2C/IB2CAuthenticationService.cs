namespace Mamey.Auth.Azure.B2C;

using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IB2CAuthenticationService
{
    Task<string> AcquireTokenAsync(string[] scopes);
    Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<List<User>?> GetUsersAsync();
    Task<bool> VerifyUserActionAsync(string userId, string token);
    Task<string> InitiatePasswordResetAsync(string userId);
}
