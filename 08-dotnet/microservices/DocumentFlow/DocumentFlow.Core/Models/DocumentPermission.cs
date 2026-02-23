namespace DocumentFlow.Core.Models;

public class DocumentPermission
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? EntityName { get; set; }
    public PermissionType PermissionType { get; set; }
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
    public bool CanDownload { get; set; }
    public bool CanPrint { get; set; }
    public bool CanComment { get; set; }
    public bool CanApprove { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid GrantedBy { get; set; }
    public DateTime GrantedAt { get; set; }

    // Navigation
    public Document? Document { get; set; }
}

public enum PermissionType
{
    User,
    Role,
    Department,
    Public
}
