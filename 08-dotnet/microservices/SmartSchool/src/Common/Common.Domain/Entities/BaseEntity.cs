namespace Common.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public abstract class TenantEntity : BaseEntity
{
    public int TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
