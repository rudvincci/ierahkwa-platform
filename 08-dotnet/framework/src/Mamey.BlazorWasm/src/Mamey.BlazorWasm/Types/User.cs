using Mamey.Auth.Identity;

namespace Mamey.BlazorWasm.Types;

public class User
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public long Expires { get; set; }
    public bool IsAdmin => Role == ClaimValues.Role.Admin;
}
