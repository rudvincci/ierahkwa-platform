namespace Mamey.Adobe;

public interface IPdfServices
{
    Task<Stream?> ConvertWordToPdfAsync(string filepath);
    Task SealPdf();
    Task ExportPdfToWord();
    Task GetPdfProperties();
    Task MergeDocumentToPdf();
}
