using Mamey.ApplicationName.BlazorWasm.Models.Support;

namespace Mamey.ApplicationName.BlazorWasm.Services.Support;

public class SupportService
{
    public List<SupportTicket> GenerateMockTickets()
    {
        return new List<SupportTicket>
        {
            new SupportTicket { Id = Guid.NewGuid(), Subject = "Login Issue", Description = "Cannot log into my account", Status = "Open", CreatedDate = DateTime.Now.AddDays(-1) },
            new SupportTicket { Id = Guid.NewGuid(), Subject = "Payment Error", Description = "Payment failed during checkout", Status = "In Progress", CreatedDate = DateTime.Now.AddDays(-3) }
        };
    }

    public List<string> GenerateFAQs()
    {
        return new List<string>
        {
            "How to reset my password?",
            "How to update my profile information?",
            "How to set up Two-Factor Authentication?"
        };
    }

    public List<string> GenerateKnowledgeBaseArticles()
    {
        return new List<string>
        {
            "Getting Started with Our Platform",
            "Security Best Practices",
            "How to Manage Your Account"
        };
    }
}