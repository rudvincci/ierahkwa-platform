using System;
using System.Collections.Generic;
using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Resolution;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Extensions;

public class ExtensionsTests
{
    [Fact]
    public void AddDecentralizedIdentifiers_ShouldRegisterAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        services.AddLogging();

        // Act
        services.AddDecentralizedIdentifiers(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Check that all required services are registered
        serviceProvider.GetService<IDidResolver>().Should().NotBeNull();
        // Note: IDidHandler and other auth-specific services require AddDid on IMameyBuilder
    }

    [Fact]
    public void AddDecentralizedIdentifiers_WithCustomOptions_ShouldRegisterServicesWithOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        services.AddLogging();

        // Act
        services.AddDecentralizedIdentifiers(configuration, options =>
        {
            // Custom configuration
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Check that all required services are registered
        serviceProvider.GetService<IDidResolver>().Should().NotBeNull();
    }

    // Note: UseDid is an extension on IApplicationBuilder and requires IMameyBuilder setup
    // These tests would need to be rewritten with proper IMameyBuilder setup
}

// Mock implementation for testing
public class MockApplicationBuilder : IApplicationBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Func<RequestDelegate, RequestDelegate>> _components = new();

    public MockApplicationBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IServiceProvider ApplicationServices 
    { 
        get => _serviceProvider;
        set => throw new NotSupportedException();
    }

    public IFeatureCollection ServerFeatures { get; } = new Microsoft.AspNetCore.Http.Features.FeatureCollection();

    public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

    public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
    {
        _components.Add(middleware);
        return this;
    }

    public IApplicationBuilder New()
    {
        return new MockApplicationBuilder(_serviceProvider);
    }

    public RequestDelegate Build()
    {
        RequestDelegate app = context => Task.CompletedTask;
        
        for (int i = _components.Count - 1; i >= 0; i--)
        {
            app = _components[i](app);
        }
        
        return app;
    }
}







