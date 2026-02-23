namespace OnlineSchool.Application.DTOs;

public class ParentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Occupation { get; set; }
    public string? Relation { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<StudentBasicDto> Children { get; set; } = new List<StudentBasicDto>();
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public class StudentBasicDto
{
    public int Id { get; set; }
    public string? StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ClassName { get; set; }
    public string? GradeName { get; set; }
}

public class CreateParentDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Occupation { get; set; }
    public string? Relation { get; set; }
    public IEnumerable<int>? StudentIds { get; set; }
}

public class UpdateParentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Occupation { get; set; }
    public string? Relation { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<int>? StudentIds { get; set; }
}
