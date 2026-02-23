using Common.Domain.Entities;

namespace Receptionist.Domain.Entities;

public class PostalDispatch : TenantEntity
{
    public string ReferenceNo { get; set; } = string.Empty;
    public string ToTitle { get; set; } = string.Empty;
    public string? ToAddress { get; set; }
    public string? FromTitle { get; set; }
    public DateTime DispatchDate { get; set; } = DateTime.UtcNow;
    public string? Type { get; set; }
    public string? Notes { get; set; }
    public string? Attachment { get; set; }
}

public class PostalReceive : TenantEntity
{
    public string ReferenceNo { get; set; } = string.Empty;
    public string FromTitle { get; set; } = string.Empty;
    public string? FromAddress { get; set; }
    public string? ToTitle { get; set; }
    public DateTime ReceiveDate { get; set; } = DateTime.UtcNow;
    public string? Type { get; set; }
    public string? Notes { get; set; }
    public string? Attachment { get; set; }
}
