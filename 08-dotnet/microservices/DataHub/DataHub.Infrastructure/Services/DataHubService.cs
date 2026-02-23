using DataHub.Core.Interfaces;
using DataHub.Core.Models;
namespace DataHub.Infrastructure.Services;

public class DataHubService : IDataHubService
{
    private readonly List<DataSource> _sources = new();
    private readonly List<DataPipeline> _pipelines = new();
    private readonly List<PipelineStep> _steps = new();
    private readonly List<PipelineExecution> _executions = new();
    private readonly List<DataMapping> _mappings = new();
    private readonly List<ApiEndpoint> _endpoints = new();
    private readonly List<Webhook> _webhooks = new();
    private readonly List<WebhookDelivery> _deliveries = new();

    public Task<DataSource> CreateDataSourceAsync(DataSource source) { source.Id = Guid.NewGuid(); source.Status = DataSourceStatus.Disconnected; source.CreatedAt = DateTime.UtcNow; _sources.Add(source); return Task.FromResult(source); }
    public Task<DataSource?> GetDataSourceByIdAsync(Guid id) => Task.FromResult(_sources.FirstOrDefault(s => s.Id == id));
    public Task<IEnumerable<DataSource>> GetDataSourcesAsync(DataSourceType? type = null, bool? isActive = null) { var q = _sources.AsEnumerable(); if (type.HasValue) q = q.Where(s => s.Type == type.Value); if (isActive.HasValue) q = q.Where(s => s.IsActive == isActive.Value); return Task.FromResult(q); }
    public Task<DataSource> UpdateDataSourceAsync(DataSource source) { var e = _sources.FirstOrDefault(s => s.Id == source.Id); if (e != null) { e.Name = source.Name; e.ConnectionString = source.ConnectionString; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? source); }
    public Task<DataSource> TestConnectionAsync(Guid id) { var s = _sources.FirstOrDefault(s => s.Id == id); if (s != null) s.Status = DataSourceStatus.Connected; return Task.FromResult(s!); }
    public Task DeleteDataSourceAsync(Guid id) { _sources.RemoveAll(s => s.Id == id); return Task.CompletedTask; }

    public Task<DataPipeline> CreatePipelineAsync(DataPipeline pipeline) { pipeline.Id = Guid.NewGuid(); pipeline.Status = PipelineStatus.Idle; pipeline.CreatedAt = DateTime.UtcNow; _pipelines.Add(pipeline); return Task.FromResult(pipeline); }
    public Task<DataPipeline?> GetPipelineByIdAsync(Guid id) { var p = _pipelines.FirstOrDefault(p => p.Id == id); if (p != null) p.Steps = _steps.Where(s => s.PipelineId == id).OrderBy(s => s.OrderIndex).ToList(); return Task.FromResult(p); }
    public Task<IEnumerable<DataPipeline>> GetPipelinesAsync(PipelineStatus? status = null, bool? isActive = null) { var q = _pipelines.AsEnumerable(); if (status.HasValue) q = q.Where(p => p.Status == status.Value); if (isActive.HasValue) q = q.Where(p => p.IsActive == isActive.Value); return Task.FromResult(q); }
    public Task<DataPipeline> UpdatePipelineAsync(DataPipeline pipeline) { var e = _pipelines.FirstOrDefault(p => p.Id == pipeline.Id); if (e != null) { e.Name = pipeline.Name; e.TransformationRules = pipeline.TransformationRules; } return Task.FromResult(e ?? pipeline); }
    public Task<DataPipeline> ActivatePipelineAsync(Guid id) { var p = _pipelines.FirstOrDefault(p => p.Id == id); if (p != null) p.IsActive = true; return Task.FromResult(p!); }
    public Task<DataPipeline> DeactivatePipelineAsync(Guid id) { var p = _pipelines.FirstOrDefault(p => p.Id == id); if (p != null) p.IsActive = false; return Task.FromResult(p!); }
    public Task DeletePipelineAsync(Guid id) { _pipelines.RemoveAll(p => p.Id == id); _steps.RemoveAll(s => s.PipelineId == id); return Task.CompletedTask; }

    public Task<PipelineStep> AddPipelineStepAsync(PipelineStep step) { step.Id = Guid.NewGuid(); step.OrderIndex = _steps.Count(s => s.PipelineId == step.PipelineId); _steps.Add(step); return Task.FromResult(step); }
    public Task<IEnumerable<PipelineStep>> GetPipelineStepsAsync(Guid pipelineId) => Task.FromResult(_steps.Where(s => s.PipelineId == pipelineId).OrderBy(s => s.OrderIndex));
    public Task<PipelineStep> UpdatePipelineStepAsync(PipelineStep step) { var e = _steps.FirstOrDefault(s => s.Id == step.Id); if (e != null) { e.Name = step.Name; e.Configuration = step.Configuration; } return Task.FromResult(e ?? step); }
    public Task RemovePipelineStepAsync(Guid stepId) { _steps.RemoveAll(s => s.Id == stepId); return Task.CompletedTask; }

    public Task<PipelineExecution> ExecutePipelineAsync(Guid pipelineId, Guid? triggeredBy = null)
    {
        var pipeline = _pipelines.FirstOrDefault(p => p.Id == pipelineId); if (pipeline == null) throw new Exception("Pipeline not found");
        var execution = new PipelineExecution { Id = Guid.NewGuid(), PipelineId = pipelineId, PipelineName = pipeline.Name, Status = ExecutionStatus.Running, StartedAt = DateTime.UtcNow, TriggeredBy = triggeredBy };
        _executions.Add(execution);
        execution.Status = ExecutionStatus.Completed; execution.CompletedAt = DateTime.UtcNow; execution.DurationMs = 1500; execution.RecordsRead = 1000; execution.RecordsWritten = 1000;
        pipeline.Status = PipelineStatus.Idle; pipeline.LastRunAt = DateTime.UtcNow; pipeline.RecordsProcessed += execution.RecordsWritten;
        return Task.FromResult(execution);
    }
    public Task<IEnumerable<PipelineExecution>> GetExecutionsAsync(Guid? pipelineId = null, ExecutionStatus? status = null) { var q = _executions.AsEnumerable(); if (pipelineId.HasValue) q = q.Where(e => e.PipelineId == pipelineId.Value); if (status.HasValue) q = q.Where(e => e.Status == status.Value); return Task.FromResult(q.OrderByDescending(e => e.StartedAt)); }
    public Task<PipelineExecution> CancelExecutionAsync(Guid executionId) { var e = _executions.FirstOrDefault(e => e.Id == executionId); if (e != null && e.Status == ExecutionStatus.Running) { e.Status = ExecutionStatus.Cancelled; e.CompletedAt = DateTime.UtcNow; } return Task.FromResult(e!); }

    public Task<DataMapping> CreateMappingAsync(DataMapping mapping) { mapping.Id = Guid.NewGuid(); mapping.CreatedAt = DateTime.UtcNow; _mappings.Add(mapping); return Task.FromResult(mapping); }
    public Task<IEnumerable<DataMapping>> GetMappingsAsync() => Task.FromResult(_mappings.Where(m => m.IsActive));
    public Task<DataMapping> UpdateMappingAsync(DataMapping mapping) { var e = _mappings.FirstOrDefault(m => m.Id == mapping.Id); if (e != null) e.MappingDefinition = mapping.MappingDefinition; return Task.FromResult(e ?? mapping); }
    public Task<object> TransformDataAsync(Guid mappingId, object sourceData) => Task.FromResult(sourceData);

    public Task<ApiEndpoint> CreateEndpointAsync(ApiEndpoint endpoint) { endpoint.Id = Guid.NewGuid(); endpoint.CreatedAt = DateTime.UtcNow; _endpoints.Add(endpoint); return Task.FromResult(endpoint); }
    public Task<IEnumerable<ApiEndpoint>> GetEndpointsAsync(Guid? dataSourceId = null) => Task.FromResult(dataSourceId.HasValue ? _endpoints.Where(e => e.DataSourceId == dataSourceId.Value && e.IsActive) : _endpoints.Where(e => e.IsActive));
    public Task<ApiEndpoint> UpdateEndpointAsync(ApiEndpoint endpoint) { var e = _endpoints.FirstOrDefault(ep => ep.Id == endpoint.Id); if (e != null) { e.Query = endpoint.Query; e.Parameters = endpoint.Parameters; } return Task.FromResult(e ?? endpoint); }
    public Task<object> ExecuteEndpointAsync(Guid endpointId, Dictionary<string, object>? parameters = null) { var e = _endpoints.FirstOrDefault(ep => ep.Id == endpointId); if (e != null) e.CallCount++; return Task.FromResult<object>(new { data = "Sample response data" }); }
    public Task DeleteEndpointAsync(Guid id) { _endpoints.RemoveAll(e => e.Id == id); return Task.CompletedTask; }

    public Task<Webhook> CreateWebhookAsync(Webhook webhook) { webhook.Id = Guid.NewGuid(); webhook.Secret = Guid.NewGuid().ToString("N"); webhook.CreatedAt = DateTime.UtcNow; _webhooks.Add(webhook); return Task.FromResult(webhook); }
    public Task<IEnumerable<Webhook>> GetWebhooksAsync() => Task.FromResult(_webhooks.Where(w => w.IsActive));
    public Task<Webhook> UpdateWebhookAsync(Webhook webhook) { var e = _webhooks.FirstOrDefault(w => w.Id == webhook.Id); if (e != null) { e.Url = webhook.Url; e.Events = webhook.Events; e.IsActive = webhook.IsActive; } return Task.FromResult(e ?? webhook); }
    public Task DeleteWebhookAsync(Guid id) { _webhooks.RemoveAll(w => w.Id == id); return Task.CompletedTask; }
    public Task<WebhookDelivery> DeliverWebhookAsync(Guid webhookId, string eventType, object payload)
    {
        var webhook = _webhooks.FirstOrDefault(w => w.Id == webhookId); if (webhook == null) throw new Exception("Webhook not found");
        var delivery = new WebhookDelivery { Id = Guid.NewGuid(), WebhookId = webhookId, Event = eventType, Payload = System.Text.Json.JsonSerializer.Serialize(payload), ResponseStatus = 200, IsSuccess = true, DurationMs = 150, DeliveredAt = DateTime.UtcNow };
        _deliveries.Add(delivery); webhook.DeliveryCount++; webhook.LastDeliveredAt = DateTime.UtcNow;
        return Task.FromResult(delivery);
    }
    public Task<IEnumerable<WebhookDelivery>> GetWebhookDeliveriesAsync(Guid webhookId) => Task.FromResult(_deliveries.Where(d => d.WebhookId == webhookId).OrderByDescending(d => d.DeliveredAt));

    public Task ProcessScheduledPipelinesAsync() => Task.CompletedTask;

    public Task<DataHubStatistics> GetStatisticsAsync() => Task.FromResult(new DataHubStatistics
    {
        TotalDataSources = _sources.Count, ConnectedSources = _sources.Count(s => s.Status == DataSourceStatus.Connected),
        TotalPipelines = _pipelines.Count, ActivePipelines = _pipelines.Count(p => p.IsActive), RunningPipelines = _pipelines.Count(p => p.Status == PipelineStatus.Running),
        TotalRecordsProcessed = _pipelines.Sum(p => p.RecordsProcessed), ExecutionsToday = _executions.Count(e => e.StartedAt.Date == DateTime.UtcNow.Date),
        FailedExecutionsToday = _executions.Count(e => e.StartedAt.Date == DateTime.UtcNow.Date && e.Status == ExecutionStatus.Failed),
        TotalEndpoints = _endpoints.Count(e => e.IsActive), TotalWebhooks = _webhooks.Count(w => w.IsActive),
        SourcesByType = _sources.GroupBy(s => s.Type.ToString()).ToDictionary(g => g.Key, g => g.Count()),
        PipelinesByStatus = _pipelines.GroupBy(p => p.Status.ToString()).ToDictionary(g => g.Key, g => g.Count())
    });
}
