using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.EF;
using Mamey.Government.Modules.CitizenshipApplications.Core.Services;
using Microsoft.AspNetCore.Http;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands.Handlers;

internal sealed class ResumeApplicationHandler : ICommandHandler<ResumeApplication>
{
    private readonly IContext _context;
    private readonly IApplicationRepository _repository;
    private readonly IApplicationTokenRepository _tokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IGeoLocationService _geoLocationService;
    private readonly CitizenshipApplicationsUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResumeApplicationHandler(
        IContext context,
        IApplicationRepository repository,
        IApplicationTokenRepository tokenRepository,
        ITokenService tokenService,
        IJwtTokenService jwtTokenService,

        IGeoLocationService geoLocationService, CitizenshipApplicationsUnitOfWork unitOfWork)
    {
        _context = context;
        _repository = repository;
        _tokenRepository = tokenRepository;
        _tokenService = tokenService;
        _jwtTokenService = jwtTokenService;
        _geoLocationService = geoLocationService;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(ResumeApplication command, CancellationToken cancellationToken = default)
    {
        
    }

    
}
