namespace Mamey.MicroMonolith.Abstractions.Auth;

public class RolePermissions
{
    public RolePermissions(Dictionary<Type, Dictionary<string, long>> permissions)
    {
        Permissions = permissions;
    }

    public Dictionary<Type, Dictionary<string, long>> Permissions { get; set; }
}