using DocumentFlow.Core.Models;

namespace DocumentFlow.Core.Interfaces;

public interface IDocumentService
{
    // Document Operations
    Task<Document> CreateDocumentAsync(Document document, Stream fileStream);
    Task<Document?> GetDocumentByIdAsync(Guid id);
    Task<Document?> GetDocumentByNumberAsync(string documentNumber);
    Task<IEnumerable<Document>> GetDocumentsByFolderAsync(Guid folderId);
    Task<IEnumerable<Document>> GetDocumentsByDepartmentAsync(string department);
    Task<IEnumerable<Document>> GetDocumentsByOwnerAsync(Guid ownerId);
    Task<IEnumerable<Document>> SearchDocumentsAsync(DocumentSearchCriteria criteria);
    Task<Document> UpdateDocumentAsync(Document document);
    Task<Document> UpdateDocumentFileAsync(Guid id, Stream fileStream, string? changeNotes);
    Task DeleteDocumentAsync(Guid id);
    Task<Document> ArchiveDocumentAsync(Guid id);
    Task<Document> RestoreDocumentAsync(Guid id);
    Task<Stream> DownloadDocumentAsync(Guid id);
    Task<string> GetDocumentPreviewUrlAsync(Guid id);

    // Version Operations
    Task<IEnumerable<DocumentVersion>> GetDocumentVersionsAsync(Guid documentId);
    Task<DocumentVersion?> GetDocumentVersionAsync(Guid documentId, int version);
    Task<Stream> DownloadDocumentVersionAsync(Guid documentId, int version);
    Task<Document> RevertToVersionAsync(Guid documentId, int version);

    // Folder Operations
    Task<Folder> CreateFolderAsync(Folder folder);
    Task<Folder?> GetFolderByIdAsync(Guid id);
    Task<IEnumerable<Folder>> GetFoldersByParentAsync(Guid? parentId);
    Task<IEnumerable<Folder>> GetFolderTreeAsync(Guid? rootId);
    Task<Folder> UpdateFolderAsync(Folder folder);
    Task DeleteFolderAsync(Guid id, bool recursive = false);
    Task<Folder> MoveFolderAsync(Guid folderId, Guid? newParentId);

    // Permission Operations
    Task<DocumentPermission> GrantPermissionAsync(DocumentPermission permission);
    Task RevokePermissionAsync(Guid permissionId);
    Task<IEnumerable<DocumentPermission>> GetDocumentPermissionsAsync(Guid documentId);
    Task<bool> CheckPermissionAsync(Guid documentId, Guid userId, string permission);

    // Comment Operations
    Task<DocumentComment> AddCommentAsync(DocumentComment comment);
    Task<IEnumerable<DocumentComment>> GetDocumentCommentsAsync(Guid documentId);
    Task<DocumentComment> UpdateCommentAsync(DocumentComment comment);
    Task DeleteCommentAsync(Guid commentId);
    Task<DocumentComment> ResolveCommentAsync(Guid commentId, Guid resolvedBy);

    // Template Operations
    Task<DocumentTemplate> CreateTemplateAsync(DocumentTemplate template, Stream fileStream);
    Task<IEnumerable<DocumentTemplate>> GetTemplatesAsync(string? category = null);
    Task<Document> CreateDocumentFromTemplateAsync(Guid templateId, Dictionary<string, string> fieldValues);

    // Workflow Operations
    Task<DocumentWorkflow> StartWorkflowAsync(Guid documentId, Guid workflowTemplateId, Guid initiatedBy);
    Task<DocumentWorkflow?> GetWorkflowAsync(Guid workflowId);
    Task<IEnumerable<DocumentWorkflow>> GetDocumentWorkflowsAsync(Guid documentId);
    Task<WorkflowStep> ProcessWorkflowStepAsync(Guid stepId, WorkflowStepStatus status, string? comments, Guid userId);
    Task<DocumentWorkflow> CancelWorkflowAsync(Guid workflowId, string? reason);

    // OCR Operations
    Task<string> ExtractTextAsync(Guid documentId);
    Task ProcessOcrAsync(Guid documentId);

    // Statistics
    Task<DocumentStatistics> GetStatisticsAsync(string? department = null);
    Task<IEnumerable<Document>> GetRecentDocumentsAsync(Guid userId, int count = 10);
    Task<IEnumerable<Document>> GetPendingApprovalsAsync(Guid userId);
}

public class DocumentSearchCriteria
{
    public string? SearchText { get; set; }
    public DocumentType? Type { get; set; }
    public DocumentStatus? Status { get; set; }
    public string? Category { get; set; }
    public string? Department { get; set; }
    public Guid? OwnerId { get; set; }
    public SecurityLevel? SecurityLevel { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? Tags { get; set; }
    public bool IncludeArchived { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

public class DocumentStatistics
{
    public int TotalDocuments { get; set; }
    public int DocumentsByStatus { get; set; }
    public long TotalStorageUsed { get; set; }
    public int DocumentsThisMonth { get; set; }
    public int PendingApprovals { get; set; }
    public int ActiveWorkflows { get; set; }
    public Dictionary<string, int> DocumentsByType { get; set; } = new();
    public Dictionary<string, int> DocumentsByDepartment { get; set; } = new();
}
