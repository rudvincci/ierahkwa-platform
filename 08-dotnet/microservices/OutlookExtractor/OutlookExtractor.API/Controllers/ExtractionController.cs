using Microsoft.AspNetCore.Mvc;
using OutlookExtractor.Core.Interfaces;
using OutlookExtractor.Core.Models;

namespace OutlookExtractor.API.Controllers;

/// <summary>
/// Email Extraction Controller
/// Handles email extraction from Office 365/Outlook/Exchange
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExtractionController : ControllerBase
{
    private readonly IEmailExtractionService _extractionService;
    private readonly IExportService _exportService;
    private readonly IStatisticsService _statisticsService;
    private readonly ILogger<ExtractionController> _logger;

    public ExtractionController(
        IEmailExtractionService extractionService,
        IExportService exportService,
        IStatisticsService statisticsService,
        ILogger<ExtractionController> logger)
    {
        _extractionService = extractionService;
        _exportService = exportService;
        _statisticsService = statisticsService;
        _logger = logger;
    }

    /// <summary>
    /// Extract email addresses from emails only
    /// </summary>
    [HttpPost("emails")]
    public async Task<IActionResult> ExtractFromEmails([FromBody] ExtractionConfig config)
    {
        try
        {
            _logger.LogInformation("Starting email extraction from emails");
            var emails = await _extractionService.ExtractFromEmailsAsync(config);
            return Ok(new
            {
                success = true,
                count = emails.Count,
                uniqueCount = emails.Select(e => e.EmailAddress).Distinct().Count(),
                emails = emails
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting from emails");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Extract email addresses from calendar events
    /// </summary>
    [HttpPost("calendar")]
    public async Task<IActionResult> ExtractFromCalendar([FromBody] ExtractionConfig config)
    {
        try
        {
            _logger.LogInformation("Starting email extraction from calendar");
            var emails = await _extractionService.ExtractFromCalendarAsync(config);
            return Ok(new
            {
                success = true,
                count = emails.Count,
                uniqueCount = emails.Select(e => e.EmailAddress).Distinct().Count(),
                emails = emails
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting from calendar");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Extract email addresses from contacts
    /// </summary>
    [HttpPost("contacts")]
    public async Task<IActionResult> ExtractFromContacts([FromBody] ExtractionConfig config)
    {
        try
        {
            _logger.LogInformation("Starting email extraction from contacts");
            var emails = await _extractionService.ExtractFromContactsAsync(config);
            return Ok(new
            {
                success = true,
                count = emails.Count,
                uniqueCount = emails.Select(e => e.EmailAddress).Distinct().Count(),
                emails = emails
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting from contacts");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Extract email addresses from all sources (emails, calendar, contacts)
    /// </summary>
    [HttpPost("all")]
    public async Task<IActionResult> ExtractAll([FromBody] ExtractionConfig config)
    {
        try
        {
            _logger.LogInformation("Starting email extraction from all sources");
            var summary = await _extractionService.ExtractAllAsync(config);
            var emails = await _extractionService.GetAllExtractedEmailsAsync();
            
            return Ok(new
            {
                success = true,
                summary = summary,
                emails = emails
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting from all sources");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get all extracted emails
    /// </summary>
    [HttpGet("results")]
    public async Task<IActionResult> GetResults()
    {
        try
        {
            var emails = await _extractionService.GetAllExtractedEmailsAsync();
            return Ok(new
            {
                success = true,
                count = emails.Count,
                uniqueCount = emails.Select(e => e.EmailAddress).Distinct().Count(),
                emails = emails
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting results");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Clear all extracted data
    /// </summary>
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearData()
    {
        try
        {
            await _extractionService.ClearExtractedDataAsync();
            return Ok(new { success = true, message = "Extracted data cleared" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing data");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get statistics on extracted emails
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        try
        {
            var emails = await _extractionService.GetAllExtractedEmailsAsync();
            
            var domainStats = await _statisticsService.GetEmailsByDomainAsync(emails);
            var sourceStats = await _statisticsService.GetEmailsBySourceAsync(emails);
            var topEmails = await _statisticsService.GetTopFrequentEmailsAsync(emails, 10);

            return Ok(new
            {
                success = true,
                totalEmails = emails.Count,
                uniqueEmails = emails.Select(e => e.EmailAddress).Distinct().Count(),
                domainStatistics = domainStats,
                sourceStatistics = sourceStats,
                topFrequentEmails = topEmails
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
