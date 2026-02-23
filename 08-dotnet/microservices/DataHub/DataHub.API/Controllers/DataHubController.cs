using DataHub.Core.Interfaces;
using DataHub.Core.Models;
using Microsoft.AspNetCore.Mvc;
namespace DataHub.API.Controllers;

[ApiController]
[Route("api/data-sources")]
public class DataSourcesController : ControllerBase
{
    private readonly IDataHubService _service;
    public DataSourcesController(IDataHubService service) => _service = service;

    [HttpPost] public async Task<ActionResult<DataSource>> Create([FromBody] DataSource source) => await _service.CreateDataSourceAsync(source);
    [HttpGet("{id}")] public async Task<ActionResult<DataSource>> GetById(Guid id) { var s = await _service.GetDataSourceByIdAsync(id); return s == null ? NotFound() : s; }
    [HttpGet] public async Task<ActionResult<IEnumerable<DataSource>>> GetAll([FromQuery] DataSourceType? type, [FromQuery] bool? isActive) => Ok(await _service.GetDataSourcesAsync(type, isActive));
    [HttpPut("{id}")] public async Task<ActionResult<DataSource>> Update(Guid id, [FromBody] DataSource source) => await _service.UpdateDataSourceAsync(source);
    [HttpPost("{id}/test")] public async Task<ActionResult<DataSource>> Test(Guid id) => await _service.TestConnectionAsync(id);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteDataSourceAsync(id); return NoContent(); }
}

[ApiController]
[Route("api/pipelines")]
public class PipelinesController : ControllerBase
{
    private readonly IDataHubService _service;
    public PipelinesController(IDataHubService service) => _service = service;

    [HttpPost] public async Task<ActionResult<DataPipeline>> Create([FromBody] DataPipeline pipeline) => await _service.CreatePipelineAsync(pipeline);
    [HttpGet("{id}")] public async Task<ActionResult<DataPipeline>> GetById(Guid id) { var p = await _service.GetPipelineByIdAsync(id); return p == null ? NotFound() : p; }
    [HttpGet] public async Task<ActionResult<IEnumerable<DataPipeline>>> GetAll([FromQuery] PipelineStatus? status, [FromQuery] bool? isActive) => Ok(await _service.GetPipelinesAsync(status, isActive));
    [HttpPut("{id}")] public async Task<ActionResult<DataPipeline>> Update(Guid id, [FromBody] DataPipeline pipeline) => await _service.UpdatePipelineAsync(pipeline);
    [HttpPost("{id}/activate")] public async Task<ActionResult<DataPipeline>> Activate(Guid id) => await _service.ActivatePipelineAsync(id);
    [HttpPost("{id}/deactivate")] public async Task<ActionResult<DataPipeline>> Deactivate(Guid id) => await _service.DeactivatePipelineAsync(id);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeletePipelineAsync(id); return NoContent(); }
    [HttpPost("{id}/steps")] public async Task<ActionResult<PipelineStep>> AddStep(Guid id, [FromBody] PipelineStep step) { step.PipelineId = id; return await _service.AddPipelineStepAsync(step); }
    [HttpGet("{id}/steps")] public async Task<ActionResult<IEnumerable<PipelineStep>>> GetSteps(Guid id) => Ok(await _service.GetPipelineStepsAsync(id));
    [HttpPost("{id}/execute")] public async Task<ActionResult<PipelineExecution>> Execute(Guid id, [FromQuery] Guid? triggeredBy) => await _service.ExecutePipelineAsync(id, triggeredBy);
    [HttpGet("{id}/executions")] public async Task<ActionResult<IEnumerable<PipelineExecution>>> GetExecutions(Guid id) => Ok(await _service.GetExecutionsAsync(id));
}

[ApiController]
[Route("api/api-endpoints")]
public class ApiEndpointsController : ControllerBase
{
    private readonly IDataHubService _service;
    public ApiEndpointsController(IDataHubService service) => _service = service;

    [HttpPost] public async Task<ActionResult<ApiEndpoint>> Create([FromBody] ApiEndpoint endpoint) => await _service.CreateEndpointAsync(endpoint);
    [HttpGet] public async Task<ActionResult<IEnumerable<ApiEndpoint>>> GetAll([FromQuery] Guid? dataSourceId) => Ok(await _service.GetEndpointsAsync(dataSourceId));
    [HttpPut("{id}")] public async Task<ActionResult<ApiEndpoint>> Update(Guid id, [FromBody] ApiEndpoint endpoint) => await _service.UpdateEndpointAsync(endpoint);
    [HttpPost("{id}/execute")] public async Task<ActionResult<object>> Execute(Guid id, [FromBody] Dictionary<string, object>? parameters) => Ok(await _service.ExecuteEndpointAsync(id, parameters));
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteEndpointAsync(id); return NoContent(); }
}

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly IDataHubService _service;
    public WebhooksController(IDataHubService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Webhook>> Create([FromBody] Webhook webhook) => await _service.CreateWebhookAsync(webhook);
    [HttpGet] public async Task<ActionResult<IEnumerable<Webhook>>> GetAll() => Ok(await _service.GetWebhooksAsync());
    [HttpPut("{id}")] public async Task<ActionResult<Webhook>> Update(Guid id, [FromBody] Webhook webhook) => await _service.UpdateWebhookAsync(webhook);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteWebhookAsync(id); return NoContent(); }
    [HttpPost("{id}/deliver")] public async Task<ActionResult<WebhookDelivery>> Deliver(Guid id, [FromQuery] string eventType, [FromBody] object payload) => await _service.DeliverWebhookAsync(id, eventType, payload);
    [HttpGet("{id}/deliveries")] public async Task<ActionResult<IEnumerable<WebhookDelivery>>> GetDeliveries(Guid id) => Ok(await _service.GetWebhookDeliveriesAsync(id));
}

[ApiController]
[Route("api/datahub")]
public class DataHubStatsController : ControllerBase
{
    private readonly IDataHubService _service;
    public DataHubStatsController(IDataHubService service) => _service = service;
    [HttpGet("statistics")] public async Task<ActionResult<DataHubStatistics>> GetStatistics() => await _service.GetStatisticsAsync();
}
