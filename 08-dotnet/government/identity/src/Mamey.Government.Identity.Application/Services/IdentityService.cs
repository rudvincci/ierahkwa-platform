using Mamey.Contexts;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Auth.Jwt;
using Mamey.Time;
using Mamey.Types;
using Mamey.Constants;
using Mamey.Government.Identity.Application.Auth;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PermissionNotFoundException = Mamey.Government.Identity.Application.Exceptions.PermissionNotFoundException;
using UserCreated = Mamey.Government.Identity.Application.Events.UserCreated;

namespace Mamey.Government.Identity.Application.Services;

internal sealed class IdentityService : IIdentityService
{
    #region Read-only Fields

    private readonly ILogger<IdentityService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;
    private readonly ITwoFactorAuthRepository _twoFactorAuthRepository;
    private readonly IMultiFactorAuthRepository _multiFactorAuthRepository;
    private readonly ICredentialRepository _credentialRepository;
    private readonly IHasher _hasher;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly IEventProcessor _eventProcessor;
    // private readonly IJwtHandler _jwtHandler;
    private readonly IJwtProvider _jwtProvider;
    // private readonly IPrivateKeyService _privateKeyGenerator;
    private readonly IContext _context;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    #endregion

    public IdentityService(
        IUserRepository userRepository,
        ISubjectRepository subjectRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        ISessionRepository sessionRepository,
        IEmailConfirmationRepository emailConfirmationRepository,
        ITwoFactorAuthRepository twoFactorAuthRepository,
        IMultiFactorAuthRepository multiFactorAuthRepository,
        ICredentialRepository credentialRepository,
        IHasher hasher,
        IClock clock,
        IMessageBroker messageBroker,
        // IJwtHandler jwtHandler,
        ILogger<IdentityService> logger,
        IEventProcessor eventProcessor,
        // IPrivateKeyService privateKeyGenerator,
        IContext context,
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _subjectRepository = subjectRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _sessionRepository = sessionRepository;
        _emailConfirmationRepository = emailConfirmationRepository;
        _twoFactorAuthRepository = twoFactorAuthRepository;
        _multiFactorAuthRepository = multiFactorAuthRepository;
        _credentialRepository = credentialRepository;
        _hasher = hasher;
        _clock = clock;
        _messageBroker = messageBroker;
        // _jwtHandler = jwtHandler;
        _logger = logger;
        _eventProcessor = eventProcessor;
        // _privateKeyGenerator = privateKeyGenerator;
        _context = context;
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _jwtProvider = jwtProvider;
    }

    #region User Management

    public async Task<UserDto?> GetUserAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(id, cancellationToken);
        return user is null ? null : MapToUserDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return user is null ? null : MapToUserDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user: {Username} ({Email})", command.Username, command.Email);

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        var user = await _userRepository.GetAsync(command.Id, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.Id);
        }

        await _messageBroker.PublishAsync(new UserCreated(user));

        return MapToUserDto(user);
    }

    public async Task<UserDto> UpdateUserAsync(UpdateUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user: {UserId}", command.Id);

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        var user = await _userRepository.GetAsync(command.Id, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.Id);
        }

        return MapToUserDto(user);
    }

    public async Task ChangeUserPasswordAsync(ChangeUserPassword command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Changing password for user: {UserId}", command.UserId);

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task ActivateUserAsync(ActivateUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating user: {UserId}", command.UserId);

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task DeactivateUserAsync(DeactivateUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating user: {UserId}", command.UserId);

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task LockUserAsync(LockUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Locking user: {UserId} until {LockedUntil}", command.UserId, command.LockedUntil);

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task UnlockUserAsync(UnlockUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Unlocking user: {UserId}", command.UserId);

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    #endregion

    #region Authentication

    public async Task<AuthDto?> SignInAsync(string usernameOrEmail, string password, string? ipAddress = null, string? userAgent = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sign in attempt for username or email: {UsernameOrEmail}", usernameOrEmail);

        // Use regex to determine if input is an email or username
        var isEmail = RegularExpressions.Email.IsMatch(usernameOrEmail);
        User? user = null;

        if (isEmail)
        {
            // Input is an email, try email lookup
            try
            {
                user = await _userRepository.GetByEmailAsync(usernameOrEmail, cancellationToken);
            }
            catch
            {
                // Email lookup failed
            }
        }
        else
        {
            // Input is a username, try username lookup
            try
            {
                user = await _userRepository.GetByUsernameAsync(usernameOrEmail, cancellationToken);
            }
            catch
            {
                // Username lookup failed
            }
        }

        // If lookup failed, throw invalid credentials
        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        var pwHashToVerify = _hasher.Hash(password);
        if (pwHashToVerify != user.PasswordHash)
        {
            throw new InvalidCredentialsException();
        }

        if (!user.IsActive)
        {
            throw new UserNotActiveException(user.Id);
        }

        if (user.IsLocked)
        {
            throw new UserLockedException(user.Id);
        }

        // Record successful login
        user.RecordLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Get subject and roles
        var subject = await _subjectRepository.GetAsync(user.SubjectId, cancellationToken);
        if (subject is null)
        {
            throw new SubjectNotFoundException(user.SubjectId);
        }

        // Create session
        var sessionId = Guid.NewGuid();
        var jwt = _jwtProvider.Create(user.Id, user.Username, user.Email, string.Join(',', subject.Roles.Select(c=> c.Value.ToString())), UserType.User, user.Status, Constants.User.Permission.None);
        var refreshToken = Guid.NewGuid().ToString("N");

        var createSessionCommand = new CreateSession(sessionId, user.Id, jwt.AccessToken, refreshToken, _clock.CurrentDate().AddHours(24), ipAddress, userAgent);
        await _commandDispatcher.SendAsync(createSessionCommand, cancellationToken);

        await _messageBroker.PublishAsync(new Mamey.Government.Identity.Application.Events.UserLoggedIn(user, ipAddress));

        // Build claims dictionary from roles
        var claims = subject.Roles.ToDictionary(
            r => $"role:{r.Value}",
            r => r.Value.ToString()
        );
        
        // Add additional claims
        claims["sub"] = user.Id.ToString();
        claims["username"] = user.Username;
        claims["email"] = user.Email.Value;
        claims["subjectId"] = user.SubjectId.Value.ToString();

        return new AuthDto
        {
            AccessToken = jwt.AccessToken,
            RefreshToken = jwt.RefreshToken,
            ExpiresAt = _clock.CurrentDate().AddHours(24),
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            Claims = claims
        };
    }

    public async Task<AuthDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Refreshing token");

        var session = await _sessionRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        if (session is null || !session.IsActive())
        {
            throw new InvalidRefreshTokenException();
        }

        var user = await _userRepository.GetAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(session.UserId);
        }

        // Get subject and roles
        var subject = await _subjectRepository.GetAsync(user.SubjectId, cancellationToken);
        if (subject is null)
        {
            throw new SubjectNotFoundException(user.SubjectId);
        }
        var newAccessToken = _jwtProvider.Create(user.Id, user.Username, user.Email, string.Join(',', subject.Roles.Select(c=> c.Value.ToString())), UserType.User, user.Status, Constants.User.Permission.None);
        // var newAccessToken = _jwtHandler.CreateToken(user.Id.ToString(), user.Username, user.Email, subject.Roles.ToDictionary(r => r.ToString(), r => r.ToString()));
        var newRefreshToken = Guid.NewGuid().ToString("N");

        var refreshSessionCommand = new RefreshSession(session.Id, newAccessToken.AccessToken, newRefreshToken, _clock.CurrentDate().AddHours(24));
        await _commandDispatcher.SendAsync(refreshSessionCommand, cancellationToken);

        // Build claims dictionary from roles
        var claims = subject.Roles.ToDictionary(
            r => $"role:{r.Value}",
            r => r.Value.ToString()
        );
        
        // Add additional claims
        claims["sub"] = user.Id.ToString();
        claims["username"] = user.Username;
        claims["email"] = user.Email.Value;
        claims["subjectId"] = user.SubjectId.Value.ToString();

        return new AuthDto
        {
            AccessToken = newAccessToken.AccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = _clock.CurrentDate().AddHours(24),
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            Claims = claims
        };
    }

    public async Task SignOutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sign out");

        var session = await _sessionRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        if (session is not null)
        {
            var revokeSessionCommand = new RevokeSession(session.Id);
            await _commandDispatcher.SendAsync(revokeSessionCommand, cancellationToken);
        }
    }

    #endregion

    #region Email Confirmation

    public async Task<string> CreateEmailConfirmationAsync(Guid userId, string email, string? ipAddress = null, string? userAgent = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating email confirmation for user: {UserId}", userId);

        var confirmationId = Guid.NewGuid();
        var confirmationCode = GenerateConfirmationCode();
        var expiresAt = _clock.CurrentDate().AddHours(24);

        var command = new CreateEmailConfirmation(confirmationId, userId, email, confirmationCode, expiresAt, ipAddress, userAgent);
        await _commandDispatcher.SendAsync(command, cancellationToken);

        return confirmationCode;
    }

    public async Task ConfirmEmailAsync(string confirmationCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Confirming email with code: {Code}", confirmationCode);

        var command = new ConfirmEmail(confirmationCode);
        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task ResendEmailConfirmationAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resending email confirmation for user: {UserId}", userId);

        var newConfirmationCode = GenerateConfirmationCode();
        var newExpiresAt = _clock.CurrentDate().AddHours(24);

        var command = new ResendEmailConfirmation(userId, newConfirmationCode, newExpiresAt);
        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    #endregion

    #region Two-Factor Authentication

    public async Task<TwoFactorSetupDto> SetupTwoFactorAuthAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting up 2FA for user: {UserId}", userId);

        var twoFactorAuthId = Guid.NewGuid();
        var secretKey = GenerateSecretKey();
        var qrCodeUrl = GenerateQrCodeUrl(userId, secretKey);

        var command = new SetupTwoFactorAuth(twoFactorAuthId, userId, secretKey, qrCodeUrl);
        await _commandDispatcher.SendAsync(command, cancellationToken);

        return new TwoFactorSetupDto
        {
            SecretKey = secretKey,
            QrCodeUrl = qrCodeUrl
        };
    }

    public async Task ActivateTwoFactorAuthAsync(Guid userId, string totpCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating 2FA for user: {UserId}", userId);

        var command = new ActivateTwoFactorAuth(userId, totpCode);
        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task<bool> VerifyTwoFactorAuthAsync(Guid userId, string totpCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying 2FA for user: {UserId}", userId);

        var command = new VerifyTwoFactorAuth(userId, totpCode);
        await _commandDispatcher.SendAsync(command, cancellationToken);

        return true; // If no exception is thrown, verification succeeded
    }

    public async Task DisableTwoFactorAuthAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Disabling 2FA for user: {UserId}", userId);

        var command = new DisableTwoFactorAuth(userId);
        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    #endregion

    #region Multi-Factor Authentication

    public async Task<MultiFactorSetupDto> SetupMultiFactorAuthAsync(Guid userId, IEnumerable<int>? enabledMethods = null, int requiredMethods = 2, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting up MFA for user: {UserId}", userId);

        var multiFactorAuthId = Guid.NewGuid();

        var command = new SetupMultiFactorAuth(multiFactorAuthId, userId, enabledMethods, requiredMethods);
        await _commandDispatcher.SendAsync(command, cancellationToken);

        return new MultiFactorSetupDto
        {
            EnabledMethods = enabledMethods ?? Enumerable.Empty<int>(),
            RequiredMethods = requiredMethods
        };
    }

    public async Task EnableMfaMethodAsync(Guid userId, int method, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Enabling MFA method {Method} for user: {UserId}", method, userId);

        var command = new EnableMfaMethod(userId, method);
        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task DisableMfaMethodAsync(Guid userId, int method, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Disabling MFA method {Method} for user: {UserId}", method, userId);

        var command = new DisableMfaMethod(userId, method);
        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task<string> CreateMfaChallengeAsync(Guid multiFactorAuthId, int method, string challengeData, string? ipAddress = null, string? userAgent = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating MFA challenge for multiFactorAuthId: {MultiFactorAuthId}", multiFactorAuthId);

        var challengeId = Guid.NewGuid();
        var expiresAt = _clock.CurrentDate().AddMinutes(10);

        var command = new CreateMfaChallenge(challengeId, multiFactorAuthId, method, challengeData, expiresAt, ipAddress, userAgent);
        await _commandDispatcher.SendAsync(command, cancellationToken);

        return challengeId.ToString();
    }

    public async Task<bool> VerifyMfaChallengeAsync(Guid challengeId, string response, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying MFA challenge: {ChallengeId}", challengeId);

        var command = new VerifyMfaChallenge(challengeId, response);
        await _commandDispatcher.SendAsync(command, cancellationToken);

        return true; // If no exception is thrown, verification succeeded
    }

    #endregion

    #region Role Management

    public async Task<RoleDto> CreateRoleAsync(CreateRole command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating role: {Name}", command.Name);

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        var role = await _roleRepository.GetAsync(command.Id, cancellationToken);
        if (role is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.RoleNotFoundException(command.Id);
        }

        return MapToRoleDto(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(UpdateRole command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating role: {RoleId}", command.Id);

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        var role = await _roleRepository.GetAsync(command.Id, cancellationToken);
        if (role is null)
        {
            throw new Mamey.Government.Identity.Domain.Exceptions.RoleNotFoundException(command.Id);
        }

        return MapToRoleDto(role);
    }

    public async Task AssignRoleToSubjectAsync(AssignRoleToSubject command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Assigning role {RoleId} to subject {SubjectId}", command.RoleId, command.SubjectId);

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task RemoveRoleFromSubjectAsync(RemoveRoleFromSubject command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing role {RoleId} from subject {SubjectId}", command.RoleId, command.SubjectId);

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    #endregion

    #region Permission Management

    public async Task<PermissionDto> CreatePermissionAsync(CreatePermission command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating permission: {Name}", command.Name);

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        var permission = await _permissionRepository.GetAsync(command.Id, cancellationToken);
        if (permission is null)
        {
            throw new PermissionNotFoundException(command.Id);
        }

        return MapToPermissionDto(permission);
    }

    public async Task<PermissionDto> UpdatePermissionAsync(UpdatePermission command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating permission: {PermissionId}", command.Id);

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        var permission = await _permissionRepository.GetAsync(command.Id, cancellationToken);
        if (permission is null)
        {
            throw new PermissionNotFoundException(command.Id);
        }

        return MapToPermissionDto(permission);
    }

    #endregion

    #region Private Helper Methods

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto(
            user.Id,
            user.SubjectId,
            user.Username,
            user.Email,
            user.IsActive,
            user.IsLocked,
            user.LockedUntil,
            user.LastLoginAt,
            user.EmailConfirmationRequired,
            user.TwoFactorEnabled,
            user.MultiFactorEnabled,
            user.EmailConfirmedAt,
            user.TwoFactorEnabledAt,
            user.MultiFactorEnabledAt,
            user.CreatedAt,
            user.ModifiedAt
        );
    }

    private static RoleDto MapToRoleDto(Role role)
    {
        return new RoleDto(
            role.Id,
            role.Name,
            role.Description,
            role.Status.ToString(),
            role.Permissions.Select(p => p.Value),
            role.CreatedAt,
            role.ModifiedAt
        );
    }

    private static PermissionDto MapToPermissionDto(Permission permission)
    {
        return new PermissionDto(
            permission.Id,
            permission.Name,
            permission.Description,
            permission.Resource,
            permission.Action,
            permission.Status.ToString(),
            permission.CreatedAt,
            permission.ModifiedAt
        );
    }

    private static string GenerateConfirmationCode()
    {
        return Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    }

    private static string GenerateSecretKey()
    {
        return Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();
    }

    private static string GenerateQrCodeUrl(Guid userId, string secretKey)
    {
        return $"otpauth://totp/GovernmentIdentity:{userId}?secret={secretKey}&issuer=GovernmentIdentity";
    }

    #endregion
}

// DTOs for service responses


