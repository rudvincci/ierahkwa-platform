using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to sign in an identity using username and password.
/// </summary>
[Contract]
public record SignIn : ICommand
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    [Required]
    public string Username { get; init; } = null!;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [Required]
    public string Password { get; init; } = null!;

    /// <summary>
    /// Gets or sets the IP address of the client.
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// Gets or sets the user agent of the client.
    /// </summary>
    public string? UserAgent { get; init; }
}

