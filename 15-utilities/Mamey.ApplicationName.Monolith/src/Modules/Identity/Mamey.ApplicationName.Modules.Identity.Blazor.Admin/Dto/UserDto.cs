namespace Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Dto;

/// <summary>
/// Represents a user in the admin UI.
/// </summary>
public class UserDto
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>Full name (first + middle + last).</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Roles assigned to the user.</summary>
    public IList<string> Roles { get; set; } = new List<string>();

    /// <summary>Date/time when the user registered.</summary>
    public DateTime DateRegistered { get; set; }

    public DateTime? LastLogin { get; set; }
}