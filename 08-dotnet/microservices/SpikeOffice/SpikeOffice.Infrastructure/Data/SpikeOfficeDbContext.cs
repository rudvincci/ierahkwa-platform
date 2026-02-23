using Microsoft.EntityFrameworkCore;
using SpikeOffice.Core.Entities;
using SpikeOffice.Core.Interfaces;

namespace SpikeOffice.Infrastructure.Data;

public class SpikeOfficeDbContext : DbContext
{
    private readonly ITenantContext? _tenantContext;
    private readonly Guid? _tenantId;

    public SpikeOfficeDbContext(DbContextOptions<SpikeOfficeDbContext> options, ITenantContext? tenantContext = null)
        : base(options)
    {
        _tenantContext = tenantContext;
        _tenantId = _tenantContext?.TenantId;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Designation> Designations => Set<Designation>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<SystemUser> SystemUsers => Set<SystemUser>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<ClockInOut> ClockInOuts => Set<ClockInOut>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<Weekend> Weekends => Set<Weekend>();
    public DbSet<Payslip> Payslips => Set<Payslip>();
    public DbSet<ChartOfAccount> ChartOfAccounts => Set<ChartOfAccount>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<TaskBoard> TaskBoards => Set<TaskBoard>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<EmployeeLoan> EmployeeLoans => Set<EmployeeLoan>();
    public DbSet<Notice> Notices => Set<Notice>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Award> Awards => Set<Award>();
    public DbSet<ExpenseClaim> ExpenseClaims => Set<ExpenseClaim>();
    public DbSet<PaymentGateway> PaymentGateways => Set<PaymentGateway>();
    public DbSet<AllowedClockInIp> AllowedClockInIps => Set<AllowedClockInIp>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Tenant has no tenant filter (it is the root)
        mb.Entity<Tenant>().HasIndex(t => t.UrlPrefix).IsUnique();

        // Multi-tenant global filter for tenant-scoped entities
        mb.Entity<Subscription>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Department>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Designation>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Role>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<SystemUser>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Employee>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Attendance>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<ClockInOut>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<LeaveType>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<LeaveRequest>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Holiday>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Weekend>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Payslip>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<ChartOfAccount>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<JournalEntry>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<JournalEntryLine>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<TaskBoard>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<TaskItem>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<EmployeeLoan>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Notice>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Message>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<Award>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<ExpenseClaim>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<PaymentGateway>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);
        mb.Entity<AllowedClockInIp>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId);

        // Relations
        mb.Entity<Subscription>().HasOne(s => s.Tenant).WithMany(t => t.Subscriptions).HasForeignKey(s => s.TenantId);
        mb.Entity<Employee>().HasOne(e => e.Department).WithMany(d => d.Employees).HasForeignKey(e => e.DepartmentId);
        mb.Entity<Employee>().HasOne(e => e.Designation).WithMany(d => d.Employees).HasForeignKey(e => e.DesignationId);
        mb.Entity<JournalEntryLine>().HasOne(l => l.JournalEntry).WithMany(j => j.Lines).HasForeignKey(l => l.JournalEntryId);
        mb.Entity<JournalEntryLine>().HasOne(l => l.Account).WithMany().HasForeignKey(l => l.AccountId);
        mb.Entity<TaskItem>().HasOne(t => t.Board).WithMany(b => b.Tasks).HasForeignKey(t => t.BoardId);
        mb.Entity<TaskItem>().HasOne(t => t.Assignee).WithMany(e => e.Tasks).HasForeignKey(t => t.AssigneeId);
    }
}
