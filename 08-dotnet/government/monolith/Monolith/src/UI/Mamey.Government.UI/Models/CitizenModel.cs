namespace Mamey.Government.UI.Models;

/// <summary>
/// Client-side citizen model.
/// </summary>
public class CitizenModel
{
    public Guid Id { get; set; }
    public string? CitizenNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Nationality { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Address
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    
    // Alias for Street that can be used as a single address line
    public string? Address
    {
        get => Street;
        set => Street = value;
    }
    
    public string FormattedAddress => string.Join(", ", 
        new[] { Street, City, State, PostalCode, Country }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
}

/// <summary>
/// Citizen summary for list views.
/// </summary>
public class CitizenSummaryModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request to create a new citizen.
/// </summary>
public class CreateCitizenRequest
{
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? PhotoUrl { get; set; }
    public Guid? ApplicationId { get; set; }
}

/// <summary>
/// Request to suspend a citizen.
/// </summary>
public class SuspendCitizenRequest
{
    public string Reason { get; set; } = string.Empty;
}
