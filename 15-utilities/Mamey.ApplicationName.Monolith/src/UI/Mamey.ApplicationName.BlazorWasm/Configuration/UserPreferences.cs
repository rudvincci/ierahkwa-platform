using ReactiveUI;

namespace Mamey.ApplicationName.BlazorWasm.Configuration
{
    public class UserPreferences : ReactiveObject
    {
        public NotificationPreferences NotificationPreferences { get; set; } = new();
        public SystemPreferences SystemPreferences { get; set; } = new();
        public ThemePreferences ThemePreferences { get; set; } = new();

        private bool _soundNotificationsEnabled = true;
        private string _lastVisitedPage = "/";

        public bool SoundNotificationsEnabled
        {
            get => _soundNotificationsEnabled;
            set => this.RaiseAndSetIfChanged(ref _soundNotificationsEnabled, value);
        }

        public string LastVisitedPage
        {
            get => _lastVisitedPage;
            set => this.RaiseAndSetIfChanged(ref _lastVisitedPage, value);
        }
    }
}