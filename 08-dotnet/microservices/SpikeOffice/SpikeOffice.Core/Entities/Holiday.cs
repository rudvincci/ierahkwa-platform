namespace SpikeOffice.Core.Entities;

public class Holiday : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int? Year { get; set; } // null = recurring yearly
    public bool IsOptional { get; set; }
}
