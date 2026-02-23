using HRM.Core.Models;

namespace HRM.Core.Interfaces;

public interface IHRMService
{
    // Dashboard
    Task<DashboardStats> GetDashboardStatsAsync();

    // Employees
    Task<IEnumerable<Employee>> GetEmployeesAsync();
    Task<Employee?> GetEmployeeAsync(Guid id);
    Task<Employee> CreateEmployeeAsync(Employee e);
    Task<Employee?> UpdateEmployeeAsync(Guid id, Employee e);
    Task<bool> DeleteEmployeeAsync(Guid id);

    // Attendance
    Task<IEnumerable<Attendance>> GetAttendanceAsync(DateTime? date = null, Guid? employeeId = null);
    Task<Attendance> CreateAttendanceAsync(Attendance a);

    // Leave
    Task<IEnumerable<Leave>> GetLeavesAsync(Guid? employeeId = null, string? status = null);
    Task<Leave> CreateLeaveAsync(Leave l);
    Task<Leave?> UpdateLeaveStatusAsync(Guid id, string status);

    // Payroll
    Task<IEnumerable<Payroll>> GetPayrollAsync(string? period = null, Guid? employeeId = null);
    Task<Payroll> CreatePayrollAsync(Payroll p);

    // Loans
    Task<IEnumerable<Loan>> GetLoansAsync(Guid? employeeId = null);
    Task<Loan> CreateLoanAsync(Loan l);

    // Recruitment
    Task<IEnumerable<Recruitment>> GetRecruitmentsAsync(string? status = null);
    Task<Recruitment> CreateRecruitmentAsync(Recruitment r);

    // Awards
    Task<IEnumerable<Award>> GetAwardsAsync(Guid? employeeId = null);
    Task<Award> CreateAwardAsync(Award a);

    // Projects
    Task<IEnumerable<Project>> GetProjectsAsync(string? status = null);
    Task<Project> CreateProjectAsync(Project p);

    // Procurement
    Task<IEnumerable<Procurement>> GetProcurementsAsync(string? status = null);
    Task<Procurement> CreateProcurementAsync(Procurement p);

    // Notifications
    Task<IEnumerable<Notification>> GetNotificationsAsync(Guid? employeeId = null, bool? unreadOnly = null);

    // Reward Points
    Task<IEnumerable<RewardPoints>> GetRewardPointsAsync(Guid employeeId);
    Task<int> GetEmployeeTotalPointsAsync(Guid employeeId);

    // Tax
    Task<IEnumerable<TaxSetup>> GetTaxSetupsAsync();
    Task<TaxSetup> CreateTaxSetupAsync(TaxSetup t);

    // Roles
    Task<IEnumerable<Role>> GetRolesAsync();
}
