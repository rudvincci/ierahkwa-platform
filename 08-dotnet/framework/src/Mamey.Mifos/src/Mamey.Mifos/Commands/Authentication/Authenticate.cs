using System.Security;

namespace Mamey.Mifos.Commands.Authentication;

public class Authenticate : MifosCommand
{
    public Authenticate(string username, SecureString password)
    {
        if (string.IsNullOrEmpty(username.Trim()))
        {
            throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
        }

        Username = username;
        Password = password ?? throw new ArgumentNullException(nameof(password));
    }

    public string Username { get; }
    public SecureString Password { get; }
}

