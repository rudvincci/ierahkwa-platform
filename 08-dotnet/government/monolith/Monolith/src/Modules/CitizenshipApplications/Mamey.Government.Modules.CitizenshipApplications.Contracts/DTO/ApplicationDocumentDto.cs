namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public record ApplicationDocumentDto(
    Guid Id,
    string DocumentType,
    string FileName,
    string StoragePath,
    long FileSize,
    DateTime UploadedAt);