using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mamey.Biometrics.Storage.MongoDB.Models;

namespace Mamey.Biometrics.Storage.MongoDB;

/// <summary>
/// Repository interface for biometric template storage in MongoDB.
/// </summary>
public interface IBiometricTemplateRepository
{
    /// <summary>
    /// Add a new biometric template.
    /// </summary>
    /// <param name="template">Template to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Added template</returns>
    Task<BiometricTemplateDocument> AddAsync(BiometricTemplateDocument template, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a template by ID.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Template or null if not found</returns>
    Task<BiometricTemplateDocument?> GetByIdAsync(Guid templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all templates for a subject.
    /// </summary>
    /// <param name="subjectId">Subject ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of templates</returns>
    Task<List<BiometricTemplateDocument>> GetBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get templates with pagination.
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged result</returns>
    Task<PagedResult<BiometricTemplateDocument>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search templates by criteria.
    /// </summary>
    /// <param name="subjectId">Optional subject ID filter</param>
    /// <param name="tags">Optional tags filter</param>
    /// <param name="minQualityScore">Minimum quality score</param>
    /// <param name="createdAfter">Created after date</param>
    /// <param name="createdBefore">Created before date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching templates</returns>
    Task<List<BiometricTemplateDocument>> SearchAsync(
        Guid? subjectId = null,
        string[]? tags = null,
        double? minQualityScore = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a template.
    /// </summary>
    /// <param name="template">Template to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated template or null if not found</returns>
    Task<BiometricTemplateDocument?> UpdateAsync(BiometricTemplateDocument template, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a template.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteAsync(Guid templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all templates (for identification).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all templates</returns>
    Task<List<BiometricTemplateDocument>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get template count.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total count</returns>
    Task<long> GetCountAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Paged result for template queries.
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Items in current page
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Total count of items
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
