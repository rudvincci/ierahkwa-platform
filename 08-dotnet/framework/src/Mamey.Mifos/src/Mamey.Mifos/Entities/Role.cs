using System.Text.Json.Serialization;

namespace Mamey.Mifos.Entities;

public class Role
{
    public Role() { }

    [JsonConstructor]
    public Role(int id, string name, string? description, bool disabled)
    {
        Id = id;
        Name = name;
        Description = description;
        Disabled = disabled;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool Disabled { get; set; } = false;
}
