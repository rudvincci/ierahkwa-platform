namespace Mamey.ApplicationName.BlazorWasm.Models.Roles;

public class UserRole
{
    public string UserName { get; set; }
    public string Role { get; set; }
    public List<string> Permissions { get; set; }
}
public class Role
{
    public Guid Id { get; set; }
    public string RoleName { get; set; }
    public List<string> Permissions { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string AssignedRole { get; set; }
}

public class UserActivity
{
    public string UserName { get; set; }
    public string Activity { get; set; }
    public DateTime Timestamp { get; set; }
}