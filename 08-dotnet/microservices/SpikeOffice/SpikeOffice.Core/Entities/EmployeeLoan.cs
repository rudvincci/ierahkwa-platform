namespace SpikeOffice.Core.Entities;

public class EmployeeLoan : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int Installments { get; set; }
    public decimal MonthlyDeduction { get; set; }
    public DateTime StartDate { get; set; }
    public string? Status { get; set; } // Active, Completed, Defaulted
    public decimal? TotalRepaid { get; set; }
}
