using CitizenCRM.Core.Models;

namespace CitizenCRM.Core.Interfaces;

public interface ICitizenService
{
    Task<Citizen> CreateCitizenAsync(Citizen citizen);
    Task<Citizen?> GetCitizenByIdAsync(Guid id);
    Task<Citizen?> GetCitizenByNationalIdAsync(string nationalId);
    Task<IEnumerable<Citizen>> SearchCitizensAsync(string? query, CitizenStatus? status = null, int page = 1, int pageSize = 20);
    Task<Citizen> UpdateCitizenAsync(Citizen citizen);
    Task<Citizen> VerifyCitizenAsync(Guid id, string method);
    Task DeleteCitizenAsync(Guid id);

    Task<CitizenCase> CreateCaseAsync(CitizenCase caseItem);
    Task<CitizenCase?> GetCaseByIdAsync(Guid id);
    Task<IEnumerable<CitizenCase>> GetCitizenCasesAsync(Guid citizenId);
    Task<IEnumerable<CitizenCase>> GetCasesByStatusAsync(CaseStatus status, string? department = null);
    Task<IEnumerable<CitizenCase>> GetAssignedCasesAsync(Guid agentId);
    Task<CitizenCase> UpdateCaseAsync(CitizenCase caseItem);
    Task<CitizenCase> AssignCaseAsync(Guid caseId, Guid agentId, string agentName);
    Task<CitizenCase> ResolveCaseAsync(Guid caseId, string resolution);
    Task<CaseNote> AddCaseNoteAsync(CaseNote note);

    Task<CitizenInteraction> LogInteractionAsync(CitizenInteraction interaction);
    Task<IEnumerable<CitizenInteraction>> GetCitizenInteractionsAsync(Guid citizenId);

    Task<CitizenDocument> AddDocumentAsync(CitizenDocument document);
    Task<IEnumerable<CitizenDocument>> GetCitizenDocumentsAsync(Guid citizenId);

    Task<ServiceRequest> CreateServiceRequestAsync(ServiceRequest request);
    Task<ServiceRequest?> GetServiceRequestAsync(Guid id);
    Task<IEnumerable<ServiceRequest>> GetCitizenServiceRequestsAsync(Guid citizenId);
    Task<ServiceRequest> ProcessServiceRequestAsync(Guid id, ServiceRequestStatus status, Guid processedBy, string? notes);

    Task<CRMStatistics> GetStatisticsAsync(string? department = null);
}

public class CRMStatistics
{
    public int TotalCitizens { get; set; }
    public int VerifiedCitizens { get; set; }
    public int TotalCases { get; set; }
    public int OpenCases { get; set; }
    public int ResolvedCasesToday { get; set; }
    public double AverageResolutionTimeHours { get; set; }
    public double AverageSatisfactionRating { get; set; }
    public int TotalInteractions { get; set; }
    public int InteractionsToday { get; set; }
    public int PendingServiceRequests { get; set; }
    public Dictionary<string, int> CasesByStatus { get; set; } = new();
    public Dictionary<string, int> CasesByType { get; set; } = new();
    public Dictionary<string, int> InteractionsByChannel { get; set; } = new();
}
