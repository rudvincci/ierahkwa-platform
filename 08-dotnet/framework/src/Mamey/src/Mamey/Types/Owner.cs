
namespace Mamey.Types;

public class Owner
{
    public Owner(Guid id, Name name, string email, Phone? phone = null)
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public Name? Name { get; set; }
    public string Email { get; set; }
    public Phone? Phone { get; set; }
}

