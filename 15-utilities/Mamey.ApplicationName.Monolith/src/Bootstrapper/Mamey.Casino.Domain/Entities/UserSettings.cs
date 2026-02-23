using System.ComponentModel.DataAnnotations;
using Mamey.Auth.Identity.Abstractions;

namespace Mamey.Casino.Domain.Entities;

public class UserSettings
{
    [Key] public Guid UserId { get; set; }
    public string JsonPayload { get; set; } = "{}";
    public ApplicationUser User { get; set; } = null!;
    public bool IsDarkMode = true;
    
}