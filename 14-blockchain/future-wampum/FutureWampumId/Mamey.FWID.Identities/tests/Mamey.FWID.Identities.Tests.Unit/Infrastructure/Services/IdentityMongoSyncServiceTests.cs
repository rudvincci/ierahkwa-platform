using System.Linq;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Options;
using Mamey.FWID.Identities.Infrastructure.Mongo.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Services;
using Mamey.FWID.Identities.Infrastructure.Security;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Security;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Infrastructure.Services;

public class IdentityMongoSyncServiceTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IdentityMongoSyncService> _logger;
    private readonly IOptions<MongoSyncOptions> _options;
    private readonly IdentityMongoSyncService _service;

    public IdentityMongoSyncServiceTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _logger = Substitute.For<ILogger<IdentityMongoSyncService>>();
        _options = Substitute.For<IOptions<MongoSyncOptions>>();
        _options.Value.Returns(new MongoSyncOptions
        {
            Enabled = true,
            InitialDelay = TimeSpan.Zero,
            SyncInterval = TimeSpan.FromSeconds(1),
            RetryDelay = TimeSpan.FromSeconds(1)
        });
        _service = new IdentityMongoSyncService(_serviceProvider, _logger, _options);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateService()
    {
        // Arrange & Act
        var service = new IdentityMongoSyncService(_serviceProvider, _logger, _options);

        // Assert
        service.ShouldNotBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WhenDisabled_ShouldReturnImmediately()
    {
        // Arrange
        _options.Value.Returns(new MongoSyncOptions
        {
            Enabled = false
        });
        var service = new IdentityMongoSyncService(_serviceProvider, _logger, _options);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));

        // Act
        // Use reflection to call protected ExecuteAsync method
        // Note: async methods are compiled with a state machine, so we need to find the method by name and signature
        var executeMethod = typeof(IdentityMongoSyncService)
            .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "ExecuteAsync" && 
                                 m.GetParameters().Length == 1 && 
                                 m.GetParameters()[0].ParameterType == typeof(CancellationToken) &&
                                 m.ReturnType == typeof(Task));
        
        if (executeMethod == null)
        {
            throw new InvalidOperationException("ExecuteAsync method not found");
        }
        
        try
        {
            var task = (Task)executeMethod.Invoke(service, new object[] { cancellationTokenSource.Token })!;
            await task;
        }
        catch (System.Reflection.TargetInvocationException ex)
        {
            // Unwrap the inner exception if it's a TargetInvocationException
            if (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
            throw;
        }

        // Assert
        // Verify that the log was called with the expected message
        _logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("MongoDB sync service is disabled")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

}

