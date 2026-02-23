using Mamey.Constants.User;

namespace Mamey.Types
{
    public interface IBaseUser<T>
    {
        T Id { get; }
        Email Email { get; }
        string Username { get; }
        UserType UserType { get; }
        Permission Permissions { get; }

        void SetUsername(string username);
        void SetUserPermissions(Permission permissions);
    }
}