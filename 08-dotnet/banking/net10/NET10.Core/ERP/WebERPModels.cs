using NET10.Core.Architecture;

namespace NET10.Core.ERP;

// ═══════════════════════════════════════════════════════════════════════════════
// WEB ERP - BUSINESS ENTITIES (LAYER 2 - BUSINESS LOGIC)
// Three-Tier Architecture Implementation
// ═══════════════════════════════════════════════════════════════════════════════

// ═══════════════════════════════════════════════════════════════════════════════
// ORGANIZATION / COMPANY
// ═══════════════════════════════════════════════════════════════════════════════
public class Organization : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    
    // Address
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    
    // Contact
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    
    // Settings
    public string Currency { get; set; } = "USD";
    public string Timezone { get; set; } = "UTC";
    public string DateFormat { get; set; } = "yyyy-MM-dd";
    public int FiscalYearStartMonth { get; set; } = 1;
    
    // Branding
    public string Logo { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = "#FFD700";
    
    public bool IsActive { get; set; } = true;
}

// ═══════════════════════════════════════════════════════════════════════════════
// USER & IDENTITY (ASP.NET Identity Integration)
// ═══════════════════════════════════════════════════════════════════════════════
public class ERPUser : AuditableEntity
{
    public Guid OrganizationId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    
    // Profile
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Phone { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    
    // Security
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; } = false;
    public string? TwoFactorSecret { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    public bool MustChangePassword { get; set; } = false;
    
    // Tokens
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetExpiry { get; set; }
}

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; } = false;
    public List<Permission> Permissions { get; set; } = [];
}

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public bool Allowed { get; set; } = true;
}

// ═══════════════════════════════════════════════════════════════════════════════
// HUMAN RESOURCES
// ═══════════════════════════════════════════════════════════════════════════════
public class Department : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public Guid? ParentDepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Employee : AuditableEntity
{
    public Guid OrganizationId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    
    // Personal
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    
    // Address
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    
    // Employment
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    
    // Compensation
    public decimal Salary { get; set; }
    public string SalaryType { get; set; } = "Monthly"; // Hourly, Monthly, Annual
    public string Currency { get; set; } = "USD";
    public string BankName { get; set; } = string.Empty;
    public string BankAccount { get; set; } = string.Empty;
    
    // Documents
    public string TaxId { get; set; } = string.Empty;
    public string SocialSecurityNumber { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
}

public enum EmploymentStatus
{
    Active,
    OnLeave,
    Suspended,
    Terminated,
    Retired
}

public enum EmploymentType
{
    FullTime,
    PartTime,
    Contract,
    Intern,
    Temporary
}

// ═══════════════════════════════════════════════════════════════════════════════
// SALES
// ═══════════════════════════════════════════════════════════════════════════════
public class SalesOrder : AuditableEntity
{
    public Guid OrganizationId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    
    // Customer
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    
    // Dates
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    
    // Shipping
    public string ShipToAddress { get; set; } = string.Empty;
    public string ShipToCity { get; set; } = string.Empty;
    public string ShipToState { get; set; } = string.Empty;
    public string ShipToCountry { get; set; } = string.Empty;
    public string ShipToPostalCode { get; set; } = string.Empty;
    public string ShippingMethod { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
    
    // Items
    public List<SalesOrderItem> Items { get; set; } = [];
    
    // Totals
    public decimal Subtotal => Items.Sum(i => i.LineTotal);
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount => Subtotal * (DiscountPercent / 100);
    public decimal TaxRate { get; set; } = 16;
    public decimal TaxAmount => (Subtotal - DiscountAmount) * (TaxRate / 100);
    public decimal ShippingCost { get; set; }
    public decimal Total => Subtotal - DiscountAmount + TaxAmount + ShippingCost;
    public string Currency { get; set; } = "USD";
    
    // Status
    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;
    public string Notes { get; set; } = string.Empty;
    
    // Payment
    public PaymentTerms PaymentTerms { get; set; } = PaymentTerms.Net30;
    public decimal AmountPaid { get; set; }
    public decimal Balance => Total - AmountPaid;
}

public class SalesOrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SalesOrderId { get; set; }
    public int LineNumber { get; set; }
    
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "pcs";
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount => UnitPrice * Quantity * (DiscountPercent / 100);
    public decimal LineTotal => (UnitPrice * Quantity) - DiscountAmount;
    
    public decimal QuantityShipped { get; set; }
    public decimal QuantityInvoiced { get; set; }
}

public enum SalesOrderStatus
{
    Draft,
    Submitted,
    Approved,
    Processing,
    PartialShipped,
    Shipped,
    Delivered,
    Completed,
    Cancelled
}

public enum PaymentTerms
{
    COD,
    Net15,
    Net30,
    Net45,
    Net60,
    Net90,
    Prepaid
}

// ═══════════════════════════════════════════════════════════════════════════════
// PURCHASING
// ═══════════════════════════════════════════════════════════════════════════════
public class PurchaseRequisition : AuditableEntity
{
    public Guid OrganizationId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    
    public Guid RequestedById { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime? RequiredDate { get; set; }
    public string Purpose { get; set; } = string.Empty;
    
    public List<PurchaseRequisitionItem> Items { get; set; } = [];
    public decimal TotalAmount => Items.Sum(i => i.EstimatedTotal);
    
    public RequisitionStatus Status { get; set; } = RequisitionStatus.Draft;
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class PurchaseRequisitionItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RequisitionId { get; set; }
    public int LineNumber { get; set; }
    
    public Guid? ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "pcs";
    public decimal EstimatedUnitPrice { get; set; }
    public decimal EstimatedTotal => Quantity * EstimatedUnitPrice;
    
    public Guid? PreferredSupplierId { get; set; }
    public string? PreferredSupplierName { get; set; }
}

public enum RequisitionStatus
{
    Draft,
    Submitted,
    UnderReview,
    Approved,
    Rejected,
    Converted,
    Cancelled
}

// ═══════════════════════════════════════════════════════════════════════════════
// MANUFACTURING
// ═══════════════════════════════════════════════════════════════════════════════
public class BillOfMaterials : AuditableEntity
{
    public Guid OrganizationId { get; set; }
    public string BOMNumber { get; set; } = string.Empty;
    
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    
    public string Version { get; set; } = "1.0";
    public decimal OutputQuantity { get; set; } = 1;
    public string OutputUnit { get; set; } = "pcs";
    
    public List<BOMComponent> Components { get; set; } = [];
    public decimal TotalMaterialCost => Components.Sum(c => c.TotalCost);
    public decimal LaborCost { get; set; }
    public decimal OverheadCost { get; set; }
    public decimal TotalCost => TotalMaterialCost + LaborCost + OverheadCost;
    
    public bool IsActive { get; set; } = true;
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class BOMComponent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BOMId { get; set; }
    public int Sequence { get; set; }
    
    public Guid ComponentId { get; set; }
    public string ComponentCode { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "pcs";
    public decimal UnitCost { get; set; }
    public decimal TotalCost => Quantity * UnitCost;
    public decimal WastePercent { get; set; } = 0;
    
    public bool IsOptional { get; set; } = false;
    public string Notes { get; set; } = string.Empty;
}

public class WorkOrder : AuditableEntity
{
    public Guid OrganizationId { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
    
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public Guid BOMId { get; set; }
    
    public decimal PlannedQuantity { get; set; }
    public decimal CompletedQuantity { get; set; }
    public decimal ScrapQuantity { get; set; }
    
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    public Guid? SalesOrderId { get; set; }
    public string? SalesOrderNumber { get; set; }
    
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Planned;
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Normal;
    public string Notes { get; set; } = string.Empty;
}

public enum WorkOrderStatus
{
    Planned,
    Released,
    InProgress,
    OnHold,
    Completed,
    Cancelled
}

public enum WorkOrderPriority
{
    Low,
    Normal,
    High,
    Urgent
}

// ═══════════════════════════════════════════════════════════════════════════════
// QUALITY CONTROL
// ═══════════════════════════════════════════════════════════════════════════════
public class QualityInspection : AuditableEntity
{
    public Guid OrganizationId { get; set; }
    public string InspectionNumber { get; set; } = string.Empty;
    
    public InspectionType Type { get; set; }
    public Guid? WorkOrderId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public Guid? ProductId { get; set; }
    
    public decimal QuantityInspected { get; set; }
    public decimal QuantityPassed { get; set; }
    public decimal QuantityFailed { get; set; }
    public decimal PassRate => QuantityInspected > 0 ? (QuantityPassed / QuantityInspected) * 100 : 0;
    
    public Guid InspectedById { get; set; }
    public string InspectedByName { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; } = DateTime.UtcNow;
    
    public List<InspectionCheckpoint> Checkpoints { get; set; } = [];
    
    public InspectionResult Result { get; set; } = InspectionResult.Pending;
    public string Notes { get; set; } = string.Empty;
    public List<string> Defects { get; set; } = [];
}

public class InspectionCheckpoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ExpectedValue { get; set; } = string.Empty;
    public string ActualValue { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public enum InspectionType
{
    Incoming,
    InProcess,
    Final,
    Random
}

public enum InspectionResult
{
    Pending,
    Passed,
    Failed,
    ConditionalPass
}

// ═══════════════════════════════════════════════════════════════════════════════
// PROJECT MANAGEMENT
// ═══════════════════════════════════════════════════════════════════════════════
public class Project : AuditableEntity
{
    public Guid OrganizationId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    
    public Guid ProjectManagerId { get; set; }
    public string ProjectManagerName { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public string Currency { get; set; } = "USD";
    
    public int Progress { get; set; } = 0; // 0-100
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;
    
    public List<ProjectTask> Tasks { get; set; } = [];
    public List<ProjectMilestone> Milestones { get; set; } = [];
    public List<Guid> TeamMemberIds { get; set; } = [];
}

public class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    
    public int Progress { get; set; } = 0;
    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    
    public Guid? ParentTaskId { get; set; }
    public List<Guid> DependsOnTaskIds { get; set; } = [];
}

public class ProjectMilestone
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public bool IsCompleted { get; set; } = false;
}

public enum ProjectStatus
{
    Planning,
    InProgress,
    OnHold,
    Completed,
    Cancelled
}

public enum ProjectPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum TaskStatus
{
    NotStarted,
    InProgress,
    OnHold,
    Completed,
    Cancelled
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Urgent
}

// ═══════════════════════════════════════════════════════════════════════════════
// CRM
// ═══════════════════════════════════════════════════════════════════════════════
public class Lead : AuditableEntity
{
    public Guid OrganizationId { get; set; }
    public string LeadNumber { get; set; } = string.Empty;
    
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    
    public string Source { get; set; } = string.Empty; // Website, Referral, Cold Call, etc.
    public string Industry { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public decimal AnnualRevenue { get; set; }
    
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public LeadRating Rating { get; set; } = LeadRating.Warm;
    public decimal EstimatedValue { get; set; }
    
    public DateTime? LastContactDate { get; set; }
    public DateTime? NextFollowUpDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    
    public List<LeadActivity> Activities { get; set; } = [];
}

public class LeadActivity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LeadId { get; set; }
    public string Type { get; set; } = string.Empty; // Call, Email, Meeting, Note
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string PerformedBy { get; set; } = string.Empty;
}

public enum LeadStatus
{
    New,
    Contacted,
    Qualified,
    Proposal,
    Negotiation,
    Won,
    Lost
}

public enum LeadRating
{
    Cold,
    Warm,
    Hot
}
