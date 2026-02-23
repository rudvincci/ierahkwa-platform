using System.ComponentModel.DataAnnotations;

namespace Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

internal class RaffleCategory
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation property if you want to see all raffles under this category
    public List<Raffle>? Raffles { get; set; }
}