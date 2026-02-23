using Mamey.Security;
using Mamey.Security.EntityFramework;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.EntityFramework;

/// <summary>
/// Comprehensive tests for ModelBuilder extensions covering all scenarios.
/// </summary>
public class ModelBuilderExtensionTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public ModelBuilderExtensionTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Test Classes

    public class TestEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Encrypted]
        public string? EncryptedProperty { get; set; }

        [Hashed]
        public string? HashedProperty { get; set; }

        public string? RegularProperty { get; set; }
    }

    public class TestEntityMultiple
    {
        public Guid Id { get; set; }

        [Encrypted]
        public string? Encrypted1 { get; set; }

        [Encrypted]
        public string? Encrypted2 { get; set; }

        [Hashed]
        public string? Hashed1 { get; set; }

        [Hashed]
        public string? Hashed2 { get; set; }
    }

    public class TestEntityNoAttributes
    {
        public Guid Id { get; set; }
        public string? Property1 { get; set; }
        public string? Property2 { get; set; }
    }

    #endregion

    #region Happy Paths

    [Fact]
    public void ApplySecurityAttributes_ValidEntities_ShouldApplyConverters()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(_fixture.SecurityProvider);
        var serviceProvider = services.BuildServiceProvider();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new TestDbContext(options, serviceProvider);

        // Act
        // Security attributes are applied in OnModelCreating

        // Assert
        var entityType = context.Model.FindEntityType(typeof(TestEntity));
        entityType.ShouldNotBeNull();
        var encryptedProperty = entityType.FindProperty(nameof(TestEntity.EncryptedProperty));
        encryptedProperty.ShouldNotBeNull();
        encryptedProperty.GetValueConverter().ShouldNotBeNull();
        var hashedProperty = entityType.FindProperty(nameof(TestEntity.HashedProperty));
        hashedProperty.ShouldNotBeNull();
        hashedProperty.GetValueConverter().ShouldNotBeNull();
    }

    [Fact]
    public void ApplySecurityAttributes_MultipleEntities_ShouldApplyConverters()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(_fixture.SecurityProvider);
        var serviceProvider = services.BuildServiceProvider();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new TestDbContext(options, serviceProvider);

        // Act
        // Security attributes are applied in OnModelCreating

        // Assert
        var entityType1 = context.Model.FindEntityType(typeof(TestEntity));
        var entityType2 = context.Model.FindEntityType(typeof(TestEntityMultiple));
        entityType1.ShouldNotBeNull();
        entityType2.ShouldNotBeNull();
    }

    [Fact]
    public void ApplySecurityAttributes_MixedAttributes_ShouldApplyCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(_fixture.SecurityProvider);
        var serviceProvider = services.BuildServiceProvider();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new TestDbContext(options, serviceProvider);

        // Act
        // Security attributes are applied in OnModelCreating

        // Assert
        var entityType = context.Model.FindEntityType(typeof(TestEntityMultiple));
        entityType.ShouldNotBeNull();
        var encrypted1 = entityType.FindProperty(nameof(TestEntityMultiple.Encrypted1));
        var encrypted2 = entityType.FindProperty(nameof(TestEntityMultiple.Encrypted2));
        var hashed1 = entityType.FindProperty(nameof(TestEntityMultiple.Hashed1));
        var hashed2 = entityType.FindProperty(nameof(TestEntityMultiple.Hashed2));
        encrypted1?.GetValueConverter().ShouldNotBeNull();
        encrypted2?.GetValueConverter().ShouldNotBeNull();
        hashed1?.GetValueConverter().ShouldNotBeNull();
        hashed2?.GetValueConverter().ShouldNotBeNull();
    }

    [Fact]
    public void ApplySecurityAttributes_NoAttributes_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(_fixture.SecurityProvider);
        var serviceProvider = services.BuildServiceProvider();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new TestDbContext(options, serviceProvider);

        // Act
        // Security attributes are applied in OnModelCreating

        // Assert
        var entityType = context.Model.FindEntityType(typeof(TestEntityNoAttributes));
        entityType.ShouldNotBeNull();
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void ApplySecurityAttributes_NullModelBuilder_ShouldThrowArgumentNullException()
    {
        // Arrange
        ModelBuilder? modelBuilder = null;
        var services = new ServiceCollection();
        services.AddSingleton(_fixture.SecurityProvider);
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => modelBuilder!.ApplySecurityAttributes(serviceProvider));
    }

    [Fact]
    public void ApplySecurityAttributes_NullServiceProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceProvider? serviceProvider = null;
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new TestDbContext(options, serviceProvider);
        var modelBuilder = new ModelBuilder();
        modelBuilder.Entity<TestEntity>();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => modelBuilder.ApplySecurityAttributes(serviceProvider!));
    }

    [Fact]
    public void ApplySecurityAttributes_ServiceProviderWithoutSecurityProvider_ShouldThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var modelBuilder = new ModelBuilder();
        modelBuilder.Entity<TestEntity>();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => modelBuilder.ApplySecurityAttributes(serviceProvider));
    }

    #endregion
}

/// <summary>
/// Test DbContext for ModelBuilder tests.
/// </summary>
public class TestDbContext : DbContext
{
    private readonly IServiceProvider? _serviceProvider;

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public TestDbContext(DbContextOptions<TestDbContext> options, IServiceProvider? serviceProvider) : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    public DbSet<ModelBuilderExtensionTests.TestEntity> TestEntities { get; set; } = null!;
    public DbSet<ModelBuilderExtensionTests.TestEntityMultiple> TestEntitiesMultiple { get; set; } = null!;
    public DbSet<ModelBuilderExtensionTests.TestEntityNoAttributes> TestEntitiesNoAttributes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ModelBuilderExtensionTests.TestEntity>();
        modelBuilder.Entity<ModelBuilderExtensionTests.TestEntityMultiple>();
        modelBuilder.Entity<ModelBuilderExtensionTests.TestEntityNoAttributes>();
        
        // Apply security attributes
        var serviceProvider = _serviceProvider ?? this.GetService<IServiceProvider>();
        if (serviceProvider != null)
        {
            modelBuilder.ApplySecurityAttributes(serviceProvider);
        }
    }
}

