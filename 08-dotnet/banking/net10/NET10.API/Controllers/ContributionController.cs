using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models;

namespace NET10.API.Controllers;

/// <summary>
/// GitHub-style contribution graph API
/// Track and visualize platform activity
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContributionController : ControllerBase
{
    private readonly IContributionService _contributionService;

    public ContributionController(IContributionService contributionService)
    {
        _contributionService = contributionService;
    }

    /// <summary>
    /// Get contribution graph for a user (current year)
    /// </summary>
    [HttpGet("graph/{userId}")]
    public async Task<ActionResult<ContributionGraph>> GetContributionGraph(string userId)
    {
        var graph = await _contributionService.GetContributionGraphAsync(userId);
        return Ok(graph);
    }

    /// <summary>
    /// Get contribution graph for a user (specific year)
    /// </summary>
    [HttpGet("graph/{userId}/{year}")]
    public async Task<ActionResult<ContributionGraph>> GetContributionGraphByYear(string userId, int year)
    {
        var graph = await _contributionService.GetContributionGraphAsync(userId, year);
        return Ok(graph);
    }

    /// <summary>
    /// Get contribution statistics for a user
    /// </summary>
    [HttpGet("stats/{userId}")]
    public async Task<ActionResult<ContributionStats>> GetContributionStats(string userId)
    {
        var stats = await _contributionService.GetContributionStatsAsync(userId);
        return Ok(stats);
    }

    /// <summary>
    /// Get daily contributions for a date range
    /// </summary>
    [HttpGet("daily/{userId}")]
    public async Task<ActionResult<List<DailyContribution>>> GetDailyContributions(
        string userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.AddYears(-1);
        var end = endDate ?? DateTime.UtcNow;
        var contributions = await _contributionService.GetDailyContributionsAsync(userId, start, end);
        return Ok(contributions);
    }

    /// <summary>
    /// Get contributions for a specific date
    /// </summary>
    [HttpGet("date/{userId}/{date}")]
    public async Task<ActionResult<DailyContribution>> GetContributionsForDate(string userId, DateTime date)
    {
        var contribution = await _contributionService.GetContributionsForDateAsync(userId, date);
        return Ok(contribution);
    }

    /// <summary>
    /// Get recent contributions for a user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Contribution>>> GetUserContributions(
        string userId,
        [FromQuery] int limit = 100)
    {
        var contributions = await _contributionService.GetUserContributionsAsync(userId, limit);
        return Ok(contributions);
    }

    /// <summary>
    /// Get contributions for a project
    /// </summary>
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<List<Contribution>>> GetProjectContributions(
        string projectId,
        [FromQuery] int limit = 100)
    {
        var contributions = await _contributionService.GetProjectContributionsAsync(projectId, limit);
        return Ok(contributions);
    }

    /// <summary>
    /// Get a single contribution by ID
    /// </summary>
    [HttpGet("{contributionId}")]
    public async Task<ActionResult<Contribution>> GetContribution(string contributionId)
    {
        var contribution = await _contributionService.GetContributionByIdAsync(contributionId);
        if (contribution == null)
            return NotFound();
        return Ok(contribution);
    }

    /// <summary>
    /// Add a new contribution
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Contribution>> AddContribution([FromBody] Contribution contribution)
    {
        var created = await _contributionService.AddContributionAsync(contribution);
        return CreatedAtAction(nameof(GetContribution), new { contributionId = created.Id }, created);
    }

    /// <summary>
    /// Add multiple contributions in batch
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<List<Contribution>>> AddContributionsBatch([FromBody] List<Contribution> contributions)
    {
        var created = await _contributionService.AddContributionsBatchAsync(contributions);
        return Ok(created);
    }

    /// <summary>
    /// Delete a contribution
    /// </summary>
    [HttpDelete("{contributionId}")]
    public async Task<ActionResult> DeleteContribution(string contributionId)
    {
        var success = await _contributionService.DeleteContributionAsync(contributionId);
        if (!success)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Get user's projects with activity
    /// </summary>
    [HttpGet("projects/{userId}")]
    public async Task<ActionResult<List<ProjectActivity>>> GetUserProjects(string userId)
    {
        var projects = await _contributionService.GetUserProjectsAsync(userId);
        return Ok(projects);
    }

    /// <summary>
    /// Get project activity details
    /// </summary>
    [HttpGet("project-activity/{projectId}")]
    public async Task<ActionResult<ProjectActivity>> GetProjectActivity(string projectId)
    {
        var activity = await _contributionService.GetProjectActivityAsync(projectId);
        if (activity == null)
            return NotFound();
        return Ok(activity);
    }

    /// <summary>
    /// Get top contributors (all time)
    /// </summary>
    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<ContributionStats>>> GetTopContributors([FromQuery] int limit = 10)
    {
        var stats = await _contributionService.GetTopContributorsAsync(limit);
        return Ok(stats);
    }

    /// <summary>
    /// Get top contributors this month
    /// </summary>
    [HttpGet("leaderboard/month")]
    public async Task<ActionResult<List<ContributionStats>>> GetTopContributorsThisMonth([FromQuery] int limit = 10)
    {
        var stats = await _contributionService.GetTopContributorsThisMonthAsync(limit);
        return Ok(stats);
    }

    /// <summary>
    /// Get top contributors this week
    /// </summary>
    [HttpGet("leaderboard/week")]
    public async Task<ActionResult<List<ContributionStats>>> GetTopContributorsThisWeek([FromQuery] int limit = 10)
    {
        var stats = await _contributionService.GetTopContributorsThisWeekAsync(limit);
        return Ok(stats);
    }
}
