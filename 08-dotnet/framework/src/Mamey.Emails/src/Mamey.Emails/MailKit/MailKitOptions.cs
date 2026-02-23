namespace Mamey.Emails;

public class MailKitOptions
{
    public string Smtp { get; set; }
    public int Port { get; set; }
    public string Password { get; set; }
    public bool UseSsl { get; set; }
}