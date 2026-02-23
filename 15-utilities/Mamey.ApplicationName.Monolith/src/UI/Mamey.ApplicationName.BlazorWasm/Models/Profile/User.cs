namespace Mamey.ApplicationName.BlazorWasm.Models.Profile;

public class User
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public bool ShareDataWithPartners { get; set; }
    public bool EnablePersonalizedAds { get; set; }
}
public class UserDevice
{
    public string DeviceName { get; set; }
    public string IPAddress { get; set; }
    public bool IsTrusted { get; set; }
}

public class LinkedAccount
{
    public string AccountName { get; set; }
    public string AccountType { get; set; }
    public bool IsLinked { get; set; }
}

public class NotificationPreference
{
    public string Type { get; set; }
    public bool Enabled { get; set; }
}