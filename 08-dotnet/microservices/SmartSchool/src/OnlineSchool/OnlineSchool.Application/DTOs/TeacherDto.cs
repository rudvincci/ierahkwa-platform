namespace OnlineSchool.Application.DTOs;

public class TeacherDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Qualification { get; set; }
    public string? Experience { get; set; }
    public DateTime? JoiningDate { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<string> Materials { get; set; } = new List<string>();
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public class CreateTeacherDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Qualification { get; set; }
    public string? Experience { get; set; }
    public DateTime? JoiningDate { get; set; }
    public IEnumerable<int>? MaterialIds { get; set; }
}

public class UpdateTeacherDto
{
    public int Id { get; set; }
    public string? EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Qualification { get; set; }
    public string? Experience { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<int>? MaterialIds { get; set; }
}
