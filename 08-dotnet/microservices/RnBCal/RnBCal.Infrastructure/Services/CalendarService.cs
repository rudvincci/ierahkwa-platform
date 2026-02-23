using RnBCal.Core.Interfaces;
using RnBCal.Core.Models;
using System.Text;
using System.Web;

namespace RnBCal.Infrastructure.Services;

/// <summary>
/// Calendar Service Implementation - IERAHKWA RnBCal
/// Generates ICS files and calendar links for multiple providers
/// </summary>
public class CalendarService : ICalendarService
{
    public string GenerateIcsFile(Booking booking)
    {
        var calEvent = new CalendarEvent
        {
            Uid = $"booking-{booking.Id}@ierahkwa.gov",
            Summary = $"{booking.ItemType} Rental: {booking.ItemName}",
            Description = $@"Booking Details:
Customer: {booking.CustomerName}
Email: {booking.CustomerEmail}
Phone: {booking.CustomerPhone}
Item: {booking.ItemName} ({booking.ItemType})
Amount: {booking.Currency} {booking.TotalAmount}
Status: {booking.Status}

{booking.Description}

Powered by IERAHKWA RnBCal System
Sovereign Government of Ierahkwa Ne Kanienke",
            StartDateTime = booking.StartDate,
            EndDateTime = booking.EndDate,
            Location = booking.Location,
            OrganizerEmail = "bookings@ierahkwa.gov",
            OrganizerName = "IERAHKWA Booking System",
            Categories = $"Booking,{booking.ItemType},{booking.Type}"
        };
        
        if (!string.IsNullOrEmpty(booking.CustomerEmail))
        {
            calEvent.AttendeeEmails.Add(booking.CustomerEmail);
        }
        
        return GenerateIcsFile(calEvent);
    }

    public string GenerateIcsFile(CalendarEvent calendarEvent)
    {
        var sb = new StringBuilder();
        
        // ICS Header
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//IERAHKWA//RnBCal v1.0//EN");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:PUBLISH");
        sb.AppendLine("X-WR-CALNAME:IERAHKWA Bookings");
        sb.AppendLine("X-WR-TIMEZONE:UTC");
        
        // Event
        sb.AppendLine("BEGIN:VEVENT");
        sb.AppendLine($"UID:{calendarEvent.Uid}");
        sb.AppendLine($"DTSTAMP:{FormatDateTime(DateTime.UtcNow)}");
        sb.AppendLine($"DTSTART:{FormatDateTime(calendarEvent.StartDateTime)}");
        sb.AppendLine($"DTEND:{FormatDateTime(calendarEvent.EndDateTime)}");
        sb.AppendLine($"SUMMARY:{EscapeText(calendarEvent.Summary)}");
        sb.AppendLine($"DESCRIPTION:{EscapeText(calendarEvent.Description)}");
        sb.AppendLine($"LOCATION:{EscapeText(calendarEvent.Location)}");
        sb.AppendLine($"STATUS:{calendarEvent.Status.ToString().ToUpper()}");
        sb.AppendLine($"SEQUENCE:0");
        sb.AppendLine($"PRIORITY:{calendarEvent.Priority}");
        
        // Organizer
        if (!string.IsNullOrEmpty(calendarEvent.OrganizerEmail))
        {
            sb.AppendLine($"ORGANIZER;CN={EscapeText(calendarEvent.OrganizerName)}:MAILTO:{calendarEvent.OrganizerEmail}");
        }
        
        // Attendees
        foreach (var attendee in calendarEvent.AttendeeEmails)
        {
            sb.AppendLine($"ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE:MAILTO:{attendee}");
        }
        
        // Alarm/Reminder
        sb.AppendLine("BEGIN:VALARM");
        sb.AppendLine("TRIGGER:-PT" + calendarEvent.ReminderMinutesBefore + "M");
        sb.AppendLine("ACTION:DISPLAY");
        sb.AppendLine($"DESCRIPTION:Reminder: {EscapeText(calendarEvent.Summary)}");
        sb.AppendLine("END:VALARM");
        
        // Categories
        if (!string.IsNullOrEmpty(calendarEvent.Categories))
        {
            sb.AppendLine($"CATEGORIES:{calendarEvent.Categories}");
        }
        
        // URL
        if (!string.IsNullOrEmpty(calendarEvent.Url))
        {
            sb.AppendLine($"URL:{calendarEvent.Url}");
        }
        
        // Custom properties
        foreach (var prop in calendarEvent.CustomProperties)
        {
            sb.AppendLine($"X-{prop.Key.ToUpper()}:{EscapeText(prop.Value)}");
        }
        
        sb.AppendLine("END:VEVENT");
        sb.AppendLine("END:VCALENDAR");
        
        return sb.ToString();
    }

    public Dictionary<CalendarProvider, CalendarLink> GenerateCalendarLinks(Booking booking)
    {
        var links = new Dictionary<CalendarProvider, CalendarLink>();
        
        var title = Uri.EscapeDataString($"{booking.ItemType} Rental: {booking.ItemName}");
        var description = Uri.EscapeDataString($"Customer: {booking.CustomerName}\n{booking.Description}");
        var location = Uri.EscapeDataString(booking.Location);
        var startDate = booking.StartDate.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
        var endDate = booking.EndDate.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
        
        // Google Calendar
        links[CalendarProvider.Google] = new CalendarLink
        {
            Provider = "Google Calendar",
            DirectLink = $"https://www.google.com/calendar/render?action=TEMPLATE&text={title}&dates={startDate}/{endDate}&details={description}&location={location}&sf=true&output=xml",
            DisplayName = "Add to Google Calendar",
            IconClass = "bi-google"
        };
        
        // Yahoo Calendar
        links[CalendarProvider.Yahoo] = new CalendarLink
        {
            Provider = "Yahoo Calendar",
            DirectLink = $"https://calendar.yahoo.com/?v=60&view=d&type=20&title={title}&st={startDate}&et={endDate}&desc={description}&in_loc={location}",
            DisplayName = "Add to Yahoo Calendar",
            IconClass = "bi-yahoo"
        };
        
        // Outlook.com
        links[CalendarProvider.Outlook] = new CalendarLink
        {
            Provider = "Outlook Calendar",
            DirectLink = $"https://outlook.live.com/calendar/0/deeplink/compose?subject={title}&startdt={booking.StartDate:yyyy-MM-ddTHH:mm:ss}&enddt={booking.EndDate:yyyy-MM-ddTHH:mm:ss}&body={description}&location={location}&path=/calendar/action/compose&rru=addevent",
            DisplayName = "Add to Outlook Calendar",
            IconClass = "bi-microsoft"
        };
        
        // Office 365
        links[CalendarProvider.Office365] = new CalendarLink
        {
            Provider = "Office 365 Calendar",
            DirectLink = $"https://outlook.office.com/calendar/0/deeplink/compose?subject={title}&startdt={booking.StartDate:yyyy-MM-ddTHH:mm:ss}&enddt={booking.EndDate:yyyy-MM-ddTHH:mm:ss}&body={description}&location={location}&path=/calendar/action/compose&rru=addevent",
            DisplayName = "Add to Office 365 Calendar",
            IconClass = "bi-calendar-check"
        };
        
        // Apple Calendar - Uses webcal: protocol with ICS
        links[CalendarProvider.Apple] = new CalendarLink
        {
            Provider = "Apple Calendar",
            DirectLink = $"data:text/calendar;charset=utf8,{Uri.EscapeDataString(GenerateIcsFile(booking))}",
            DisplayName = "Download for Apple Calendar",
            IconClass = "bi-apple"
        };
        
        // AOL Calendar - Uses ICS download
        links[CalendarProvider.AOL] = new CalendarLink
        {
            Provider = "AOL Calendar",
            DirectLink = $"data:text/calendar;charset=utf8,{Uri.EscapeDataString(GenerateIcsFile(booking))}",
            DisplayName = "Download for AOL Calendar",
            IconClass = "bi-calendar-event"
        };
        
        return links;
    }

    public async Task<CalendarSyncResult> SyncBookingToCalendar(Booking booking)
    {
        return await Task.Run(() =>
        {
            try
            {
                var icsContent = GenerateIcsFile(booking);
                var calendarLinks = GenerateCalendarLinks(booking);
                
                return new CalendarSyncResult
                {
                    Success = true,
                    Message = "Calendar sync completed successfully",
                    IcsFileContent = icsContent,
                    IcsFileName = $"booking-{booking.Id}.ics",
                    CalendarLinks = calendarLinks,
                    SyncedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new CalendarSyncResult
                {
                    Success = false,
                    Message = "Calendar sync failed",
                    Errors = new List<string> { ex.Message },
                    SyncedAt = DateTime.UtcNow
                };
            }
        });
    }

    // Helper Methods
    private string FormatDateTime(DateTime dt)
    {
        return dt.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
    }

    private string EscapeText(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        
        return text
            .Replace("\\", "\\\\")
            .Replace(",", "\\,")
            .Replace(";", "\\;")
            .Replace("\n", "\\n")
            .Replace("\r", "");
    }
}
