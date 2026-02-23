using Mamey.Government.Modules.Documents.Core.Domain.Entities;
using Mamey.Government.Modules.Documents.Core.DTO;

namespace Mamey.Government.Modules.Documents.Core.Mappings;

internal static class DocumentMappings
{
    public static DocumentDto AsDto(this Document document)
        => new()
        {
            Id = document.Id.Value,
            TenantId = document.TenantId.Value,
            FileName = document.FileName,
            ContentType = document.ContentType,
            FileSize = document.FileSize,
            StorageBucket = document.StorageBucket,
            StorageKey = document.StorageKey,
            Category = document.Category,
            Description = document.Description,
            Metadata = document.Metadata,
            IsActive = document.IsActive,
            DeletedAt = document.DeletedAt,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };

    public static DocumentSummaryDto AsSummaryDto(this Document document)
        => new()
        {
            Id = document.Id.Value,
            FileName = document.FileName,
            ContentType = document.ContentType,
            FileSize = document.FileSize,
            Category = document.Category,
            CreatedAt = document.CreatedAt,
            IsActive = document.IsActive
        };
}
