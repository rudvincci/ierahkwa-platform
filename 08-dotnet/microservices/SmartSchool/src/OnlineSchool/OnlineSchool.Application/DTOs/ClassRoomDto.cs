namespace OnlineSchool.Application.DTOs;

public class ClassRoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int GradeId { get; set; }
    public string? GradeName { get; set; }
    public int Capacity { get; set; }
    public int StudentCount { get; set; }
    public bool IsActive { get; set; }
}

public class CreateClassRoomDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int GradeId { get; set; }
    public int Capacity { get; set; } = 30;
}

public class UpdateClassRoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int GradeId { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
}
