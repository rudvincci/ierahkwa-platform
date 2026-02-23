using System.Collections.ObjectModel;
using Mamey.ApplicationName.BlazorWasm.Models.Statements;
using Mamey.ApplicationName.BlazorWasm.Services.Statements;
using MudBlazor;
using ReactiveUI;

namespace Mamey.ApplicationName.BlazorWasm.ViewModels.Statements;

public class StatementsViewModel : ReactiveObject
{
    public string SearchQuery { get; set; }
    public DateRange? DateRange { get; set; }
    public ObservableCollection<Statement> Statements { get; set; }
    public ObservableCollection<Document> Documents { get; set; }

    private StatementService _service = new();

    public StatementsViewModel(StatementService statementService)
    {
        Statements = new ObservableCollection<Statement>(_service.GenerateMockStatements());
        Documents = new ObservableCollection<Document>(_service.GenerateMockDocuments());
    }

    public IEnumerable<Statement> FilteredStatements =>
        string.IsNullOrEmpty(SearchQuery)
            ? Statements
            : Statements.Where(s => s.AccountName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

    public void ShowGenerateStatementDialog() { /* Logic to show dialog */ }
    public void ShowUploadDocumentDialog() { /* Logic to show dialog */ }
}