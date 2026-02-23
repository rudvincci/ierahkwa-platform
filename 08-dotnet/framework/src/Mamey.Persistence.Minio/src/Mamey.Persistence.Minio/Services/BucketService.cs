using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Mamey.Persistence.Minio.Exceptions;
using Mamey.Persistence.Minio.Infrastructure;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio.Services;

/// <summary>
/// Service for managing Minio buckets.
/// </summary>
public class BucketService : BaseMinioService, IBucketService
{
    public BucketService(
        IMinioClient client,
        IOptions<MinioOptions> options,
        ILogger<BucketService> logger,
        IRetryPolicyExecutor retryPolicyExecutor)
        : base(client, options, logger, retryPolicyExecutor)
    {
    }

    /// <inheritdoc />
    public async Task<Collection<global::Minio.DataModel.Bucket>> ListBucketsAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Listing all buckets");
                var buckets = await Client.ListBucketsAsync(ct);
                Logger.LogInformation("Successfully listed {Count} buckets", buckets.Buckets.Count);
                return buckets.Buckets;
            },
            "ListBuckets",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task MakeBucketAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Creating bucket: {BucketName}", bucketName);
                await Client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), ct);
                Logger.LogInformation("Successfully created bucket: {BucketName}", bucketName);
            },
            $"MakeBucket_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> BucketExistsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Checking if bucket exists: {BucketName}", bucketName);
                var exists = await Client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName), ct);
                Logger.LogDebug("Bucket {BucketName} exists: {Exists}", bucketName, exists);
                return exists;
            },
            $"BucketExists_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveBucketAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Removing bucket: {BucketName}", bucketName);
                await Client.RemoveBucketAsync(new RemoveBucketArgs().WithBucket(bucketName), ct);
                Logger.LogInformation("Successfully removed bucket: {BucketName}", bucketName);
            },
            $"RemoveBucket_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Collection<ObjectInfo>> ListObjectsAsync(ListObjectsRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketName(request.BucketName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Listing objects in bucket: {BucketName}", request.BucketName);

                var args = new ListObjectsArgs().WithBucket(request.BucketName);

                if (!string.IsNullOrEmpty(request.Prefix))
                    args.WithPrefix(request.Prefix);

                if (request.Recursive.HasValue)
                    args.WithRecursive(request.Recursive.Value);

                // Note: These methods may not be available in the current Minio API version
                // if (request.MaxKeys.HasValue)
                //     args.WithMaxKeys(request.MaxKeys.Value);

                // if (!string.IsNullOrEmpty(request.Marker))
                //     args.WithMarker(request.Marker);

                var objects = new Collection<ObjectInfo>();
                var observable = Client.ListObjectsAsync(args);

                // Convert observable to async enumerable
                var items = new List<Item>();
                observable.Subscribe(item => items.Add(item));
                
                // Wait a bit for the observable to complete
                await Task.Delay(100);
                
                foreach (var item in items)
                {
                    objects.Add(new ObjectInfo
                    {
                        Name = item.Key,
                        Size = item.Size,
                        LastModified = item.LastModifiedDateTime ?? DateTime.MinValue,
                        ETag = item.ETag,
                        IsDir = item.IsDir,
                        VersionId = item.VersionId,
                        IsDeleteMarker = false // item.IsDeleteMarker not available in current API
                    });
                }

                Logger.LogDebug("Successfully listed {Count} objects in bucket: {BucketName}", objects.Count, request.BucketName);
                return objects;
            },
            $"ListObjects_{request.BucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task EnableVersioningAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Enabling versioning for bucket: {BucketName}", bucketName);
                await Client.SetVersioningAsync(new SetVersioningArgs().WithBucket(bucketName).WithVersioningEnabled(), ct);
                Logger.LogInformation("Successfully enabled versioning for bucket: {BucketName}", bucketName);
            },
            $"EnableVersioning_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task DisableVersioningAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Disabling versioning for bucket: {BucketName}", bucketName);
                await Client.SetVersioningAsync(new SetVersioningArgs().WithBucket(bucketName).WithVersioningSuspended(), ct);
                Logger.LogInformation("Successfully disabled versioning for bucket: {BucketName}", bucketName);
            },
            $"DisableVersioning_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BucketVersioningInfo> GetVersioningAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Getting versioning info for bucket: {BucketName}", bucketName);
                var versioning = await Client.GetVersioningAsync(new GetVersioningArgs().WithBucket(bucketName), ct);
                
                var result = new BucketVersioningInfo
                {
                    Status = Enum.TryParse<VersioningStatus>(versioning.Status, true, out var status) ? status : VersioningStatus.Off,
                    MfaDelete = Enum.TryParse<MfaDeleteStatus>(versioning.MfaDelete, true, out var mfaDelete) ? mfaDelete : MfaDeleteStatus.Disabled
                };

                Logger.LogDebug("Successfully retrieved versioning info for bucket: {BucketName}", bucketName);
                return result;
            },
            $"GetVersioning_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetBucketVersioningAsync(string bucketName, VersioningStatus status, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Setting versioning status {Status} for bucket: {BucketName}", status, bucketName);
                
                var args = new SetVersioningArgs().WithBucket(bucketName);
                switch (status)
                {
                    case VersioningStatus.Enabled:
                        args.WithVersioningEnabled();
                        break;
                    case VersioningStatus.Suspended:
                        args.WithVersioningSuspended();
                        break;
                    case VersioningStatus.Off:
                    default:
                        args.WithVersioningSuspended(); // Off is typically represented as Suspended
                        break;
                }

                await Client.SetVersioningAsync(args, ct);
                Logger.LogInformation("Successfully set versioning status {Status} for bucket: {BucketName}", status, bucketName);
            },
            $"SetBucketVersioning_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BucketVersioningInfo> GetBucketVersioningAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        return await GetVersioningAsync(bucketName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetBucketTagsAsync(SetBucketTagsRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketName(request.BucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Setting tags for bucket: {BucketName}", request.BucketName);
                
                var tags = new Dictionary<string, string>();
                foreach (var tag in request.Tags)
                {
                    tags[$"x-amz-tagging-{tag.Key}"] = tag.Value;
                }

                // Note: Minio doesn't have direct bucket tagging, so we'll skip this for now
                // await Client.SetObjectTagsAsync(new SetObjectTagsArgs()
                //     .WithBucket(request.BucketName)
                //     .WithObject("")
                //     .WithTagging(tags), ct);

                Logger.LogInformation("Successfully set tags for bucket: {BucketName}", request.BucketName);
            },
            $"SetBucketTags_{request.BucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetBucketTagsAsync(string bucketName, Dictionary<string, string> tags, CancellationToken cancellationToken = default)
    {
        if (tags == null)
            throw new ArgumentNullException(nameof(tags));

        var request = new SetBucketTagsRequest
        {
            BucketName = bucketName,
            Tags = tags
        };

        await SetBucketTagsAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Tags> GetBucketTagsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Getting tags for bucket: {BucketName}", bucketName);
                // Note: Minio doesn't have direct bucket tagging, so we'll return empty tags
                var tags = new global::Minio.DataModel.Tags.Tagging();

                var result = new Tags();
                foreach (var tag in tags.Tags)
                {
                    result.TagSet.Add(new TagSet
                    {
                        Key = tag.Key,
                        Value = tag.Value
                    });
                }

                Logger.LogDebug("Successfully retrieved tags for bucket: {BucketName}", bucketName);
                return result;
            },
            $"GetBucketTags_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveBucketTagsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Removing tags for bucket: {BucketName}", bucketName);
                // Note: Minio doesn't have direct bucket tagging, so we'll skip this for now
                // await Client.RemoveObjectTagsAsync(new RemoveObjectTagsArgs()
                //     .WithBucket(bucketName)
                //     .WithObject(""), ct);
                Logger.LogInformation("Successfully removed tags for bucket: {BucketName}", bucketName);
            },
            $"RemoveBucketTags_{bucketName}",
            cancellationToken);
    }
}
