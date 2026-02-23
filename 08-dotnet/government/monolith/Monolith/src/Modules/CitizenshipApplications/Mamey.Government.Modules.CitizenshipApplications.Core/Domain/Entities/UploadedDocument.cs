using Mamey.MicroMonolith.Infrastructure.Mongo;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;
/// <summary>
/// Uploaded document for citizenship application.
/// </summary>
internal class UploadedDocument : IIdentifiable<Guid>
{
    public UploadedDocument(
        Guid id,
        string fileName,
        string storagePath,
        string documentType,
        long fileSize)
    {
        Id = id;
        FileName = fileName;
        StoragePath = storagePath;
        DocumentType = documentType;
        FileSize = fileSize;
        UploadedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public AppId ApplicationId { get; set; }
    public string FileName { get; }
    public string StoragePath { get; } // MinIO path
    public string DocumentType { get; }
    public long FileSize { get; }
    public DateTime UploadedAt { get; }
    
    // Navigation property
    public CitizenshipApplication? Application { get; set; }
}