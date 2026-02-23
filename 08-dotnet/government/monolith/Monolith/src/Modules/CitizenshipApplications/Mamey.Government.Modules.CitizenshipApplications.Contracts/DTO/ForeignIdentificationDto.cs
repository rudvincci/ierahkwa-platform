namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record ForeignIdentificationDto(
    string Number,
    string IssuingCountry,
    DateTime? ExpiryDate);