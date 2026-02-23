namespace HRM.Core.Models;

public class Payroll
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Period { get; set; } = string.Empty; // e.g. "2026-01"
    public DateTime PayDate { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal TaxDeduction { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Processed, Paid
    public string? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
}
