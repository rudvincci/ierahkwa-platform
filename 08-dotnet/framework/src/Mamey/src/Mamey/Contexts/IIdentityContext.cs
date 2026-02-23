namespace Mamey.Contexts;

public interface IIdentityContext
{
    Guid Id { get; }
    // Guid OrganizationId { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    IDictionary<string, string> Claims { get; }
    // void SetOrganizationId(Guid organizationId);
}
