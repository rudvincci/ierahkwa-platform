using Mamey.Contexts;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Application.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Security;
using Mamey.Time;
using Mamey.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Application.Services;

internal sealed class UserService : IUserService
{
    #region Read-only Fields

    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ISubjectRepository _subjectRepository;
    // private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IHasher _hasher;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly IEventProcessor _eventProcessor;
    // private readonly IPrivateKeyService _privateKeyGenerator;
    private readonly IContext _context;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    #endregion

    public UserService(
        IUserRepository userRepository,
        ISubjectRepository subjectRepository,
        // IPasswordHasher<User> passwordHasher,
        IHasher _hasher,
        IClock clock,
        IMessageBroker messageBroker,
        IEventProcessor eventProcessor,
        // IPrivateKeyService privateKeyGenerator,
        IContext context,
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _subjectRepository = subjectRepository;
        this._hasher = _hasher;
        // _passwordHasher = passwordHasher;
        _clock = clock;
        _messageBroker = messageBroker;
        _eventProcessor = eventProcessor;
        // _privateKeyGenerator = privateKeyGenerator;
        _context = context;
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _logger = logger;
    }

    #region User CRUD Operations

    public async Task<Mamey.Government.Identity.Contracts.DTO.UserDto?> GetUserAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var query = new Mamey.Government.Identity.Contracts.Queries.GetUser(id);
        return await _queryDispatcher.QueryAsync<Mamey.Government.Identity.Contracts.Queries.GetUser, Mamey.Government.Identity.Contracts.DTO.UserDto>(query, cancellationToken);
    }

    public async Task<Mamey.Government.Identity.Contracts.DTO.UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var query = new Mamey.Government.Identity.Contracts.Queries.GetUserByEmail(email);
        return await _queryDispatcher.QueryAsync<Mamey.Government.Identity.Contracts.Queries.GetUserByEmail, Mamey.Government.Identity.Contracts.DTO.UserDto>(query, cancellationToken);
    }

    public async Task<Mamey.Government.Identity.Contracts.DTO.UserDto> CreateUserAsync(CreateUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user: {Username} ({Email})", command.Username, command.Email);

        // Validate subject exists
        var subject = await _subjectRepository.GetAsync(command.SubjectId, cancellationToken);
        if (subject is null)
        {
            throw new SubjectNotFoundException(command.SubjectId);
        }

        // Check if user already exists
        var email = new Email(command.Email);
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser is not null)
        {
            throw new UserAlreadyExistsException(command.Email);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        var user = await _userRepository.GetAsync(command.Id, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.Id);
        }

        await _messageBroker.PublishAsync(new UserCreated(user));

        return MapToUserDto(user);
    }

    public async Task<Mamey.Government.Identity.Contracts.DTO.UserDto> UpdateUserAsync(UpdateUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user: {UserId}", command.Id);

        var user = await _userRepository.GetAsync(command.Id, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.Id);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        user = await _userRepository.GetAsync(command.Id, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.Id);
        }

        return MapToUserDto(user);
    }

    public async Task DeleteUserAsync(UserId id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting user: {UserId}", id);

        var user = await _userRepository.GetAsync(id, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(id);
        }

        // Soft delete by deactivating
        var deactivateCommand = new DeactivateUser(id);
        await _commandDispatcher.SendAsync(deactivateCommand, cancellationToken);
    }

    #endregion

    #region User Status Management

    public async Task ActivateUserAsync(ActivateUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating user: {UserId}", command.UserId);

        var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task DeactivateUserAsync(DeactivateUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating user: {UserId}", command.UserId);

        var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task LockUserAsync(LockUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Locking user: {UserId} until {LockedUntil}", command.UserId, command.LockedUntil);

        var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task UnlockUserAsync(UnlockUser command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Unlocking user: {UserId}", command.UserId);

        var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    #endregion

    #region Password Management

    public async Task ChangeUserPasswordAsync(ChangeUserPassword command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Changing password for user: {UserId}", command.UserId);

        var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task ResetPasswordAsync(string email, string newPassword, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resetting password for email: {Email}", email);

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(email);
        }

        var hashedPassword = _hasher.Hash(newPassword);
        var command = new ChangeUserPassword(user.Id, hashedPassword);
        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    #endregion

    #region User Search and Filtering

    public async Task<IEnumerable<Mamey.Government.Identity.Contracts.DTO.UserDto>> GetUsersByStatusAsync(bool isActive, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByStatusAsync(UserStatus.Active, cancellationToken);
        return users.Select(MapToUserDto);
    }

    public async Task<IEnumerable<Mamey.Government.Identity.Contracts.DTO.UserDto>> GetUsersByRoleAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByRoleIdAsync(roleId, cancellationToken);
        return users.Select(MapToUserDto);
    }

    public async Task<IEnumerable<Mamey.Government.Identity.Contracts.DTO.UserDto>> GetRecentlyActiveUsersAsync(TimeSpan timeSpan, CancellationToken cancellationToken = default)
    {
        var cutoffDate = _clock.CurrentDate().Subtract(timeSpan);
        var users = await _userRepository.GetRecentlyAuthenticatedAsync(cutoffDate, cancellationToken);
        return users.Select(MapToUserDto);
    }

    #endregion

    #region User Statistics

    public async Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var totalUsers = await _userRepository.CountAsync(cancellationToken);
        var activeUsers = await _userRepository.CountByStatusAsync(UserStatus.Active, cancellationToken);
        var lockedUsers = await _userRepository.CountLockedAsync(cancellationToken);
        var usersWith2FA = await _userRepository.CountWithTwoFactorAsync(cancellationToken);
        var usersWithMFA = await _userRepository.CountWithMultiFactorAsync(cancellationToken);

        return new UserStatisticsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = totalUsers - activeUsers,
            LockedUsers = lockedUsers,
            UsersWithTwoFactor = usersWith2FA,
            UsersWithMultiFactor = usersWithMFA
        };
    }

    #endregion

    #region Private Helper Methods

    private static Mamey.Government.Identity.Contracts.DTO.UserDto MapToUserDto(User user)
    {
        return new Mamey.Government.Identity.Contracts.DTO.UserDto(
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

    #endregion
}

