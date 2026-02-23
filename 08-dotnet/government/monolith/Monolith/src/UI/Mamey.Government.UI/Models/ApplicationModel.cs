using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;
using Mamey.Types;

namespace Mamey.Government.UI.Models;

/// <summary>
/// Client-side citizenship application model.
/// </summary>
public class ApplicationModel
{
    public Guid Id { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? Nickname { get; set; }
    public string ApplicantName => $"{FirstName} {LastName}";
    public DateTime DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Step { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovedBy { get; set; }
    public string? RejectedBy { get; set; }
    
    // Address (legacy support)
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    
    // New DTO properties
    public Address? Address { get; set; }
    public PersonalDetailsDto? PersonalDetails { get; set; }
    public ContactInformationDto? ContactInformation { get; set; }
    public ForeignIdentificationDto? ForeignIdentification { get; set; }
    public IReadOnlyList<DependentDto>? Dependents { get; set; }
    public IReadOnlyList<ResidencyDto>? ResidencyHistory { get; set; }
    public IReadOnlyList<ImmigrationHistoryDto>? ImmigrationHistories { get; set; }
    public IReadOnlyList<EducationQualificationDto>? EducationQualifications { get; set; }
    public IReadOnlyList<EmploymentHistoryDto>? EmploymentHistory { get; set; }
    public IReadOnlyList<ForeignCitizenshipApplicationDto>? ForeignCitizenshipApplications { get; set; }
    public IReadOnlyList<CriminalHistoryDto>? CriminalHistory { get; set; }
    public IReadOnlyList<ReferenceDto>? References { get; set; }
    public IReadOnlyList<ApplicationDocumentDto>? Documents { get; set; }
    
    // Additional properties from ApplicationDto
    public Guid TenantId { get; set; }
    public bool IsPrimaryApplication { get; set; }
    public bool? HaveForeignCitizenshipApplication { get; set; }
    public bool? HaveCriminalRecord { get; set; }
    public Guid? PaymentTransactionId { get; set; }
    public bool IsPaymentProcessed { get; set; }
    public decimal Fee { get; set; }
    public decimal IdentificationCardFee { get; set; }
    public bool RushToCitizen { get; set; }
    public bool RushToDiplomacy { get; set; }
    public Guid? ReviewedBy { get; set; }
    public string? PhotoUrl { get; set; }
}