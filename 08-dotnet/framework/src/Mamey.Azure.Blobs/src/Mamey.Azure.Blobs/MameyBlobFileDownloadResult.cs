using Azure.Storage.Blobs.Models;

namespace Mamey.Azure.Blobs;

public class MameyBlobFileDownloadResult
{
    internal MameyBlobFileDownloadResult()
    {

    }

    internal MameyBlobFileDownloadResult(BlobDownloadInfo? downloadInfo, Stream? content)
    {
        DownloadInfo = downloadInfo;
        Success = downloadInfo is null ? false : true;
        Value = downloadInfo is null
            ? null
            : BlobsModelFactory.BlobDownloadResult(BinaryData.FromStream(downloadInfo.Content), downloadInfo.Details);
    }

    /// <summary>
    /// True when blob download is successful
    /// </summary>
    public bool Success { get; internal set; } = false;

    /// <summary>
    /// Details returned when downloading a Blob
    /// </summary>
    public BlobDownloadResult? Value { get; internal set; } = null;
    /// <summary>
    /// Details returned when downloading a Blob
    /// </summary>
    public BlobDownloadInfo? DownloadInfo { get; internal set; } = null;

    public BlobDownloadDetails? Details => DownloadInfo?.Details;

    public static MameyBlobFileDownloadResult FromResult(BlobDownloadInfo? downloadInfo)
    {
        if(downloadInfo is null)
        {
            return new MameyBlobFileDownloadResult(null, null);
        }

        return new MameyBlobFileDownloadResult(downloadInfo, downloadInfo.Content);
    }
        
}