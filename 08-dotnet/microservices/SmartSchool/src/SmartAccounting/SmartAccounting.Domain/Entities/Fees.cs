using Common.Domain.Entities;

namespace SmartAccounting.Domain.Entities;

public class Fees : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public int SchoolYearId { get; set; }
    public int? GradeId { get; set; }
    public FeeType Type { get; set; } = FeeType.Tuition;
    public bool IsMandatory { get; set; } = true;
    public DateTime? DueDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual SchoolYear? SchoolYear { get; set; }
}

public enum FeeType
{
    Tuition,
    Registration,
    Library,
    Laboratory,
    Sports,
    Transportation,
    Uniform,
    Books,
    Examination,
    Other
}
