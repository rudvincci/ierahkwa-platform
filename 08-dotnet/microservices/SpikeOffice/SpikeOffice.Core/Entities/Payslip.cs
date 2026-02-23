namespace SpikeOffice.Core.Entities;

public class Payslip : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public string? ComponentsJson { get; set; } // allowances, taxes, etc.
    public DateTime? PaidAt { get; set; }
    public string? PaymentReference { get; set; }
}
