using Mamey.ApplicationName.BlazorWasm.Models.Statements;

namespace Mamey.ApplicationName.BlazorWasm.Services.Statements;

public class StatementService
{
    public List<Statement> GenerateMockStatements()
    {
        return new List<Statement>
        {
            new Statement { Id = Guid.NewGuid(), AccountName = "Checking Account", Date = DateTime.Today.AddMonths(-1), Type = "Monthly", Format = "PDF", Amount = 1200.50m },
            new Statement { Id = Guid.NewGuid(), AccountName = "Savings Account", Date = DateTime.Today.AddMonths(-2), Type = "Monthly", Format = "CSV", Amount = 800.25m }
        };
    }

    public List<Document> GenerateMockDocuments()
    {
        return new List<Document>
        {
            new Document { Id = Guid.NewGuid(), FileName = "KYC_Document.pdf", Category = "KYC", UploadedDate = DateTime.Now.AddDays(-10), Description = "Verification Document" },
            new Document { Id = Guid.NewGuid(), FileName = "Tax_Form_2023.pdf", Category = "Tax", UploadedDate = DateTime.Now.AddDays(-30), Description = "Annual Tax Form" }
        };
    }
}