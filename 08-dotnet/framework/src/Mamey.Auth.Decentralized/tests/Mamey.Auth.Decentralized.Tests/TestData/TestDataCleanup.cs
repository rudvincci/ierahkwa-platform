using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mamey.Auth.Decentralized.Caching;
using Mamey.Auth.Decentralized.Persistence.Read;
using Mamey.Auth.Decentralized.Persistence.Write;

namespace Mamey.Auth.Decentralized.Tests.TestData;

/// <summary>
/// Provides utilities for cleaning up test data after test execution.
/// </summary>
public static class TestDataCleanup
{
    /// <summary>
    /// Cleans up all test data from the test environment.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the cleanup operation.</returns>
    public static async Task CleanupAllTestDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
        
        try
        {
            logger.LogInformation("Starting test data cleanup");
            
            // Clean up cache
            await CleanupCacheAsync(scope.ServiceProvider);
            
            // Clean up read database
            await CleanupReadDatabaseAsync(scope.ServiceProvider);
            
            // Clean up write database
            await CleanupWriteDatabaseAsync(scope.ServiceProvider);
            
            logger.LogInformation("Test data cleanup completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during test data cleanup");
            throw;
        }
    }

    /// <summary>
    /// Cleans up cache data.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the cleanup operation.</returns>
    public static async Task CleanupCacheAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var cache = serviceProvider.GetService<IDidDocumentCache>();
            if (cache != null)
            {
                await cache.ClearAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
            logger.LogWarning(ex, "Error cleaning up cache");
        }
    }

    /// <summary>
    /// Cleans up read database data.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the cleanup operation.</returns>
    public static async Task CleanupReadDatabaseAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var didDocumentRepo = serviceProvider.GetService<IDidDocumentReadRepository>();
            var verificationMethodRepo = serviceProvider.GetService<IVerificationMethodReadRepository>();
            var serviceEndpointRepo = serviceProvider.GetService<IServiceEndpointReadRepository>();

            // Clean up DID documents
            if (didDocumentRepo != null)
            {
                var didDocuments = await didDocumentRepo.GetAllAsync();
                foreach (var doc in didDocuments)
                {
                    await CleanupDidDocumentAsync(doc, serviceProvider);
                }
            }

            // Clean up verification methods
            if (verificationMethodRepo != null)
            {
                var verificationMethods = await verificationMethodRepo.GetAllAsync();
                foreach (var vm in verificationMethods)
                {
                    await CleanupVerificationMethodAsync(vm, serviceProvider);
                }
            }

            // Clean up service endpoints
            if (serviceEndpointRepo != null)
            {
                var serviceEndpoints = await serviceEndpointRepo.GetAllAsync();
                foreach (var se in serviceEndpoints)
                {
                    await CleanupServiceEndpointAsync(se, serviceProvider);
                }
            }
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
            logger.LogWarning(ex, "Error cleaning up read database");
        }
    }

    /// <summary>
    /// Cleans up write database data.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the cleanup operation.</returns>
    public static async Task CleanupWriteDatabaseAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var unitOfWork = serviceProvider.GetService<IDidUnitOfWork>();
            if (unitOfWork != null)
            {
                // Clean up DID documents
                var didDocuments = await unitOfWork.DidDocuments.GetAllAsync();
                foreach (var doc in didDocuments)
                {
                    await unitOfWork.DidDocuments.DeleteAsync(doc.Id);
                }

                // Clean up verification methods
                var verificationMethods = await unitOfWork.VerificationMethods.GetAllAsync();
                foreach (var vm in verificationMethods)
                {
                    await unitOfWork.VerificationMethods.DeleteAsync(vm.Id);
                }

                // Clean up service endpoints
                var serviceEndpoints = await unitOfWork.ServiceEndpoints.GetAllAsync();
                foreach (var se in serviceEndpoints)
                {
                    await unitOfWork.ServiceEndpoints.DeleteAsync(se.Id);
                }

                await unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
            logger.LogWarning(ex, "Error cleaning up write database");
        }
    }

    /// <summary>
    /// Cleans up a specific DID document.
    /// </summary>
    /// <param name="didDocument">The DID document to clean up.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the cleanup operation.</returns>
    public static async Task CleanupDidDocumentAsync(Mamey.Auth.Decentralized.Core.DidDocument didDocument, IServiceProvider serviceProvider)
    {
        try
        {
            var didDocumentRepo = serviceProvider.GetService<IDidDocumentReadRepository>();
            if (didDocumentRepo != null)
            {
                await didDocumentRepo.DeleteAsync(didDocument.Id);
            }
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
            logger.LogWarning(ex, "Error cleaning up DID document {DidId}", didDocument.Id);
        }
    }

    /// <summary>
    /// Cleans up a specific verification method.
    /// </summary>
    /// <param name="verificationMethod">The verification method to clean up.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the cleanup operation.</returns>
    public static async Task CleanupVerificationMethodAsync(Mamey.Auth.Decentralized.Core.VerificationMethod verificationMethod, IServiceProvider serviceProvider)
    {
        try
        {
            var verificationMethodRepo = serviceProvider.GetService<IVerificationMethodReadRepository>();
            if (verificationMethodRepo != null)
            {
                await verificationMethodRepo.DeleteAsync(verificationMethod.Id);
            }
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
            logger.LogWarning(ex, "Error cleaning up verification method {VmId}", verificationMethod.Id);
        }
    }

    /// <summary>
    /// Cleans up a specific service endpoint.
    /// </summary>
    /// <param name="serviceEndpoint">The service endpoint to clean up.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the cleanup operation.</returns>
    public static async Task CleanupServiceEndpointAsync(Mamey.Auth.Decentralized.Core.ServiceEndpoint serviceEndpoint, IServiceProvider serviceProvider)
    {
        try
        {
            var serviceEndpointRepo = serviceProvider.GetService<IServiceEndpointReadRepository>();
            if (serviceEndpointRepo != null)
            {
                await serviceEndpointRepo.DeleteAsync(serviceEndpoint.Id);
            }
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
            logger.LogWarning(ex, "Error cleaning up service endpoint {SeId}", serviceEndpoint.Id);
        }
    }

    /// <summary>
    /// Cleans up test data by pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match for cleanup.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the cleanup operation.</returns>
    public static async Task CleanupByPatternAsync(string pattern, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
        
        try
        {
            logger.LogInformation("Cleaning up test data matching pattern: {Pattern}", pattern);
            
            // Clean up DID documents matching pattern
            var didDocumentRepo = scope.ServiceProvider.GetService<IDidDocumentReadRepository>();
            if (didDocumentRepo != null)
            {
                var didDocuments = await didDocumentRepo.GetAllAsync();
                var matchingDocs = didDocuments.Where(d => d.Id.Contains(pattern));
                foreach (var doc in matchingDocs)
                {
                    await CleanupDidDocumentAsync(doc, scope.ServiceProvider);
                }
            }

            // Clean up verification methods matching pattern
            var verificationMethodRepo = scope.ServiceProvider.GetService<IVerificationMethodReadRepository>();
            if (verificationMethodRepo != null)
            {
                var verificationMethods = await verificationMethodRepo.GetAllAsync();
                var matchingVms = verificationMethods.Where(vm => vm.Id.Contains(pattern));
                foreach (var vm in matchingVms)
                {
                    await CleanupVerificationMethodAsync(vm, scope.ServiceProvider);
                }
            }

            // Clean up service endpoints matching pattern
            var serviceEndpointRepo = scope.ServiceProvider.GetService<IServiceEndpointReadRepository>();
            if (serviceEndpointRepo != null)
            {
                var serviceEndpoints = await serviceEndpointRepo.GetAllAsync();
                var matchingSes = serviceEndpoints.Where(se => se.Id.Contains(pattern));
                foreach (var se in matchingSes)
                {
                    await CleanupServiceEndpointAsync(se, scope.ServiceProvider);
                }
            }
            
            logger.LogInformation("Cleanup by pattern completed for pattern: {Pattern}", pattern);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cleaning up test data by pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Cleans up test data older than a specified time.
    /// </summary>
    /// <param name="olderThan">The cutoff time for cleanup.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the cleanup operation.</returns>
    public static async Task CleanupOlderThanAsync(DateTime olderThan, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
        
        try
        {
            logger.LogInformation("Cleaning up test data older than: {OlderThan}", olderThan);
            
            // Note: This would require adding timestamp fields to the entities
            // For now, we'll clean up all test data
            await CleanupAllTestDataAsync(serviceProvider);
            
            logger.LogInformation("Cleanup older than completed for: {OlderThan}", olderThan);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cleaning up test data older than: {OlderThan}", olderThan);
            throw;
        }
    }

    /// <summary>
    /// Validates that test data cleanup was successful.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the validation operation.</returns>
    public static async Task<bool> ValidateCleanupAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
        
        try
        {
            logger.LogInformation("Validating test data cleanup");
            
            var didDocumentRepo = scope.ServiceProvider.GetService<IDidDocumentReadRepository>();
            var verificationMethodRepo = scope.ServiceProvider.GetService<IVerificationMethodReadRepository>();
            var serviceEndpointRepo = scope.ServiceProvider.GetService<IServiceEndpointReadRepository>();

            var didDocumentCount = 0;
            var verificationMethodCount = 0;
            var serviceEndpointCount = 0;

            if (didDocumentRepo != null)
            {
                var didDocuments = await didDocumentRepo.GetAllAsync();
                didDocumentCount = didDocuments.Count();
            }

            if (verificationMethodRepo != null)
            {
                var verificationMethods = await verificationMethodRepo.GetAllAsync();
                verificationMethodCount = verificationMethods.Count();
            }

            if (serviceEndpointRepo != null)
            {
                var serviceEndpoints = await serviceEndpointRepo.GetAllAsync();
                serviceEndpointCount = serviceEndpoints.Count();
            }

            var isClean = didDocumentCount == 0 && verificationMethodCount == 0 && serviceEndpointCount == 0;
            
            logger.LogInformation("Cleanup validation result: {IsClean}. DID Documents: {DidCount}, Verification Methods: {VmCount}, Service Endpoints: {SeCount}", 
                isClean, didDocumentCount, verificationMethodCount, serviceEndpointCount);
            
            return isClean;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating test data cleanup");
            return false;
        }
    }

    /// <summary>
    /// Gets statistics about test data in the system.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the statistics operation.</returns>
    public static async Task<TestDataStatistics> GetTestDataStatisticsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestDataCleanup>>();
        
        try
        {
            logger.LogInformation("Getting test data statistics");
            
            var statistics = new TestDataStatistics();
            
            var didDocumentRepo = scope.ServiceProvider.GetService<IDidDocumentReadRepository>();
            var verificationMethodRepo = scope.ServiceProvider.GetService<IVerificationMethodReadRepository>();
            var serviceEndpointRepo = scope.ServiceProvider.GetService<IServiceEndpointReadRepository>();

            if (didDocumentRepo != null)
            {
                var didDocuments = await didDocumentRepo.GetAllAsync();
                statistics.DidDocumentCount = didDocuments.Count();
            }

            if (verificationMethodRepo != null)
            {
                var verificationMethods = await verificationMethodRepo.GetAllAsync();
                statistics.VerificationMethodCount = verificationMethods.Count();
            }

            if (serviceEndpointRepo != null)
            {
                var serviceEndpoints = await serviceEndpointRepo.GetAllAsync();
                statistics.ServiceEndpointCount = serviceEndpoints.Count();
            }

            logger.LogInformation("Test data statistics: {Statistics}", statistics);
            
            return statistics;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting test data statistics");
            return new TestDataStatistics();
        }
    }
}

/// <summary>
/// Represents statistics about test data in the system.
/// </summary>
public class TestDataStatistics
{
    /// <summary>
    /// Gets or sets the number of DID documents.
    /// </summary>
    public int DidDocumentCount { get; set; }

    /// <summary>
    /// Gets or sets the number of verification methods.
    /// </summary>
    public int VerificationMethodCount { get; set; }

    /// <summary>
    /// Gets or sets the number of service endpoints.
    /// </summary>
    public int ServiceEndpointCount { get; set; }

    /// <summary>
    /// Gets the total number of test data items.
    /// </summary>
    public int TotalCount => DidDocumentCount + VerificationMethodCount + ServiceEndpointCount;

    /// <summary>
    /// Returns a string representation of the statistics.
    /// </summary>
    /// <returns>A string representation of the statistics.</returns>
    public override string ToString()
    {
        return $"DID Documents: {DidDocumentCount}, Verification Methods: {VerificationMethodCount}, Service Endpoints: {ServiceEndpointCount}, Total: {TotalCount}";
    }
}
