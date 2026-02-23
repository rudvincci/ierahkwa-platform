namespace HRM.Core.Models;

public class Loan
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string LoanType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int Installments { get; set; }
    public decimal MonthlyPayment { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Repaying, Completed
    public int PaidInstallments { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
