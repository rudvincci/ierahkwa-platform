using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Application.Interfaces;
using OnlineSchool.Persistence;
using SmartAccounting.Persistence;
using UserManagement.Domain.Entities;

namespace SmartSchool.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly OnlineSchoolDbContext _schoolContext;
    private readonly SmartAccountingDbContext _accountingContext;
    private readonly ICurrentUserService _currentUser;

    public DashboardController(
        OnlineSchoolDbContext schoolContext,
        SmartAccountingDbContext accountingContext,
        ICurrentUserService currentUser)
    {
        _schoolContext = schoolContext;
        _accountingContext = accountingContext;
        _currentUser = currentUser;
    }

    public async Task<IActionResult> Index()
    {
        var roles = _currentUser.Roles.ToList();
        
        if (roles.Contains(DefaultRoles.Admin))
            return RedirectToAction("AdminDashboard");
        
        if (roles.Contains(DefaultRoles.SchoolAdmin))
            return RedirectToAction("SchoolAdminDashboard");
        
        if (roles.Contains(DefaultRoles.Accountant))
            return RedirectToAction("AccountantDashboard");
        
        if (roles.Contains(DefaultRoles.Teacher))
            return RedirectToAction("TeacherDashboard");
        
        if (roles.Contains(DefaultRoles.Student))
            return RedirectToAction("StudentDashboard");
        
        if (roles.Contains(DefaultRoles.Parent))
            return RedirectToAction("ParentDashboard");
        
        if (roles.Contains(DefaultRoles.Receptionist))
            return RedirectToAction("ReceptionistDashboard");
        
        if (roles.Contains(DefaultRoles.Librarian))
            return RedirectToAction("LibrarianDashboard");

        return View();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminDashboard()
    {
        var model = new AdminDashboardViewModel
        {
            TotalSchools = await _schoolContext.Grades.Select(g => g.TenantId).Distinct().CountAsync(),
            TotalTeachers = await _schoolContext.Teachers.CountAsync(),
            TotalStudents = await _schoolContext.Students.CountAsync(),
            TotalParents = await _schoolContext.Parents.CountAsync()
        };
        
        return View(model);
    }

    [Authorize(Roles = "SchoolAdmin")]
    public async Task<IActionResult> SchoolAdminDashboard()
    {
        var tenantId = _currentUser.TenantId;
        
        var model = new SchoolAdminDashboardViewModel
        {
            TotalGrades = await _schoolContext.Grades.Where(g => g.TenantId == tenantId).CountAsync(),
            TotalClassRooms = await _schoolContext.ClassRooms.Where(c => c.TenantId == tenantId).CountAsync(),
            TotalTeachers = await _schoolContext.Teachers.Where(t => t.TenantId == tenantId).CountAsync(),
            TotalStudents = await _schoolContext.Students.Where(s => s.TenantId == tenantId).CountAsync(),
            TotalMaterials = await _schoolContext.Materials.Where(m => m.TenantId == tenantId).CountAsync()
        };
        
        return View(model);
    }

    [Authorize(Roles = "Accountant")]
    public async Task<IActionResult> AccountantDashboard()
    {
        var tenantId = _currentUser.TenantId;
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        
        var model = new AccountantDashboardViewModel
        {
            TotalFeesCollected = await _accountingContext.Invoices
                .Where(i => i.TenantId == tenantId && i.Type == SmartAccounting.Domain.Entities.InvoiceType.FeesInvoice)
                .SumAsync(i => i.PaidAmount),
            TotalFeesDue = await _accountingContext.Invoices
                .Where(i => i.TenantId == tenantId && i.Type == SmartAccounting.Domain.Entities.InvoiceType.FeesInvoice)
                .SumAsync(i => i.TotalAmount - i.PaidAmount),
            MonthlyRevenue = await _accountingContext.Invoices
                .Where(i => i.TenantId == tenantId && i.InvoiceDate >= startOfMonth)
                .SumAsync(i => i.PaidAmount),
            TotalProducts = await _accountingContext.Products.Where(p => p.TenantId == tenantId).CountAsync()
        };
        
        return View(model);
    }

    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> TeacherDashboard()
    {
        var userId = _currentUser.UserId;
        var teacher = await _schoolContext.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
        
        if (teacher == null)
            return View(new TeacherDashboardViewModel());

        var model = new TeacherDashboardViewModel
        {
            TotalClasses = await _schoolContext.Schedules.Where(s => s.TeacherId == teacher.Id).Select(s => s.ClassRoomId).Distinct().CountAsync(),
            TotalHomeworks = await _schoolContext.Homeworks.Where(h => h.TeacherId == teacher.Id).CountAsync(),
            PendingGrading = await _schoolContext.HomeworkAnswers
                .Include(a => a.Homework)
                .Where(a => a.Homework!.TeacherId == teacher.Id && !a.IsGraded)
                .CountAsync()
        };
        
        return View(model);
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> StudentDashboard()
    {
        var userId = _currentUser.UserId;
        var student = await _schoolContext.Students
            .Include(s => s.ClassRoom)
            .ThenInclude(c => c!.Grade)
            .FirstOrDefaultAsync(s => s.UserId == userId);
        
        if (student == null)
            return View(new StudentDashboardViewModel());

        var model = new StudentDashboardViewModel
        {
            ClassName = student.ClassRoom?.Name,
            GradeName = student.ClassRoom?.Grade?.Name,
            TotalHomeworks = await _schoolContext.Homeworks.Where(h => h.ClassRoomId == student.ClassRoomId).CountAsync(),
            CompletedHomeworks = await _schoolContext.HomeworkAnswers.Where(a => a.StudentId == student.Id).CountAsync(),
            AverageScore = await _schoolContext.HomeworkAnswers
                .Where(a => a.StudentId == student.Id && a.IsGraded && a.Score.HasValue)
                .AverageAsync(a => (decimal?)a.Score) ?? 0
        };
        
        return View(model);
    }

    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> ParentDashboard()
    {
        var userId = _currentUser.UserId;
        var parent = await _schoolContext.Parents
            .Include(p => p.StudentParents)
            .ThenInclude(sp => sp.Student)
            .ThenInclude(s => s!.ClassRoom)
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (parent == null)
            return View(new ParentDashboardViewModel());

        var children = parent.StudentParents.Select(sp => sp.Student).Where(s => s != null).ToList();
        
        var model = new ParentDashboardViewModel
        {
            Children = children.Select(c => new ChildProgressViewModel
            {
                StudentId = c!.Id,
                StudentName = c.FullName,
                ClassName = c.ClassRoom?.Name
            }).ToList()
        };
        
        return View(model);
    }

    [Authorize(Roles = "Receptionist")]
    public IActionResult ReceptionistDashboard()
    {
        return View();
    }

    [Authorize(Roles = "Librarian")]
    public IActionResult LibrarianDashboard()
    {
        return View();
    }
}

// View Models
public class AdminDashboardViewModel
{
    public int TotalSchools { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalParents { get; set; }
}

public class SchoolAdminDashboardViewModel
{
    public int TotalGrades { get; set; }
    public int TotalClassRooms { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalMaterials { get; set; }
}

public class AccountantDashboardViewModel
{
    public decimal TotalFeesCollected { get; set; }
    public decimal TotalFeesDue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int TotalProducts { get; set; }
}

public class TeacherDashboardViewModel
{
    public int TotalClasses { get; set; }
    public int TotalHomeworks { get; set; }
    public int PendingGrading { get; set; }
}

public class StudentDashboardViewModel
{
    public string? ClassName { get; set; }
    public string? GradeName { get; set; }
    public int TotalHomeworks { get; set; }
    public int CompletedHomeworks { get; set; }
    public decimal AverageScore { get; set; }
}

public class ParentDashboardViewModel
{
    public List<ChildProgressViewModel> Children { get; set; } = new();
}

public class ChildProgressViewModel
{
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? ClassName { get; set; }
}
