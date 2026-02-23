namespace Mamey.Portal.Citizenship.Application.Models;

public class ValidationError
{
    public string FieldName { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public object? OriginalValue { get; set; }
    public object? CorrectedValue { get; set; }
    public bool WasTruncated { get; set; }
    public bool WasSanitized { get; set; }
}


