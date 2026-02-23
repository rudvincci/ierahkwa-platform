using Mamey.Types;

namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record ApplicationDto(
    Guid Id,
    Guid TenantId,
    string ApplicationNumber,
    string FirstName,
    string LastName,
    string? MiddleName,
    string? Nickname,
    DateTime DateOfBirth,
    string Status,
    string Step,
    string? Email,
    string? Phone,
    Address? Address,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    DateTime? RejectedAt,
    string? RejectionReason,
    string? ApprovedBy,
    string? RejectedBy,
    Guid? ReviewedBy,
    string? PhotoUrl,
    // Extended properties
    bool IsPrimaryApplication,
    bool? HaveForeignCitizenshipApplication,
    bool? HaveCriminalRecord,
    PersonalDetailsDto? PersonalDetails,
    ContactInformationDto? ContactInformation,
    ForeignIdentificationDto? ForeignIdentification,
    IReadOnlyList<DependentDto>? Dependents,
    IReadOnlyList<ResidencyDto>? ResidencyHistory,
    IReadOnlyList<ImmigrationHistoryDto>? ImmigrationHistories,
    IReadOnlyList<EducationQualificationDto>? EducationQualifications,
    IReadOnlyList<EmploymentHistoryDto>? EmploymentHistory,
    IReadOnlyList<ForeignCitizenshipApplicationDto>? ForeignCitizenshipApplications,
    IReadOnlyList<CriminalHistoryDto>? CriminalHistory,
    IReadOnlyList<ReferenceDto>? References,
    // Payment and fees
    Guid? PaymentTransactionId,
    bool IsPaymentProcessed,
    decimal Fee,
    decimal IdentificationCardFee,
    // Rush processing
    bool RushToCitizen,
    bool RushToDiplomacy,
    // Documents
    IReadOnlyList<ApplicationDocumentDto> Documents);

// Value Object DTOs