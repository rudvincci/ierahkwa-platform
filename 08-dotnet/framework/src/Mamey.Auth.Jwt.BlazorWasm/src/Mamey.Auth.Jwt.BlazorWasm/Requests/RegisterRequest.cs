namespace Mamey.Auth.Jwt.BlazorWasm.Requests;

public class RegisterRequest
{
    public Guid OrganizationId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
}
