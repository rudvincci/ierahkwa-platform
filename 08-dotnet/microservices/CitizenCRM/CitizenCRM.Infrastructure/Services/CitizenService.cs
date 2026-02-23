using CitizenCRM.Core.Interfaces;
using CitizenCRM.Core.Models;

namespace CitizenCRM.Infrastructure.Services;

public class CitizenService : ICitizenService
{
    private readonly List<Citizen> _citizens = new();
    private readonly List<CitizenCase> _cases = new();
    private readonly List<CitizenInteraction> _interactions = new();
    private readonly List<CitizenDocument> _documents = new();
    private readonly List<CaseNote> _notes = new();
    private readonly List<CaseActivity> _activities = new();
    private readonly List<ServiceRequest> _serviceRequests = new();

    public Task<Citizen> CreateCitizenAsync(Citizen citizen)
    {
        citizen.Id = Guid.NewGuid();
        citizen.CitizenId = $"CIT-{DateTime.UtcNow:yyyyMMdd}-{_citizens.Count + 1:D6}";
        citizen.Status = CitizenStatus.Active;
        citizen.CreatedAt = DateTime.UtcNow;
        _citizens.Add(citizen);
        return Task.FromResult(citizen);
    }

    public Task<Citizen?> GetCitizenByIdAsync(Guid id) => Task.FromResult(_citizens.FirstOrDefault(c => c.Id == id));
    public Task<Citizen?> GetCitizenByNationalIdAsync(string nationalId) => Task.FromResult(_citizens.FirstOrDefault(c => c.CitizenId == nationalId));

    public Task<IEnumerable<Citizen>> SearchCitizensAsync(string? query, CitizenStatus? status = null, int page = 1, int pageSize = 20)
    {
        var q = _citizens.AsEnumerable();
        if (!string.IsNullOrEmpty(query)) q = q.Where(c => c.FullName.Contains(query, StringComparison.OrdinalIgnoreCase) || c.Email.Contains(query, StringComparison.OrdinalIgnoreCase) || c.CitizenId.Contains(query));
        if (status.HasValue) q = q.Where(c => c.Status == status.Value);
        return Task.FromResult(q.Skip((page - 1) * pageSize).Take(pageSize));
    }

    public Task<Citizen> UpdateCitizenAsync(Citizen citizen)
    {
        var e = _citizens.FirstOrDefault(c => c.Id == citizen.Id);
        if (e != null) { e.FirstName = citizen.FirstName; e.LastName = citizen.LastName; e.Email = citizen.Email; e.Phone = citizen.Phone; e.Address = citizen.Address; e.UpdatedAt = DateTime.UtcNow; }
        return Task.FromResult(e ?? citizen);
    }

    public Task<Citizen> VerifyCitizenAsync(Guid id, string method)
    {
        var c = _citizens.FirstOrDefault(c => c.Id == id);
        if (c != null) { c.IsVerified = true; c.VerifiedAt = DateTime.UtcNow; c.VerificationMethod = method; }
        return Task.FromResult(c!);
    }

    public Task DeleteCitizenAsync(Guid id) { _citizens.RemoveAll(c => c.Id == id); return Task.CompletedTask; }

    public Task<CitizenCase> CreateCaseAsync(CitizenCase caseItem)
    {
        caseItem.Id = Guid.NewGuid();
        caseItem.CaseNumber = $"CASE-{DateTime.UtcNow:yyyyMMdd}-{_cases.Count + 1:D5}";
        caseItem.Status = CaseStatus.New;
        caseItem.CreatedAt = DateTime.UtcNow;
        _cases.Add(caseItem);
        return Task.FromResult(caseItem);
    }

    public Task<CitizenCase?> GetCaseByIdAsync(Guid id) => Task.FromResult(_cases.FirstOrDefault(c => c.Id == id));
    public Task<IEnumerable<CitizenCase>> GetCitizenCasesAsync(Guid citizenId) => Task.FromResult(_cases.Where(c => c.CitizenId == citizenId));
    public Task<IEnumerable<CitizenCase>> GetCasesByStatusAsync(CaseStatus status, string? department = null)
    {
        var q = _cases.Where(c => c.Status == status);
        if (!string.IsNullOrEmpty(department)) q = q.Where(c => c.Department == department);
        return Task.FromResult(q);
    }
    public Task<IEnumerable<CitizenCase>> GetAssignedCasesAsync(Guid agentId) => Task.FromResult(_cases.Where(c => c.AssignedTo == agentId));

    public Task<CitizenCase> UpdateCaseAsync(CitizenCase caseItem)
    {
        var e = _cases.FirstOrDefault(c => c.Id == caseItem.Id);
        if (e != null) { e.Title = caseItem.Title; e.Status = caseItem.Status; e.Priority = caseItem.Priority; e.UpdatedAt = DateTime.UtcNow; }
        return Task.FromResult(e ?? caseItem);
    }

    public Task<CitizenCase> AssignCaseAsync(Guid caseId, Guid agentId, string agentName)
    {
        var c = _cases.FirstOrDefault(c => c.Id == caseId);
        if (c != null) { c.AssignedTo = agentId; c.AssignedToName = agentName; c.Status = CaseStatus.Open; c.UpdatedAt = DateTime.UtcNow; }
        return Task.FromResult(c!);
    }

    public Task<CitizenCase> ResolveCaseAsync(Guid caseId, string resolution)
    {
        var c = _cases.FirstOrDefault(c => c.Id == caseId);
        if (c != null) { c.Status = CaseStatus.Resolved; c.Resolution = resolution; c.ResolvedAt = DateTime.UtcNow; c.UpdatedAt = DateTime.UtcNow; }
        return Task.FromResult(c!);
    }

    public Task<CaseNote> AddCaseNoteAsync(CaseNote note) { note.Id = Guid.NewGuid(); note.CreatedAt = DateTime.UtcNow; _notes.Add(note); return Task.FromResult(note); }
    public Task<CitizenInteraction> LogInteractionAsync(CitizenInteraction interaction) { interaction.Id = Guid.NewGuid(); interaction.InteractionAt = DateTime.UtcNow; _interactions.Add(interaction); var c = _citizens.FirstOrDefault(c => c.Id == interaction.CitizenId); if (c != null) c.LastInteractionAt = DateTime.UtcNow; return Task.FromResult(interaction); }
    public Task<IEnumerable<CitizenInteraction>> GetCitizenInteractionsAsync(Guid citizenId) => Task.FromResult(_interactions.Where(i => i.CitizenId == citizenId).OrderByDescending(i => i.InteractionAt));
    public Task<CitizenDocument> AddDocumentAsync(CitizenDocument document) { document.Id = Guid.NewGuid(); document.UploadedAt = DateTime.UtcNow; _documents.Add(document); return Task.FromResult(document); }
    public Task<IEnumerable<CitizenDocument>> GetCitizenDocumentsAsync(Guid citizenId) => Task.FromResult(_documents.Where(d => d.CitizenId == citizenId));

    public Task<ServiceRequest> CreateServiceRequestAsync(ServiceRequest request)
    {
        request.Id = Guid.NewGuid();
        request.RequestNumber = $"SRQ-{DateTime.UtcNow:yyyyMMdd}-{_serviceRequests.Count + 1:D5}";
        request.Status = ServiceRequestStatus.Submitted;
        request.RequestedAt = DateTime.UtcNow;
        _serviceRequests.Add(request);
        return Task.FromResult(request);
    }

    public Task<ServiceRequest?> GetServiceRequestAsync(Guid id) => Task.FromResult(_serviceRequests.FirstOrDefault(s => s.Id == id));
    public Task<IEnumerable<ServiceRequest>> GetCitizenServiceRequestsAsync(Guid citizenId) => Task.FromResult(_serviceRequests.Where(s => s.CitizenId == citizenId));

    public Task<ServiceRequest> ProcessServiceRequestAsync(Guid id, ServiceRequestStatus status, Guid processedBy, string? notes)
    {
        var r = _serviceRequests.FirstOrDefault(s => s.Id == id);
        if (r != null) { r.Status = status; r.ProcessedBy = processedBy; r.ProcessedAt = DateTime.UtcNow; r.Notes = notes; if (status == ServiceRequestStatus.Completed) r.CompletedAt = DateTime.UtcNow; }
        return Task.FromResult(r!);
    }

    public Task<CRMStatistics> GetStatisticsAsync(string? department = null)
    {
        var cases = string.IsNullOrEmpty(department) ? _cases : _cases.Where(c => c.Department == department).ToList();
        return Task.FromResult(new CRMStatistics
        {
            TotalCitizens = _citizens.Count,
            VerifiedCitizens = _citizens.Count(c => c.IsVerified),
            TotalCases = cases.Count,
            OpenCases = cases.Count(c => c.Status == CaseStatus.Open || c.Status == CaseStatus.InProgress),
            ResolvedCasesToday = cases.Count(c => c.ResolvedAt?.Date == DateTime.UtcNow.Date),
            AverageSatisfactionRating = cases.Where(c => c.SatisfactionRating.HasValue).DefaultIfEmpty().Average(c => c?.SatisfactionRating ?? 0),
            TotalInteractions = _interactions.Count,
            InteractionsToday = _interactions.Count(i => i.InteractionAt.Date == DateTime.UtcNow.Date),
            PendingServiceRequests = _serviceRequests.Count(s => s.Status == ServiceRequestStatus.Submitted || s.Status == ServiceRequestStatus.UnderReview),
            CasesByStatus = cases.GroupBy(c => c.Status.ToString()).ToDictionary(g => g.Key, g => g.Count()),
            CasesByType = cases.GroupBy(c => c.Type.ToString()).ToDictionary(g => g.Key, g => g.Count()),
            InteractionsByChannel = _interactions.GroupBy(i => i.Channel.ToString()).ToDictionary(g => g.Key, g => g.Count())
        });
    }
}
