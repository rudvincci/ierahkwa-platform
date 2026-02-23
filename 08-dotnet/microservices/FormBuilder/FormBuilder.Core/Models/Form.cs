namespace FormBuilder.Core.Models;

public class Form
{
    public Guid Id { get; set; }
    public string FormCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FormType Type { get; set; }
    public string? Category { get; set; }
    public string? Department { get; set; }
    public FormStatus Status { get; set; }
    public string FieldsDefinition { get; set; } = "[]";
    public string? ValidationRules { get; set; }
    public string? ConditionalLogic { get; set; }
    public string? Theme { get; set; }
    public bool RequiresAuthentication { get; set; }
    public bool AllowAnonymous { get; set; }
    public bool AllowMultipleSubmissions { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int SubmissionCount { get; set; }
    public Guid? WorkflowId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public List<FormField> Fields { get; set; } = new();
}

public class FormField
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRule { get; set; }
    public string? Options { get; set; }
    public int OrderIndex { get; set; }
    public string? ConditionalDisplay { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public string? Section { get; set; }
    public int Width { get; set; } = 12;
}

public class FormSubmission
{
    public Guid Id { get; set; }
    public string SubmissionNumber { get; set; } = string.Empty;
    public Guid FormId { get; set; }
    public string FormName { get; set; } = string.Empty;
    public Guid? SubmittedBy { get; set; }
    public string? SubmitterName { get; set; }
    public string? SubmitterEmail { get; set; }
    public string Data { get; set; } = "{}";
    public SubmissionStatus Status { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public Guid? ProcessedBy { get; set; }
    public string? Notes { get; set; }
    public List<FormAttachment> Attachments { get; set; } = new();
}

public class FormAttachment
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class FormTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string FieldsDefinition { get; set; } = "[]";
    public string? PreviewImage { get; set; }
    public bool IsPublic { get; set; }
    public int UsageCount { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum FormType { Survey, Application, Registration, Feedback, Request, Report, Assessment, Checklist, Other }
public enum FormStatus { Draft, Published, Closed, Archived }
public enum FieldType { Text, TextArea, Number, Email, Phone, Date, DateTime, Time, Select, MultiSelect, Radio, Checkbox, File, Signature, Rating, Scale, Address, Name, Section, HTML, Hidden }
public enum SubmissionStatus { Pending, Reviewed, Approved, Rejected, InProgress, Completed }
