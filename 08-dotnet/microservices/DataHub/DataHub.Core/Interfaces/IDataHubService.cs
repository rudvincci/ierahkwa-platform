using DataHub.Core.Models;
namespace DataHub.Core.Interfaces;

public interface IDataHubService
{
    Task<DataSource> CreateDataSourceAsync(DataSource source);
    Task<DataSource?> GetDataSourceByIdAsync(Guid id);
    Task<IEnumerable<DataSource>> GetDataSourcesAsync(DataSourceType? type = null, bool? isActive = null);
    Task<DataSource> UpdateDataSourceAsync(DataSource source);
    Task<DataSource> TestConnectionAsync(Guid id);
    Task DeleteDataSourceAsync(Guid id);

    Task<DataPipeline> CreatePipelineAsync(DataPipeline pipeline);
    Task<DataPipeline?> GetPipelineByIdAsync(Guid id);
    Task<IEnumerable<DataPipeline>> GetPipelinesAsync(PipelineStatus? status = null, bool? isActive = null);
    Task<DataPipeline> UpdatePipelineAsync(DataPipeline pipeline);
    Task<DataPipeline> ActivatePipelineAsync(Guid id);
    Task<DataPipeline> DeactivatePipelineAsync(Guid id);
    Task DeletePipelineAsync(Guid id);

    Task<PipelineStep> AddPipelineStepAsync(PipelineStep step);
    Task<IEnumerable<PipelineStep>> GetPipelineStepsAsync(Guid pipelineId);
    Task<PipelineStep> UpdatePipelineStepAsync(PipelineStep step);
    Task RemovePipelineStepAsync(Guid stepId);

    Task<PipelineExecution> ExecutePipelineAsync(Guid pipelineId, Guid? triggeredBy = null);
    Task<IEnumerable<PipelineExecution>> GetExecutionsAsync(Guid? pipelineId = null, ExecutionStatus? status = null);
    Task<PipelineExecution> CancelExecutionAsync(Guid executionId);

    Task<DataMapping> CreateMappingAsync(DataMapping mapping);
    Task<IEnumerable<DataMapping>> GetMappingsAsync();
    Task<DataMapping> UpdateMappingAsync(DataMapping mapping);
    Task<object> TransformDataAsync(Guid mappingId, object sourceData);

    Task<ApiEndpoint> CreateEndpointAsync(ApiEndpoint endpoint);
    Task<IEnumerable<ApiEndpoint>> GetEndpointsAsync(Guid? dataSourceId = null);
    Task<ApiEndpoint> UpdateEndpointAsync(ApiEndpoint endpoint);
    Task<object> ExecuteEndpointAsync(Guid endpointId, Dictionary<string, object>? parameters = null);
    Task DeleteEndpointAsync(Guid id);

    Task<Webhook> CreateWebhookAsync(Webhook webhook);
    Task<IEnumerable<Webhook>> GetWebhooksAsync();
    Task<Webhook> UpdateWebhookAsync(Webhook webhook);
    Task DeleteWebhookAsync(Guid id);
    Task<WebhookDelivery> DeliverWebhookAsync(Guid webhookId, string eventType, object payload);
    Task<IEnumerable<WebhookDelivery>> GetWebhookDeliveriesAsync(Guid webhookId);

    Task ProcessScheduledPipelinesAsync();
    Task<DataHubStatistics> GetStatisticsAsync();
}

public class DataHubStatistics
{
    public int TotalDataSources { get; set; }
    public int ConnectedSources { get; set; }
    public int TotalPipelines { get; set; }
    public int ActivePipelines { get; set; }
    public int RunningPipelines { get; set; }
    public long TotalRecordsProcessed { get; set; }
    public int ExecutionsToday { get; set; }
    public int FailedExecutionsToday { get; set; }
    public int TotalEndpoints { get; set; }
    public int TotalWebhooks { get; set; }
    public Dictionary<string, int> SourcesByType { get; set; } = new();
    public Dictionary<string, int> PipelinesByStatus { get; set; } = new();
}
