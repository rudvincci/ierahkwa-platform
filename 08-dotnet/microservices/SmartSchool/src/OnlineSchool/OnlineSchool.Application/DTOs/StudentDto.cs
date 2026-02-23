namespace OnlineSchool.Application.DTOs;

public class StudentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string? ProfileImage { get; set; }
    public int? ClassRoomId { get; set; }
    public string? ClassRoomName { get; set; }
    public string? GradeName { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<ParentDto> Parents { get; set; } = new List<ParentDto>();
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public class CreateStudentDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodGroup { get; set; }
    public int ClassRoomId { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public IEnumerable<int>? ParentIds { get; set; }
}

public class UpdateStudentDto
{
    public int Id { get; set; }
    public string? StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodGroup { get; set; }
    public int? ClassRoomId { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<int>? ParentIds { get; set; }
}
