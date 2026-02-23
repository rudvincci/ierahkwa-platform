namespace Mamey.Types;

public abstract class BaseUser : AggregateRoot<UserId>, IBaseUser<UserId>
{
    protected BaseUser() : base(Guid.Empty){}
    public BaseUser(UserId id, string username,
        string email, UserType userType, Constants.User.Permission permissions, int version = 0)
        : base(id, version)
    {
        Id = id;
        Email = email;
        Username = username;
        UserType = userType;
        Permissions = permissions;
    }
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public string Username { get; private set; }
    public UserType UserType { get; private set; }
    public Constants.User.Permission Permissions { get; private set; }

    public void SetUserPermissions(Constants.User.Permission permissions)
    {
        Permissions = permissions;
    }
    public virtual void SetUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentNullException(nameof(username));
        }
        Username = username;
    }
}

