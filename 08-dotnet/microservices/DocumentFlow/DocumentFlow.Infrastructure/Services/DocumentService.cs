using DocumentFlow.Core.Interfaces;
using DocumentFlow.Core.Models;
using System.Security.Cryptography;

namespace DocumentFlow.Infrastructure.Services;

public class DocumentService : IDocumentService
{
    private readonly List<Document> _documents = new();
    private readonly List<DocumentVersion> _versions = new();
    private readonly List<Folder> _folders = new();
    private readonly List<DocumentComment> _comments = new();
    private readonly List<DocumentPermission> _permissions = new();
    private readonly List<DocumentWorkflow> _workflows = new();
    private readonly List<WorkflowStep> _workflowSteps = new();
    private readonly List<DocumentTemplate> _templates = new();

    public async Task<Document> CreateDocumentAsync(Document document, Stream fileStream)
    {
        document.Id = Guid.NewGuid();
        document.DocumentNumber = GenerateDocumentNumber();
        document.Version = 1;
        document.CreatedAt = DateTime.UtcNow;
        document.HashSHA256 = await ComputeHashAsync(fileStream);
        
        // Store file (in production, use Azure Blob Storage or similar)
        document.StoragePath = $"/documents/{document.Id}/{document.FileName}";
        
        _documents.Add(document);

        // Create initial version
        var version = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            VersionNumber = 1,
            FileName = document.FileName,
            StoragePath = document.StoragePath,
            FileSize = document.FileSize,
            HashSHA256 = document.HashSHA256,
            CreatedBy = document.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };
        _versions.Add(version);

        return await Task.FromResult(document);
    }

    public Task<Document?> GetDocumentByIdAsync(Guid id)
    {
        var document = _documents.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
        return Task.FromResult(document);
    }

    public Task<Document?> GetDocumentByNumberAsync(string documentNumber)
    {
        var document = _documents.FirstOrDefault(d => d.DocumentNumber == documentNumber && !d.IsDeleted);
        return Task.FromResult(document);
    }

    public Task<IEnumerable<Document>> GetDocumentsByFolderAsync(Guid folderId)
    {
        var documents = _documents.Where(d => !d.IsDeleted);
        return Task.FromResult(documents);
    }

    public Task<IEnumerable<Document>> GetDocumentsByDepartmentAsync(string department)
    {
        var documents = _documents.Where(d => d.Department == department && !d.IsDeleted);
        return Task.FromResult(documents);
    }

    public Task<IEnumerable<Document>> GetDocumentsByOwnerAsync(Guid ownerId)
    {
        var documents = _documents.Where(d => d.OwnerId == ownerId && !d.IsDeleted);
        return Task.FromResult(documents);
    }

    public Task<IEnumerable<Document>> SearchDocumentsAsync(DocumentSearchCriteria criteria)
    {
        var query = _documents.Where(d => !d.IsDeleted);

        if (!string.IsNullOrEmpty(criteria.SearchText))
        {
            var searchLower = criteria.SearchText.ToLower();
            query = query.Where(d => 
                d.Title.ToLower().Contains(searchLower) ||
                (d.Description?.ToLower().Contains(searchLower) ?? false) ||
                (d.OcrContent?.ToLower().Contains(searchLower) ?? false));
        }

        if (criteria.Type.HasValue)
            query = query.Where(d => d.Type == criteria.Type.Value);

        if (criteria.Status.HasValue)
            query = query.Where(d => d.Status == criteria.Status.Value);

        if (!string.IsNullOrEmpty(criteria.Department))
            query = query.Where(d => d.Department == criteria.Department);

        if (criteria.OwnerId.HasValue)
            query = query.Where(d => d.OwnerId == criteria.OwnerId.Value);

        if (criteria.CreatedFrom.HasValue)
            query = query.Where(d => d.CreatedAt >= criteria.CreatedFrom.Value);

        if (criteria.CreatedTo.HasValue)
            query = query.Where(d => d.CreatedAt <= criteria.CreatedTo.Value);

        if (!criteria.IncludeArchived)
            query = query.Where(d => !d.IsArchived);

        var results = query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize);

        return Task.FromResult(results);
    }

    public async Task<Document> UpdateDocumentAsync(Document document)
    {
        var existing = _documents.FirstOrDefault(d => d.Id == document.Id);
        if (existing != null)
        {
            existing.Title = document.Title;
            existing.Description = document.Description;
            existing.Category = document.Category;
            existing.Tags = document.Tags;
            existing.Metadata = document.Metadata;
            existing.SecurityLevel = document.SecurityLevel;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.LastModifiedBy = document.LastModifiedBy;
        }
        return await Task.FromResult(existing ?? document);
    }

    public async Task<Document> UpdateDocumentFileAsync(Guid id, Stream fileStream, string? changeNotes)
    {
        var document = _documents.FirstOrDefault(d => d.Id == id);
        if (document == null) throw new Exception("Document not found");

        document.Version++;
        document.HashSHA256 = await ComputeHashAsync(fileStream);
        document.UpdatedAt = DateTime.UtcNow;

        var version = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            VersionNumber = document.Version,
            FileName = document.FileName,
            StoragePath = $"/documents/{document.Id}/v{document.Version}/{document.FileName}",
            FileSize = document.FileSize,
            HashSHA256 = document.HashSHA256,
            ChangeNotes = changeNotes,
            CreatedBy = document.LastModifiedBy ?? document.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };
        _versions.Add(version);

        return document;
    }

    public Task DeleteDocumentAsync(Guid id)
    {
        var document = _documents.FirstOrDefault(d => d.Id == id);
        if (document != null)
        {
            document.IsDeleted = true;
            document.UpdatedAt = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }

    public Task<Document> ArchiveDocumentAsync(Guid id)
    {
        var document = _documents.FirstOrDefault(d => d.Id == id);
        if (document != null)
        {
            document.IsArchived = true;
            document.Status = DocumentStatus.Archived;
            document.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(document!);
    }

    public Task<Document> RestoreDocumentAsync(Guid id)
    {
        var document = _documents.FirstOrDefault(d => d.Id == id);
        if (document != null)
        {
            document.IsArchived = false;
            document.IsDeleted = false;
            document.Status = DocumentStatus.Draft;
            document.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(document!);
    }

    public Task<Stream> DownloadDocumentAsync(Guid id)
    {
        // In production, retrieve from storage
        return Task.FromResult<Stream>(new MemoryStream());
    }

    public Task<string> GetDocumentPreviewUrlAsync(Guid id)
    {
        return Task.FromResult($"/api/documents/{id}/preview");
    }

    public Task<IEnumerable<DocumentVersion>> GetDocumentVersionsAsync(Guid documentId)
    {
        var versions = _versions.Where(v => v.DocumentId == documentId).OrderByDescending(v => v.VersionNumber);
        return Task.FromResult(versions.AsEnumerable());
    }

    public Task<DocumentVersion?> GetDocumentVersionAsync(Guid documentId, int version)
    {
        var ver = _versions.FirstOrDefault(v => v.DocumentId == documentId && v.VersionNumber == version);
        return Task.FromResult(ver);
    }

    public Task<Stream> DownloadDocumentVersionAsync(Guid documentId, int version)
    {
        return Task.FromResult<Stream>(new MemoryStream());
    }

    public async Task<Document> RevertToVersionAsync(Guid documentId, int version)
    {
        var document = _documents.FirstOrDefault(d => d.Id == documentId);
        var targetVersion = _versions.FirstOrDefault(v => v.DocumentId == documentId && v.VersionNumber == version);
        
        if (document != null && targetVersion != null)
        {
            document.Version++;
            document.UpdatedAt = DateTime.UtcNow;
            
            var newVersion = new DocumentVersion
            {
                Id = Guid.NewGuid(),
                DocumentId = documentId,
                VersionNumber = document.Version,
                FileName = targetVersion.FileName,
                StoragePath = targetVersion.StoragePath,
                FileSize = targetVersion.FileSize,
                HashSHA256 = targetVersion.HashSHA256,
                ChangeNotes = $"Reverted to version {version}",
                CreatedAt = DateTime.UtcNow
            };
            _versions.Add(newVersion);
        }
        
        return await Task.FromResult(document!);
    }

    public Task<Folder> CreateFolderAsync(Folder folder)
    {
        folder.Id = Guid.NewGuid();
        folder.CreatedAt = DateTime.UtcNow;
        folder.Path = BuildFolderPath(folder);
        _folders.Add(folder);
        return Task.FromResult(folder);
    }

    public Task<Folder?> GetFolderByIdAsync(Guid id)
    {
        var folder = _folders.FirstOrDefault(f => f.Id == id);
        return Task.FromResult(folder);
    }

    public Task<IEnumerable<Folder>> GetFoldersByParentAsync(Guid? parentId)
    {
        var folders = _folders.Where(f => f.ParentFolderId == parentId);
        return Task.FromResult(folders);
    }

    public Task<IEnumerable<Folder>> GetFolderTreeAsync(Guid? rootId)
    {
        var folders = _folders.Where(f => f.ParentFolderId == rootId);
        return Task.FromResult(folders);
    }

    public Task<Folder> UpdateFolderAsync(Folder folder)
    {
        var existing = _folders.FirstOrDefault(f => f.Id == folder.Id);
        if (existing != null)
        {
            existing.Name = folder.Name;
            existing.Description = folder.Description;
            existing.Color = folder.Color;
            existing.Icon = folder.Icon;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(existing ?? folder);
    }

    public Task DeleteFolderAsync(Guid id, bool recursive = false)
    {
        var folder = _folders.FirstOrDefault(f => f.Id == id);
        if (folder != null)
        {
            _folders.Remove(folder);
        }
        return Task.CompletedTask;
    }

    public Task<Folder> MoveFolderAsync(Guid folderId, Guid? newParentId)
    {
        var folder = _folders.FirstOrDefault(f => f.Id == folderId);
        if (folder != null)
        {
            folder.ParentFolderId = newParentId;
            folder.Path = BuildFolderPath(folder);
            folder.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(folder!);
    }

    public Task<DocumentPermission> GrantPermissionAsync(DocumentPermission permission)
    {
        permission.Id = Guid.NewGuid();
        permission.GrantedAt = DateTime.UtcNow;
        _permissions.Add(permission);
        return Task.FromResult(permission);
    }

    public Task RevokePermissionAsync(Guid permissionId)
    {
        var permission = _permissions.FirstOrDefault(p => p.Id == permissionId);
        if (permission != null) _permissions.Remove(permission);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<DocumentPermission>> GetDocumentPermissionsAsync(Guid documentId)
    {
        var permissions = _permissions.Where(p => p.DocumentId == documentId);
        return Task.FromResult(permissions);
    }

    public Task<bool> CheckPermissionAsync(Guid documentId, Guid userId, string permission)
    {
        var perm = _permissions.FirstOrDefault(p => p.DocumentId == documentId && p.UserId == userId);
        if (perm == null) return Task.FromResult(false);

        return permission.ToLower() switch
        {
            "view" => Task.FromResult(perm.CanView),
            "edit" => Task.FromResult(perm.CanEdit),
            "delete" => Task.FromResult(perm.CanDelete),
            "share" => Task.FromResult(perm.CanShare),
            "download" => Task.FromResult(perm.CanDownload),
            _ => Task.FromResult(false)
        };
    }

    public Task<DocumentComment> AddCommentAsync(DocumentComment comment)
    {
        comment.Id = Guid.NewGuid();
        comment.CreatedAt = DateTime.UtcNow;
        _comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task<IEnumerable<DocumentComment>> GetDocumentCommentsAsync(Guid documentId)
    {
        var comments = _comments.Where(c => c.DocumentId == documentId);
        return Task.FromResult(comments);
    }

    public Task<DocumentComment> UpdateCommentAsync(DocumentComment comment)
    {
        var existing = _comments.FirstOrDefault(c => c.Id == comment.Id);
        if (existing != null)
        {
            existing.Content = comment.Content;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(existing ?? comment);
    }

    public Task DeleteCommentAsync(Guid commentId)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == commentId);
        if (comment != null) _comments.Remove(comment);
        return Task.CompletedTask;
    }

    public Task<DocumentComment> ResolveCommentAsync(Guid commentId, Guid resolvedBy)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == commentId);
        if (comment != null)
        {
            comment.IsResolved = true;
            comment.ResolvedBy = resolvedBy;
            comment.ResolvedAt = DateTime.UtcNow;
        }
        return Task.FromResult(comment!);
    }

    public Task<DocumentTemplate> CreateTemplateAsync(DocumentTemplate template, Stream fileStream)
    {
        template.Id = Guid.NewGuid();
        template.CreatedAt = DateTime.UtcNow;
        _templates.Add(template);
        return Task.FromResult(template);
    }

    public Task<IEnumerable<DocumentTemplate>> GetTemplatesAsync(string? category = null)
    {
        var templates = _templates.Where(t => t.IsActive);
        if (!string.IsNullOrEmpty(category))
            templates = templates.Where(t => t.Category == category);
        return Task.FromResult(templates);
    }

    public async Task<Document> CreateDocumentFromTemplateAsync(Guid templateId, Dictionary<string, string> fieldValues)
    {
        var template = _templates.FirstOrDefault(t => t.Id == templateId);
        if (template == null) throw new Exception("Template not found");

        var document = new Document
        {
            Title = template.Name,
            Type = template.DocumentType,
            Category = template.Category,
            Department = template.Department,
            FileName = template.FileName,
            ContentType = template.ContentType,
            Status = DocumentStatus.Draft
        };

        return await CreateDocumentAsync(document, new MemoryStream());
    }

    public Task<DocumentWorkflow> StartWorkflowAsync(Guid documentId, Guid workflowTemplateId, Guid initiatedBy)
    {
        var workflow = new DocumentWorkflow
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            WorkflowTemplateId = workflowTemplateId,
            Status = WorkflowStatus.InProgress,
            CurrentStep = 1,
            InitiatedBy = initiatedBy,
            StartedAt = DateTime.UtcNow
        };
        _workflows.Add(workflow);
        return Task.FromResult(workflow);
    }

    public Task<DocumentWorkflow?> GetWorkflowAsync(Guid workflowId)
    {
        var workflow = _workflows.FirstOrDefault(w => w.Id == workflowId);
        return Task.FromResult(workflow);
    }

    public Task<IEnumerable<DocumentWorkflow>> GetDocumentWorkflowsAsync(Guid documentId)
    {
        var workflows = _workflows.Where(w => w.DocumentId == documentId);
        return Task.FromResult(workflows);
    }

    public Task<WorkflowStep> ProcessWorkflowStepAsync(Guid stepId, WorkflowStepStatus status, string? comments, Guid userId)
    {
        var step = _workflowSteps.FirstOrDefault(s => s.Id == stepId);
        if (step != null)
        {
            step.Status = status;
            step.Comments = comments;
            step.CompletedBy = userId;
            step.CompletedAt = DateTime.UtcNow;
        }
        return Task.FromResult(step!);
    }

    public Task<DocumentWorkflow> CancelWorkflowAsync(Guid workflowId, string? reason)
    {
        var workflow = _workflows.FirstOrDefault(w => w.Id == workflowId);
        if (workflow != null)
        {
            workflow.Status = WorkflowStatus.Cancelled;
            workflow.Comments = reason;
        }
        return Task.FromResult(workflow!);
    }

    public Task<string> ExtractTextAsync(Guid documentId)
    {
        return Task.FromResult("OCR extracted text would appear here");
    }

    public Task ProcessOcrAsync(Guid documentId)
    {
        var document = _documents.FirstOrDefault(d => d.Id == documentId);
        if (document != null)
        {
            document.OcrContent = "OCR processed content";
            document.UpdatedAt = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }

    public Task<DocumentStatistics> GetStatisticsAsync(string? department = null)
    {
        var query = _documents.Where(d => !d.IsDeleted);
        if (!string.IsNullOrEmpty(department))
            query = query.Where(d => d.Department == department);

        return Task.FromResult(new DocumentStatistics
        {
            TotalDocuments = query.Count(),
            TotalStorageUsed = query.Sum(d => d.FileSize),
            PendingApprovals = query.Count(d => d.Status == DocumentStatus.PendingReview),
            ActiveWorkflows = _workflows.Count(w => w.Status == WorkflowStatus.InProgress),
            DocumentsByType = query.GroupBy(d => d.Type.ToString()).ToDictionary(g => g.Key, g => g.Count())
        });
    }

    public Task<IEnumerable<Document>> GetRecentDocumentsAsync(Guid userId, int count = 10)
    {
        var documents = _documents
            .Where(d => !d.IsDeleted && (d.OwnerId == userId || d.CreatedBy == userId))
            .OrderByDescending(d => d.UpdatedAt ?? d.CreatedAt)
            .Take(count);
        return Task.FromResult(documents);
    }

    public Task<IEnumerable<Document>> GetPendingApprovalsAsync(Guid userId)
    {
        var documents = _documents.Where(d => d.Status == DocumentStatus.PendingReview && !d.IsDeleted);
        return Task.FromResult(documents);
    }

    private string GenerateDocumentNumber()
    {
        return $"DOC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    private async Task<string> ComputeHashAsync(Stream stream)
    {
        using var sha256 = SHA256.Create();
        var hash = await sha256.ComputeHashAsync(stream);
        stream.Position = 0;
        return Convert.ToHexString(hash);
    }

    private string BuildFolderPath(Folder folder)
    {
        if (folder.ParentFolderId == null)
            return $"/{folder.Name}";

        var parent = _folders.FirstOrDefault(f => f.Id == folder.ParentFolderId);
        return parent != null ? $"{parent.Path}/{folder.Name}" : $"/{folder.Name}";
    }
}
