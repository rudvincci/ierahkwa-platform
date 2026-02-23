namespace Mamey.ApplicationName.BlazorWasm.Models;

/// <summary>
/// Represents the response from the authentication API containing access and refresh tokens.
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// The access token used for authenticated requests.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// The refresh token used to obtain a new access token when the current one expires.
    /// </summary>
    public string? RefreshToken { get; set; }
}