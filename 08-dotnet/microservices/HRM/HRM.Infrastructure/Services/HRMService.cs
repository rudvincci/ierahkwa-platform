using HRM.Core.Interfaces;
using HRM.Core.Models;

namespace HRM.Infrastructure.Services;

public class HRMService : IHRMService
{
    private readonly List<Employee> _employees = new();
    private readonly List<Attendance> _attendance = new();
    private readonly List<Leave> _leaves = new();
    private readonly List<Payroll> _payrolls = new();
    private readonly List<Loan> _loans = new();
    private readonly List<Recruitment> _recruitments = new();
    private readonly List<Award> _awards = new();
    private readonly List<Project> _projects = new();
    private readonly List<Procurement> _procurements = new();
    private readonly List<Notification> _notifications = new();
    private readonly List<RewardPoints> _rewardPoints = new();
    private readonly List<TaxSetup> _taxSetups = new();
    private readonly List<Role> _roles = new();
    private int _employeeCounter = 1;

    public HRMService()
    {
        SeedData();
    }

    private void SeedData()
    {
        var emp1 = new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeCode = "EMP001",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@ierahkwa.gov",
            Department = "Finance",
            Designation = "Senior Analyst",
            JoinDate = new DateTime(2024, 1, 15),
            BaseSalary = 5500,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _employees.Add(emp1);

        _employees.Add(new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeCode = "EMP002",
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@ierahkwa.gov",
            Department = "HR",
            Designation = "HR Manager",
            JoinDate = new DateTime(2023, 6, 1),
            BaseSalary = 6200,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        _attendance.Add(new Attendance
        {
            Id = Guid.NewGuid(),
            EmployeeId = emp1.Id,
            Date = DateTime.Today,
            CheckIn = DateTime.Today.AddHours(9),
            Status = "Present"
        });

        _recruitments.Add(new Recruitment
        {
            Id = Guid.NewGuid(),
            JobTitle = "Software Developer",
            Department = "IT",
            Status = "Open",
            Vacancies = 2,
            Applicants = 5,
            PostedDate = DateTime.Today.AddDays(-7),
            Deadline = DateTime.Today.AddDays(14),
            CreatedAt = DateTime.UtcNow
        });

        _projects.Add(new Project
        {
            Id = Guid.NewGuid(),
            Name = "IERAHKWA Digital Platform",
            Status = "InProgress",
            StartDate = new DateTime(2025, 1, 1),
            Progress = 65,
            RewardPointsOnCompletion = 500,
            CreatedAt = DateTime.UtcNow
        });

        _notifications.Add(new Notification
        {
            Id = Guid.NewGuid(),
            Title = "Bienvenido al HRM IERAHKWA",
            Message = "Sistema de Gestión de Recursos Humanos integrado a la plataforma soberana.",
            Type = "Info",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        _taxSetups.Add(new TaxSetup
        {
            Id = Guid.NewGuid(),
            Name = "Income Tax Standard",
            Rate = 15,
            MinAmount = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        _roles.Add(new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Description = "Full system access",
            Permissions = new List<string> { "employees", "attendance", "payroll", "leave", "recruitment", "reports", "settings" },
            CreatedAt = DateTime.UtcNow
        });
        _roles.Add(new Role
        {
            Id = Guid.NewGuid(),
            Name = "Employee",
            Description = "Employee self-service",
            Permissions = new List<string> { "leave", "attendance", "profile" },
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<DashboardStats> GetDashboardStatsAsync()
    {
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var stats = new DashboardStats
        {
            TotalEmployees = _employees.Count,
            ActiveEmployees = _employees.Count(e => e.IsActive),
            PresentToday = _attendance.Count(a => a.Date == today && a.Status == "Present"),
            OnLeave = _leaves.Count(l => l.Status == "Approved" && l.StartDate <= today && l.EndDate >= today),
            PendingLeaves = _leaves.Count(l => l.Status == "Pending"),
            OpenRecruitments = _recruitments.Count(r => r.Status == "Open"),
            ActiveProjects = _projects.Count(p => p.Status == "InProgress"),
            MonthlyPayroll = _payrolls.Where(p => p.Period == monthStart.ToString("yyyy-MM")).Sum(p => p.NetSalary),
            AttendanceChart = _attendance.GroupBy(a => a.Date.ToString("dd/MM")).Take(7).Select(g => new ChartData { Label = g.Key, Value = g.Count(a => a.Status == "Present") }).ToList(),
            DepartmentChart = _employees.GroupBy(e => e.Department ?? "N/A").Select(g => new ChartData { Label = g.Key, Value = g.Count() }).ToList()
        };
        return Task.FromResult(stats);
    }

    public Task<IEnumerable<Employee>> GetEmployeesAsync() => Task.FromResult(_employees.AsEnumerable());
    public Task<Employee?> GetEmployeeAsync(Guid id) => Task.FromResult(_employees.FirstOrDefault(e => e.Id == id));

    public Task<Employee> CreateEmployeeAsync(Employee e)
    {
        e.Id = Guid.NewGuid();
        e.EmployeeCode ??= $"EMP{_employeeCounter++:D4}";
        e.CreatedAt = DateTime.UtcNow;
        _employees.Add(e);
        return Task.FromResult(e);
    }

    public Task<Employee?> UpdateEmployeeAsync(Guid id, Employee e)
    {
        var i = _employees.FindIndex(x => x.Id == id);
        if (i < 0) return Task.FromResult<Employee?>(null);
        e.Id = id;
        e.UpdatedAt = DateTime.UtcNow;
        _employees[i] = e;
        return Task.FromResult<Employee?>(e);
    }

    public Task<bool> DeleteEmployeeAsync(Guid id)
    {
        var i = _employees.FindIndex(x => x.Id == id);
        if (i < 0) return Task.FromResult(false);
        _employees.RemoveAt(i);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<Attendance>> GetAttendanceAsync(DateTime? date = null, Guid? employeeId = null)
    {
        var q = _attendance.AsEnumerable();
        if (date.HasValue) q = q.Where(a => a.Date.Date == date.Value.Date);
        if (employeeId.HasValue) q = q.Where(a => a.EmployeeId == employeeId.Value);
        return Task.FromResult(q.OrderByDescending(a => a.Date).AsEnumerable());
    }

    public Task<Attendance> CreateAttendanceAsync(Attendance a)
    {
        a.Id = Guid.NewGuid();
        _attendance.Add(a);
        return Task.FromResult(a);
    }

    public Task<IEnumerable<Leave>> GetLeavesAsync(Guid? employeeId = null, string? status = null)
    {
        var q = _leaves.AsEnumerable();
        if (employeeId.HasValue) q = q.Where(l => l.EmployeeId == employeeId.Value);
        if (!string.IsNullOrEmpty(status)) q = q.Where(l => l.Status == status);
        return Task.FromResult(q.OrderByDescending(l => l.CreatedAt).AsEnumerable());
    }

    public Task<Leave> CreateLeaveAsync(Leave l)
    {
        l.Id = Guid.NewGuid();
        l.CreatedAt = DateTime.UtcNow;
        _leaves.Add(l);
        return Task.FromResult(l);
    }

    public Task<Leave?> UpdateLeaveStatusAsync(Guid id, string status)
    {
        var i = _leaves.FindIndex(x => x.Id == id);
        if (i < 0) return Task.FromResult<Leave?>(null);
        _leaves[i].Status = status;
        _leaves[i].ApprovedAt = DateTime.UtcNow;
        return Task.FromResult<Leave?>(_leaves[i]);
    }

    public Task<IEnumerable<Payroll>> GetPayrollAsync(string? period = null, Guid? employeeId = null)
    {
        var q = _payrolls.AsEnumerable();
        if (!string.IsNullOrEmpty(period)) q = q.Where(p => p.Period == period);
        if (employeeId.HasValue) q = q.Where(p => p.EmployeeId == employeeId.Value);
        return Task.FromResult(q.OrderByDescending(p => p.PayDate).AsEnumerable());
    }

    public Task<Payroll> CreatePayrollAsync(Payroll p)
    {
        p.Id = Guid.NewGuid();
        p.CreatedAt = DateTime.UtcNow;
        _payrolls.Add(p);
        return Task.FromResult(p);
    }

    public Task<IEnumerable<Loan>> GetLoansAsync(Guid? employeeId = null)
    {
        var q = _loans.AsEnumerable();
        if (employeeId.HasValue) q = q.Where(l => l.EmployeeId == employeeId.Value);
        return Task.FromResult(q.OrderByDescending(l => l.CreatedAt).AsEnumerable());
    }

    public Task<Loan> CreateLoanAsync(Loan l)
    {
        l.Id = Guid.NewGuid();
        l.CreatedAt = DateTime.UtcNow;
        _loans.Add(l);
        return Task.FromResult(l);
    }

    public Task<IEnumerable<Recruitment>> GetRecruitmentsAsync(string? status = null)
    {
        var q = _recruitments.AsEnumerable();
        if (!string.IsNullOrEmpty(status)) q = q.Where(r => r.Status == status);
        return Task.FromResult(q.OrderByDescending(r => r.PostedDate).AsEnumerable());
    }

    public Task<Recruitment> CreateRecruitmentAsync(Recruitment r)
    {
        r.Id = Guid.NewGuid();
        r.CreatedAt = DateTime.UtcNow;
        _recruitments.Add(r);
        return Task.FromResult(r);
    }

    public Task<IEnumerable<Award>> GetAwardsAsync(Guid? employeeId = null)
    {
        var q = _awards.AsEnumerable();
        if (employeeId.HasValue) q = q.Where(a => a.EmployeeId == employeeId.Value);
        return Task.FromResult(q.OrderByDescending(a => a.AwardDate).AsEnumerable());
    }

    public Task<Award> CreateAwardAsync(Award a)
    {
        a.Id = Guid.NewGuid();
        a.CreatedAt = DateTime.UtcNow;
        _awards.Add(a);
        if (a.RewardPoints.HasValue && a.RewardPoints > 0)
            _rewardPoints.Add(new RewardPoints { Id = Guid.NewGuid(), EmployeeId = a.EmployeeId, Points = a.RewardPoints.Value, Source = "Award", ReferenceId = a.Id.ToString(), CreatedAt = DateTime.UtcNow });
        return Task.FromResult(a);
    }

    public Task<IEnumerable<Project>> GetProjectsAsync(string? status = null)
    {
        var q = _projects.AsEnumerable();
        if (!string.IsNullOrEmpty(status)) q = q.Where(p => p.Status == status);
        return Task.FromResult(q.OrderByDescending(p => p.CreatedAt).AsEnumerable());
    }

    public Task<Project> CreateProjectAsync(Project p)
    {
        p.Id = Guid.NewGuid();
        p.CreatedAt = DateTime.UtcNow;
        _projects.Add(p);
        return Task.FromResult(p);
    }

    public Task<IEnumerable<Procurement>> GetProcurementsAsync(string? status = null)
    {
        var q = _procurements.AsEnumerable();
        if (!string.IsNullOrEmpty(status)) q = q.Where(p => p.Status == status);
        return Task.FromResult(q.OrderByDescending(p => p.RequestDate).AsEnumerable());
    }

    public Task<Procurement> CreateProcurementAsync(Procurement p)
    {
        p.Id = Guid.NewGuid();
        p.CreatedAt = DateTime.UtcNow;
        _procurements.Add(p);
        return Task.FromResult(p);
    }

    public Task<IEnumerable<Notification>> GetNotificationsAsync(Guid? employeeId = null, bool? unreadOnly = null)
    {
        var q = _notifications.AsEnumerable();
        if (employeeId.HasValue) q = q.Where(n => n.EmployeeId == null || n.EmployeeId == employeeId.Value);
        if (unreadOnly == true) q = q.Where(n => !n.IsRead);
        return Task.FromResult(q.OrderByDescending(n => n.CreatedAt).AsEnumerable());
    }

    public Task<IEnumerable<RewardPoints>> GetRewardPointsAsync(Guid employeeId) =>
        Task.FromResult(_rewardPoints.Where(r => r.EmployeeId == employeeId).OrderByDescending(r => r.CreatedAt).AsEnumerable());

    public Task<int> GetEmployeeTotalPointsAsync(Guid employeeId) =>
        Task.FromResult(_rewardPoints.Where(r => r.EmployeeId == employeeId).Sum(r => r.Points));

    public Task<IEnumerable<TaxSetup>> GetTaxSetupsAsync() => Task.FromResult(_taxSetups.AsEnumerable());

    public Task<TaxSetup> CreateTaxSetupAsync(TaxSetup t)
    {
        t.Id = Guid.NewGuid();
        t.CreatedAt = DateTime.UtcNow;
        _taxSetups.Add(t);
        return Task.FromResult(t);
    }

    public Task<IEnumerable<Role>> GetRolesAsync() => Task.FromResult(_roles.AsEnumerable());
}
