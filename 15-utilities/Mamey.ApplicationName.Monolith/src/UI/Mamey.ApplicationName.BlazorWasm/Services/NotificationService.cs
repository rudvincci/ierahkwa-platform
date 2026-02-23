using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using Notification = Mamey.ApplicationName.BlazorWasm.Models.Notification;

namespace Mamey.ApplicationName.BlazorWasm.Services;

internal class NotificationService : ReactiveObject, IAsyncDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private HubConnection? _hubConnection;
    private readonly ObservableCollection<Notification> _notifications = new();
    public ReadOnlyObservableCollection<Notification> Notifications { get; }

    private readonly Subject<Notification> _notificationStream = new();
    public IObservable<Notification> NotificationStream => _notificationStream.AsObservable();

    public NotificationService(NavigationManager navigationManager, IHttpClientFactory httpClientFactory)
    {
        _navigationManager = navigationManager;
        _httpClient = httpClientFactory.CreateClient("BankApi");
        Notifications = new ReadOnlyObservableCollection<Notification>(_notifications);
        // InitializeHubConnection();
    }

    // private async void InitializeHubConnection()
    // {
    //     _hubConnection = new HubConnectionBuilder()
    //         .WithUrl(_navigationManager.ToAbsoluteUri("https://localhost:51816/notifications-module/notificationsHub"))
    //         .WithAutomaticReconnect()
    //         .Build();
    //
    //     _hubConnection.On<Notification>("ReceiveNotification", notification =>
    //     {
    //         AddNotification(notification);
    //     });
    //
    //     await StartConnectionAsync();
    // }

    private async Task StartConnectionAsync()
    {
        if (_hubConnection != null && _hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
        }
    }

    public async Task LoadInitialNotifications()
    {
        var notifications = await _httpClient.GetFromJsonAsync<List<Notification>>("/notifications-module/notifications");
        if (notifications != null)
        {
            foreach (var notification in notifications)
            {
                _notifications.Add(notification);
            }
        }
    }

    public void AddNotification(Notification notification)
    {
        _notifications.Add(notification);
        _notificationStream.OnNext(notification);
    }

    public void MarkAsRead(Guid notificationId)
    {
        var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            this.RaisePropertyChanged(nameof(Notifications));
        }
    }

    public void ClearAllNotifications()
    {
        _notifications.Clear();
        this.RaisePropertyChanged(nameof(Notifications));
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
