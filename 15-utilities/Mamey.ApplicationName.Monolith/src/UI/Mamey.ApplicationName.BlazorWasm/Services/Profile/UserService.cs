using Mamey.ApplicationName.BlazorWasm.Models.Profile;

namespace Mamey.ApplicationName.BlazorWasm.Services.Profile;

public class UserService
{
    public User GetUser()
    {
        return new User
        {
            FullName = "John Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "123-456-7890",
            Address = "123 Main St, Springfield",
            ShareDataWithPartners = true,
            EnablePersonalizedAds = false
        };
    }

    public List<UserDevice> GetUserDevices() => new()
    {
        new UserDevice { DeviceName = "iPhone 12", IPAddress = "192.168.1.10", IsTrusted = true },
        new UserDevice { DeviceName = "Work Laptop", IPAddress = "192.168.1.11", IsTrusted = false }
    };

    public List<LinkedAccount> GetLinkedAccounts() => new()
    {
        new LinkedAccount { AccountName = "PayPal", AccountType = "Payment Service", IsLinked = true },
        new LinkedAccount { AccountName = "Chase Bank", AccountType = "Bank Account", IsLinked = false }
    };

    public List<NotificationPreference> GetNotificationPreferences() => new()
    {
        new NotificationPreference { Type = "Email", Enabled = true },
        new NotificationPreference { Type = "SMS", Enabled = false },
        new NotificationPreference { Type = "Push Notifications", Enabled = true }
    };
}