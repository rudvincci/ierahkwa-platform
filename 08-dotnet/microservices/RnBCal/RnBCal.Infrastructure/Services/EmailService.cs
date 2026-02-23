using RnBCal.Core.Interfaces;
using RnBCal.Core.Models;
using System.Net;
using System.Net.Mail;

namespace RnBCal.Infrastructure.Services;

/// <summary>
/// Email Service Implementation - IERAHKWA RnBCal
/// Sends booking confirmation emails with calendar attachments
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailConfiguration _config;
    private readonly ICalendarService _calendarService;

    public EmailService(EmailConfiguration config, ICalendarService calendarService)
    {
        _config = config;
        _calendarService = calendarService;
    }

    public async Task<bool> SendBookingConfirmationEmail(Booking booking, string icsContent)
    {
        try
        {
            var calendarLinks = _calendarService.GenerateCalendarLinks(booking);
            
            var subject = $"Booking Confirmation: {booking.ItemName}";
            var body = BuildBookingConfirmationEmail(booking, calendarLinks);
            
            return await SendEmailWithIcsAttachment(
                booking.CustomerEmail,
                booking.CustomerName,
                subject,
                body,
                icsContent,
                $"booking-{booking.Id}.ics"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Email send failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendCalendarUpdateEmail(Booking booking, string icsContent)
    {
        try
        {
            var calendarLinks = _calendarService.GenerateCalendarLinks(booking);
            
            var subject = $"Booking Update: {booking.ItemName}";
            var body = BuildBookingUpdateEmail(booking, calendarLinks);
            
            return await SendEmailWithIcsAttachment(
                booking.CustomerEmail,
                booking.CustomerName,
                subject,
                body,
                icsContent,
                $"booking-{booking.Id}.ics"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Email send failed: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> SendEmailWithIcsAttachment(
        string toEmail,
        string toName,
        string subject,
        string body,
        string icsContent,
        string icsFileName)
    {
        if (!_config.Enabled)
        {
            Console.WriteLine("📧 Email sending is disabled in configuration");
            return false;
        }

        using var message = new MailMessage();
        message.From = new MailAddress(_config.FromEmail, _config.FromName);
        message.To.Add(new MailAddress(toEmail, toName));
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;
        
        // Attach ICS file
        var icsBytes = System.Text.Encoding.UTF8.GetBytes(icsContent);
        var icsStream = new MemoryStream(icsBytes);
        var attachment = new Attachment(icsStream, icsFileName, "text/calendar");
        message.Attachments.Add(attachment);
        
        using var client = new SmtpClient(_config.SmtpHost, _config.SmtpPort);
        client.EnableSsl = _config.UseSsl;
        client.Credentials = new NetworkCredential(_config.Username, _config.Password);
        
        await client.SendMailAsync(message);
        Console.WriteLine($"✅ Email sent to {toEmail}");
        return true;
    }

    private string BuildBookingConfirmationEmail(Booking booking, Dictionary<CalendarProvider, CalendarLink> calendarLinks)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; background: #f5f5f5; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #FFD700 0%, #B8860B 100%); color: white; padding: 30px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .header p {{ margin: 5px 0 0 0; opacity: 0.9; }}
        .content {{ padding: 30px; }}
        .booking-details {{ background: #f9f9f9; border-left: 4px solid #FFD700; padding: 15px; margin: 20px 0; }}
        .booking-details h3 {{ margin-top: 0; color: #333; }}
        .detail-row {{ margin: 10px 0; color: #666; }}
        .detail-label {{ font-weight: bold; color: #333; }}
        .calendar-buttons {{ margin: 25px 0; }}
        .calendar-button {{ display: inline-block; margin: 5px; padding: 12px 20px; background: #FFD700; color: #333; text-decoration: none; border-radius: 5px; font-weight: bold; }}
        .calendar-button:hover {{ background: #B8860B; color: white; }}
        .footer {{ background: #0a0e17; color: #999; padding: 20px; text-align: center; font-size: 12px; }}
        .footer a {{ color: #FFD700; text-decoration: none; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🏛️ Booking Confirmed!</h1>
            <p>IERAHKWA Rental & Booking System</p>
        </div>
        <div class='content'>
            <p>Dear {booking.CustomerName},</p>
            <p>Your booking has been confirmed. Here are the details:</p>
            
            <div class='booking-details'>
                <h3>📅 Booking Information</h3>
                <div class='detail-row'><span class='detail-label'>Booking ID:</span> {booking.Id}</div>
                <div class='detail-row'><span class='detail-label'>Item:</span> {booking.ItemName} ({booking.ItemType})</div>
                <div class='detail-row'><span class='detail-label'>Start Date:</span> {booking.StartDate:dddd, MMMM d, yyyy h:mm tt}</div>
                <div class='detail-row'><span class='detail-label'>End Date:</span> {booking.EndDate:dddd, MMMM d, yyyy h:mm tt}</div>
                <div class='detail-row'><span class='detail-label'>Location:</span> {booking.Location}</div>
                <div class='detail-row'><span class='detail-label'>Total Amount:</span> {booking.Currency} {booking.TotalAmount:N2}</div>
                <div class='detail-row'><span class='detail-label'>Status:</span> {booking.Status}</div>
            </div>
            
            <h3>📆 Add to Your Calendar</h3>
            <p>Click one of the buttons below to add this booking to your calendar:</p>
            <div class='calendar-buttons'>
                {string.Join("", calendarLinks.Select(kvp => 
                    $"<a href='{kvp.Value.DirectLink}' class='calendar-button'>{kvp.Value.DisplayName}</a>"))}
            </div>
            
            <p><strong>Note:</strong> A calendar file (.ics) is attached to this email for your convenience.</p>
            
            <p>If you have any questions, please contact us at bookings@ierahkwa.gov</p>
            
            <p>Thank you for choosing IERAHKWA!</p>
        </div>
        <div class='footer'>
            <p><strong>🏛️ Sovereign Government of Ierahkwa Ne Kanienke</strong></p>
            <p>IERAHKWA RnBCal System • Rental & Booking Calendar Sync</p>
            <p><a href='https://ierahkwa.gov'>ierahkwa.gov</a> • © 2026 All Rights Reserved</p>
        </div>
    </div>
</body>
</html>";
    }

    private string BuildBookingUpdateEmail(Booking booking, Dictionary<CalendarProvider, CalendarLink> calendarLinks)
    {
        return BuildBookingConfirmationEmail(booking, calendarLinks)
            .Replace("Booking Confirmed!", "Booking Updated!")
            .Replace("Your booking has been confirmed", "Your booking has been updated");
    }
}

public class EmailConfiguration
{
    public bool Enabled { get; set; } = false;
    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "bookings@ierahkwa.gov";
    public string FromName { get; set; } = "IERAHKWA Booking System";
}
