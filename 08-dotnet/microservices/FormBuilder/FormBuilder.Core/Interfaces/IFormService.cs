using FormBuilder.Core.Models;
namespace FormBuilder.Core.Interfaces;

public interface IFormService
{
    Task<Form> CreateFormAsync(Form form);
    Task<Form?> GetFormByIdAsync(Guid id);
    Task<Form?> GetFormByCodeAsync(string code);
    Task<IEnumerable<Form>> GetFormsAsync(FormType? type = null, FormStatus? status = null, string? department = null);
    Task<Form> UpdateFormAsync(Form form);
    Task<Form> PublishFormAsync(Guid id);
    Task<Form> CloseFormAsync(Guid id);
    Task DeleteFormAsync(Guid id);

    Task<FormField> AddFieldAsync(FormField field);
    Task<FormField> UpdateFieldAsync(FormField field);
    Task RemoveFieldAsync(Guid fieldId);
    Task ReorderFieldsAsync(Guid formId, List<Guid> fieldOrder);

    Task<FormSubmission> SubmitFormAsync(FormSubmission submission);
    Task<FormSubmission?> GetSubmissionByIdAsync(Guid id);
    Task<IEnumerable<FormSubmission>> GetSubmissionsAsync(Guid formId, SubmissionStatus? status = null, DateTime? from = null, DateTime? to = null);
    Task<FormSubmission> UpdateSubmissionStatusAsync(Guid submissionId, SubmissionStatus status, Guid? processedBy, string? notes);
    Task DeleteSubmissionAsync(Guid id);
    Task<byte[]> ExportSubmissionsAsync(Guid formId, string format);

    Task<FormTemplate> CreateTemplateAsync(FormTemplate template);
    Task<IEnumerable<FormTemplate>> GetTemplatesAsync(string? category = null, bool? isPublic = null);
    Task<Form> CreateFormFromTemplateAsync(Guid templateId, string name, Guid createdBy);

    Task<FormStatistics> GetStatisticsAsync(Guid? formId = null, string? department = null);
}

public class FormStatistics
{
    public int TotalForms { get; set; }
    public int PublishedForms { get; set; }
    public int TotalSubmissions { get; set; }
    public int SubmissionsToday { get; set; }
    public int PendingSubmissions { get; set; }
    public double AverageCompletionTime { get; set; }
    public Dictionary<string, int> SubmissionsByForm { get; set; } = new();
    public Dictionary<string, int> SubmissionsByStatus { get; set; } = new();
    public List<DailySubmissions> DailyTrend { get; set; } = new();
}

public class DailySubmissions { public DateTime Date { get; set; } public int Count { get; set; } }
