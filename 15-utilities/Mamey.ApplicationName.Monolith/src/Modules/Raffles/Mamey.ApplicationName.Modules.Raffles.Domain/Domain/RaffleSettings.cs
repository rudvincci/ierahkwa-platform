using System.ComponentModel.DataAnnotations;

namespace Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

internal class RaffleSettings
{
    [Key] public int Id { get; set; }

    public int RaffleId { get; set; }
    public Raffle Raffle { get; set; } = null!;

    /// <summary>
    /// Key name for the setting. E.g. "splitRatio", "numTiers", "jackpotRolloverPercent"
    /// </summary>
    [MaxLength(100)]
    public string SettingKey { get; set; } = string.Empty;

    /// <summary>
    /// Setting value in string form (could be numeric, boolean, JSON, etc.)
    /// </summary>
    public string? SettingValue { get; set; }
}