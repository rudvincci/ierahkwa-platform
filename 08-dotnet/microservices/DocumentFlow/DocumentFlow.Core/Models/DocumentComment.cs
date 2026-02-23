namespace DocumentFlow.Core.Models;

public class DocumentComment
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public CommentType Type { get; set; }
    public int? PageNumber { get; set; }
    public string? Coordinates { get; set; }
    public Guid CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public bool IsResolved { get; set; }
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Document? Document { get; set; }
    public List<DocumentComment> Replies { get; set; } = new();
}

public enum CommentType
{
    General,
    Annotation,
    Suggestion,
    Question,
    Correction,
    Approval
}
