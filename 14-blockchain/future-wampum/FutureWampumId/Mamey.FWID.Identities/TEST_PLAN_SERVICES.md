# Test Plan - Application & Infrastructure Services

## Overview

This document outlines the comprehensive test plan for Application layer services and related Infrastructure services in the `Mamey.FWID.Identities` microservice.

## Application Layer Services

### IBiometricStorageService (Interface)

**Location**: `Mamey.FWID.Identities.Application/Services/IBiometricStorageService.cs`

**Implementation**: `BiometricStorageService` in Infrastructure layer

**Test File**: `tests/Mamey.FWID.Identities.Tests.Unit/Infrastructure/Services/BiometricStorageServiceTests.cs`

#### Test Scenarios

##### 1. UploadBiometricAsync

**Happy Paths:**
- ✅ Upload biometric data successfully
- ✅ Upload with custom object name
- ✅ Upload with different biometric types (Facial, Fingerprint, Voice, Iris)
- ✅ Upload with metadata (identityId, biometricType, uploadedAt)

**Sad Paths:**
- ❌ Upload with null identityId → `ArgumentNullException`
- ❌ Upload with null or empty data → `ArgumentException`
- ❌ Upload with invalid biometric type → Validation error
- ❌ Upload when MinIO is unavailable → Exception handling
- ❌ Upload when bucket doesn't exist → Exception handling
- ❌ Upload with network timeout → Retry logic

**Test Cases:**
```csharp
- UploadBiometricAsync_WithValidData_ReturnsObjectName()
- UploadBiometricAsync_WithCustomObjectName_ReturnsCustomName()
- UploadBiometricAsync_WithNullIdentityId_ThrowsArgumentNullException()
- UploadBiometricAsync_WithNullData_ThrowsArgumentException()
- UploadBiometricAsync_WithEmptyData_ThrowsArgumentException()
- UploadBiometricAsync_WhenMinIOUnavailable_HandlesException()
- UploadBiometricAsync_WithNetworkTimeout_RetriesOperation()
```

##### 2. DownloadBiometricAsync

**Happy Paths:**
- ✅ Download biometric data successfully
- ✅ Download with custom object name
- ✅ Download with default object name (generated)
- ✅ Download different biometric types

**Sad Paths:**
- ❌ Download with null identityId → `ArgumentNullException`
- ❌ Download non-existent object → Exception handling
- ❌ Download when MinIO is unavailable → Exception handling
- ❌ Download with network timeout → Retry logic

**Test Cases:**
```csharp
- DownloadBiometricAsync_WithValidObjectName_ReturnsData()
- DownloadBiometricAsync_WithDefaultObjectName_ReturnsData()
- DownloadBiometricAsync_WithNullIdentityId_ThrowsArgumentNullException()
- DownloadBiometricAsync_WithNonExistentObject_ThrowsException()
- DownloadBiometricAsync_WhenMinIOUnavailable_HandlesException()
```

##### 3. DeleteBiometricAsync

**Happy Paths:**
- ✅ Delete biometric data successfully
- ✅ Delete with custom object name
- ✅ Delete with default object name

**Sad Paths:**
- ❌ Delete with null identityId → `ArgumentNullException`
- ❌ Delete non-existent object → Exception handling (should not throw)
- ❌ Delete when MinIO is unavailable → Exception handling

**Test Cases:**
```csharp
- DeleteBiometricAsync_WithValidObjectName_DeletesSuccessfully()
- DeleteBiometricAsync_WithDefaultObjectName_DeletesSuccessfully()
- DeleteBiometricAsync_WithNullIdentityId_ThrowsArgumentNullException()
- DeleteBiometricAsync_WithNonExistentObject_HandlesGracefully()
```

##### 4. GetBiometricPresignedUrlAsync

**Happy Paths:**
- ✅ Generate presigned URL successfully
- ✅ Generate with custom expiry time
- ✅ Generate with default expiry (3600 seconds)
- ✅ Generate with custom object name

**Sad Paths:**
- ❌ Generate with null identityId → `ArgumentNullException`
- ❌ Generate with invalid expiry (negative) → Validation error
- ❌ Generate when MinIO is unavailable → Exception handling

**Test Cases:**
```csharp
- GetBiometricPresignedUrlAsync_WithValidParams_ReturnsPresignedUrl()
- GetBiometricPresignedUrlAsync_WithCustomExpiry_ReturnsUrlWithExpiry()
- GetBiometricPresignedUrlAsync_WithDefaultExpiry_ReturnsUrlWith3600Seconds()
- GetBiometricPresignedUrlAsync_WithNullIdentityId_ThrowsArgumentNullException()
- GetBiometricPresignedUrlAsync_WithNegativeExpiry_HandlesValidation()
```

##### 5. GetBiometricMetadataAsync

**Happy Paths:**
- ✅ Get metadata successfully
- ✅ Get metadata with custom object name
- ✅ Get metadata with default object name
- ✅ Metadata includes size, content type, etc.

**Sad Paths:**
- ❌ Get metadata with null identityId → `ArgumentNullException`
- ❌ Get metadata for non-existent object → Exception handling
- ❌ Get metadata when MinIO is unavailable → Exception handling

**Test Cases:**
```csharp
- GetBiometricMetadataAsync_WithValidObjectName_ReturnsMetadata()
- GetBiometricMetadataAsync_WithDefaultObjectName_ReturnsMetadata()
- GetBiometricMetadataAsync_WithNullIdentityId_ThrowsArgumentNullException()
- GetBiometricMetadataAsync_WithNonExistentObject_ThrowsException()
```

## Infrastructure Services

### EventMapper

**Location**: `Mamey.FWID.Identities.Infrastructure/Services/EventMapper.cs`

**Test File**: `tests/Mamey.FWID.Identities.Tests.Unit/Infrastructure/Services/EventMapperTests.cs`

#### Test Scenarios

##### 1. Map (Single Event)

**Happy Paths:**
- ✅ Map `IdentityModified` → `IdentityUpdated`
- ✅ Map `IdentityRemoved` → `IdentityDeleted`
- ✅ Map `IdentityCreated` → `null` (handled by handler)

**Sad Paths:**
- ❌ Map unknown domain event → `null`
- ❌ Map null event → `null` (should handle gracefully)

**Test Cases:**
```csharp
- Map_WithIdentityModified_ReturnsIdentityUpdated()
- Map_WithIdentityRemoved_ReturnsIdentityDeleted()
- Map_WithIdentityCreated_ReturnsNull()
- Map_WithUnknownEvent_ReturnsNull()
- Map_WithNullEvent_ReturnsNull()
```

##### 2. MapAll (Multiple Events)

**Happy Paths:**
- ✅ Map multiple events successfully
- ✅ Map empty collection → Empty result
- ✅ Map mixed events (some mapped, some null)

**Sad Paths:**
- ❌ Map null collection → Exception handling

**Test Cases:**
```csharp
- MapAll_WithMultipleEvents_ReturnsMappedEvents()
- MapAll_WithEmptyCollection_ReturnsEmptyCollection()
- MapAll_WithMixedEvents_ReturnsMixedResults()
- MapAll_WithNullCollection_HandlesGracefully()
```

### BucketInitializationService

**Location**: `Mamey.FWID.Identities.Infrastructure/MinIO/Services/BucketInitializationService.cs`

**Test File**: `tests/Mamey.FWID.Identities.Tests.Unit/Infrastructure/Services/BucketInitializationServiceTests.cs`

#### Test Scenarios

##### 1. ExecuteAsync (Background Service)

**Happy Paths:**
- ✅ Bucket exists → No action needed
- ✅ Bucket doesn't exist → Creates bucket
- ✅ Service starts successfully
- ✅ Service stops gracefully

**Sad Paths:**
- ❌ MinIO unavailable → Retry logic
- ❌ Bucket creation fails → Error handling
- ❌ Service cancellation → Graceful shutdown

**Test Cases:**
```csharp
- ExecuteAsync_WhenBucketExists_NoActionTaken()
- ExecuteAsync_WhenBucketNotExists_CreatesBucket()
- ExecuteAsync_WhenMinIOUnavailable_RetriesOperation()
- ExecuteAsync_WhenCancellationRequested_StopsGracefully()
- ExecuteAsync_WhenBucketCreationFails_HandlesError()
```

**Note**: This is a hosted service, so tests should use `TestHost` or mock `IHostedService` lifecycle.

### IdentityMongoSyncService

**Location**: `Mamey.FWID.Identities.Infrastructure/Mongo/Services/IdentityMongoSyncService.cs`

**Test File**: `tests/Mamey.FWID.Identities.Tests.Unit/Infrastructure/Services/IdentityMongoSyncServiceTests.cs`

#### Test Scenarios

##### 1. ExecuteAsync (Background Service)

**Happy Paths:**
- ✅ Sync enabled → Syncs entities
- ✅ Sync disabled → No action
- ✅ Initial delay → Waits before first sync
- ✅ Periodic sync → Syncs at intervals
- ✅ New entities → Adds to MongoDB
- ✅ Existing entities → Updates in MongoDB
- ✅ Empty PostgreSQL → Skips sync

**Sad Paths:**
- ❌ MongoDB unavailable → Retry logic
- ❌ PostgreSQL unavailable → Error handling
- ❌ Sync fails for one entity → Continues with others
- ❌ Service cancellation → Graceful shutdown

**Test Cases:**
```csharp
- ExecuteAsync_WhenSyncEnabled_SyncsEntities()
- ExecuteAsync_WhenSyncDisabled_NoAction()
- ExecuteAsync_WithInitialDelay_WaitsBeforeSync()
- ExecuteAsync_WithNewEntities_AddsToMongoDB()
- ExecuteAsync_WithExistingEntities_UpdatesMongoDB()
- ExecuteAsync_WithEmptyPostgreSQL_SkipsSync()
- ExecuteAsync_WhenMongoDBUnavailable_RetriesOperation()
- ExecuteAsync_WhenSyncFailsForOneEntity_ContinuesWithOthers()
- ExecuteAsync_WhenCancellationRequested_StopsGracefully()
```

##### 2. SyncIdentitysToMongo (Private Method - Test via Public)

**Test Scenarios:**
- ✅ Syncs all entities from PostgreSQL
- ✅ Handles entity-by-entity errors
- ✅ Logs sync statistics

**Test Cases:**
```csharp
- SyncIdentitysToMongo_WithMultipleEntities_SyncsAll()
- SyncIdentitysToMongo_WithEntityError_ContinuesWithOthers()
- SyncIdentitysToMongo_LogsSyncStatistics()
```

### IdentityRedisSyncService

**Location**: `Mamey.FWID.Identities.Infrastructure/Redis/Services/IdentityRedisSyncService.cs`

**Test File**: `tests/Mamey.FWID.Identities.Tests.Unit/Infrastructure/Services/IdentityRedisSyncServiceTests.cs`

#### Test Scenarios

##### 1. ExecuteAsync (Background Service)

**Happy Paths:**
- ✅ Sync enabled → Syncs entities
- ✅ Sync disabled → No action
- ✅ Initial delay → Waits before first sync
- ✅ Periodic sync → Syncs at intervals
- ✅ New entities → Adds to Redis
- ✅ Existing entities → Updates in Redis
- ✅ Empty PostgreSQL → Skips sync

**Sad Paths:**
- ❌ Redis unavailable → Retry logic
- ❌ PostgreSQL unavailable → Error handling
- ❌ Sync fails for one entity → Continues with others
- ❌ Service cancellation → Graceful shutdown

**Test Cases:**
```csharp
- ExecuteAsync_WhenSyncEnabled_SyncsEntities()
- ExecuteAsync_WhenSyncDisabled_NoAction()
- ExecuteAsync_WithInitialDelay_WaitsBeforeSync()
- ExecuteAsync_WithNewEntities_AddsToRedis()
- ExecuteAsync_WithExistingEntities_UpdatesRedis()
- ExecuteAsync_WithEmptyPostgreSQL_SkipsSync()
- ExecuteAsync_WhenRedisUnavailable_RetriesOperation()
- ExecuteAsync_WhenSyncFailsForOneEntity_ContinuesWithOthers()
- ExecuteAsync_WhenCancellationRequested_StopsGracefully()
```

##### 2. SyncIdentitysToRedis (Private Method - Test via Public)

**Test Scenarios:**
- ✅ Syncs all entities from PostgreSQL
- ✅ Handles entity-by-entity errors
- ✅ Logs sync statistics

**Test Cases:**
```csharp
- SyncIdentitysToRedis_WithMultipleEntities_SyncsAll()
- SyncIdentitysToRedis_WithEntityError_ContinuesWithOthers()
- SyncIdentitysToRedis_LogsSyncStatistics()
```

## Required Test Fixtures

### 1. MinIOFixture

**Location**: `tests/Mamey.FWID.Identities.Tests.Shared/Fixtures/MinIOFixture.cs`

**Purpose**: Setup and teardown MinIO test environment

**Features:**
- Start/stop MinIO test container
- Create test buckets
- Cleanup test data
- Provide connection string

**Usage:**
```csharp
public class MinIOFixture : IDisposable
{
    public string ConnectionString { get; private set; }
    public string AccessKey { get; private set; }
    public string SecretKey { get; private set; }
    
    public MinIOFixture()
    {
        // Start MinIO test container
        // Initialize connection string
    }
    
    public void Dispose()
    {
        // Stop container
        // Cleanup
    }
}
```

### 2. MongoDBFixture

**Location**: `tests/Mamey.FWID.Identities.Tests.Shared/Fixtures/MongoDBFixture.cs`

**Purpose**: Setup and teardown MongoDB test environment

**Features:**
- Start/stop MongoDB test container
- Create test database
- Cleanup test data
- Provide connection string

**Usage:**
```csharp
public class MongoDBFixture : IDisposable
{
    public string ConnectionString { get; private set; }
    public string DatabaseName { get; private set; }
    
    public MongoDBFixture()
    {
        // Start MongoDB test container
        // Initialize connection string
    }
    
    public void Dispose()
    {
        // Stop container
        // Cleanup database
    }
}
```

### 3. RedisFixture

**Location**: `tests/Mamey.FWID.Identities.Tests.Shared/Fixtures/RedisFixture.cs`

**Purpose**: Setup and teardown Redis test environment

**Features:**
- Start/stop Redis test container
- Flush database
- Provide connection string

**Usage:**
```csharp
public class RedisFixture : IDisposable
{
    public string ConnectionString { get; private set; }
    public int Database { get; private set; }
    
    public RedisFixture()
    {
        // Start Redis test container
        // Initialize connection string
    }
    
    public void Dispose()
    {
        // Stop container
        // Flush database
    }
}
```

### 4. TestDataFactory

**Location**: `tests/Mamey.FWID.Identities.Tests.Shared/Factories/TestDataFactory.cs`

**Purpose**: Create test data for services

**Methods:**
```csharp
public static class TestDataFactory
{
    public static IdentityId CreateIdentityId()
    public static BiometricType CreateBiometricType()
    public static byte[] CreateBiometricData(int size = 1024)
    public static string CreateObjectName(IdentityId identityId, BiometricType type)
    public static Identity CreateTestIdentity()
    public static List<Identity> CreateTestIdentities(int count)
}
```

## Test Organization

### Unit Tests Structure

```
tests/Mamey.FWID.Identities.Tests.Unit/
├── Infrastructure/
│   └── Services/
│       ├── BiometricStorageServiceTests.cs
│       ├── EventMapperTests.cs
│       ├── BucketInitializationServiceTests.cs
│       ├── IdentityMongoSyncServiceTests.cs
│       └── IdentityRedisSyncServiceTests.cs
```

### Integration Tests Structure

```
tests/Mamey.FWID.Identities.Tests.Integration/
├── Infrastructure/
│   └── Services/
│       ├── BiometricStorageServiceIntegrationTests.cs
│       ├── IdentityMongoSyncServiceIntegrationTests.cs
│       └── IdentityRedisSyncServiceIntegrationTests.cs
```

## Test Coverage Goals

- **Unit Tests**: 90%+ coverage for service logic
- **Integration Tests**: 80%+ coverage for external dependencies
- **Error Handling**: 100% coverage of exception paths
- **Retry Logic**: 100% coverage of retry scenarios

## Test Execution Strategy

1. **Unit Tests**: Fast, isolated, mock external dependencies
2. **Integration Tests**: Use test containers (Testcontainers.NET)
3. **Background Services**: Use `TestHost` or mock `IHostedService`
4. **Error Scenarios**: Test all exception paths
5. **Retry Logic**: Test retry policies and circuit breakers

## Notes

- **BiometricStorageService** is in Infrastructure layer but implements Application interface
- **EventMapper** is a singleton service
- **Background Services** require special testing approach (TestHost or mocking)
- **Sync Services** need to test both enabled and disabled states
- **MinIO Operations** should test with real MinIO test container for integration tests
- **MongoDB/Redis Sync** should test with real containers for integration tests

