using System.ComponentModel.DataAnnotations;

namespace Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

internal class Participant
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number for SMS notifications
    /// </summary>
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// (Optional) Link to ASP.NET Identity user if your system uses identity
    /// </summary>
    public string? AspNetUserId { get; set; }

    /// <summary>
    /// Navigation property for their tickets
    /// </summary>
    public List<Ticket> Tickets { get; set; } = new();
}