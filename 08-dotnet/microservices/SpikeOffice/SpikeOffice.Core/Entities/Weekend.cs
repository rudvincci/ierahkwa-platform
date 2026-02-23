namespace SpikeOffice.Core.Entities;

/// <summary>DayOfWeek: 0=Sunday, 6=Saturday</summary>
public class Weekend : BaseEntity
{
    public int DayOfWeek { get; set; }
    public bool IsHalfDay { get; set; }
}
