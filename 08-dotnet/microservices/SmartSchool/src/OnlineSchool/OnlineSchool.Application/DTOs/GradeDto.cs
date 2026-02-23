namespace OnlineSchool.Application.DTOs;

public class GradeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; }
    public int ClassRoomCount { get; set; }
}

public class CreateGradeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
}

public class UpdateGradeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; }
}
