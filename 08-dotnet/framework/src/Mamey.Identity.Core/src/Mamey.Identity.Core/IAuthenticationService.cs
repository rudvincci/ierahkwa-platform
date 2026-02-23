using System.Security.Claims;

namespace Mamey.Identity.Core
{
    /// <summary>
    /// Interface for authentication service to be implemented by different authentication providers.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticate a user with the given credentials.
        /// </summary>
        /// <param name="credentials">The user credentials.</param>
        /// <returns>ClaimsPrincipal representing the authenticated user.</returns>
        ClaimsPrincipal Authenticate(string credentials);

        /// <summary>
        /// Authorize a user for the specified resource.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <param name="resource">The resource to authorize for.</param>
        /// <returns>True if authorized, false otherwise.</returns>
        bool Authorize(ClaimsPrincipal user, string resource);

        /// <summary>
        /// Initialize the authentication service asynchronously.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Event triggered when the authenticated user changes.
        /// </summary>
        event Action<AuthenticatedUser?> AuthenticatedUserChanged;

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        AuthenticatedUser? AuthenticatedUser { get; }

        /// <summary>
        /// Indicates whether the user is currently authenticated.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Sign in a user with the given username and password.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A task representing the sign-in process.</returns>
        Task SignIn(string username, string password);

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        void Logout();
    }
}
