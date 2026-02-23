namespace ESignature.Core.Models;

public class SignatureField
{
    public Guid Id { get; set; }
    public Guid SignatureRequestId { get; set; }
    public Guid? AssignedSignerId { get; set; }
    public FieldType FieldType { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Placeholder { get; set; }
    public bool IsRequired { get; set; }
    public int PageNumber { get; set; }
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public string? Value { get; set; }
    public string? ValidationRule { get; set; }
    public string? DefaultValue { get; set; }
    public string? Options { get; set; } // JSON for dropdown/checkbox options
    public DateTime? FilledAt { get; set; }
    public Guid? FilledBy { get; set; }

    // Navigation
    public SignatureRequest? SignatureRequest { get; set; }
    public Signer? AssignedSigner { get; set; }
}

public enum FieldType
{
    Signature,
    Initials,
    DateSigned,
    Text,
    Checkbox,
    RadioButton,
    Dropdown,
    Attachment,
    Company,
    Title,
    Name,
    Email,
    Phone,
    Address,
    CustomDate,
    Number
}
