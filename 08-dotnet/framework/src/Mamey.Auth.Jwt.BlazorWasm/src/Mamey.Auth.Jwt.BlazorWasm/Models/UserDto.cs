namespace Mamey.Auth.Jwt.BlazorWasm.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class UserDetailsDto : UserDto
{
    public string Email { get; set; }
    public string Role { get; set; }
    public IEnumerable<string> Permissions { get; set; }
}
public class AuthDto
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public long Expires { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public IDictionary<string, IEnumerable<string>> Claims { get; set; }
}
