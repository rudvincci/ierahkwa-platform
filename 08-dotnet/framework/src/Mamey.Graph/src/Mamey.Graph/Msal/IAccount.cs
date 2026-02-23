namespace Mamey.Graph.Msal;

public interface IAccount
{
    string HomeAccountId { get; }
    string Environment { get; }
    string TenantId { get; }
    string Username { get; }
}
