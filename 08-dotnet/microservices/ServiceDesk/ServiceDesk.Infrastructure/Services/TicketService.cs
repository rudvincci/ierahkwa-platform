using ServiceDesk.Core.Interfaces;
using ServiceDesk.Core.Models;

namespace ServiceDesk.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly List<Ticket> _tickets = new();
    private readonly List<TicketComment> _comments = new();
    private readonly List<TicketAttachment> _attachments = new();
    private readonly List<KnowledgeArticle> _articles = new();
    private readonly List<SupportAgent> _agents = new();
    private readonly List<SupportGroup> _groups = new();
    private readonly List<SLAPolicy> _slas = new();

    public Task<Ticket> CreateTicketAsync(Ticket ticket)
    {
        ticket.Id = Guid.NewGuid();
        ticket.TicketNumber = $"TKT-{DateTime.UtcNow:yyyyMMdd}-{_tickets.Count + 1:D5}";
        ticket.Status = TicketStatus.New;
        ticket.CreatedAt = DateTime.UtcNow;
        _tickets.Add(ticket);
        return Task.FromResult(ticket);
    }

    public Task<Ticket?> GetTicketByIdAsync(Guid id) => Task.FromResult(_tickets.FirstOrDefault(t => t.Id == id));
    public Task<Ticket?> GetTicketByNumberAsync(string ticketNumber) => Task.FromResult(_tickets.FirstOrDefault(t => t.TicketNumber == ticketNumber));

    public Task<IEnumerable<Ticket>> GetTicketsAsync(TicketStatus? status = null, TicketPriority? priority = null, string? department = null, int page = 1, int pageSize = 20)
    {
        var q = _tickets.AsEnumerable();
        if (status.HasValue) q = q.Where(t => t.Status == status.Value);
        if (priority.HasValue) q = q.Where(t => t.Priority == priority.Value);
        if (!string.IsNullOrEmpty(department)) q = q.Where(t => t.Department == department);
        return Task.FromResult(q.OrderByDescending(t => t.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize));
    }

    public Task<IEnumerable<Ticket>> GetRequesterTicketsAsync(Guid requesterId) => Task.FromResult(_tickets.Where(t => t.RequesterId == requesterId));
    public Task<IEnumerable<Ticket>> GetAssignedTicketsAsync(Guid agentId) => Task.FromResult(_tickets.Where(t => t.AssignedTo == agentId));
    public Task<IEnumerable<Ticket>> GetGroupTicketsAsync(Guid groupId) => Task.FromResult(_tickets.Where(t => t.AssignedGroup == groupId));

    public Task<Ticket> UpdateTicketAsync(Ticket ticket) { var e = _tickets.FirstOrDefault(t => t.Id == ticket.Id); if (e != null) { e.Subject = ticket.Subject; e.Priority = ticket.Priority; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? ticket); }

    public Task<Ticket> AssignTicketAsync(Guid ticketId, Guid? agentId, Guid? groupId)
    {
        var t = _tickets.FirstOrDefault(t => t.Id == ticketId);
        if (t != null) { t.AssignedTo = agentId; t.AssignedGroup = groupId; t.Status = TicketStatus.Open; t.UpdatedAt = DateTime.UtcNow; }
        return Task.FromResult(t!);
    }

    public Task<Ticket> UpdateStatusAsync(Guid ticketId, TicketStatus status)
    {
        var t = _tickets.FirstOrDefault(t => t.Id == ticketId);
        if (t != null) { t.Status = status; t.UpdatedAt = DateTime.UtcNow; if (status == TicketStatus.Closed) t.ClosedAt = DateTime.UtcNow; }
        return Task.FromResult(t!);
    }

    public Task<Ticket> ResolveTicketAsync(Guid ticketId, string resolution)
    {
        var t = _tickets.FirstOrDefault(t => t.Id == ticketId);
        if (t != null) { t.Status = TicketStatus.Resolved; t.Resolution = resolution; t.ResolvedAt = DateTime.UtcNow; t.UpdatedAt = DateTime.UtcNow; }
        return Task.FromResult(t!);
    }

    public Task<Ticket> ReopenTicketAsync(Guid ticketId)
    {
        var t = _tickets.FirstOrDefault(t => t.Id == ticketId);
        if (t != null) { t.Status = TicketStatus.Open; t.ResolvedAt = null; t.ClosedAt = null; t.UpdatedAt = DateTime.UtcNow; }
        return Task.FromResult(t!);
    }

    public Task DeleteTicketAsync(Guid id) { _tickets.RemoveAll(t => t.Id == id); return Task.CompletedTask; }

    public Task<TicketComment> AddCommentAsync(TicketComment comment)
    {
        comment.Id = Guid.NewGuid(); comment.CreatedAt = DateTime.UtcNow; _comments.Add(comment);
        var t = _tickets.FirstOrDefault(t => t.Id == comment.TicketId);
        if (t != null && t.FirstResponseAt == null && !comment.IsFromRequester) t.FirstResponseAt = DateTime.UtcNow;
        return Task.FromResult(comment);
    }

    public Task<IEnumerable<TicketComment>> GetCommentsAsync(Guid ticketId) => Task.FromResult(_comments.Where(c => c.TicketId == ticketId).OrderBy(c => c.CreatedAt));
    public Task<TicketAttachment> AddAttachmentAsync(TicketAttachment attachment) { attachment.Id = Guid.NewGuid(); attachment.UploadedAt = DateTime.UtcNow; _attachments.Add(attachment); return Task.FromResult(attachment); }

    public Task<KnowledgeArticle> CreateArticleAsync(KnowledgeArticle article) { article.Id = Guid.NewGuid(); article.ArticleNumber = $"KB-{_articles.Count + 1:D5}"; article.Status = ArticleStatus.Draft; article.CreatedAt = DateTime.UtcNow; _articles.Add(article); return Task.FromResult(article); }
    public Task<KnowledgeArticle?> GetArticleByIdAsync(Guid id) => Task.FromResult(_articles.FirstOrDefault(a => a.Id == id));
    public Task<IEnumerable<KnowledgeArticle>> SearchArticlesAsync(string? query, string? category = null)
    {
        var q = _articles.Where(a => a.Status == ArticleStatus.Published);
        if (!string.IsNullOrEmpty(query)) q = q.Where(a => a.Title.Contains(query, StringComparison.OrdinalIgnoreCase) || a.Content.Contains(query, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(category)) q = q.Where(a => a.Category == category);
        return Task.FromResult(q);
    }
    public Task<KnowledgeArticle> UpdateArticleAsync(KnowledgeArticle article) { var e = _articles.FirstOrDefault(a => a.Id == article.Id); if (e != null) { e.Title = article.Title; e.Content = article.Content; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? article); }
    public Task<KnowledgeArticle> PublishArticleAsync(Guid id) { var a = _articles.FirstOrDefault(a => a.Id == id); if (a != null) { a.Status = ArticleStatus.Published; a.PublishedAt = DateTime.UtcNow; } return Task.FromResult(a!); }
    public Task IncrementArticleViewAsync(Guid id) { var a = _articles.FirstOrDefault(a => a.Id == id); if (a != null) a.ViewCount++; return Task.CompletedTask; }
    public Task RateArticleAsync(Guid id, bool helpful) { var a = _articles.FirstOrDefault(a => a.Id == id); if (a != null) { if (helpful) a.HelpfulCount++; else a.NotHelpfulCount++; } return Task.CompletedTask; }

    public Task<SupportAgent> CreateAgentAsync(SupportAgent agent) { agent.Id = Guid.NewGuid(); agent.Status = AgentStatus.Available; _agents.Add(agent); return Task.FromResult(agent); }
    public Task<IEnumerable<SupportAgent>> GetAgentsAsync(Guid? groupId = null) => Task.FromResult(groupId.HasValue ? _agents.Where(a => a.GroupId == groupId) : _agents.AsEnumerable());
    public Task<SupportAgent> UpdateAgentStatusAsync(Guid agentId, AgentStatus status) { var a = _agents.FirstOrDefault(a => a.Id == agentId); if (a != null) a.Status = status; return Task.FromResult(a!); }

    public Task<SupportGroup> CreateGroupAsync(SupportGroup group) { group.Id = Guid.NewGuid(); _groups.Add(group); return Task.FromResult(group); }
    public Task<IEnumerable<SupportGroup>> GetGroupsAsync() => Task.FromResult(_groups.AsEnumerable());

    public Task<SLAPolicy> CreateSLAAsync(SLAPolicy policy) { policy.Id = Guid.NewGuid(); _slas.Add(policy); return Task.FromResult(policy); }
    public Task<IEnumerable<SLAPolicy>> GetSLAsAsync() => Task.FromResult(_slas.AsEnumerable());
    public Task CheckSLABreachAsync(Guid ticketId) { var t = _tickets.FirstOrDefault(t => t.Id == ticketId); if (t != null && t.DueDate.HasValue && DateTime.UtcNow > t.DueDate) t.SlaBreached = true; return Task.CompletedTask; }

    public Task<ServiceDeskStatistics> GetStatisticsAsync(string? department = null)
    {
        var tickets = string.IsNullOrEmpty(department) ? _tickets : _tickets.Where(t => t.Department == department).ToList();
        return Task.FromResult(new ServiceDeskStatistics
        {
            TotalTickets = tickets.Count, OpenTickets = tickets.Count(t => t.Status == TicketStatus.Open || t.Status == TicketStatus.New),
            ResolvedToday = tickets.Count(t => t.ResolvedAt?.Date == DateTime.UtcNow.Date),
            OverdueTickets = tickets.Count(t => t.SlaBreached),
            AverageSatisfactionRating = tickets.Where(t => t.SatisfactionRating.HasValue).DefaultIfEmpty().Average(t => t?.SatisfactionRating ?? 0),
            TotalAgents = _agents.Count, AvailableAgents = _agents.Count(a => a.Status == AgentStatus.Available),
            TotalArticles = _articles.Count(a => a.Status == ArticleStatus.Published),
            TicketsByStatus = tickets.GroupBy(t => t.Status.ToString()).ToDictionary(g => g.Key, g => g.Count()),
            TicketsByPriority = tickets.GroupBy(t => t.Priority.ToString()).ToDictionary(g => g.Key, g => g.Count())
        });
    }
}
