using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Commands.Handlers;

public class SignInHandlerTests
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IEventProcessor _eventProcessor;
    private readonly SignInHandler _handler;

    public SignInHandlerTests()
    {
        _authenticationService = Substitute.For<IAuthenticationService>();
        _eventProcessor = Substitute.For<IEventProcessor>();
        _handler = new SignInHandler(_authenticationService, _eventProcessor);
    }

    [Fact]
    public async Task HandleAsync_WithValidCredentials_ShouldCallAuthenticationService()
    {
        // Arrange
        var command = new SignIn
        {
            Username = "testuser",
            Password = "password123",
            IpAddress = "192.168.1.1",
            UserAgent = "Mozilla/5.0"
        };

        var authResult = new AuthenticationResult
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            SessionId = new SessionId(),
            IdentityId = new IdentityId(Guid.NewGuid()),
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _authenticationService.SignInAsync(
            command.Username,
            command.Password,
            command.IpAddress,
            command.UserAgent,
            Arg.Any<CancellationToken>())
            .Returns(authResult);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        await _authenticationService.Received(1).SignInAsync(
            command.Username,
            command.Password,
            command.IpAddress,
            command.UserAgent,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithInvalidCredentials_ShouldThrowException()
    {
        // Arrange
        var command = new SignIn
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        _authenticationService.SignInAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromException<AuthenticationResult>(new InvalidOperationException("Invalid username or password")));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command));
    }
}

