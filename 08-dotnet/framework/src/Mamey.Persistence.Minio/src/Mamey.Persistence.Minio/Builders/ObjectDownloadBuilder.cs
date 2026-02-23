using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio.Builders;

/// <summary>
/// Fluent builder for configuring object download operations.
/// </summary>
public class ObjectDownloadBuilder
{
    private readonly string _bucketName;
    private readonly string _objectName;
    private readonly Dictionary<string, string> _headers = new();
    private string? _versionId;
    private long? _rangeStart;
    private long? _rangeEnd;
    private string? _ifMatch;
    private string? _ifNoneMatch;
    private DateTime? _ifModifiedSince;
    private DateTime? _ifUnmodifiedSince;

    internal ObjectDownloadBuilder(string bucketName, string objectName)
    {
        _bucketName = bucketName;
        _objectName = objectName;
    }

    /// <summary>
    /// Sets the version ID for the download.
    /// </summary>
    /// <param name="versionId">The version ID.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder WithVersion(string versionId)
    {
        _versionId = versionId;
        return this;
    }

    /// <summary>
    /// Sets a range for partial download.
    /// </summary>
    /// <param name="start">The start byte position.</param>
    /// <param name="end">The end byte position (inclusive).</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder WithRange(long start, long? end = null)
    {
        _rangeStart = start;
        _rangeEnd = end;
        return this;
    }

    /// <summary>
    /// Sets a range from start to end of object.
    /// </summary>
    /// <param name="start">The start byte position.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder FromByte(long start)
    {
        _rangeStart = start;
        _rangeEnd = null;
        return this;
    }

    /// <summary>
    /// Sets a range from start to a specific byte.
    /// </summary>
    /// <param name="start">The start byte position.</param>
    /// <param name="length">The number of bytes to download.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder WithLength(long start, long length)
    {
        _rangeStart = start;
        _rangeEnd = start + length - 1;
        return this;
    }

    /// <summary>
    /// Sets conditional download based on ETag match.
    /// </summary>
    /// <param name="etag">The ETag to match.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder IfMatch(string etag)
    {
        _ifMatch = etag;
        return this;
    }

    /// <summary>
    /// Sets conditional download based on ETag not match.
    /// </summary>
    /// <param name="etag">The ETag to not match.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder IfNoneMatch(string etag)
    {
        _ifNoneMatch = etag;
        return this;
    }

    /// <summary>
    /// Sets conditional download based on modification date.
    /// </summary>
    /// <param name="modifiedSince">Download only if modified since this date.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder IfModifiedSince(DateTime modifiedSince)
    {
        _ifModifiedSince = modifiedSince;
        return this;
    }

    /// <summary>
    /// Sets conditional download based on modification date.
    /// </summary>
    /// <param name="unmodifiedSince">Download only if not modified since this date.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder IfUnmodifiedSince(DateTime unmodifiedSince)
    {
        _ifUnmodifiedSince = unmodifiedSince;
        return this;
    }

    /// <summary>
    /// Adds a custom header to the download request.
    /// </summary>
    /// <param name="key">The header key.</param>
    /// <param name="value">The header value.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder WithHeader(string key, string value)
    {
        _headers[key] = value;
        return this;
    }

    /// <summary>
    /// Adds multiple headers to the download request.
    /// </summary>
    /// <param name="headers">The headers dictionary.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder WithHeaders(Dictionary<string, string> headers)
    {
        foreach (var kvp in headers)
        {
            _headers[kvp.Key] = kvp.Value;
        }
        return this;
    }

    /// <summary>
    /// Sets the response content type.
    /// </summary>
    /// <param name="contentType">The content type.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder WithResponseContentType(string contentType)
    {
        _headers["response-content-type"] = contentType;
        return this;
    }

    /// <summary>
    /// Sets the response content disposition.
    /// </summary>
    /// <param name="contentDisposition">The content disposition.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder WithResponseContentDisposition(string contentDisposition)
    {
        _headers["response-content-disposition"] = contentDisposition;
        return this;
    }

    /// <summary>
    /// Sets the response cache control.
    /// </summary>
    /// <param name="cacheControl">The cache control.</param>
    /// <returns>The builder instance.</returns>
    public ObjectDownloadBuilder WithResponseCacheControl(string cacheControl)
    {
        _headers["response-cache-control"] = cacheControl;
        return this;
    }

    /// <summary>
    /// Builds the download configuration.
    /// </summary>
    /// <returns>The download configuration.</returns>
    public ObjectDownloadConfiguration Build()
    {
        var headers = new Dictionary<string, string>(_headers);

        // Add conditional headers
        if (!string.IsNullOrEmpty(_ifMatch))
            headers["If-Match"] = _ifMatch;
        if (!string.IsNullOrEmpty(_ifNoneMatch))
            headers["If-None-Match"] = _ifNoneMatch;
        if (_ifModifiedSince.HasValue)
            headers["If-Modified-Since"] = _ifModifiedSince.Value.ToString("R");
        if (_ifUnmodifiedSince.HasValue)
            headers["If-Unmodified-Since"] = _ifUnmodifiedSince.Value.ToString("R");

        // Add range header
        if (_rangeStart.HasValue)
        {
            var rangeValue = _rangeEnd.HasValue 
                ? $"bytes={_rangeStart.Value}-{_rangeEnd.Value}"
                : $"bytes={_rangeStart.Value}-";
            headers["Range"] = rangeValue;
        }

        return new ObjectDownloadConfiguration
        {
            BucketName = _bucketName,
            ObjectName = _objectName,
            VersionId = _versionId,
            Headers = headers.Count > 0 ? headers : null
        };
    }
}

/// <summary>
/// Configuration for object download operations.
/// </summary>
public class ObjectDownloadConfiguration
{
    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the object name.
    /// </summary>
    public string ObjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version ID.
    /// </summary>
    public string? VersionId { get; set; }

    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }
}
