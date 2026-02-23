using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Clients.Users.DTO;

public class ApplicationUserDto
{
    public ApplicationUserDto(){}
    public ApplicationUserDto(Guid id, string email, Name name, bool emailConfirmed, bool lockoutEnabled)
    {
        this.Id = id;
        this.Email = email;
        this.Name = name;
        this.EmailConfirmed = emailConfirmed;
        this.LockoutEnabled = lockoutEnabled;
    }
    public Guid Id { get; }
    public string Email { get; }
    public Name Name { get; }
    public bool EmailConfirmed { get; }
    public bool LockoutEnabled { get; }
}