using RnBCal.Core.Models;

namespace RnBCal.Core.Interfaces;

/// <summary>
/// Calendar Service Interface - IERAHKWA RnBCal
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// Generate ICS file content from a booking
    /// </summary>
    string GenerateIcsFile(Booking booking);
    
    /// <summary>
    /// Generate ICS file from a calendar event
    /// </summary>
    string GenerateIcsFile(CalendarEvent calendarEvent);
    
    /// <summary>
    /// Generate direct calendar links for all providers
    /// </summary>
    Dictionary<CalendarProvider, CalendarLink> GenerateCalendarLinks(Booking booking);
    
    /// <summary>
    /// Sync booking to calendar and return result
    /// </summary>
    Task<CalendarSyncResult> SyncBookingToCalendar(Booking booking);
}

/// <summary>
/// Email Service Interface
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send booking confirmation email with calendar attachment
    /// </summary>
    Task<bool> SendBookingConfirmationEmail(Booking booking, string icsContent);
    
    /// <summary>
    /// Send calendar update email
    /// </summary>
    Task<bool> SendCalendarUpdateEmail(Booking booking, string icsContent);
}

/// <summary>
/// Google Calendar Auto-Sync Service
/// </summary>
public interface IGoogleCalendarService
{
    /// <summary>
    /// Auto-sync to Google Calendar using API
    /// </summary>
    Task<bool> AutoSyncToGoogleCalendar(Booking booking);
    
    /// <summary>
    /// Get OAuth URL for Google Calendar
    /// </summary>
    string GetOAuthUrl();
}
