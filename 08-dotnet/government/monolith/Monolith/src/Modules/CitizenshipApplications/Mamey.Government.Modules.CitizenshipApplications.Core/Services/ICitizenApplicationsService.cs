using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;
using Mamey.Government.Modules.CitizenshipApplications.Core.Commands;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.EF;
using Microsoft.AspNetCore.Http;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

public interface ICitizenApplicationsService
{
    Task<ResumeApplicationResponse?> ResumeApplicationAsync(ResumeApplication command, CancellationToken cancellationToken = default);
}

internal class CitizenApplicationsService : ICitizenApplicationsService
{
    private readonly IContext _context;
    private readonly IApplicationRepository _repository;
    private readonly IApplicationTokenRepository _tokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IGeoLocationService _geoLocationService;
    private readonly CitizenshipApplicationsUnitOfWork _unitOfWork;

    public CitizenApplicationsService(IContext context, IApplicationRepository repository, IApplicationTokenRepository tokenRepository, 
        ITokenService tokenService, IJwtTokenService jwtTokenService, IGeoLocationService geoLocationService, 
        CitizenshipApplicationsUnitOfWork unitOfWork)
    {
        _context = context;
        _repository = repository;
        _tokenRepository = tokenRepository;
        _tokenService = tokenService;
        _jwtTokenService = jwtTokenService;
        _geoLocationService = geoLocationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResumeApplicationResponse?> ResumeApplicationAsync(ResumeApplication command, CancellationToken cancellationToken = default)
    {
        ResumeApplicationResponse? response = null;
        await _unitOfWork.ExecuteAsync(async () =>
        {
            // Hash the provided token
            var tokenHash = _tokenService.HashToken(command.Token);

            // Look up token in repository
            var tokenEntity = await _tokenRepository.GetByTokenHashAsync(tokenHash, command.Email, cancellationToken);

            if (tokenEntity == null || !tokenEntity.IsValid)
            {
                throw new InvalidOperationException("Invalid or expired token.");
            }

            // Retrieve application
            var application = await _repository.GetAsync(tokenEntity.ApplicationId, cancellationToken);
            if (application == null)
            {
                throw new InvalidOperationException("Application not found.");
            }

            // Mark token as used
            //await _tokenRepository.MarkAsUsedAsync(tokenEntity.Id, cancellationToken);

            // Add access log  TODO: Find bug
            // application.AddAccessLog(await BuildAccessLogAsync("Resume", cancellationToken));
            // await _repository.UpdateAsync(application, cancellationToken);

            // Generate JWT token
            var jwtToken = await _jwtTokenService.GenerateApplicantTokenAsync(
                command.Email,
                application.Id.Value,
                cancellationToken);
            
            var appliationDto = new ApplicationDto(
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
                null,// application.PersonalDetails != null ? MapPersonalDetails(application.PersonalDetails) : null,
                null,// application.ContactInformation != null ? MapContactInformation(application.ContactInformation) : null,
                null,// application.ForeignIdentification != null ? MapForeignIdentification(application.ForeignIdentification) : null,
                null,// application.Dependents?.Select(MapDependent).ToList(),
                null,// application.ResidencyHistory?.Select(MapResidency).ToList(),
                null,// application.ImmigrationHistories?.Select(MapImmigrationHistory).ToList(),
                null,// application.EducationQualifications?.Select(MapEducationQualification).ToList(),
                null,// application.EmploymentHistory?.Select(MapEmploymentHistory).ToList(),
                null,// application.ForeignCitizenshipApplications?.Select(MapForeignCitizenshipApplication).ToList(),
                null,// application.CriminalHistory?.Select(MapCriminalHistory).ToList(),
                null,// application.References?.Select(MapReference).ToList(),
                application.PaymentTransactionId,
                application.IsPaymentProcessed,
                application.Fee,
                application.IdentificationCardFee,
                application.RushToCitizen,
                application.RushToDiplomacy,
                new List<ApplicationDocumentDto>());
            
                response = new ResumeApplicationResponse
                {
                    Application = appliationDto, ApplicationNumber = application.ApplicationNumber, JwtToken = jwtToken
                };
        });

        return response;
    }
    private async Task<ApplicationAccessLog> BuildAccessLogAsync(string action, CancellationToken cancellationToken)
    {
        var geo = await _geoLocationService.LookupAsync(_context.IpAddress, _context.UserAgent, cancellationToken);

        return new ApplicationAccessLog(
            DateTime.UtcNow,
            action,
            _context.DeviceType,
            _context.DeviceId,
            _context.IpAddress,
            _context.MacAddress,
            _context.UserAgent,
            _context.Platform,
            _context.Browser,
            _context.OsVersion,
            _context.AppVersion,
            _context.ScreenResolution,
            _context.Language,
            _context.Timezone,
            _context.Referrer,
            _context.NetworkType,
            geo?.Country,
            geo?.CountryCode,
            geo?.Region,
            geo?.City,
            geo?.PostalCode,
            geo?.Timezone,
            geo?.Latitude ?? _context.Latitude,
            geo?.Longitude ?? _context.Longitude);
    }
}