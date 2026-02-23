using Common.Domain.Entities;

namespace UserManagement.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; } = false;
    
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePolicy> RolePolicies { get; set; } = new List<RolePolicy>();
}

public static class DefaultRoles
{
    public const string Admin = "Admin";
    public const string SchoolAdmin = "SchoolAdmin";
    public const string Accountant = "Accountant";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
    public const string Parent = "Parent";
    public const string Receptionist = "Receptionist";
    public const string Librarian = "Librarian";
    
    public static readonly string[] AllRoles = 
    {
        Admin, SchoolAdmin, Accountant, Teacher, Student, Parent, Receptionist, Librarian
    };
}
