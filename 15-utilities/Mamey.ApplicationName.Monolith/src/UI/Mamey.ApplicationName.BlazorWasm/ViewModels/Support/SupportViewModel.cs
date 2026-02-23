using System.Collections.ObjectModel;
using Mamey.ApplicationName.BlazorWasm.Models.Support;
using Mamey.ApplicationName.BlazorWasm.Services.Support;
using ReactiveUI;

namespace Mamey.ApplicationName.BlazorWasm.ViewModels.Support;

public class SupportViewModel : ReactiveObject
{
    public string SearchQuery { get; set; }
    public ObservableCollection<SupportTicket> Tickets { get; set; }
    public ObservableCollection<string> FAQs { get; set; }
    public ObservableCollection<string> KnowledgeBaseArticles { get; set; }

    private readonly SupportService _service;

    public SupportViewModel(SupportService service)
    {
        _service = service;
        Tickets = new ObservableCollection<SupportTicket>(_service.GenerateMockTickets());
        FAQs = new ObservableCollection<string>(_service.GenerateFAQs());
        KnowledgeBaseArticles = new ObservableCollection<string>(_service.GenerateKnowledgeBaseArticles());
    }

    public void AddTicket(SupportTicket ticket)
    {
        Tickets.Add(ticket);
    }
}