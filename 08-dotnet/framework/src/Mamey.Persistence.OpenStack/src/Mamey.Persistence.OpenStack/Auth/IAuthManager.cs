namespace Mamey.Persistence.OpenStack.OCS.Auth;

internal interface IAuthManager
{
    Task<AuthData> Authenticate();
}