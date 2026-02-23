using System.Security.Claims;
using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IAppBuilderService _builder;
    private readonly IInvoiceService _invoices;
    private readonly ILogger<UsersController> _log;

    public UsersController(IAuthService auth, IAppBuilderService builder, IInvoiceService invoices, ILogger<UsersController> log)
    {
        _auth = auth;
        _builder = builder;
        _invoices = invoices;
        _log = log;
    }

    private string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var u = _auth.GetUserById(UserId!);
        if (u == null) return NotFound(new { error = "User not found" });
        return Ok(new { u.Id, u.Email, u.Name, u.AvatarUrl, u.PlanTier, u.BuildCredits, u.CreatedAt, u.LastLoginAt });
    }

    /// <summary>GDPR: User data export. Profile, projects, builds, invoices.</summary>
    [HttpGet("export-data")]
    public IActionResult ExportData()
    {
        var id = UserId!;
        var u = _auth.GetUserById(id);
        if (u == null) return NotFound(new { error = "User not found" });
        var projects = _builder.GetProjectsByUser(id);
        var builds = projects.SelectMany(p => _builder.GetBuildsByProject(p.Id));
        var invs = _invoices.GetByUser(id);
        var data = new
        {
            exportedAt = DateTime.UtcNow,
            user = new { u.Id, u.Email, u.Name, u.PlanTier, u.BuildCredits, u.CreatedAt },
            projects = projects.Select(p => new { p.Id, p.Name, p.SourceUrl, p.CreatedAt }),
            builds = builds.Select(b => new { b.Id, b.AppProjectId, b.Platform, b.Version, b.Status, b.CreatedAt }),
            invoices = invs.Select(i => new { i.Id, i.Number, i.Amount, i.Status, i.CreatedAt })
        };
        return Ok(data);
    }

    /// <summary>GDPR: Account deletion. Removes user and their projects/builds.</summary>
    [HttpDelete("delete-account")]
    public IActionResult DeleteAccount()
    {
        var id = UserId!;
        var projects = _builder.GetProjectsByUser(id).ToList();
        foreach (var p in projects) _builder.DeleteProject(p.Id);
        if (!_auth.DeleteUser(id)) return NotFound(new { error = "User not found" });
        _log.LogInformation("IERAHKWA Appy: GDPR account deletion {UserId}", id);
        return NoContent();
    }
}
