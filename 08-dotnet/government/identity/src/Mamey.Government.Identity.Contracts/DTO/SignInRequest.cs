namespace Mamey.Government.Identity.Contracts.DTO;

public class SignInRequest
{
    public SignInRequest()
    {
    }

    public SignInRequest(string usernameOrEmail, string password)
        => (UsernameOrEmail, Password) = (usernameOrEmail, password);
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

