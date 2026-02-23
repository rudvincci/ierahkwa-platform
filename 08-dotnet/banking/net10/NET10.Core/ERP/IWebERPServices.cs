using NET10.Core.Architecture;

namespace NET10.Core.ERP;

// ═══════════════════════════════════════════════════════════════════════════════
// BUSINESS LOGIC LAYER - SERVICE INTERFACES
// Three-Tier Architecture - Layer 2 Contracts
// ═══════════════════════════════════════════════════════════════════════════════

// ═══════════════════════════════════════════════════════════════════════════════
// ORGANIZATION SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface IOrganizationService
{
    Task<Result<Organization>> GetByIdAsync(Guid id);
    Task<Result<Organization>> CreateAsync(Organization org);
    Task<Result<Organization>> UpdateAsync(Organization org);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<Organization>> GetByCodeAsync(string code);
}

// ═══════════════════════════════════════════════════════════════════════════════
// USER SERVICE (Identity)
// ═══════════════════════════════════════════════════════════════════════════════
public interface IERPUserService
{
    Task<Result<ERPUser>> GetByIdAsync(Guid id);
    Task<Result<ERPUser>> GetByUsernameAsync(string username);
    Task<Result<ERPUser>> GetByEmailAsync(string email);
    Task<Result<List<ERPUser>>> GetByOrganizationAsync(Guid organizationId);
    
    Task<Result<ERPUser>> CreateAsync(CreateUserRequest request);
    Task<Result<ERPUser>> UpdateAsync(ERPUser user);
    Task<Result<bool>> DeleteAsync(Guid id);
    
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken);
    Task<Result<bool>> LogoutAsync(Guid userId);
    
    Task<Result<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<Result<string>> ForgotPasswordAsync(string email);
    Task<Result<bool>> ResetPasswordAsync(string token, string newPassword);
    
    Task<Result<bool>> Enable2FAAsync(Guid userId);
    Task<Result<bool>> Verify2FAAsync(Guid userId, string code);
}

public class CreateUserRequest
{
    public Guid OrganizationId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? TwoFactorCode { get; set; }
    public bool RememberMe { get; set; } = false;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
    public ERPUser User { get; set; } = new();
    public bool RequiresTwoFactor { get; set; } = false;
}

// ═══════════════════════════════════════════════════════════════════════════════
// HR SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface IHRService
{
    // Departments
    Task<Result<List<Department>>> GetDepartmentsAsync(Guid organizationId);
    Task<Result<Department>> CreateDepartmentAsync(Department department);
    Task<Result<Department>> UpdateDepartmentAsync(Department department);
    Task<Result<bool>> DeleteDepartmentAsync(Guid id);
    
    // Employees
    Task<Result<PagedResult<Employee>>> GetEmployeesAsync(Guid organizationId, PaginationParams pagination);
    Task<Result<Employee>> GetEmployeeByIdAsync(Guid id);
    Task<Result<Employee>> CreateEmployeeAsync(Employee employee);
    Task<Result<Employee>> UpdateEmployeeAsync(Employee employee);
    Task<Result<bool>> TerminateEmployeeAsync(Guid id, DateTime terminationDate, string reason);
    
    Task<Result<string>> GenerateEmployeeNumberAsync(Guid organizationId);
    Task<Result<List<Employee>>> GetEmployeesByDepartmentAsync(Guid departmentId);
    Task<Result<List<Employee>>> GetDirectReportsAsync(Guid managerId);
}

// ═══════════════════════════════════════════════════════════════════════════════
// SALES SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface ISalesService
{
    Task<Result<PagedResult<SalesOrder>>> GetOrdersAsync(Guid organizationId, PaginationParams pagination);
    Task<Result<SalesOrder>> GetOrderByIdAsync(Guid id);
    Task<Result<SalesOrder>> GetOrderByNumberAsync(string orderNumber);
    Task<Result<SalesOrder>> CreateOrderAsync(SalesOrder order);
    Task<Result<SalesOrder>> UpdateOrderAsync(SalesOrder order);
    Task<Result<bool>> CancelOrderAsync(Guid id, string reason);
    
    Task<Result<SalesOrder>> SubmitOrderAsync(Guid id);
    Task<Result<SalesOrder>> ApproveOrderAsync(Guid id, Guid approvedById);
    Task<Result<SalesOrder>> ShipOrderAsync(Guid id, string trackingNumber);
    
    Task<Result<string>> GenerateOrderNumberAsync(Guid organizationId);
    Task<Result<List<SalesOrder>>> GetOrdersByCustomerAsync(Guid customerId);
    Task<Result<SalesDashboard>> GetDashboardAsync(Guid organizationId);
}

public class SalesDashboard
{
    public decimal TodaySales { get; set; }
    public decimal WeekSales { get; set; }
    public decimal MonthSales { get; set; }
    public decimal YearSales { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int ShippedOrders { get; set; }
    public List<TopCustomer> TopCustomers { get; set; } = [];
    public List<TopProduct> TopProducts { get; set; } = [];
}

public class TopCustomer
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalPurchases { get; set; }
    public int OrderCount { get; set; }
}

public class TopProduct
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal TotalSold { get; set; }
    public int QuantitySold { get; set; }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PURCHASING SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface IPurchasingService
{
    // Requisitions
    Task<Result<PagedResult<PurchaseRequisition>>> GetRequisitionsAsync(Guid organizationId, PaginationParams pagination);
    Task<Result<PurchaseRequisition>> GetRequisitionByIdAsync(Guid id);
    Task<Result<PurchaseRequisition>> CreateRequisitionAsync(PurchaseRequisition requisition);
    Task<Result<PurchaseRequisition>> SubmitRequisitionAsync(Guid id);
    Task<Result<PurchaseRequisition>> ApproveRequisitionAsync(Guid id, Guid approvedById);
    Task<Result<PurchaseRequisition>> RejectRequisitionAsync(Guid id, string reason);
    
    Task<Result<string>> GenerateRequisitionNumberAsync(Guid organizationId);
    Task<Result<List<PurchaseRequisition>>> GetPendingApprovalsAsync(Guid organizationId);
}

// ═══════════════════════════════════════════════════════════════════════════════
// MANUFACTURING SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface IManufacturingService
{
    // BOM
    Task<Result<List<BillOfMaterials>>> GetBOMsAsync(Guid organizationId);
    Task<Result<BillOfMaterials>> GetBOMByIdAsync(Guid id);
    Task<Result<BillOfMaterials>> GetBOMByProductAsync(Guid productId);
    Task<Result<BillOfMaterials>> CreateBOMAsync(BillOfMaterials bom);
    Task<Result<BillOfMaterials>> UpdateBOMAsync(BillOfMaterials bom);
    Task<Result<decimal>> CalculateBOMCostAsync(Guid bomId);
    
    // Work Orders
    Task<Result<PagedResult<WorkOrder>>> GetWorkOrdersAsync(Guid organizationId, PaginationParams pagination);
    Task<Result<WorkOrder>> GetWorkOrderByIdAsync(Guid id);
    Task<Result<WorkOrder>> CreateWorkOrderAsync(WorkOrder workOrder);
    Task<Result<WorkOrder>> UpdateWorkOrderAsync(WorkOrder workOrder);
    Task<Result<WorkOrder>> ReleaseWorkOrderAsync(Guid id);
    Task<Result<WorkOrder>> StartWorkOrderAsync(Guid id);
    Task<Result<WorkOrder>> CompleteWorkOrderAsync(Guid id, decimal completedQty, decimal scrapQty);
    
    Task<Result<string>> GenerateWorkOrderNumberAsync(Guid organizationId);
    Task<Result<ManufacturingDashboard>> GetDashboardAsync(Guid organizationId);
}

public class ManufacturingDashboard
{
    public int PlannedOrders { get; set; }
    public int InProgressOrders { get; set; }
    public int CompletedToday { get; set; }
    public decimal TotalOutputToday { get; set; }
    public decimal OverallEfficiency { get; set; }
    public List<WorkOrderSummary> ActiveWorkOrders { get; set; } = [];
}

public class WorkOrderSummary
{
    public string WorkOrderNumber { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal PlannedQty { get; set; }
    public decimal CompletedQty { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime DueDate { get; set; }
}

// ═══════════════════════════════════════════════════════════════════════════════
// QUALITY SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface IQualityService
{
    Task<Result<PagedResult<QualityInspection>>> GetInspectionsAsync(Guid organizationId, PaginationParams pagination);
    Task<Result<QualityInspection>> GetInspectionByIdAsync(Guid id);
    Task<Result<QualityInspection>> CreateInspectionAsync(QualityInspection inspection);
    Task<Result<QualityInspection>> UpdateInspectionAsync(QualityInspection inspection);
    Task<Result<QualityInspection>> CompleteInspectionAsync(Guid id, InspectionResult result);
    
    Task<Result<string>> GenerateInspectionNumberAsync(Guid organizationId);
    Task<Result<QualityDashboard>> GetDashboardAsync(Guid organizationId);
}

public class QualityDashboard
{
    public int TotalInspections { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public decimal OverallPassRate { get; set; }
    public List<DefectSummary> TopDefects { get; set; } = [];
}

public class DefectSummary
{
    public string DefectType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PROJECT SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface IProjectService
{
    Task<Result<PagedResult<Project>>> GetProjectsAsync(Guid organizationId, PaginationParams pagination);
    Task<Result<Project>> GetProjectByIdAsync(Guid id);
    Task<Result<Project>> CreateProjectAsync(Project project);
    Task<Result<Project>> UpdateProjectAsync(Project project);
    Task<Result<bool>> DeleteProjectAsync(Guid id);
    
    // Tasks
    Task<Result<ProjectTask>> CreateTaskAsync(ProjectTask task);
    Task<Result<ProjectTask>> UpdateTaskAsync(ProjectTask task);
    Task<Result<ProjectTask>> CompleteTaskAsync(Guid taskId);
    Task<Result<List<ProjectTask>>> GetTasksByProjectAsync(Guid projectId);
    Task<Result<List<ProjectTask>>> GetTasksByAssigneeAsync(Guid userId);
    
    // Milestones
    Task<Result<ProjectMilestone>> CreateMilestoneAsync(ProjectMilestone milestone);
    Task<Result<ProjectMilestone>> CompleteMilestoneAsync(Guid milestoneId);
    
    Task<Result<string>> GenerateProjectCodeAsync(Guid organizationId);
    Task<Result<ProjectDashboard>> GetDashboardAsync(Guid organizationId);
}

public class ProjectDashboard
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int OverdueProjects { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalActualCost { get; set; }
    public List<ProjectSummary> RecentProjects { get; set; } = [];
}

public class ProjectSummary
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int Progress { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime EndDate { get; set; }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CRM SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface ICRMService
{
    Task<Result<PagedResult<Lead>>> GetLeadsAsync(Guid organizationId, PaginationParams pagination);
    Task<Result<Lead>> GetLeadByIdAsync(Guid id);
    Task<Result<Lead>> CreateLeadAsync(Lead lead);
    Task<Result<Lead>> UpdateLeadAsync(Lead lead);
    Task<Result<bool>> DeleteLeadAsync(Guid id);
    
    Task<Result<Lead>> ConvertLeadToCustomerAsync(Guid leadId);
    Task<Result<Lead>> AddActivityAsync(Guid leadId, LeadActivity activity);
    Task<Result<Lead>> UpdateStatusAsync(Guid leadId, LeadStatus status);
    
    Task<Result<string>> GenerateLeadNumberAsync(Guid organizationId);
    Task<Result<List<Lead>>> GetLeadsByAssigneeAsync(Guid userId);
    Task<Result<CRMDashboard>> GetDashboardAsync(Guid organizationId);
}

public class CRMDashboard
{
    public int TotalLeads { get; set; }
    public int NewLeads { get; set; }
    public int QualifiedLeads { get; set; }
    public int WonLeads { get; set; }
    public int LostLeads { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal PipelineValue { get; set; }
    public List<LeadsBySource> LeadsBySource { get; set; } = [];
    public List<LeadsByStatus> LeadsByStatus { get; set; } = [];
}

public class LeadsBySource
{
    public string Source { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Value { get; set; }
}

public class LeadsByStatus
{
    public LeadStatus Status { get; set; }
    public int Count { get; set; }
    public decimal Value { get; set; }
}
