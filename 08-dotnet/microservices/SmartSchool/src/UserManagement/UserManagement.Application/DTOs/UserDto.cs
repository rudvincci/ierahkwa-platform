namespace UserManagement.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; }
    public int? TenantId { get; set; }
    public string? TenantName { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public class CreateUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int? TenantId { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class UpdateUserDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? ProfileImage { get; set; }
    public bool? IsActive { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
