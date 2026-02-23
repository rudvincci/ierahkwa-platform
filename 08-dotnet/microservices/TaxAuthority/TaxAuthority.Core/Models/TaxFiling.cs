namespace TaxAuthority.Core.Models;

public class TaxFiling
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CitizenId { get; set; } = string.Empty;
    public string CitizenName { get; set; } = string.Empty;
    public string TaxYear { get; set; } = DateTime.Now.Year.ToString();
    public string FilingType { get; set; } = "Annual"; // Annual, Quarterly, Property, Business
    public string Period { get; set; } = string.Empty; // Q1, Q2, Q3, Q4, or Full Year
    
    // Income
    public decimal GrossIncome { get; set; }
    public decimal EmploymentIncome { get; set; }
    public decimal BusinessIncome { get; set; }
    public decimal InvestmentIncome { get; set; }
    public decimal RentalIncome { get; set; }
    public decimal OtherIncome { get; set; }
    
    // Deductions
    public decimal TotalDeductions { get; set; }
    public decimal StandardDeduction { get; set; }
    public decimal ItemizedDeductions { get; set; }
    
    // Tax Calculation
    public decimal TaxableIncome { get; set; }
    public decimal TaxRate { get; set; } = 0.15m; // 15% default
    public decimal TaxOwed { get; set; }
    public decimal TaxPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public decimal Refund { get; set; }
    
    // Status
    public FilingStatus Status { get; set; } = FilingStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime DueDate { get; set; }
    
    // Compliance
    public int ComplianceScore { get; set; } = 100;
    public List<string> Documents { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}

public enum FilingStatus
{
    Draft,
    Pending,
    UnderReview,
    Approved,
    Rejected,
    Amended,
    Overdue
}

public class TaxStats
{
    public decimal TotalCollected { get; set; }
    public int TotalFilings { get; set; }
    public int PendingFilings { get; set; }
    public int ApprovedFilings { get; set; }
    public decimal ComplianceRate { get; set; }
    public DateTime? NextDeadline { get; set; }
}

public class TaxPayment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FilingId { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // ACH, Credit Card, Wire
    public string Reference { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}

public enum PaymentStatus
{
    Pending,
    Processing,
    Confirmed,
    Failed,
    Refunded
}
