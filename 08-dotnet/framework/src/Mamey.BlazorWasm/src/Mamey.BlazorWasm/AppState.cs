

//// Use this to notify when any global state change requires refreshing the grid
//private readonly Subject<Unit> _refreshTrigger = new Subject<Unit>();
//public IObservable<Unit> RefreshTrigger => _refreshTrigger.AsObservable();
//public void TriggerGridRefresh()
//{
//    _refreshTrigger.OnNext(Unit.Default);
//}

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;

namespace Mamey.BlazorWasm;

public class AppState : ReactiveObject
{
    private readonly object _stateLock = new object();

    private string _accessToken;
    private UserState _currentUser;
    private Dictionary<string, object> _settings = new Dictionary<string, object>();

    public event Action OnAccessTokenChange;

    public string AccessToken
    {
        get => _accessToken;
        set
        {
            lock (_stateLock)
            {
                if (string.IsNullOrWhiteSpace(value) || !IsValidToken(value))
                {
                    PromptForNewToken();
                }
                else
                {
                    this.RaiseAndSetIfChanged(ref _accessToken, value);
                    OnAccessTokenChange?.Invoke();
                }
            }
        }
    }

    public UserState CurrentUser
    {
        get => _currentUser;
        set
        {
            lock (_stateLock)
            {
                this.RaiseAndSetIfChanged(ref _currentUser, value);
            }
        }
    }

    public void SetSetting(string key, object value)
    {
        lock (_stateLock)
        {
            if (_settings.ContainsKey(key))
            {
                _settings[key] = value;
            }
            else
            {
                _settings.Add(key, value);
            }
            this.RaisePropertyChanged(nameof(_settings));
        }
    }

    public object GetSetting(string key)
    {
        lock (_stateLock)
        {
            _settings.TryGetValue(key, out var value);
            return value;
        }
    }

    private readonly Subject<Notification> _notificationStream = new Subject<Notification>();
    private readonly TimeSpan _notificationThrottleTimeSpan = TimeSpan.FromSeconds(1);

    public IObservable<Notification> Notifications => _notificationStream.AsObservable().Throttle(_notificationThrottleTimeSpan);

    public void PushNotification(Notification notification)
    {
        lock (_stateLock)
        {
            _notificationStream.OnNext(notification);
        }
    }

    private void NotifyStateChanged() => OnAccessTokenChange?.Invoke();

    private bool IsValidToken(string token)
    {
        // Placeholder: Insert real token validation logic here
        return !string.IsNullOrWhiteSpace(token); // Simplified check
    }

    private void PromptForNewToken()
    {
        // Placeholder: Implement user notification and re-authentication logic
        Console.WriteLine("Your session has expired. Please log in again.");
    }

    private readonly Subject<Unit> _refreshTrigger = new Subject<Unit>();

    public IObservable<Unit> RefreshTrigger => _refreshTrigger.AsObservable();

    public void TriggerGridRefresh()
    {
        lock (_stateLock)
        {
            _refreshTrigger.OnNext(Unit.Default);
        }
    }
}

public class UserState
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public DateTime LastLoginTime { get; set; }
}

public class Notification
{
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public NotificationType Type { get; set; }
}

public enum NotificationType
{
    Alert,
    Message,
    Reminder
}
