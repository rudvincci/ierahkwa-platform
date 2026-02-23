namespace Mamey.Blockchain.Lending;

public class CreateLoanRequest
{
    public string BorrowerId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string InterestRate { get; set; } = string.Empty;
    public uint TermMonths { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public List<CollateralInfo> Collateral { get; set; } = new();
}

public class CreateLoanResult
{
    public string LoanId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class LoanInfo
{
    public string LoanId { get; set; } = string.Empty;
    public string BorrowerId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string InterestRate { get; set; } = string.Empty;
    public uint TermMonths { get; set; }
    public string RemainingBalance { get; set; } = string.Empty;
    public string NextPaymentAmount { get; set; } = string.Empty;
    public DateTime NextPaymentDate { get; set; }
    public LoanStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class CollateralInfo
{
    public string CollateralId { get; set; } = string.Empty;
    public string CollateralType { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public enum LoanStatus
{
    Pending = 0,
    Approved = 1,
    Active = 2,
    Completed = 3,
    Defaulted = 4,
    Forgiven = 5
}

public class CreditRiskResult
{
    public string RiskLevel { get; set; } = string.Empty;
    public double RiskScore { get; set; }
    public List<string> RiskFactors { get; set; } = new();
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}




