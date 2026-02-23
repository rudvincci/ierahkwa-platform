using AppBuilder.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PreviewController : ControllerBase
{
    private readonly IAppBuilderService _builder;

    public PreviewController(IAppBuilderService builder) => _builder = builder;

    /// <summary>Browser-based app preview. No install. Appy: Test apps instantly.</summary>
    [HttpGet("{projectId}")]
    public IActionResult Index(string projectId)
    {
        var p = _builder.GetProjectById(projectId);
        if (p == null) return NotFound(new { error = "Project not found" });
        var d = p.Design;
        var themeColor = d.PrimaryColor ?? "#1a237e";
        var bg = d.BackgroundColor ?? "#0d1117";
        var html = $@"<!DOCTYPE html><html><head><meta charset=""utf-8""/><meta name=""viewport"" content=""width=device-width,initial-scale=1""/>
<title>Preview – {System.Net.WebUtility.HtmlEncode(p.Name)}</title>
<style>
*{{box-sizing:border-box;}} body{{margin:0;font-family:Segoe UI,sans-serif;background:{bg};color:#e6e6e6;display:flex;flex-direction:column;align-items:center;padding:1rem;min-height:100vh;}}
h1{{color:{themeColor};font-size:1.1rem;}}
.frame{{border:2px solid {themeColor};border-radius:12px;overflow:hidden;box-shadow:0 8px 24px rgba(0,0,0,.4);max-width:420px;width:100%;height:700px;background:#fff;}}
iframe{{width:100%;height:100%;border:0;}}
.footer{{margin-top:1rem;font-size:0.8rem;opacity:0.7;}}
</style></head><body>
<h1>IERAHKWA Appy – Browser Preview</h1>
<p>{System.Net.WebUtility.HtmlEncode(p.Name)}</p>
<div class=""frame""><iframe src=""{(p.SourceUrl ?? "").Replace("&", "&amp;").Replace("\"", "&quot;")}"" title=""{System.Net.WebUtility.HtmlEncode(p.Name)}""></iframe></div>
<div class=""footer"">Sovereign Government of Ierahkwa Ne Kanienke · No install required</div>
</body></html>";
        return Content(html, "text/html");
    }
}
