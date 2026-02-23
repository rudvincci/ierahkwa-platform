using ServiceDesk.Core.Models;

namespace ServiceDesk.Core.Interfaces;

public interface ITicketService
{
    Task<Ticket> CreateTicketAsync(Ticket ticket);
    Task<Ticket?> GetTicketByIdAsync(Guid id);
    Task<Ticket?> GetTicketByNumberAsync(string ticketNumber);
    Task<IEnumerable<Ticket>> GetTicketsAsync(TicketStatus? status = null, TicketPriority? priority = null, string? department = null, int page = 1, int pageSize = 20);
    Task<IEnumerable<Ticket>> GetRequesterTicketsAsync(Guid requesterId);
    Task<IEnumerable<Ticket>> GetAssignedTicketsAsync(Guid agentId);
    Task<IEnumerable<Ticket>> GetGroupTicketsAsync(Guid groupId);
    Task<Ticket> UpdateTicketAsync(Ticket ticket);
    Task<Ticket> AssignTicketAsync(Guid ticketId, Guid? agentId, Guid? groupId);
    Task<Ticket> UpdateStatusAsync(Guid ticketId, TicketStatus status);
    Task<Ticket> ResolveTicketAsync(Guid ticketId, string resolution);
    Task<Ticket> ReopenTicketAsync(Guid ticketId);
    Task DeleteTicketAsync(Guid id);

    Task<TicketComment> AddCommentAsync(TicketComment comment);
    Task<IEnumerable<TicketComment>> GetCommentsAsync(Guid ticketId);
    Task<TicketAttachment> AddAttachmentAsync(TicketAttachment attachment);

    Task<KnowledgeArticle> CreateArticleAsync(KnowledgeArticle article);
    Task<KnowledgeArticle?> GetArticleByIdAsync(Guid id);
    Task<IEnumerable<KnowledgeArticle>> SearchArticlesAsync(string? query, string? category = null);
    Task<KnowledgeArticle> UpdateArticleAsync(KnowledgeArticle article);
    Task<KnowledgeArticle> PublishArticleAsync(Guid id);
    Task IncrementArticleViewAsync(Guid id);
    Task RateArticleAsync(Guid id, bool helpful);

    Task<SupportAgent> CreateAgentAsync(SupportAgent agent);
    Task<IEnumerable<SupportAgent>> GetAgentsAsync(Guid? groupId = null);
    Task<SupportAgent> UpdateAgentStatusAsync(Guid agentId, AgentStatus status);

    Task<SupportGroup> CreateGroupAsync(SupportGroup group);
    Task<IEnumerable<SupportGroup>> GetGroupsAsync();

    Task<SLAPolicy> CreateSLAAsync(SLAPolicy policy);
    Task<IEnumerable<SLAPolicy>> GetSLAsAsync();
    Task CheckSLABreachAsync(Guid ticketId);

    Task<ServiceDeskStatistics> GetStatisticsAsync(string? department = null);
}

public class ServiceDeskStatistics
{
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int ResolvedToday { get; set; }
    public int OverdueTickets { get; set; }
    public double AverageResolutionHours { get; set; }
    public double AverageFirstResponseMinutes { get; set; }
    public double SLACompliancePercent { get; set; }
    public double AverageSatisfactionRating { get; set; }
    public int TotalAgents { get; set; }
    public int AvailableAgents { get; set; }
    public int TotalArticles { get; set; }
    public Dictionary<string, int> TicketsByStatus { get; set; } = new();
    public Dictionary<string, int> TicketsByPriority { get; set; } = new();
    public Dictionary<string, int> TicketsByCategory { get; set; } = new();
}
