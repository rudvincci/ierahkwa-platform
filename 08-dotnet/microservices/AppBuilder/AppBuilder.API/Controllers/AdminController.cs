using AppBuilder.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IAppBuilderService _builder;
    private readonly IPushNotificationService _push;
    private readonly ILogger<AdminController> _log;

    public AdminController(IAuthService auth, IAppBuilderService builder, IPushNotificationService push, ILogger<AdminController> log)
    {
        _auth = auth;
        _builder = builder;
        _push = push;
        _log = log;
    }

    [HttpGet("stats")]
    public IActionResult Stats()
    {
        var projects = _builder.GetAllProjects();
        var builds = _builder.GetAllBuilds();
        var notifications = _push.GetAll();
        return Ok(new
        {
            users = _auth.GetUserCount(),
            projects = projects.Count,
            builds = builds.Count,
            buildsSuccess = builds.Count(b => b.Status == AppBuilder.Core.Models.BuildStatus.Success),
            buildsFailed = builds.Count(b => b.Status == AppBuilder.Core.Models.BuildStatus.Failed),
            pushNotifications = notifications.Count
        });
    }

    [HttpGet("builds")]
    public IActionResult Builds([FromQuery] int limit = 50)
    {
        var list = _builder.GetAllBuilds().Take(limit).ToList();
        return Ok(list);
    }

    [HttpGet("notifications")]
    public IActionResult Notifications([FromQuery] int limit = 50)
    {
        var list = _push.GetAll().Take(limit).ToList();
        return Ok(list);
    }
}
