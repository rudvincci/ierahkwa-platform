using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;
using Mamey.Types;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Queries.Handlers;

internal sealed class GetApplicationHandler : IQueryHandler<GetApplication, ApplicationDto?>
{
    private readonly IApplicationRepository _repository;

    public GetApplicationHandler(IApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApplicationDto?> HandleAsync(GetApplication query, CancellationToken cancellationToken = default)
    {
        var application = await _repository.GetAsync(new AppId(query.ApplicationId), cancellationToken);
        if (application is null)
        {
            return null;
        }

        return MapToDto(application);
    }

    internal static ApplicationDto MapToDto(CitizenshipApplication application)
    {
        var documents = application.Uploads.Select(d => new ApplicationDocumentDto(
            d.Id,
            d.DocumentType,
            d.FileName,
            d.StoragePath,
            d.FileSize,
            d.UploadedAt)).ToList();

        return new ApplicationDto(
            application.Id.Value,
            application.TenantId.Value,
            application.ApplicationNumber.Value,
            application.ApplicantName.FirstName,
            application.ApplicantName.LastName,
            application.ApplicantName.MiddleName,
            application.ApplicantName.Nickname,
            application.DateOfBirth,
            application.Status.ToString(),
            application.Step.ToString(),
            application.Email?.Value,
            application.Phone?.ToString(),
            application.Address != null ? application.Address : null,
            application.CreatedAt,
            application.UpdatedAt,
            application.SubmittedAt,
            application.ApprovedAt,
            application.RejectedAt,
            application.RejectionReason,
            application.ApprovedBy,
            application.RejectedBy,
            application.ReviewedBy?.Value,
            null, // PhotoUrl - not implemented yet
            application.IsPrimaryApplication,
            application.HaveForeignCitizenshipApplication,
            application.HaveCriminalRecord,
            application.PersonalDetails != null ? MapPersonalDetails(application.PersonalDetails) : null,
            application.ContactInformation != null ? MapContactInformation(application.ContactInformation) : null,
            application.ForeignIdentification != null ? MapForeignIdentification(application.ForeignIdentification) : null,
            application.Dependents?.Select(MapDependent).ToList(),
            application.ResidencyHistory?.Select(MapResidency).ToList(),
            application.ImmigrationHistories?.Select(MapImmigrationHistory).ToList(),
            application.EducationQualifications?.Select(MapEducationQualification).ToList(),
            application.EmploymentHistory?.Select(MapEmploymentHistory).ToList(),
            application.ForeignCitizenshipApplications?.Select(MapForeignCitizenshipApplication).ToList(),
            application.CriminalHistory?.Select(MapCriminalHistory).ToList(),
            application.References?.Select(MapReference).ToList(),
            application.PaymentTransactionId,
            application.IsPaymentProcessed,
            application.Fee,
            application.IdentificationCardFee,
            application.RushToCitizen,
            application.RushToDiplomacy,
            documents);
    }

    private static AddressDto MapAddress(Address address)
    {
        return new AddressDto(
            address.FirmName,
            address.Line,
            address.Line2,
            address.Line3,
            address.Urbanization,
            address.City,
            address.State,
            address.Zip5,
            address.Zip4,
            address.PostalCode,
            address.Country,
            address.Province,
            address.Type.ToString(),
            address.IsDefault);
    }

    private static PersonalDetailsDto MapPersonalDetails(PersonalDetails details)
    {
        return new PersonalDetailsDto(
            details.DateOfBirth,
            details.PlaceOfBirth,
            details.EyeColor.ToString(),
            details.HairColor.ToString(),
            details.HeightInInches,
            details.WeightInPounds,
            details.Gender.ToString(),
            details.MaritalStatus.ToString(),
            details.SpouseName != null ? MapName(details.SpouseName) : null,
            details.CurrentNationalities,
            details.Aliases);
    }

    private static ContactInformationDto MapContactInformation(ContactInformation contact)
    {
        return new ContactInformationDto(
            contact.Email?.Value,
            contact.ResidentialAddress != null ? MapAddress(contact.ResidentialAddress) : null,
            contact.PostalAddress != null ? MapAddress(contact.PostalAddress) : null,
            contact.PhoneNumbers.Select(p => p.ToString()).ToList(),
            contact.EmergencyContactName,
            contact.EmergencyContactPhoneNumber,
            contact.EmergencyContactEmail,
            contact.EmergencyContactAddress,
            contact.EmergencyContactRelationship);
    }

    private static ForeignIdentificationDto MapForeignIdentification(ForeignIdentification identification)
    {
        return new ForeignIdentificationDto(
            identification.Number,
            identification.IssuingCountry,
            identification.ExpiryDate);
    }

    private static DependentDto MapDependent(Dependent dependent)
    {
        return new DependentDto(
            MapName(dependent.Name),
            dependent.DateOfBirth,
            dependent.Gender.ToString(),
            dependent.Age);
    }

    private static ResidencyDto MapResidency(Residency residency)
    {
        return new ResidencyDto(
            residency.CountryCode,
            residency.EffectiveDate,
            residency.Status);
    }

    private static ImmigrationHistoryDto MapImmigrationHistory(ImmigrationHistory history)
    {
        return new ImmigrationHistoryDto(
            history.PreviouslyDeportedFromCountry,
            history.PreviouslyDeportedFromCountryCode,
            history.PreviouslyDeportedFromCountryAt,
            history.PreviouslyDeportedFromCountryDetails);
    }

    private static EducationQualificationDto MapEducationQualification(EducationQualification qualification)
    {
        return new EducationQualificationDto(
            qualification.Name,
            qualification.AwardedBy,
            qualification.EffectiveDate);
    }

    private static EmploymentHistoryDto MapEmploymentHistory(EmploymentHistory employment)
    {
        return new EmploymentHistoryDto(
            employment.Employer,
            employment.Occupation,
            employment.EmployedFrom,
            employment.EmployedTo,
            employment.EmployerAddress != null ? MapAddress(employment.EmployerAddress) : null,
            employment.EmployerPhone?.ToString());
    }

    private static ForeignCitizenshipApplicationDto MapForeignCitizenshipApplication(ForeignCitizenshipApplication app)
    {
        return new ForeignCitizenshipApplicationDto(
            app.CountryCode,
            app.EffectiveDate,
            app.Status,
            app.ForeignCitizenshipApplicationDetails);
    }

    private static CriminalHistoryDto MapCriminalHistory(CriminalHistory history)
    {
        return new CriminalHistoryDto(
            history.Charge,
            history.EffectiveDate,
            history.CriminalChargeJurisdiction,
            history.Convicted,
            history.CriminalRecordDetails);
    }

    private static ReferenceDto MapReference(Reference reference)
    {
        return new ReferenceDto(
            MapName(reference.Name),
            MapAddress(reference.Address),
            reference.Phone.ToString(),
            reference.Relationship);
    }

    private static NameDto MapName(Name name)
    {
        return new NameDto(
            name.FirstName,
            name.MiddleName,
            name.LastName,
            name.Nickname);
    }
}
