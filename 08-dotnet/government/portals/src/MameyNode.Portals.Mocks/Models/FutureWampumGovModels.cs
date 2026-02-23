namespace MameyNode.Portals.Mocks.Models;

// Models based on government.proto and payments.proto for FutureWampumGov
// Note: DisbursementInfo is already defined in PaymentModels.cs

public class FutureWampumGovDisbursementInfo
{
    public string DisbursementId { get; set; } = string.Empty;
    public string ProgramId { get; set; } = string.Empty;
    public string RecipientId { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Purpose { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string BatchId { get; set; } = string.Empty;
}

public class DisbursementBatchInfo
{
    public string BatchId { get; set; } = string.Empty;
    public string ProgramId { get; set; } = string.Empty;
    public int TotalRecipients { get; set; }
    public int ProcessedRecipients { get; set; }
    public string TotalAmount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class UBIProgramInfo
{
    public string ProgramId { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string MonthlyAmount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public int TotalRecipients { get; set; }
    public int ActiveRecipients { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active";
    public string Budget { get; set; } = "0";
    public string Disbursed { get; set; } = "0";
}

public class UBIRecipientInfo
{
    public string RecipientId { get; set; } = string.Empty;
    public string ProgramId { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    public string MonthlyAmount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime? LastDisbursement { get; set; }
    public int TotalDisbursements { get; set; }
}

public class BudgetAllocationInfo
{
    public string AllocationId { get; set; } = string.Empty;
    public string MinistryId { get; set; } = string.Empty;
    public string MinistryName { get; set; } = string.Empty;
    public string AllocatedAmount { get; set; } = "0";
    public string SpentAmount { get; set; } = "0";
    public string RemainingAmount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string FiscalYear { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Active";
}

public class ProgramInfo
{
    public string ProgramId { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string ProgramType { get; set; } = string.Empty;
    public string Budget { get; set; } = "0";
    public string Allocated { get; set; } = "0";
    public string Disbursed { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public int TotalRecipients { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active";
    public string MinistryId { get; set; } = string.Empty;
}

public class TransparencyDashboardData
{
    public string TotalBudget { get; set; } = "0";
    public string TotalDisbursed { get; set; } = "0";
    public string TotalPrograms { get; set; } = "0";
    public string ActivePrograms { get; set; } = "0";
    public int TotalRecipients { get; set; }
    public Dictionary<string, string> MinistryBudgets { get; set; } = new();
    public Dictionary<string, int> ProgramRecipients { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

