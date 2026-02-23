namespace DataHub.Core.Models;

public class DataSource
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DataSourceType Type { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string? Schema { get; set; }
    public string? AuthType { get; set; }
    public string? Credentials { get; set; }
    public DataSourceStatus Status { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastSyncAt { get; set; }
    public int SyncIntervalMinutes { get; set; }
    public string? Metadata { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DataPipeline
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid SourceId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public Guid? DestinationId { get; set; }
    public string? DestinationName { get; set; }
    public PipelineType Type { get; set; }
    public string? TransformationRules { get; set; }
    public string? MappingConfig { get; set; }
    public string? FilterCriteria { get; set; }
    public PipelineStatus Status { get; set; }
    public string? Schedule { get; set; }
    public bool IsActive { get; set; } = true;
    public int RecordsProcessed { get; set; }
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PipelineStep> Steps { get; set; } = new();
}

public class PipelineStep
{
    public Guid Id { get; set; }
    public Guid PipelineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public StepType Type { get; set; }
    public string? Configuration { get; set; }
    public int OrderIndex { get; set; }
    public bool IsEnabled { get; set; } = true;
}

public class PipelineExecution
{
    public Guid Id { get; set; }
    public Guid PipelineId { get; set; }
    public string PipelineName { get; set; } = string.Empty;
    public ExecutionStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? DurationMs { get; set; }
    public int RecordsRead { get; set; }
    public int RecordsWritten { get; set; }
    public int RecordsFailed { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ExecutionLog { get; set; }
    public Guid? TriggeredBy { get; set; }
}

public class DataMapping
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SourceEntity { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public string MappingDefinition { get; set; } = "[]";
    public string? TransformationScript { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class ApiEndpoint
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public Guid DataSourceId { get; set; }
    public string? Query { get; set; }
    public string? Parameters { get; set; }
    public string? ResponseSchema { get; set; }
    public bool RequiresAuth { get; set; } = true;
    public string? RateLimit { get; set; }
    public bool IsActive { get; set; } = true;
    public int CallCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Webhook
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Secret { get; set; }
    public string Events { get; set; } = string.Empty;
    public string? Headers { get; set; }
    public bool IsActive { get; set; } = true;
    public int DeliveryCount { get; set; }
    public int FailureCount { get; set; }
    public DateTime? LastDeliveredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WebhookDelivery
{
    public Guid Id { get; set; }
    public Guid WebhookId { get; set; }
    public string Event { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public int ResponseStatus { get; set; }
    public string? ResponseBody { get; set; }
    public int? DurationMs { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime DeliveredAt { get; set; }
}

public enum DataSourceType { Database, API, File, Queue, Stream, S3, FTP, SFTP, Blockchain }
public enum DataSourceStatus { Connected, Disconnected, Error, Syncing }
public enum PipelineType { ETL, Sync, Replication, Migration, Export, Import }
public enum PipelineStatus { Idle, Running, Paused, Error, Completed }
public enum StepType { Extract, Transform, Filter, Validate, Enrich, Load, Notify }
public enum ExecutionStatus { Running, Completed, Failed, Cancelled }
