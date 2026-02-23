namespace OnlineSchool.Application.DTOs;

public class MaterialDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool IsActive { get; set; }
}

public class CreateMaterialDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
}

public class UpdateMaterialDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool IsActive { get; set; }
}
