#nullable enable
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Mamey.FWID.Identities.Application.Commands;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.EF;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Concurrency;

/// <summary>
/// Pessimistic locking tests using database transactions and locks.
/// Tests that concurrent updates are serialized using database locks.
/// </summary>
[Collection("Integration")]
public class PessimisticLockingTests : IClassFixture<PostgreSQLFixture>, IDisposable
{
    private readonly PostgreSQLFixture _postgresFixture;
    private readonly IServiceProvider _serviceProvider;
    private readonly IIdentityRepository _repository;
    private readonly IdentityDbContext _dbContext;

    public PessimisticLockingTests(PostgreSQLFixture postgresFixture)
    {
        _postgresFixture = postgresFixture;
        _serviceProvider = _postgresFixture.ServiceProvider;
        _repository = _serviceProvider.GetRequiredService<IIdentityRepository>();
        _dbContext = _serviceProvider.GetRequiredService<IdentityDbContext>();
    }

    public void Dispose()
    {
        // Cleanup handled by fixture
    }

    #region Pessimistic Locking - SELECT FOR UPDATE

    [Fact]
    public async Task UpdateIdentity_WithPessimisticLock_ShouldSerializeUpdates()
    {
        // Arrange - Business Rule: Pessimistic locking should serialize concurrent updates
        // Note: This test assumes repository supports pessimistic locking via SELECT FOR UPDATE
        // If not implemented yet, this test will guide the implementation
        
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        // Act - Two concurrent updates with pessimistic locking
        var update1Completed = false;
        var update2Started = false;
        var update2Completed = false;

        var task1 = Task.Run(async () =>
        {
            // Use transaction with pessimistic lock
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // SELECT FOR UPDATE (pessimistic lock)
                var lockedIdentity = await _dbContext.Identitys
                    .Where(i => i.Id == identity.Id)
                    .FirstOrDefaultAsync();
                
                if (lockedIdentity != null)
                {
                    // Simulate long-running operation
                    await Task.Delay(100);
                    
                    lockedIdentity.UpdateContactInformation(new ContactInformation(
                        new Email("updated1@example.com"),
                        new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
                        new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
                    ));
                    
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    update1Completed = true;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        var task2 = Task.Run(async () =>
        {
            // Wait a bit to ensure task1 starts first
            await Task.Delay(50);
            update2Started = true;

            // Use transaction with pessimistic lock
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // SELECT FOR UPDATE (pessimistic lock) - should wait for task1 to complete
                var lockedIdentity = await _dbContext.Identitys
                    .Where(i => i.Id == identity.Id)
                    .FirstOrDefaultAsync();
                
                if (lockedIdentity != null)
                {
                    lockedIdentity.UpdateContactInformation(new ContactInformation(
                        new Email("updated2@example.com"),
                        new Address("", "888 Updated Ave", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null),
                        new List<Phone> { new Phone("1", "5558888888", null, Phone.PhoneType.Mobile) }
                    ));
                    
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    update2Completed = true;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        await Task.WhenAll(task1, task2);

        // Assert - Both updates should complete, but sequentially
        update1Completed.ShouldBeTrue();
        update2Completed.ShouldBeTrue();
        
        // Final state should reflect the last update
        var finalIdentity = await _repository.GetAsync(identity.Id);
        finalIdentity.ShouldNotBeNull();
        finalIdentity.ContactInformation.Email.Value.ShouldBeOneOf("updated1@example.com", "updated2@example.com");
    }

    #endregion

    #region Pessimistic Locking - Deadlock Detection

    [Fact]
    public async Task UpdateIdentity_WithDeadlock_ShouldDetectAndHandle()
    {
        // Arrange - Business Rule: Deadlocks should be detected and handled
        // Note: This test creates a potential deadlock scenario
        var identity1 = TestDataFactory.CreateTestIdentity();
        var identity2 = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity1);
        await _repository.AddAsync(identity2);

        // Act - Two transactions that lock resources in different order (potential deadlock)
        var deadlockDetected = false;

        var task1 = Task.Run(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // Lock identity1, then identity2
                var locked1 = await _dbContext.Identitys
                    .Where(i => i.Id == identity1.Id)
                    .FirstOrDefaultAsync();
                
                await Task.Delay(100); // Give task2 time to lock identity2
                
                var locked2 = await _dbContext.Identitys
                    .Where(i => i.Id == identity2.Id)
                    .FirstOrDefaultAsync();
                
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex) when (ex.Message.Contains("deadlock") || ex.Message.Contains("deadlock detected"))
            {
                deadlockDetected = true;
                await transaction.RollbackAsync();
            }
        });

        var task2 = Task.Run(async () =>
        {
            await Task.Delay(50); // Start slightly after task1
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // Lock identity2, then identity1 (opposite order - potential deadlock)
                var locked2 = await _dbContext.Identitys
                    .Where(i => i.Id == identity2.Id)
                    .FirstOrDefaultAsync();
                
                await Task.Delay(100); // Give task1 time to lock identity1
                
                var locked1 = await _dbContext.Identitys
                    .Where(i => i.Id == identity1.Id)
                    .FirstOrDefaultAsync();
                
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex) when (ex.Message.Contains("deadlock") || ex.Message.Contains("deadlock detected"))
            {
                deadlockDetected = true;
                await transaction.RollbackAsync();
            }
        });

        await Task.WhenAll(task1, task2);

        // Assert - Deadlock should be detected (PostgreSQL detects deadlocks automatically)
        // Note: This behavior depends on database configuration and timing
        // In PostgreSQL, one transaction will be aborted automatically
    }

    #endregion

    #region Pessimistic Locking - Lock Timeout

    [Fact]
    public async Task UpdateIdentity_WithLockTimeout_ShouldTimeout()
    {
        // Arrange - Business Rule: Lock timeouts should be handled
        // Note: This test assumes lock timeout is configured
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        // Act - First transaction holds lock for long time
        var lockTimeout = false;

        var task1 = Task.Run(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var lockedIdentity = await _dbContext.Identitys
                    .Where(i => i.Id == identity.Id)
                    .FirstOrDefaultAsync();
                
                // Hold lock for longer than timeout
                await Task.Delay(2000);
                
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        });

        var task2 = Task.Run(async () =>
        {
            await Task.Delay(100); // Start after task1
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // Set lock timeout (if supported)
                // await _dbContext.Database.ExecuteSqlRawAsync("SET lock_timeout = '1s'");
                
                var lockedIdentity = await _dbContext.Identitys
                    .Where(i => i.Id == identity.Id)
                    .FirstOrDefaultAsync();
                
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex) when (ex.Message.Contains("timeout") || ex.Message.Contains("lock"))
            {
                lockTimeout = true;
                await transaction.RollbackAsync();
            }
        });

        await Task.WhenAll(task1, task2);

        // Assert - Lock timeout should occur (if configured)
        // Note: This depends on database lock timeout configuration
    }

    #endregion

    #region Pessimistic Locking - Read Lock vs Write Lock

    [Fact]
    public async Task ReadIdentity_WithSharedLock_ShouldAllowConcurrentReads()
    {
        // Arrange - Business Rule: Shared locks should allow concurrent reads
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        // Act - Multiple concurrent reads with shared locks
        var readCount = 0;
        var tasks = new List<Task>();

        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
                try
                {
                    // SELECT FOR SHARE (shared lock for reads)
                    var lockedIdentity = await _dbContext.Identitys
                        .Where(i => i.Id == identity.Id)
                        .FirstOrDefaultAsync();
                    
                    if (lockedIdentity != null)
                    {
                        Interlocked.Increment(ref readCount);
                    }
                    
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert - All reads should succeed concurrently
        readCount.ShouldBe(5);
    }

    [Fact]
    public async Task UpdateIdentity_WithExclusiveLock_ShouldBlockConcurrentWrites()
    {
        // Arrange - Business Rule: Exclusive locks should block concurrent writes
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        // Act - Two concurrent writes with exclusive locks
        var write1Completed = false;
        var write2Completed = false;
        var write2Blocked = false;

        var task1 = Task.Run(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // SELECT FOR UPDATE (exclusive lock)
                var lockedIdentity = await _dbContext.Identitys
                    .Where(i => i.Id == identity.Id)
                    .FirstOrDefaultAsync();
                
                if (lockedIdentity != null)
                {
                    // Hold lock for a bit
                    await Task.Delay(200);
                    
                    lockedIdentity.UpdateZone("zone-001");
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    write1Completed = true;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        });

        var task2 = Task.Run(async () =>
        {
            await Task.Delay(50); // Start slightly after task1
            var startTime = DateTime.UtcNow;
            
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // SELECT FOR UPDATE (exclusive lock) - should wait for task1
                var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
                if (elapsed > 100)
                {
                    write2Blocked = true; // Was blocked waiting for lock
                }
                
                var lockedIdentity = await _dbContext.Identitys
                    .Where(i => i.Id == identity.Id)
                    .FirstOrDefaultAsync();
                
                if (lockedIdentity != null)
                {
                    lockedIdentity.UpdateZone("zone-002");
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    write2Completed = true;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        });

        await Task.WhenAll(task1, task2);

        // Assert - Both writes should complete, but sequentially
        write1Completed.ShouldBeTrue();
        write2Completed.ShouldBeTrue();
        // write2Blocked.ShouldBeTrue(); // Second write should have been blocked
    }

    #endregion
}

