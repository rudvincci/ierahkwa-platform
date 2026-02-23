namespace SpikeOffice.Core.Entities;

/// <summary>
/// Dynamic role for RBAC. Permissions stored as JSON or separate table.
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PermissionsJson { get; set; } // e.g. ["employee.create","payroll.view"]
    public bool IsSystem { get; set; }
}
