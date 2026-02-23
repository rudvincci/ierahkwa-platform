namespace Common.Domain.Entities;

public class ErrorLog : BaseEntity
{
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public string Level { get; set; } = "Error";
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public int? TenantId { get; set; }
}
