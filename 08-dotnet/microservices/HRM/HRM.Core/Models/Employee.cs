namespace HRM.Core.Models;

public class Employee
{
    public Guid Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public DateTime JoinDate { get; set; }
    public string? SalaryGrade { get; set; }
    public decimal BaseSalary { get; set; }
    public bool IsActive { get; set; } = true;
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
