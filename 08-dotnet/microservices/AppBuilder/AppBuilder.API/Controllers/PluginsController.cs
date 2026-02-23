using AppBuilder.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PluginsController : ControllerBase
{
    private readonly IPluginService _plugins;

    public PluginsController(IPluginService plugins) => _plugins = plugins;

    [HttpGet("platforms")]
    public IActionResult GetPlatforms() => Ok(_plugins.GetPlatforms());
}
