using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Mamey.Identity.Azure.Graph;

/// <summary>
/// Microsoft Graph service interface.
/// </summary>
public interface IGraphService
{
    /// <summary>
    /// Gets the Microsoft Graph client.
    /// </summary>
    GraphServiceClient Client { get; }

    /// <summary>
    /// Gets user information by ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>User information.</returns>
    Task<User?> GetUserAsync(string userId);

    /// <summary>
    /// Gets user information by user principal name.
    /// </summary>
    /// <param name="userPrincipalName">The user principal name.</param>
    /// <returns>User information.</returns>
    Task<User?> GetUserByPrincipalNameAsync(string userPrincipalName);

    /// <summary>
    /// Gets user groups.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>User groups.</returns>
    Task<IEnumerable<Group>> GetUserGroupsAsync(string userId);

    /// <summary>
    /// Gets all users in the tenant.
    /// </summary>
    /// <param name="top">Number of users to return.</param>
    /// <param name="skip">Number of users to skip.</param>
    /// <returns>List of users.</returns>
    Task<IEnumerable<User>> GetUsersAsync(int top = 100, int skip = 0);

    /// <summary>
    /// Gets all groups in the tenant.
    /// </summary>
    /// <param name="top">Number of groups to return.</param>
    /// <param name="skip">Number of groups to skip.</param>
    /// <returns>List of groups.</returns>
    Task<IEnumerable<Group>> GetGroupsAsync(int top = 100, int skip = 0);

    /// <summary>
    /// Searches for users.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <returns>Matching users.</returns>
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);

    /// <summary>
    /// Searches for groups.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <returns>Matching groups.</returns>
    Task<IEnumerable<Group>> SearchGroupsAsync(string searchTerm);
}
