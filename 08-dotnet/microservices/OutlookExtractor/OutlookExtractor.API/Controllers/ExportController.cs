using Microsoft.AspNetCore.Mvc;
using OutlookExtractor.Core.Interfaces;
using OutlookExtractor.Core.Models;

namespace OutlookExtractor.API.Controllers;

/// <summary>
/// Export Controller
/// Handles exporting extracted emails to Text and Excel files
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly IEmailExtractionService _extractionService;
    private readonly IExportService _exportService;
    private readonly ILogger<ExportController> _logger;

    public ExportController(
        IEmailExtractionService extractionService,
        IExportService exportService,
        ILogger<ExportController> logger)
    {
        _extractionService = extractionService;
        _exportService = exportService;
        _logger = logger;
    }

    /// <summary>
    /// Export extracted emails to Text file
    /// </summary>
    [HttpPost("text")]
    public async Task<IActionResult> ExportToText([FromBody] ExportRequest request)
    {
        try
        {
            var emails = await _extractionService.GetAllExtractedEmailsAsync();
            
            if (emails.Count == 0)
            {
                return BadRequest(new { success = false, message = "No emails to export. Please extract emails first." });
            }

            var outputPath = request.OutputPath ?? Path.Combine(Directory.GetCurrentDirectory(), "exports", $"emails_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt");
            var result = await _exportService.ExportToTextAsync(emails, outputPath);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting to text");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Export extracted emails to Excel file
    /// </summary>
    [HttpPost("excel")]
    public async Task<IActionResult> ExportToExcel([FromBody] ExportRequest request)
    {
        try
        {
            var emails = await _extractionService.GetAllExtractedEmailsAsync();
            
            if (emails.Count == 0)
            {
                return BadRequest(new { success = false, message = "No emails to export. Please extract emails first." });
            }

            var outputPath = request.OutputPath ?? Path.Combine(Directory.GetCurrentDirectory(), "exports", $"emails_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx");
            var result = await _exportService.ExportToExcelAsync(emails, outputPath);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting to Excel");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Export extracted emails to both Text and Excel files
    /// </summary>
    [HttpPost("both")]
    public async Task<IActionResult> ExportBoth([FromBody] ExportRequest request)
    {
        try
        {
            var emails = await _extractionService.GetAllExtractedEmailsAsync();
            
            if (emails.Count == 0)
            {
                return BadRequest(new { success = false, message = "No emails to export. Please extract emails first." });
            }

            var outputDirectory = request.OutputDirectory ?? Path.Combine(Directory.GetCurrentDirectory(), "exports");
            var result = await _exportService.ExportAsync(emails, outputDirectory, ExportFormat.Both);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting files");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Download exported file
    /// </summary>
    [HttpGet("download/{fileName}")]
    public IActionResult DownloadFile(string fileName)
    {
        try
        {
            var exportsDir = Path.Combine(Directory.GetCurrentDirectory(), "exports");
            var filePath = Path.Combine(exportsDir, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { success = false, message = "File not found" });
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = fileName.EndsWith(".xlsx") ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/plain";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}

/// <summary>
/// Export request model
/// </summary>
public class ExportRequest
{
    public string? OutputPath { get; set; }
    public string? OutputDirectory { get; set; }
}
