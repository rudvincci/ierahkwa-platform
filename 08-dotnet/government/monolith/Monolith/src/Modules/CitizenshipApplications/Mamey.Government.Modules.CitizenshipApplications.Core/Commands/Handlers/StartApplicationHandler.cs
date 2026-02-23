using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CitizenshipApplications.Core.Clients;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CitizenshipApplications.Core.EF;
using Mamey.Government.Modules.CitizenshipApplications.Core.Services;
using Mamey.Types;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands.Handlers;

internal sealed class StartApplicationHandler : ICommandHandler<StartApplication>
{
    private readonly INotificationsClient _notificationsClient;
    private readonly IContext _context;
    private readonly IApplicationRepository _repository;
    private readonly IApplicationNumberService _numberService;
    private readonly IGeoLocationService _geoLocationService;
    private readonly ITokenService _tokenService;
    private readonly IApplicationTokenRepository _tokenRepository;
    private readonly CitizenshipApplicationsUnitOfWork _unitOfWork;

    public StartApplicationHandler(
        INotificationsClient notificationsClient,
        IContext context,
        IApplicationRepository repository,
        IApplicationNumberService numberService,
        IGeoLocationService geoLocationService,
        ITokenService tokenService,
        IApplicationTokenRepository tokenRepository, CitizenshipApplicationsUnitOfWork unitOfWork)
    {
        _notificationsClient = notificationsClient;
        _context = context;
        _repository = repository;
        _numberService = numberService;
        _geoLocationService = geoLocationService;
        _tokenService = tokenService;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(StartApplication command, CancellationToken cancellationToken = default)
    {
        string baseUrl;
        await _unitOfWork.ExecuteAsync(async ()=>
        {
            var tenantId = _context.TenantId ?? throw new InvalidOperationException("TenantId is required to start an application.");
            var existingDocuments = await _repository.GetAllByApplicationEmail(command.Email);
            var existingActiveDocuments = existingDocuments.Where(c=> c.Email == command.Email 
                && (c.Status != ApplicationStatus.Approved || c.Status != ApplicationStatus.Rejected)).ToList();
            CitizenshipApplication application;
            if (existingActiveDocuments.Count == 0)
            {
                var applicationNumber = await _numberService.GenerateAsync("INK-CITAPP", cancellationToken); // TODO: Get from appsettings.json
                var applicantName = new Name(
                    command.FirstName ?? "Applicant",
                    command.LastName ?? "Pending");
                application = new CitizenshipApplication(
                    new AppId(Guid.NewGuid()),
                    new TenantId(tenantId),
                    new ApplicationNumber(applicationNumber),
                    applicantName,
                    DateTime.UtcNow,
                    new Email(command.Email));
            
                application.AddAccessLog(await BuildAccessLogAsync("Start", cancellationToken));
                await _repository.AddAsync(application, cancellationToken);
            }
            else
            {
                application = existingActiveDocuments.FirstOrDefault();
            }
            
            // Generate and store token for resuming application
            var token = await _tokenService.GenerateTokenAsync(cancellationToken);
            var tokenHash = _tokenService.HashToken(token);
            var tokenEntity = new ApplicationToken(
                Guid.NewGuid(),
                application.Id,
                tokenHash,
                command.Email,
                DateTime.UtcNow.AddMinutes(15)); // Token expires in 15 minutes

            await _tokenRepository.AddAsync(tokenEntity, cancellationToken);
            
            var baseUrl = _context.BaseUrl ?? throw new InvalidOperationException("Unable to determine base URL from request context for application resume link.");
            var resumeUrl = $"{baseUrl.TrimEnd('/')}/rca/{token}?email={Uri.EscapeDataString(command.Email)}";
            // Build resume URL with token - use base URL from context for security

            var firstName = command.FirstName ?? _context.DisplayName?.Split(' ').FirstOrDefault();
            var lastName = command.LastName ?? _context.DisplayName?.Split(' ').Skip(1).FirstOrDefault();

            await _notificationsClient.SendApplicationStartEmailAsync(
                command.Email,
                resumeUrl, // Use resume URL with token instead of base application URL
                firstName,
                lastName,
                cancellationToken);
        });
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
