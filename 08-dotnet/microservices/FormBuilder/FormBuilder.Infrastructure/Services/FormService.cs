using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using System.Text;
namespace FormBuilder.Infrastructure.Services;

public class FormService : IFormService
{
    private readonly List<Form> _forms = new();
    private readonly List<FormField> _fields = new();
    private readonly List<FormSubmission> _submissions = new();
    private readonly List<FormTemplate> _templates = new();

    public Task<Form> CreateFormAsync(Form form) { form.Id = Guid.NewGuid(); form.FormCode = $"FRM-{DateTime.UtcNow:yyyyMM}-{_forms.Count + 1:D4}"; form.Status = FormStatus.Draft; form.CreatedAt = DateTime.UtcNow; _forms.Add(form); return Task.FromResult(form); }
    public Task<Form?> GetFormByIdAsync(Guid id) { var f = _forms.FirstOrDefault(f => f.Id == id); if (f != null) f.Fields = _fields.Where(x => x.FormId == id).OrderBy(x => x.OrderIndex).ToList(); return Task.FromResult(f); }
    public Task<Form?> GetFormByCodeAsync(string code) => Task.FromResult(_forms.FirstOrDefault(f => f.FormCode == code));
    public Task<IEnumerable<Form>> GetFormsAsync(FormType? type = null, FormStatus? status = null, string? department = null) { var q = _forms.AsEnumerable(); if (type.HasValue) q = q.Where(f => f.Type == type.Value); if (status.HasValue) q = q.Where(f => f.Status == status.Value); if (!string.IsNullOrEmpty(department)) q = q.Where(f => f.Department == department); return Task.FromResult(q); }
    public Task<Form> UpdateFormAsync(Form form) { var e = _forms.FirstOrDefault(f => f.Id == form.Id); if (e != null) { e.Name = form.Name; e.FieldsDefinition = form.FieldsDefinition; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? form); }
    public Task<Form> PublishFormAsync(Guid id) { var f = _forms.FirstOrDefault(f => f.Id == id); if (f != null) { f.Status = FormStatus.Published; f.PublishedAt = DateTime.UtcNow; } return Task.FromResult(f!); }
    public Task<Form> CloseFormAsync(Guid id) { var f = _forms.FirstOrDefault(f => f.Id == id); if (f != null) f.Status = FormStatus.Closed; return Task.FromResult(f!); }
    public Task DeleteFormAsync(Guid id) { _forms.RemoveAll(f => f.Id == id); _fields.RemoveAll(f => f.FormId == id); return Task.CompletedTask; }

    public Task<FormField> AddFieldAsync(FormField field) { field.Id = Guid.NewGuid(); field.OrderIndex = _fields.Count(f => f.FormId == field.FormId); _fields.Add(field); return Task.FromResult(field); }
    public Task<FormField> UpdateFieldAsync(FormField field) { var e = _fields.FirstOrDefault(f => f.Id == field.Id); if (e != null) { e.Label = field.Label; e.IsRequired = field.IsRequired; e.Options = field.Options; } return Task.FromResult(e ?? field); }
    public Task RemoveFieldAsync(Guid fieldId) { _fields.RemoveAll(f => f.Id == fieldId); return Task.CompletedTask; }
    public Task ReorderFieldsAsync(Guid formId, List<Guid> fieldOrder) { for (int i = 0; i < fieldOrder.Count; i++) { var f = _fields.FirstOrDefault(f => f.Id == fieldOrder[i]); if (f != null) f.OrderIndex = i; } return Task.CompletedTask; }

    public Task<FormSubmission> SubmitFormAsync(FormSubmission submission) { submission.Id = Guid.NewGuid(); submission.SubmissionNumber = $"SUB-{DateTime.UtcNow:yyyyMMdd}-{_submissions.Count + 1:D5}"; submission.Status = SubmissionStatus.Pending; submission.SubmittedAt = DateTime.UtcNow; _submissions.Add(submission); var form = _forms.FirstOrDefault(f => f.Id == submission.FormId); if (form != null) form.SubmissionCount++; return Task.FromResult(submission); }
    public Task<FormSubmission?> GetSubmissionByIdAsync(Guid id) => Task.FromResult(_submissions.FirstOrDefault(s => s.Id == id));
    public Task<IEnumerable<FormSubmission>> GetSubmissionsAsync(Guid formId, SubmissionStatus? status = null, DateTime? from = null, DateTime? to = null) { var q = _submissions.Where(s => s.FormId == formId); if (status.HasValue) q = q.Where(s => s.Status == status.Value); if (from.HasValue) q = q.Where(s => s.SubmittedAt >= from.Value); if (to.HasValue) q = q.Where(s => s.SubmittedAt <= to.Value); return Task.FromResult(q.OrderByDescending(s => s.SubmittedAt)); }
    public Task<FormSubmission> UpdateSubmissionStatusAsync(Guid submissionId, SubmissionStatus status, Guid? processedBy, string? notes) { var s = _submissions.FirstOrDefault(s => s.Id == submissionId); if (s != null) { s.Status = status; s.ProcessedBy = processedBy; s.ProcessedAt = DateTime.UtcNow; s.Notes = notes; } return Task.FromResult(s!); }
    public Task DeleteSubmissionAsync(Guid id) { _submissions.RemoveAll(s => s.Id == id); return Task.CompletedTask; }
    public Task<byte[]> ExportSubmissionsAsync(Guid formId, string format) => Task.FromResult(Encoding.UTF8.GetBytes("CSV Export"));

    public Task<FormTemplate> CreateTemplateAsync(FormTemplate template) { template.Id = Guid.NewGuid(); template.CreatedAt = DateTime.UtcNow; _templates.Add(template); return Task.FromResult(template); }
    public Task<IEnumerable<FormTemplate>> GetTemplatesAsync(string? category = null, bool? isPublic = null) { var q = _templates.AsEnumerable(); if (!string.IsNullOrEmpty(category)) q = q.Where(t => t.Category == category); if (isPublic.HasValue) q = q.Where(t => t.IsPublic == isPublic.Value); return Task.FromResult(q); }
    public async Task<Form> CreateFormFromTemplateAsync(Guid templateId, string name, Guid createdBy) { var t = _templates.FirstOrDefault(t => t.Id == templateId); if (t == null) throw new Exception("Template not found"); var form = new Form { Name = name, FieldsDefinition = t.FieldsDefinition, CreatedBy = createdBy }; t.UsageCount++; return await CreateFormAsync(form); }

    public Task<FormStatistics> GetStatisticsAsync(Guid? formId = null, string? department = null)
    {
        var forms = _forms.AsEnumerable(); if (!string.IsNullOrEmpty(department)) forms = forms.Where(f => f.Department == department);
        var submissions = formId.HasValue ? _submissions.Where(s => s.FormId == formId.Value).ToList() : _submissions;
        return Task.FromResult(new FormStatistics { TotalForms = forms.Count(), PublishedForms = forms.Count(f => f.Status == FormStatus.Published), TotalSubmissions = submissions.Count, SubmissionsToday = submissions.Count(s => s.SubmittedAt.Date == DateTime.UtcNow.Date), PendingSubmissions = submissions.Count(s => s.Status == SubmissionStatus.Pending), SubmissionsByStatus = submissions.GroupBy(s => s.Status.ToString()).ToDictionary(g => g.Key, g => g.Count()) });
    }
}
