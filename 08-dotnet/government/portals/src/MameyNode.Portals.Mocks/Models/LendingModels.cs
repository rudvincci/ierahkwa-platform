namespace MameyNode.Portals.Mocks.Models;

// Models based on lending.proto
public enum LoanStatus
{
    Unspecified = 0,
    Pending = 1,
    Approved = 2,
    Active = 3,
    Completed = 4,
    Defaulted = 5,
    Forgiven = 6
}

public enum MicroloanStatus
{
    Unspecified = 0,
    Pending = 1,
    Active = 2,
    Completed = 3,
    Defaulted = 4
}

public enum RepaymentStatus
{
    Unspecified = 0,
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4
}

public class LoanInfo
{
    public string LoanId { get; set; } = string.Empty;
    public string BorrowerId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string InterestRate { get; set; } = string.Empty;
    public int TermMonths { get; set; }
    public string RemainingBalance { get; set; } = string.Empty;
    public string NextPaymentAmount { get; set; } = string.Empty;
    public DateTime NextPaymentDate { get; set; }
    public LoanStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class MicroloanInfo
{
    public string LoanId { get; set; } = string.Empty;
    public string BorrowerId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string RemainingBalance { get; set; } = string.Empty;
    public MicroloanStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DueDate { get; set; }
}

public class RepaymentInfo
{
    public string RepaymentId { get; set; } = string.Empty;
    public string LoanId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public DateTime PaymentDate { get; set; }
    public RepaymentStatus Status { get; set; }
}

public class CollateralInfo
{
    public string CollateralId { get; set; } = string.Empty;
    public string CollateralType { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string Description { get; set; } = string.Empty;
}

