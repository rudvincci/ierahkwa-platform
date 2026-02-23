using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.RequestResponses;
using Mamey.Government.UI.Models;
using Mamey.Types;
using Microsoft.Extensions.Logging;
using RejectApplicationRequest = Mamey.Government.UI.Models.RejectApplicationRequest;
using StartApplicationRequest = Mamey.Government.UI.Models.StartApplicationRequest;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Applications service implementation.
/// Uses DTOs from Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO
/// and maps them to UI Models for consumption by the UI layer.
/// </summary>
public class ApplicationsService : ApiServiceBase, IApplicationsService
{
    private const string BaseUrl = "/api/citizenship-applications";

    public ApplicationsService(HttpClient httpClient, ILogger<ApplicationsService> logger, AppOptions appOptions) 
        : base(httpClient, logger, appOptions)
    {
    }

    public async Task<PagedResult<ApplicationSummaryModel>> BrowseAsync(
        Guid tenantId,
        string? status = null,
        string? searchTerm = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQueryString(
            ("tenantId", tenantId),
            ("status", status),
            ("searchTerm", searchTerm),
            ("fromDate", fromDate?.ToString("o")),
            ("toDate", toDate?.ToString("o")),
            ("page", page),
            ("pageSize", pageSize));
        
        // Get PagedResult from API - deserialize using Contracts DTO structure
        var result = await base.GetAsync<PagedResultDto<ApplicationSummaryDto>>(
            $"{BaseUrl}?{query}", 
            cancellationToken);
        
        if (result == null || result.Items == null || result.Items.Count == 0)
        {
            return PagedResult<ApplicationSummaryModel>.Empty;
        }

        // Map from Contracts DTO to UI Model
        var items = result.Items.Select(MapToSummaryModel).ToList();
        
        return new PagedResult<ApplicationSummaryModel>
        {
            Items = items,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages,
            TotalResults = result.TotalResults
        };
    }

    public async Task<ApplicationModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dto = await base.GetAsync<ApplicationDto>($"{BaseUrl}/{id}", cancellationToken);
        return dto != null ? MapToModel(dto) : null;
    }

    public async Task<ApplicationModel?> GetByNumberAsync(string applicationNumber, CancellationToken cancellationToken = default)
    {
        var dto = await base.GetAsync<ApplicationDto>($"{BaseUrl}/by-number/{Uri.EscapeDataString(applicationNumber)}", cancellationToken);
        return dto != null ? MapToModel(dto) : null;
    }

    public async Task<Guid?> CreateAsync(CreateApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<CreateApplicationRequest, CreateApplicationResponse>(
            BaseUrl, 
            request, 
            cancellationToken);
        
        return response?.ApplicationId;
    }

    public async Task<bool> StartAsync(StartApplicationRequest request, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/start", request, cancellationToken);
    }

    public async Task<ResumeApplicationResponse?> ResumeApplicationAsync(string token, string email, CancellationToken cancellationToken = default)
    {
        var request = new ResumeApplicationRequest(token, email);
        var response  = await PostAsync<ResumeApplicationRequest, ResumeApplicationResponse>(
                                   $"{BaseUrl}/resume",
                                   request,
                                   cancellationToken);
        return response;
    }

    public async Task<bool> ResumeAsync(ResumeApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var response = await ResumeApplicationAsync(request.Token, request.Email, cancellationToken);
        return response != null;
    }

    public async Task<bool> SubmitAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/{id}/submit", cancellationToken);
    }

    public async Task<bool> ApproveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/{id}/approve", cancellationToken);
    }

    public async Task<bool> RejectAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/{id}/reject", new RejectApplicationRequest { Reason = reason }, cancellationToken);
    }

    /// <summary>
    /// Maps ApplicationSummaryDto from Contracts to ApplicationSummaryModel for UI.
    /// </summary>
    private static ApplicationSummaryModel MapToSummaryModel(ApplicationSummaryDto dto)
    {
        // Parse ApplicantName to extract FirstName and LastName
        // The DTO provides ApplicantName as a single string, we need to split it
        var nameParts = dto.ApplicantName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
        var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

        return new ApplicationSummaryModel
        {
            Id = dto.Id,
            ApplicationNumber = dto.ApplicationNumber,
            FirstName = firstName,
            LastName = lastName,
            Email = dto.Email,
            Status = dto.Status,
            Step = dto.Step,
            CreatedAt = dto.CreatedAt,
            SubmittedAt = dto.SubmittedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    /// <summary>
    /// Maps ApplicationDto from Contracts to ApplicationModel for UI.
    /// Includes all extended properties and legacy address support.
    /// </summary>
    private static ApplicationModel MapToModel(ApplicationDto dto)
    {
        // Extract legacy address components from AddressDto if available
        string? street = null;
        string? city = null;
        string? state = null;
        string? postalCode = null;
        string? country = null;

        if (dto.Address != null)
        {
            street = dto.Address.Line;
            city = dto.Address.City;
            state = dto.Address.State;
            postalCode = dto.Address.PostalCode ?? dto.Address.Zip5;
            country = dto.Address.Country;
        }

        return new ApplicationModel
        {
            Id = dto.Id,
            ApplicationNumber = dto.ApplicationNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            Nickname = dto.Nickname,
            DateOfBirth = dto.DateOfBirth,
            Email = dto.Email,
            Phone = dto.Phone,
            Status = dto.Status,
            Step = dto.Step,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            SubmittedAt = dto.SubmittedAt,
            ApprovedAt = dto.ApprovedAt,
            RejectedAt = dto.RejectedAt,
            RejectionReason = dto.RejectionReason,
            ApprovedBy = dto.ApprovedBy,
            RejectedBy = dto.RejectedBy,
            // Legacy address support
            Street = street,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country,
            // New DTO properties
            Address = dto.Address,
            PersonalDetails = dto.PersonalDetails,
            ContactInformation = dto.ContactInformation,
            ForeignIdentification = dto.ForeignIdentification,
            Dependents = dto.Dependents,
            ResidencyHistory = dto.ResidencyHistory,
            ImmigrationHistories = dto.ImmigrationHistories,
            EducationQualifications = dto.EducationQualifications,
            EmploymentHistory = dto.EmploymentHistory,
            ForeignCitizenshipApplications = dto.ForeignCitizenshipApplications,
            CriminalHistory = dto.CriminalHistory,
            References = dto.References,
            Documents = dto.Documents,
            // Additional properties
            TenantId = dto.TenantId,
            IsPrimaryApplication = dto.IsPrimaryApplication,
            HaveForeignCitizenshipApplication = dto.HaveForeignCitizenshipApplication,
            HaveCriminalRecord = dto.HaveCriminalRecord,
            PaymentTransactionId = dto.PaymentTransactionId,
            IsPaymentProcessed = dto.IsPaymentProcessed,
            Fee = dto.Fee,
            IdentificationCardFee = dto.IdentificationCardFee,
            RushToCitizen = dto.RushToCitizen,
            RushToDiplomacy = dto.RushToDiplomacy,
            ReviewedBy = dto.ReviewedBy,
            PhotoUrl = dto.PhotoUrl
        };
    }

    // Wrapper for deserializing PagedResult from API (matches Mamey.Types.PagedResult structure)
    private class PagedResultDto<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public long TotalResults { get; set; }
    }

    private class CreateApplicationResponse
    {
        public Guid ApplicationId { get; set; }
    }
}
