namespace Mamey.Authentik.Models;

/// <summary>
/// Base response wrapper for Authentik API responses.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public class AuthentikResponse<T>
{
    /// <summary>
    /// Gets or sets the response data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the request was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message, if any.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
