using Mamey.Portal.Library.Application.Models;
using Mamey.Portal.Library.Application.Requests;

namespace Mamey.Portal.Library.Application.Services;

public interface ILibraryService
{
    // Public / portal views
    Task<IReadOnlyList<LibraryItem>> GetPublishedPublicAsync(string? searchTerm = null, CancellationToken ct = default);
    Task<IReadOnlyList<LibraryItem>> GetPublishedForCurrentUserAsync(string? searchTerm = null, CancellationToken ct = default);

    // Management (role-guarded in implementation)
    Task<IReadOnlyList<LibraryItem>> GetAllAsync(string? searchTerm = null, CancellationToken ct = default);
    Task<LibraryItem> CreateDraftAsync(
        string category,
        string title,
        string? summary,
        LibraryVisibility visibility,
        LibraryUploadFile file,
        CancellationToken ct = default);

    Task<LibraryItem> UpdateDraftAsync(
        Guid id,
        string category,
        string title,
        string? summary,
        LibraryVisibility visibility,
        LibraryUploadFile? replaceFile,
        CancellationToken ct = default);

    Task<LibraryItem> PublishAsync(Guid id, CancellationToken ct = default);
    Task<LibraryItem> UnpublishAsync(Guid id, CancellationToken ct = default);
}



