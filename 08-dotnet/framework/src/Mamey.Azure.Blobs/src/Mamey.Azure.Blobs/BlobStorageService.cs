using System.Net.Mime;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Primitives;
using Azure.Storage.Sas;

namespace Mamey.Azure.Blobs;

public class BlobStorageService : IBlobStorageService
{
    private readonly ResourceNameValidator _validator;
    private readonly BlobServiceClient _blobServiceClient;


    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _validator = new ResourceNameValidator();
        _blobServiceClient = blobServiceClient;
    }
    #region Naming Validation

    public bool ValidateBlobName(string blobName)
    {
        return _validator.IsValidBlobName(blobName);
    }

    public bool ValidateContainerName(string containerName)
    {
        return _validator.IsValidContainerName(containerName);
    }

    public bool ValidateDirectoryName(string directoryName)
    {
        return _validator.IsValidDirectoryName(directoryName);
    }
    #endregion

    #region Account Operations

    public virtual BlobLeaseClient GetLeaseClient(BlobContainerClient containerClient, string leaseId = null)
    {
        return containerClient.GetBlobLeaseClient(leaseId);
    }

    public async Task SetBlobServicePropertiesAsync(BlobServiceProperties properties)
    {
        await _blobServiceClient.SetPropertiesAsync(properties);
    }

    public async Task<BlobServiceProperties> GetBlobServicePropertiesAsync()
    {
        return await _blobServiceClient.GetPropertiesAsync();
    }
    public async Task<BlobCorsRule[]> PreflightBlobRequestAsync()
    {
        var properties = await GetBlobServicePropertiesAsync();
        return properties.Cors.ToArray();
    }
    public async Task<BlobServiceStatistics> GetBlobServiceStatsAsync()
    {
        return await _blobServiceClient.GetStatisticsAsync();
    }
    public async Task<AccountInfo> GetAccountInfoAsync()
    {
        return await _blobServiceClient.GetAccountInfoAsync();
    }
    public async Task<UserDelegationKey> GetUserDelegationKeyAsync(DateTimeOffset startsOn, DateTimeOffset expiresOn)
    {
        return await _blobServiceClient.GetUserDelegationKeyAsync(startsOn, expiresOn);
    }
    #endregion

    #region ContainerOperations

    public async Task CreateBlobContainerIfNotExistsAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
    }

    public async Task<IEnumerable<BlobItem>> ListBlobsAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobs = containerClient.GetBlobsAsync();
        return blobs.ToBlockingEnumerable();
    }

    public async Task<string> GenerateBlobSasUrl(string containerName, string blobName, int validMinutes = 60)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b", // b for blob, c for container
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(validMinutes)
        };

        // Specify read permissions for the blob
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        // Generate SAS token
        var sasToken = blobClient.GenerateSasUri(sasBuilder).ToString();

        return sasToken;
    }
    public async Task<MameyBlobFileDownloadResult> DownloadBlobAsync(string containerName, string blobName, string downloadFilePath)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient($"{downloadFilePath}{blobName}");

        // Download the blob
        BlobDownloadInfo downloadInfo = await blobClient.DownloadAsync();

        return MameyBlobFileDownloadResult.FromResult(downloadInfo);
    }
    public async Task<IEnumerable<KeyValuePair<string, StringValues>>> GetBlobPropertiesAsync(string container)
    {
        try
        {
            var c = await _blobServiceClient.GetBlobContainerClient(container)
                .GetPropertiesAsync();
            Console.WriteLine(c.Value.DefaultEncryptionScope);
            Console.WriteLine(c.Value.DeletedOn);
            Console.WriteLine(c.Value.ETag);
            Console.WriteLine(c.Value.HasImmutabilityPolicy);
            Console.WriteLine(c.Value.HasImmutableStorageWithVersioning);
            Console.WriteLine(c.Value.HasLegalHold);
            Console.WriteLine(c.Value.LastModified);
            Console.WriteLine(c.Value.LeaseDuration);
            Console.WriteLine(c.Value.LeaseState);
            Console.WriteLine(c.Value.LeaseStatus);
            Console.WriteLine(c.Value.Metadata);
            Console.WriteLine(c.Value.PreventEncryptionScopeOverride);
            Console.WriteLine(c.Value.PublicAccess);
            Console.WriteLine(c.Value.RemainingRetentionDays);
            var cpair = c.ToKeyValuePairList();
            return cpair;
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
            Console.WriteLine(e.Message);
            Console.ReadLine();
            throw;
        }
    }

    public async Task<BlobContainerClient> CreateContainerAsync(string containerName, PublicAccessType publicAccessType = PublicAccessType.None)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateAsync(publicAccessType);
        return containerClient;
    }

    public async Task<BlobContainerProperties> GetContainerPropertiesAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        return await containerClient.GetPropertiesAsync();
    }

    public async Task<IDictionary<string, string>> GetContainerMetadataAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var properties = await containerClient.GetPropertiesAsync();
        return properties.Value.Metadata;
    }

    public async Task SetContainerMetadataAsync(string containerName, IDictionary<string, string> metadata)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.SetMetadataAsync(metadata);
    }

    public async Task<BlobContainerAccessPolicy> GetContainerAclAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        return await containerClient.GetAccessPolicyAsync();
    }

    public async Task SetContainerAclAsync(string containerName, PublicAccessType publicAccessType, IEnumerable<BlobSignedIdentifier> permissions = null)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.SetAccessPolicyAsync(publicAccessType, permissions);
    }

    public async Task DeleteContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.DeleteAsync();
    }

    public async Task<string> LeaseContainerAsync(string containerName, BlobLeaseClient leaseClient, TimeSpan leaseTime)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var leaseResponse = await leaseClient.AcquireAsync(leaseTime);
        return leaseResponse.Value.LeaseId;
    }

    public async Task<List<string>> ListBlobNamesAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobs = new List<string>();

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            blobs.Add(blobItem.Name);
        }

        return blobs;
    }
    public async Task<IEnumerable<BlobItem>> ListBlobItemsAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobs = containerClient.GetBlobsAsync();
        return blobs.ToBlockingEnumerable();
    }

    public async Task RestoreContainerAsync(string deletedContainerName, string targetContainerName)
    {
        await _blobServiceClient.UndeleteBlobContainerAsync(deletedContainerName, targetContainerName);
    }
    #endregion

    #region Blob Operations

    public async Task<Uri> UploadFileAsync(string containerName, string blobName, Stream content, string contentType = MediaTypeNames.Application.Octet, bool overwrite = false, string? md5Hash = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
    {
        // Ensure that the stream position is at the start
        if (content.CanSeek)
        {
            content.Position = 0;
        }
        else
        {
            throw new ArgumentException("The provided stream does not support seeking.");
        }

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(
            content: content,

            options: new BlobUploadOptions()
            {
                HttpHeaders = md5Hash is null
                    ? new BlobHttpHeaders()
                    {
                        ContentType = contentType
                    }
                    : new BlobHttpHeaders()
                    {
                        ContentType = contentType
                    },
                Metadata = metadata
            }, cancellationToken);

        return blobClient.Uri;
    }
    

    #region Block Blob Operations
    public async Task<Response<BlobContentInfo>> UploadBlockBlobAsync(string containerName, string blobName, Stream content, bool overwrite = false)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        if (!overwrite && await blobClient.ExistsAsync())
        {
            throw new InvalidOperationException($"Blob '{blobName}' already exists in container '{containerName}'.");
        }

        return await blobClient.UploadAsync(content, overwrite: overwrite);
    }

    public async Task<Stream> DownloadBlockBlobAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        BlobDownloadInfo download = await blobClient.DownloadAsync();
        return download.Content;
    }
    public async Task<BlobProperties> GetBlobPropertiesAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.GetPropertiesAsync();
    }
    public async Task<Response<BlobInfo>> SetBlobPropertiesAsync(string containerName, string blobName, BlobHttpHeaders headers)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.SetHttpHeadersAsync(headers);
    }

    public async Task<IDictionary<string, string>> GetBlobTagsAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.GetTagsAsync();
        return response.Value.Tags;
    }
    public async Task<Response> SetBlobTagsAsync(string containerName, string blobName, IDictionary<string, string> tags)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.SetTagsAsync(tags);
    }
    public async Task<IEnumerable<string>> FindBlobsByTagsAsync(string containerName, string tagFilterSqlExpression)
    {
        var blobs = new List<string>();
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await foreach (var blobItem in containerClient.FindBlobsByTagsAsync(tagFilterSqlExpression))
        {
            blobs.Add(blobItem.BlobName);
        }
        return blobs;
    }
    public async Task<IDictionary<string, string>> GetBlobMetadataAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        var properties = await blobClient.GetPropertiesAsync();
        return properties.Value.Metadata;
    }
    public async Task SetBlobMetadataAsync(string containerName, string blobName, IDictionary<string, string> metadata)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.SetMetadataAsync(metadata);
    }
    public async Task<string> LeaseBlobAsync(string containerName, string blobName, TimeSpan duration)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        var leaseClient = blobClient.GetBlobLeaseClient();
        var lease = await leaseClient.AcquireAsync(duration);
        return lease.Value.LeaseId;
    }
    public async Task<BlobClient> SnapshotBlobAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        var snapshot = await blobClient.CreateSnapshotAsync();
        return blobClient.WithSnapshot(snapshot.Value.Snapshot);
    }
    public async Task CopyBlobAsync(string containerName, string sourceBlobName, string destinationBlobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var sourceBlob = containerClient.GetBlobClient(sourceBlobName);
        var destinationBlob = containerClient.GetBlobClient(destinationBlobName);
        await destinationBlob.StartCopyFromUriAsync(sourceBlob.Uri);
    }
    public async Task CopyBlobFromUrlAsync(string containerName, Uri sourceUri, string destinationBlobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var destinationBlob = containerClient.GetBlobClient(destinationBlobName);
        await destinationBlob.StartCopyFromUriAsync(sourceUri);
    }
    public async Task AbortCopyBlobAsync(string containerName, string blobName, string copyId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.AbortCopyFromUriAsync(copyId);
    }
    public async Task DeleteBlobAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }
    public async Task UndeleteBlobAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UndeleteAsync();
    }
    public async Task SetBlobTierAsync(string containerName, string blobName, AccessTier tier)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.SetAccessTierAsync(tier);
    }

    #endregion

    #region Page Blob Operations
    public async Task<Response<PageInfo>> WritePagesAsync(string containerName, string blobName, Stream content, long offset)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var pageBlobClient = containerClient.GetPageBlobClient(blobName);

        return await pageBlobClient.UploadPagesAsync(content, offset);
    }
    public async Task<IEnumerable<HttpRange>> GetPageRangesAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var pageBlobClient = containerClient.GetPageBlobClient(blobName);

        return (await pageBlobClient.GetPageRangesAsync()).Value.PageRanges;
    }

    public async Task<Response<PageInfo>> PutPageFromUrlAsync(string containerName, string blobName, Uri sourceUri, long sourceOffset, long sourceLength, long destinationOffset)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var pageBlobClient = containerClient.GetPageBlobClient(blobName);

        // Define the source and destination ranges
        var sourceRange = new HttpRange(sourceOffset, sourceLength);
        var destinationRange = new HttpRange(destinationOffset, sourceLength);

        // Upload the pages from the URI to the page blob
        return await pageBlobClient.UploadPagesFromUriAsync(sourceUri, sourceRange, destinationRange);
    }
    public async Task IncrementalCopyBlobAsync(string containerName, string sourceBlobName, string destinationBlobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var sourceBlob = containerClient.GetPageBlobClient(sourceBlobName);
        var destinationBlob = containerClient.GetPageBlobClient(destinationBlobName);
        await destinationBlob.StartCopyIncrementalAsync(sourceBlob.Uri, sourceBlob.Uri.ToString());
    }

    #endregion

    #region Append Blob Operations
    public async Task<Response<BlobAppendInfo>> AppendBlockAsync(string containerName, string blobName, Stream content)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var appendBlobClient = containerClient.GetAppendBlobClient(blobName);

        return await appendBlobClient.AppendBlockAsync(content);
    }
    public async Task AppendBlockFromUrlAsync(string containerName, string blobName, Uri sourceUri)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var appendBlobClient = containerClient.GetAppendBlobClient(blobName);
        await appendBlobClient.AppendBlockFromUriAsync(sourceUri);
    }
    #endregion


    #endregion
}
