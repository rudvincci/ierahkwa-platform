using RnBCal.Core.Interfaces;
using RnBCal.Core.Models;
using System.Text.Json;

namespace RnBCal.Infrastructure.Services;

/// <summary>
/// Google Calendar Auto-Sync Service - IERAHKWA RnBCal
/// Implements OAuth 2.0 and Google Calendar API integration
/// </summary>
public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleCalendarConfig _config;

    public GoogleCalendarService(HttpClient httpClient, GoogleCalendarConfig config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<bool> AutoSyncToGoogleCalendar(Booking booking)
    {
        try
        {
            if (!_config.Enabled || string.IsNullOrEmpty(_config.AccessToken))
            {
                Console.WriteLine("⚠️  Google Calendar auto-sync is not configured");
                return false;
            }

            var calendarEvent = new
            {
                summary = $"{booking.ItemType} Rental: {booking.ItemName}",
                description = $@"Customer: {booking.CustomerName}
Email: {booking.CustomerEmail}
Phone: {booking.CustomerPhone}
Amount: {booking.Currency} {booking.TotalAmount}
Status: {booking.Status}

{booking.Description}

Powered by IERAHKWA RnBCal System",
                location = booking.Location,
                start = new
                {
                    dateTime = booking.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    timeZone = "UTC"
                },
                end = new
                {
                    dateTime = booking.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    timeZone = "UTC"
                },
                attendees = new[]
                {
                    new { email = booking.CustomerEmail }
                },
                reminders = new
                {
                    useDefault = false,
                    overrides = new[]
                    {
                        new { method = "email", minutes = 24 * 60 },
                        new { method = "popup", minutes = 60 }
                    }
                }
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.AccessToken}");

            var content = new StringContent(
                JsonSerializer.Serialize(calendarEvent),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"https://www.googleapis.com/calendar/v3/calendars/{_config.CalendarId}/events",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ Synced booking {booking.Id} to Google Calendar");
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Google Calendar sync failed: {error}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Google Calendar sync error: {ex.Message}");
            return false;
        }
    }

    public string GetOAuthUrl()
    {
        var scopes = Uri.EscapeDataString("https://www.googleapis.com/auth/calendar");
        var redirectUri = Uri.EscapeDataString(_config.RedirectUri);
        
        return $"https://accounts.google.com/o/oauth2/v2/auth?" +
               $"client_id={_config.ClientId}&" +
               $"redirect_uri={redirectUri}&" +
               $"response_type=code&" +
               $"scope={scopes}&" +
               $"access_type=offline&" +
               $"prompt=consent";
    }
}

public class GoogleCalendarConfig
{
    public bool Enabled { get; set; } = false;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = "http://localhost:5055/api/calendar/google/callback";
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string CalendarId { get; set; } = "primary";
}
