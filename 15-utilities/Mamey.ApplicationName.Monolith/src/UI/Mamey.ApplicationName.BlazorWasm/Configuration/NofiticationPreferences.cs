using ReactiveUI;

namespace Mamey.ApplicationName.BlazorWasm.Configuration
{
    /// <summary>
    /// Represents notification preferences for the user.
    /// </summary>
    public class NotificationPreferences : ReactiveObject
    {
        private bool _notificationsEnabled = true;
        private bool _emailNotificationsEnabled = true;

        /// <summary>
        /// Gets or sets whether notifications are enabled for the user.
        /// </summary>
        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set => this.RaiseAndSetIfChanged(ref _notificationsEnabled, value);
        }

        /// <summary>
        /// Gets or sets whether email notifications are enabled for the user.
        /// </summary>
        public bool EmailNotificationsEnabled
        {
            get => _emailNotificationsEnabled;
            set => this.RaiseAndSetIfChanged(ref _emailNotificationsEnabled, value);
        }

        /// <summary>
        /// Toggles notifications.
        /// </summary>
        public void ToggleNotificationsEnabled() => NotificationsEnabled = !NotificationsEnabled;

        /// <summary>
        /// Toggles email notifications.
        /// </summary>
        public void ToggleEmailNotificationsEnabled() => EmailNotificationsEnabled = !EmailNotificationsEnabled;
    }
}
