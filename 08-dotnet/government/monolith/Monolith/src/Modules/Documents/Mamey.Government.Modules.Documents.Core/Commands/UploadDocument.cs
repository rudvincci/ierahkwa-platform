using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Documents.Core.Commands;

public record UploadDocument(
    Guid TenantId,
    string FileName,
    string ContentType,
    long FileSize,
    string StorageBucket,
    string StorageKey,
    string? Category = null,
    string? Description = null) : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
