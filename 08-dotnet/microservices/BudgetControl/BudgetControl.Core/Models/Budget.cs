namespace BudgetControl.Core.Models;

public class Budget
{
    public Guid Id { get; set; }
    public string BudgetCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public BudgetType Type { get; set; }
    public int FiscalYear { get; set; }
    public string? Department { get; set; }
    public string? CostCenter { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal CommittedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal AvailableAmount => AllocatedAmount - CommittedAmount - SpentAmount;
    public decimal UtilizationPercent => AllocatedAmount > 0 ? ((CommittedAmount + SpentAmount) / AllocatedAmount) * 100 : 0;
    public string Currency { get; set; } = "USD";
    public BudgetStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? ParentBudgetId { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<BudgetLine> Lines { get; set; } = new();
    public List<BudgetTransaction> Transactions { get; set; } = new();
}

public class BudgetLine
{
    public Guid Id { get; set; }
    public Guid BudgetId { get; set; }
    public string LineCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal CommittedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal AvailableAmount => AllocatedAmount - CommittedAmount - SpentAmount;
    public string? Notes { get; set; }
}

public class BudgetTransaction
{
    public Guid Id { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public Guid BudgetId { get; set; }
    public Guid? BudgetLineId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BudgetTransfer
{
    public Guid Id { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public Guid FromBudgetId { get; set; }
    public string FromBudgetName { get; set; } = string.Empty;
    public Guid ToBudgetId { get; set; }
    public string ToBudgetName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public TransferStatus Status { get; set; }
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class BudgetForecast
{
    public Guid Id { get; set; }
    public Guid BudgetId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal ForecastedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal Variance => ActualAmount - ForecastedAmount;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum BudgetType { Operating, Capital, Project, Contingency, Revenue, Expense }
public enum BudgetStatus { Draft, Pending, Approved, Active, Frozen, Closed }
public enum TransactionType { Allocation, Commitment, Expense, Release, Adjustment, Transfer }
public enum TransferStatus { Pending, Approved, Rejected, Completed }
