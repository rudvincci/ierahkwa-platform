namespace ServiceDesk.Core.Models;

public class Ticket
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketType Type { get; set; }
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketSource Source { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public string? Department { get; set; }
    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public Guid? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public Guid? AssignedGroup { get; set; }
    public string? AssignedGroupName { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? FirstResponseAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? Resolution { get; set; }
    public string? Tags { get; set; }
    public bool SlaBreached { get; set; }
    public int? SatisfactionRating { get; set; }
    public string? SatisfactionComment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<TicketComment> Comments { get; set; } = new();
    public List<TicketAttachment> Attachments { get; set; } = new();
    public List<TicketActivity> Activities { get; set; } = new();
}

public class TicketComment
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public bool IsFromRequester { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TicketAttachment
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class TicketActivity
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public Guid PerformedBy { get; set; }
    public string PerformedByName { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
}

public class KnowledgeArticle
{
    public Guid Id { get; set; }
    public string ArticleNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public ArticleStatus Status { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int HelpfulCount { get; set; }
    public int NotHelpfulCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class SupportAgent
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? GroupId { get; set; }
    public string? GroupName { get; set; }
    public AgentStatus Status { get; set; }
    public int OpenTickets { get; set; }
    public int ResolvedToday { get; set; }
    public double AverageRating { get; set; }
    public List<string> Skills { get; set; } = new();
}

public class SupportGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Department { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public int AgentCount { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SLAPolicy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TicketPriority Priority { get; set; }
    public int FirstResponseMinutes { get; set; }
    public int ResolutionMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EscalateOnBreach { get; set; }
    public Guid? EscalateToId { get; set; }
}

public enum TicketType { Incident, ServiceRequest, Problem, Change, Question }
public enum TicketStatus { New, Open, Pending, OnHold, Resolved, Closed, Cancelled }
public enum TicketPriority { Low, Medium, High, Urgent, Critical }
public enum TicketSource { Portal, Email, Phone, Chat, API, Walk_In }
public enum ArticleStatus { Draft, InReview, Published, Archived }
public enum AgentStatus { Available, Busy, Away, Offline }
