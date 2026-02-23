using Microsoft.AspNetCore.Mvc;
using ReportEngine.Core.Interfaces;
using ReportEngine.Core.Models;
namespace ReportEngine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;
    public ReportsController(IReportService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Report>> Create([FromBody] Report report) => await _service.CreateReportAsync(report);
    [HttpGet("{id}")] public async Task<ActionResult<Report>> GetById(Guid id) { var r = await _service.GetReportByIdAsync(id); return r == null ? NotFound() : r; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Report>>> GetAll([FromQuery] ReportCategory? category, [FromQuery] string? department, [FromQuery] bool? isPublic) => Ok(await _service.GetReportsAsync(category, department, isPublic));
    [HttpPut("{id}")] public async Task<ActionResult<Report>> Update(Guid id, [FromBody] Report report) => await _service.UpdateReportAsync(report);
    [HttpPost("{id}/publish")] public async Task<ActionResult<Report>> Publish(Guid id) => await _service.PublishReportAsync(id);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteReportAsync(id); return NoContent(); }
    [HttpPost("{id}/execute")] public async Task<ActionResult<ReportExecution>> Execute(Guid id, [FromQuery] Guid userId, [FromQuery] string userName, [FromQuery] OutputFormat format) => await _service.ExecuteReportAsync(id, userId, userName, format);
    [HttpGet("{id}/executions")] public async Task<ActionResult<IEnumerable<ReportExecution>>> GetExecutions(Guid id) => Ok(await _service.GetExecutionsAsync(id));
    [HttpGet("statistics")] public async Task<ActionResult<ReportStatistics>> GetStatistics() => await _service.GetStatisticsAsync();
}

[ApiController]
[Route("api/[controller]")]
public class DashboardsController : ControllerBase
{
    private readonly IReportService _service;
    public DashboardsController(IReportService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Dashboard>> Create([FromBody] Dashboard dashboard) => await _service.CreateDashboardAsync(dashboard);
    [HttpGet("{id}")] public async Task<ActionResult<Dashboard>> GetById(Guid id) { var d = await _service.GetDashboardByIdAsync(id); return d == null ? NotFound() : d; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Dashboard>>> GetAll([FromQuery] string? department, [FromQuery] bool? isPublic) => Ok(await _service.GetDashboardsAsync(department, isPublic));
    [HttpPut("{id}")] public async Task<ActionResult<Dashboard>> Update(Guid id, [FromBody] Dashboard dashboard) => await _service.UpdateDashboardAsync(dashboard);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteDashboardAsync(id); return NoContent(); }
    [HttpPost("{id}/widgets")] public async Task<ActionResult<DashboardWidget>> AddWidget(Guid id, [FromBody] DashboardWidget widget) { widget.DashboardId = id; return await _service.AddWidgetAsync(widget); }
    [HttpGet("widgets/{widgetId}/data")] public async Task<ActionResult<object>> GetWidgetData(Guid widgetId) => Ok(await _service.GetWidgetDataAsync(widgetId));
}
