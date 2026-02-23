using System.Collections.ObjectModel;
using Mamey.ApplicationName.BlazorWasm.Models.Profile;
using Mamey.ApplicationName.BlazorWasm.Services.Profile;
using ReactiveUI;

namespace Mamey.ApplicationName.BlazorWasm.ViewModels.Profile;

public class ProfileViewModel : ReactiveObject
{
    public User User { get; set; }
    public ObservableCollection<UserDevice> UserDevices { get; set; }
    public ObservableCollection<LinkedAccount> LinkedAccounts { get; set; }
    public ObservableCollection<NotificationPreference> UserPreferences { get; set; }

    private readonly UserService _service;

    // Constructor accepting UserService as a dependency
    public ProfileViewModel(UserService service)
    {
        _service = service;
        LoadData();
    }

    private void LoadData()
    {
        User = _service.GetUser();
        UserDevices = new ObservableCollection<UserDevice>(_service.GetUserDevices());
        LinkedAccounts = new ObservableCollection<LinkedAccount>(_service.GetLinkedAccounts());
        UserPreferences = new ObservableCollection<NotificationPreference>(_service.GetNotificationPreferences());
    }

    public void SaveProfile() { /* Save profile logic */ }
    public void SavePrivacySettings() { /* Save privacy settings logic */ }
    public void SavePreferences() { /* Save notification preferences logic */ }
}