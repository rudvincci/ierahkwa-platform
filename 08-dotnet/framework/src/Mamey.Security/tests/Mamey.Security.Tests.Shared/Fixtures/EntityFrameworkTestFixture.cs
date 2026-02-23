using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Security.Tests.Shared.Fixtures;

namespace Mamey.Security.Tests.Shared.Fixtures;

/// <summary>
/// Test fixture for Entity Framework Core integration tests.
/// </summary>
public class EntityFrameworkTestFixture : SecurityTestFixture
{
    public DbContextOptions<TestDbContext> DbContextOptions { get; }

    public EntityFrameworkTestFixture(bool encryptionEnabled = true) : base(encryptionEnabled)
    {
        var builder = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        DbContextOptions = builder.Options;
    }
}

/// <summary>
/// Test DbContext for Entity Framework tests.
/// </summary>
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

/// <summary>
/// Test entity for Entity Framework tests.
/// </summary>
public class TestEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? EncryptedProperty { get; set; }
    public string? HashedProperty { get; set; }
}

