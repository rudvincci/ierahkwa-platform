using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers;

/// <summary>
/// Google Maps Data Scraper PRO Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GoogleMapsScraperController : ControllerBase
{
    private readonly IGoogleMapsScraperService _scraperService;
    
    public GoogleMapsScraperController(IGoogleMapsScraperService scraperService)
    {
        _scraperService = scraperService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PROJECT MANAGEMENT
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Create new scraping project
    /// </summary>
    [HttpPost("projects")]
    public async Task<ActionResult<ScrapingProject>> CreateProject([FromBody] CreateScrapingProjectRequest request)
    {
        try
        {
            var project = await _scraperService.CreateProjectAsync(request);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get all projects
    /// </summary>
    [HttpGet("projects")]
    public async Task<ActionResult<List<ScrapingProject>>> GetAllProjects()
    {
        var projects = await _scraperService.GetAllProjectsAsync();
        return Ok(projects);
    }
    
    /// <summary>
    /// Get project by ID
    /// </summary>
    [HttpGet("projects/{id}")]
    public async Task<ActionResult<ScrapingProject>> GetProject(Guid id)
    {
        var project = await _scraperService.GetProjectAsync(id);
        if (project == null) return NotFound();
        return Ok(project);
    }
    
    /// <summary>
    /// Delete project
    /// </summary>
    [HttpDelete("projects/{id}")]
    public async Task<ActionResult> DeleteProject(Guid id)
    {
        var result = await _scraperService.DeleteProjectAsync(id);
        return result ? NoContent() : NotFound();
    }
    
    // ═══════════════════════════════════════════════════════════════
    // SCRAPING CONTROL
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Start scraping
    /// </summary>
    [HttpPost("projects/{id}/start")]
    public async Task<ActionResult<ScrapingProject>> StartScraping(Guid id)
    {
        try
        {
            var project = await _scraperService.StartScrapingAsync(id);
            return Ok(project);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Stop scraping
    /// </summary>
    [HttpPost("projects/{id}/stop")]
    public async Task<ActionResult<ScrapingProject>> StopScraping(Guid id)
    {
        try
        {
            var project = await _scraperService.StopScrapingAsync(id);
            return Ok(project);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Pause scraping
    /// </summary>
    [HttpPost("projects/{id}/pause")]
    public async Task<ActionResult<ScrapingProject>> PauseScraping(Guid id)
    {
        try
        {
            var project = await _scraperService.PauseScrapingAsync(id);
            return Ok(project);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Resume scraping
    /// </summary>
    [HttpPost("projects/{id}/resume")]
    public async Task<ActionResult<ScrapingProject>> ResumeScraping(Guid id)
    {
        try
        {
            var project = await _scraperService.ResumeScrapingAsync(id);
            return Ok(project);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PROGRESS & DATA
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get scraping progress
    /// </summary>
    [HttpGet("projects/{id}/progress")]
    public async Task<ActionResult<ScrapingProgress>> GetProgress(Guid id)
    {
        try
        {
            var progress = await _scraperService.GetProgressAsync(id);
            return Ok(progress);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get scraped organizations
    /// </summary>
    [HttpGet("projects/{id}/organizations")]
    public async Task<ActionResult<List<ScrapedOrganization>>> GetOrganizations(
        Guid id, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var orgs = await _scraperService.GetOrganizationsAsync(id, page, pageSize);
            return Ok(orgs);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get organization by ID
    /// </summary>
    [HttpGet("organizations/{id}")]
    public async Task<ActionResult<ScrapedOrganization>> GetOrganization(Guid id)
    {
        var org = await _scraperService.GetOrganizationAsync(id);
        if (org == null) return NotFound();
        return Ok(org);
    }
    
    /// <summary>
    /// Get organization reviews
    /// </summary>
    [HttpGet("organizations/{id}/reviews")]
    public async Task<ActionResult<List<Review>>> GetOrganizationReviews(Guid id)
    {
        var reviews = await _scraperService.GetOrganizationReviewsAsync(id);
        return Ok(reviews);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // EXPORT
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Export to XLSX
    /// </summary>
    [HttpPost("projects/{id}/export/xlsx")]
    public async Task<ActionResult<ExportResult>> ExportToXlsx(Guid id, [FromQuery] string? outputPath = null)
    {
        try
        {
            var result = await _scraperService.ExportToXlsxAsync(id, outputPath);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Export to CSV
    /// </summary>
    [HttpPost("projects/{id}/export/csv")]
    public async Task<ActionResult<ExportResult>> ExportToCsv(Guid id, [FromQuery] string? outputPath = null)
    {
        try
        {
            var result = await _scraperService.ExportToCsvAsync(id, outputPath);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Export to JSON
    /// </summary>
    [HttpPost("projects/{id}/export/json")]
    public async Task<ActionResult<ExportResult>> ExportToJson(Guid id, [FromQuery] string? outputPath = null)
    {
        try
        {
            var result = await _scraperService.ExportToJsonAsync(id, outputPath);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Export to MySQL
    /// </summary>
    [HttpPost("projects/{id}/export/mysql")]
    public async Task<ActionResult<ExportResult>> ExportToMySql(Guid id, [FromBody] MySqlExportRequest request)
    {
        try
        {
            var result = await _scraperService.ExportToMySqlAsync(id, request.ConnectionString);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Export to XML
    /// </summary>
    [HttpPost("projects/{id}/export/xml")]
    public async Task<ActionResult<ExportResult>> ExportToXml(Guid id, [FromQuery] string? outputPath = null)
    {
        try
        {
            var result = await _scraperService.ExportToXmlAsync(id, outputPath);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class MySqlExportRequest
{
    public string ConnectionString { get; set; } = string.Empty;
}
