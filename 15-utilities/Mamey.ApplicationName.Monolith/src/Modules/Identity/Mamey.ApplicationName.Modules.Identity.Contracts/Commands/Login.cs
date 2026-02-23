using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Identity.Contracts.Commands;

public class Login : ICommand
{
    public Login(string username, string password, bool rememberMe = false, string? returnUrl = null)
    {
        Username = username;
        Password = password;
        RememberMe = rememberMe;
        ReturnUrl = returnUrl;
    }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
    public string ReturnUrl { get; set; } = string.Empty;
}