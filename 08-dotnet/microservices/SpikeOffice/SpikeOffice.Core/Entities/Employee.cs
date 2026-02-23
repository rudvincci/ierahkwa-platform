namespace SpikeOffice.Core.Entities;

public class Employee : BaseEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? JoinDate { get; set; }
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public Guid? DesignationId { get; set; }
    public Designation? Designation { get; set; }
    public decimal? BasicSalary { get; set; }
    public string? BankAccount { get; set; }
    public string? Address { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? EmployeePortalPasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public bool CanClockInFromAnyIp { get; set; } = false;

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<ClockInOut> ClockInOuts { get; set; } = new List<ClockInOut>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
    public ICollection<EmployeeLoan> Loans { get; set; } = new List<EmployeeLoan>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<ExpenseClaim> ExpenseClaims { get; set; } = new List<ExpenseClaim>();
}
