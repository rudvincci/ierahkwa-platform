using System.Net.Mime;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Primitives;

namespace Mamey.Azure.Blobs
{
    public interface IBlobStorageService
    {
        // Containter Operations
        Task CreateBlobContainerIfNotExistsAsync(string containerName);
        Task<IEnumerable<BlobItem>> ListBlobsAsync(string containerName);

        // Blob Operations
        Task<Uri> UploadFileAsync(string containerName, string blobName, Stream content, string contentType = MediaTypeNames.Application.Octet, bool overwrite = false, string? md5Hash = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);





        Task AbortCopyBlobAsync(string containerName, string blobName, string copyId);
        Task<Response<BlobAppendInfo>> AppendBlockAsync(string containerName, string blobName, Stream content);
        Task AppendBlockFromUrlAsync(string containerName, string blobName, Uri sourceUri);
        Task CopyBlobAsync(string containerName, string sourceBlobName, string destinationBlobName);
        Task CopyBlobFromUrlAsync(string containerName, Uri sourceUri, string destinationBlobName);
        Task<BlobContainerClient> CreateContainerAsync(string containerName, PublicAccessType publicAccessType = PublicAccessType.None);
        Task DeleteBlobAsync(string containerName, string blobName);
        Task DeleteContainerAsync(string containerName);
        Task<Stream> DownloadBlockBlobAsync(string containerName, string blobName);
        Task<IEnumerable<string>> FindBlobsByTagsAsync(string containerName, string tagFilterSqlExpression);
        Task<string> GenerateBlobSasUrl(string containerName, string blobName, int validMinutes = 60);
        Task<AccountInfo> GetAccountInfoAsync();
        Task<IDictionary<string, string>> GetBlobMetadataAsync(string containerName, string blobName);
        Task<IEnumerable<KeyValuePair<string, StringValues>>> GetBlobPropertiesAsync(string container);
        Task<BlobProperties> GetBlobPropertiesAsync(string containerName, string blobName);
        Task<BlobServiceProperties> GetBlobServicePropertiesAsync();
        Task<BlobServiceStatistics> GetBlobServiceStatsAsync();
        Task<IDictionary<string, string>> GetBlobTagsAsync(string containerName, string blobName);
        Task<BlobContainerAccessPolicy> GetContainerAclAsync(string containerName);
        Task<IDictionary<string, string>> GetContainerMetadataAsync(string containerName);
        Task<BlobContainerProperties> GetContainerPropertiesAsync(string containerName);
        BlobLeaseClient GetLeaseClient(BlobContainerClient containerClient, string leaseId = null);
        Task<IEnumerable<HttpRange>> GetPageRangesAsync(string containerName, string blobName);
        Task<UserDelegationKey> GetUserDelegationKeyAsync(DateTimeOffset startsOn, DateTimeOffset expiresOn);
        Task IncrementalCopyBlobAsync(string containerName, string sourceBlobName, string destinationBlobName);
        Task<string> LeaseBlobAsync(string containerName, string blobName, TimeSpan duration);
        Task<string> LeaseContainerAsync(string containerName, BlobLeaseClient leaseClient, TimeSpan leaseTime);
        Task<IEnumerable<BlobItem>> ListBlobItemsAsync(string containerName);
        Task<List<string>> ListBlobNamesAsync(string containerName);
        Task<BlobCorsRule[]> PreflightBlobRequestAsync();
        Task<Response<PageInfo>> PutPageFromUrlAsync(string containerName, string blobName, Uri sourceUri, long sourceOffset, long sourceLength, long destinationOffset);
        Task RestoreContainerAsync(string deletedContainerName, string targetContainerName);
        Task SetBlobMetadataAsync(string containerName, string blobName, IDictionary<string, string> metadata);
        Task<Response<BlobInfo>> SetBlobPropertiesAsync(string containerName, string blobName, BlobHttpHeaders headers);
        Task SetBlobServicePropertiesAsync(BlobServiceProperties properties);
        Task<Response> SetBlobTagsAsync(string containerName, string blobName, IDictionary<string, string> tags);
        Task SetBlobTierAsync(string containerName, string blobName, AccessTier tier);
        Task SetContainerAclAsync(string containerName, PublicAccessType publicAccessType, IEnumerable<BlobSignedIdentifier> permissions = null);
        Task SetContainerMetadataAsync(string containerName, IDictionary<string, string> metadata);
        Task<BlobClient> SnapshotBlobAsync(string containerName, string blobName);
        Task UndeleteBlobAsync(string containerName, string blobName);
        Task<Response<BlobContentInfo>> UploadBlockBlobAsync(string containerName, string blobName, Stream content, bool overwrite = false);
        bool ValidateBlobName(string blobName);
        bool ValidateContainerName(string containerName);
        bool ValidateDirectoryName(string directoryName);
        Task<Response<PageInfo>> WritePagesAsync(string containerName, string blobName, Stream content, long offset);
    }
}