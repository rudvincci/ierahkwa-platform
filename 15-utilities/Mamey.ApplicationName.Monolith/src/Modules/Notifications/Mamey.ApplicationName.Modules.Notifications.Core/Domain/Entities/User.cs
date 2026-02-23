using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;

internal class User
{
    private User(){}

    public User(UserId id, Name name, Email email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
    public UserId Id { get; private set; }
    public Name Name { get; private set; }
    public Email Email { get; private set; }
}