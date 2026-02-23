using System.ComponentModel.DataAnnotations;

namespace Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

internal class AuditLog
{
    [Key] public int Id { get; set; }

    /// <summary>
    /// Timestamp of the logged event
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// A textual message describing the event (e.g., "User #12 purchased 5 tickets for Raffle #3").
    /// </summary>
    [Required]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The user or system actor who triggered this event.
    /// Could be a user ID for admins or participants.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// (Optional) Additional JSON or structured data about the event for deeper logging.
    /// </summary>
    public string? Details { get; set; }
}