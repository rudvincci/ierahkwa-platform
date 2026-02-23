namespace SpikeOffice.Core.Entities;

public class Notice : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public DateTime? PublishFrom { get; set; }
    public DateTime? PublishTo { get; set; }
    public bool IsPinned { get; set; }
}
