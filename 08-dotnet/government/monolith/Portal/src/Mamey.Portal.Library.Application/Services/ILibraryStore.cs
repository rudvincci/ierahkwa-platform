using Mamey.Portal.Library.Application.Models;

namespace Mamey.Portal.Library.Application.Services;

public interface ILibraryStore
{
    Task<IReadOnlyList<LibraryItemSnapshot>> GetPublishedPublicAsync(
        string tenantId,
        int take,
        string? searchTerm = null,
        CancellationToken ct = default);
    Task<IReadOnlyList<LibraryItemSnapshot>> GetPublishedForVisibilityAsync(
        string tenantId,
        LibraryVisibility maxVisibility,
        int take,
        string? searchTerm = null,
        CancellationToken ct = default);
    Task<IReadOnlyList<LibraryItemSnapshot>> GetAllAsync(
        string tenantId,
        int take,
        string? searchTerm = null,
        CancellationToken ct = default);
    Task<LibraryItemSnapshot?> GetAsync(string tenantId, Guid id, CancellationToken ct = default);
    Task<LibraryItemSnapshot> CreateDraftAsync(
        string tenantId,
        string category,
        string title,
        string? summary,
        LibraryVisibility visibility,
        string fileName,
        string contentType,
        long size,
        string storageBucket,
        string storageKey,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<LibraryItemSnapshot> UpdateDraftAsync(
        string tenantId,
        Guid id,
        string category,
        string title,
        string? summary,
        LibraryVisibility visibility,
        string? fileName,
        string? contentType,
        long? size,
        string? storageBucket,
        string? storageKey,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<LibraryItemSnapshot> PublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<LibraryItemSnapshot> UnpublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default);
}
