namespace AssetTracker.Core.Models;

public class Asset
{
    public Guid Id { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AssetCategory Category { get; set; }
    public AssetStatus Status { get; set; }
    public string? SerialNumber { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }
    public string? Building { get; set; }
    public string? Room { get; set; }
    public Guid? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? AssignedDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public DateTime? WarrantyExpiry { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal DepreciationRate { get; set; }
    public DepreciationMethod DepreciationMethod { get; set; }
    public int UsefulLifeMonths { get; set; }
    public decimal SalvageValue { get; set; }
    public string? QrCode { get; set; }
    public string? RfidTag { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DisposedAt { get; set; }
    public string? DisposalReason { get; set; }

    public List<AssetMaintenance> MaintenanceHistory { get; set; } = new();
    public List<AssetAssignment> AssignmentHistory { get; set; } = new();
    public List<AssetDocument> Documents { get; set; } = new();
}

public class AssetMaintenance
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public MaintenanceType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MaintenanceStatus Status { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal? Cost { get; set; }
    public string? Vendor { get; set; }
    public Guid? PerformedBy { get; set; }
    public string? PerformedByName { get; set; }
    public string? Notes { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
}

public class AssetAssignment
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Location { get; set; }
    public DateTime AssignedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public string? Condition { get; set; }
    public string? Notes { get; set; }
    public Guid AssignedBy { get; set; }
}

public class AssetDocument
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class AssetAudit
{
    public Guid Id { get; set; }
    public string AuditNumber { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Location { get; set; }
    public AuditStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid ConductedBy { get; set; }
    public string ConductedByName { get; set; } = string.Empty;
    public int TotalAssets { get; set; }
    public int FoundAssets { get; set; }
    public int MissingAssets { get; set; }
    public int DiscrepancyAssets { get; set; }
    public string? Notes { get; set; }
}

public enum AssetCategory { IT, Furniture, Vehicle, Equipment, Building, Land, Software, Other }
public enum AssetStatus { Available, InUse, UnderMaintenance, Retired, Disposed, Lost, Damaged }
public enum MaintenanceType { Preventive, Corrective, Emergency, Inspection, Upgrade }
public enum MaintenanceStatus { Scheduled, InProgress, Completed, Cancelled, Overdue }
public enum DepreciationMethod { StraightLine, DecliningBalance, SumOfYears, UnitsOfProduction }
public enum AuditStatus { Planned, InProgress, Completed, Cancelled }
