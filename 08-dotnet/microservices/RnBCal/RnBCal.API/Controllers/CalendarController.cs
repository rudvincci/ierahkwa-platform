using Microsoft.AspNetCore.Mvc;
using RnBCal.Core.Interfaces;
using RnBCal.Core.Models;

namespace RnBCal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;
    private readonly IEmailService _emailService;
    private readonly IGoogleCalendarService _googleCalendarService;
    private readonly ILogger<CalendarController> _logger;

    public CalendarController(
        ICalendarService calendarService,
        IEmailService emailService,
        IGoogleCalendarService googleCalendarService,
        ILogger<CalendarController> logger)
    {
        _calendarService = calendarService;
        _emailService = emailService;
        _googleCalendarService = googleCalendarService;
        _logger = logger;
    }

    /// <summary>
    /// Generate ICS file for a booking
    /// </summary>
    [HttpPost("generate-ics")]
    public IActionResult GenerateIcs([FromBody] Booking booking)
    {
        try
        {
            var icsContent = _calendarService.GenerateIcsFile(booking);
            var fileName = $"booking-{booking.Id}.ics";
            
            return File(
                System.Text.Encoding.UTF8.GetBytes(icsContent),
                "text/calendar",
                fileName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate ICS file");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get direct calendar links for all providers
    /// </summary>
    [HttpPost("calendar-links")]
    public IActionResult GetCalendarLinks([FromBody] Booking booking)
    {
        try
        {
            var links = _calendarService.GenerateCalendarLinks(booking);
            return Ok(links);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate calendar links");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Sync booking to calendar (generate ICS + links)
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncBooking([FromBody] Booking booking)
    {
        try
        {
            var result = await _calendarService.SyncBookingToCalendar(booking);
            
            // Attempt auto-sync to Google Calendar if configured
            if (result.Success)
            {
                _ = _googleCalendarService.AutoSyncToGoogleCalendar(booking);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync booking");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Send booking confirmation email with calendar attachment
    /// </summary>
    [HttpPost("send-confirmation")]
    public async Task<IActionResult> SendConfirmationEmail([FromBody] Booking booking)
    {
        try
        {
            var icsContent = _calendarService.GenerateIcsFile(booking);
            var success = await _emailService.SendBookingConfirmationEmail(booking, icsContent);
            
            if (success)
            {
                return Ok(new { message = "Confirmation email sent successfully" });
            }
            else
            {
                return StatusCode(500, new { error = "Failed to send email" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get Google Calendar OAuth URL
    /// </summary>
    [HttpGet("google/oauth-url")]
    public IActionResult GetGoogleOAuthUrl()
    {
        var url = _googleCalendarService.GetOAuthUrl();
        return Ok(new { oauthUrl = url });
    }

    /// <summary>
    /// Google Calendar OAuth callback (placeholder for OAuth flow)
    /// </summary>
    [HttpGet("google/callback")]
    public IActionResult GoogleCallback([FromQuery] string code)
    {
        // In production, exchange code for access token
        return Ok(new { message = "OAuth callback received", code });
    }
}
