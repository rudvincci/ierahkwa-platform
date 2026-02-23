using Mamey.Barcode.Http;
using Spire.Doc.Fields;
using Document = Spire.Doc.Document;

namespace Mamey.Word;

public class WordService : IWordService
{
    private readonly IMameyBarcodeApiClient _barcodeApiClient;

    public WordService(IMameyBarcodeApiClient barcodeApiClient)
    {
        _barcodeApiClient = barcodeApiClient;
    }
    //public async Task<Stream?> GenerateWordStreamAsync<T>(T templateModel, string assemblyName, string outputFilePath = null) where T : IWordTemplateModel
    //{
    //    if (string.IsNullOrEmpty(templateModel.Resource))
    //    {
    //        throw new ArgumentException($"'{nameof(templateModel.Resource)}' cannot be null or empty.", nameof(templateModel.Resource));
    //    }

    //    if (string.IsNullOrEmpty(outputFilePath))
    //    {
    //        throw new ArgumentException($"'{nameof(outputFilePath)}' cannot be null or empty.", nameof(outputFilePath));
    //    }

    //    var generatedDocument = await GenerateWordTemplateAsync<T>(templateModel, assemblyName);
    //        generatedDocument.SaveToPath(outputFilePath);
    //    //MemoryStream stream = new MemoryStream();
    //    //using (MemoryStream stream = new MemoryStream())
    //    //{

    //        stream.Position = 0;
    //        // Save the document to the MemoryStream

    //        // Optionally, you can reset the stream position if needed for further operations

    //        // Further operations with the stream can be done here
    //        //return stream;
    //    //}
    //}

    /// <summary>
    /// Existing generic path using embedded resource + reflection replacement.
    /// </summary>
    public async Task<string> GenerateWordToPathAsync<T>(T templateModel, string assemblyName,
        string? outputFilePath = null)
        where T: IWordTemplateModel
    {
        if (templateModel is null) throw new ArgumentNullException(nameof(templateModel));
        if (string.IsNullOrWhiteSpace(templateModel.Resource))
            throw new ArgumentException($"'{nameof(templateModel.Resource)}' cannot be null or empty.", nameof(templateModel.Resource));
        // if (string.IsNullOrWhiteSpace(outputFilePath))
        //     throw new ArgumentException($"'{nameof(outputFilePath)}' cannot be null or empty.", nameof(outputFilePath));

        var directory = string.IsNullOrEmpty(outputFilePath) ? Environment.CurrentDirectory : outputFilePath;
        
        var dirInfo = new FileInfo(directory);
        if (!Directory.Exists(dirInfo.DirectoryName))
        {
            Directory.CreateDirectory(dirInfo.DirectoryName);
        }

        var generatedDocument = await GenerateWordTemplateAsync<T>(templateModel, assemblyName);

        //new FileInfo(outputFilePath).Directory.Create();
        generatedDocument.SaveToPath(dirInfo.FullName);

        return await Task.FromResult(dirInfo.FullName);
    }
    
    public async Task<string> GenerateWordToPathAsync(
        string resource,
        string assemblyName,
        IDictionary<string, object> values,
        WordDocumentProperties props,
        Dictionary<string, byte[]>? imageSelectors,
        string outputFilePath)
    {
        if (string.IsNullOrWhiteSpace(resource)) throw new ArgumentException("Required", nameof(resource));
        if (string.IsNullOrWhiteSpace(assemblyName)) throw new ArgumentException("Required", nameof(assemblyName));
        if (string.IsNullOrWhiteSpace(outputFilePath)) throw new ArgumentException("Required", nameof(outputFilePath));

        var dir = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

        var documentTemplate = WordProcessor.LoadDocument(resource, assemblyName);

        // built-in properties
        documentTemplate.BuiltinDocumentProperties.Title = props.Title;
        documentTemplate.BuiltinDocumentProperties.Author = props.Author;
        documentTemplate.BuiltinDocumentProperties.Company = props.Company;
        documentTemplate.BuiltinDocumentProperties.Keywords = props.Keywords;
        documentTemplate.BuiltinDocumentProperties.Subject = props.Subject;
        documentTemplate.BuiltinDocumentProperties.Category = props.Category;
        if (!string.IsNullOrEmpty(props.Comments))
            documentTemplate.BuiltinDocumentProperties.Comments = props.Comments;
        documentTemplate.BuiltinDocumentProperties.CreateDate = DateTime.Today;

        // map-based placeholder replacement
        documentTemplate.ReplaceText(values);

        // image placeholders
        if (imageSelectors is not null && imageSelectors.Any())
            ReplaceFieldsWithImage(documentTemplate, imageSelectors);

        documentTemplate.SaveToPath(outputFilePath);
        return await Task.FromResult(outputFilePath);
    }

    /// <summary>
    /// NEW: file-path overload so templates can be loaded from disk (e.g., after fetching from GridFS/S3).
    /// </summary>
    public async Task<string> GenerateWordFromFileAsync(
        string templateFilePath,
        IDictionary<string, object> values,
        WordDocumentProperties props,
        Dictionary<string, byte[]>? imageSelectors,
        string outputFilePath)
    {
        if (string.IsNullOrWhiteSpace(templateFilePath)) throw new ArgumentException("Required", nameof(templateFilePath));
        if (!File.Exists(templateFilePath)) throw new FileNotFoundException("Template file not found.", templateFilePath);
        if (string.IsNullOrWhiteSpace(outputFilePath)) throw new ArgumentException("Required", nameof(outputFilePath));

        var dir = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

        var doc = new Document();
        doc.LoadFromFile(templateFilePath);

        // Built-in props
        doc.BuiltinDocumentProperties.Title = props.Title;
        doc.BuiltinDocumentProperties.Author = props.Author;
        doc.BuiltinDocumentProperties.Company = props.Company;
        doc.BuiltinDocumentProperties.Keywords = props.Keywords;
        doc.BuiltinDocumentProperties.Subject = props.Subject;
        doc.BuiltinDocumentProperties.Category = props.Category;
        if (!string.IsNullOrEmpty(props.Comments))
            doc.BuiltinDocumentProperties.Comments = props.Comments;
        doc.BuiltinDocumentProperties.CreateDate = DateTime.Today;

        // Map-based replacements
        foreach (var (k, v) in values)
        {
            var placeholder = $"{{{{{k}}}}}";
            doc.Replace(placeholder, v?.ToString() ?? string.Empty, true, true);
        }

        // Images
        if (imageSelectors is not null && imageSelectors.Any())
            ReplaceFieldsWithImage(doc, imageSelectors);

        doc.SaveToFile(outputFilePath, Spire.Doc.FileFormat.Docx);
        return await Task.FromResult(outputFilePath);
    }
    
    private async Task<Spire.Doc.Document?> GenerateWordTemplateAsync<T>(T templateModel, string assemblyName) where T : IWordTemplateModel
    {
        try
        {
            if (string.IsNullOrEmpty(templateModel.Resource))
            {
                throw new ArgumentException($"'{nameof(templateModel.Resource)}' cannot be null or empty.", nameof(templateModel.Resource));
            }
            

            var documentTemplate = WordProcessor.LoadDocument(templateModel.Resource, assemblyName);

            documentTemplate.BuiltinDocumentProperties.Title = templateModel.DocumentProperties.Title;
            documentTemplate.BuiltinDocumentProperties.Author = templateModel.DocumentProperties.Author;
            documentTemplate.BuiltinDocumentProperties.Company = templateModel.DocumentProperties.Company;
            documentTemplate.BuiltinDocumentProperties.Keywords = templateModel.DocumentProperties.Keywords;
            documentTemplate.BuiltinDocumentProperties.Subject = templateModel.DocumentProperties.Subject;
            documentTemplate.BuiltinDocumentProperties.Category = templateModel.DocumentProperties.Category;

            if (!string.IsNullOrEmpty(templateModel.DocumentProperties.Comments))
            {
                documentTemplate.BuiltinDocumentProperties.Comments = templateModel.DocumentProperties.Comments;
            }

            documentTemplate.BuiltinDocumentProperties.CreateDate = DateTime.Today;

            var generatedDocument = documentTemplate.ReplaceText<T>(templateModel);

            if (templateModel.ImageSelectors is not null && templateModel.ImageSelectors.Any())
            {
                ReplaceFieldsWithImage(generatedDocument, templateModel.ImageSelectors);
            }

            return generatedDocument;
        }
        catch (Exception ex)
        {
            var m = ex.Message;
            throw;
        }
    }
    
    /// <summary>
    /// Replaces occurrences of keys (e.g., "{{QR}}") with image bytes.
    /// </summary>
    private void ReplaceFieldsWithImage(Document generatedDocument, Dictionary<string, byte[]> imageSelectors)
    {
        if (generatedDocument is null) throw new ArgumentNullException(nameof(generatedDocument));
        if (imageSelectors is null) throw new ArgumentNullException(nameof(imageSelectors));
        foreach (var item in imageSelectors)
        {
            var qrCodeSelections = generatedDocument.FindAllString(item.Key, true, true);
            foreach (Spire.Doc.Documents.TextSelection selection in qrCodeSelections)
            {
                DocPicture pic = new DocPicture(generatedDocument);
                // todo: load image from 
                pic.LoadImage(item.Value);

                TextRange range = selection.GetAsOneRange();
                int index = range.OwnerParagraph.ChildObjects.IndexOf(range);
                range.OwnerParagraph.ChildObjects.Insert(index, pic);
                range.OwnerParagraph.ChildObjects.Remove(range);
            }
        }
    }
}
public interface IWordService
{
    Task<string> GenerateWordToPathAsync<T>(T templateModel, string assemblyName,
        string? outputDirectory = null) where T : IWordTemplateModel;
    
    Task<string> GenerateWordToPathAsync(
        string resource,
        string assemblyName,
        IDictionary<string, object> values,
        WordDocumentProperties props,
        Dictionary<string, byte[]>? imageSelectors,
        string outputFilePath);

    // New: map-based, file-path source
    Task<string> GenerateWordFromFileAsync(
        string templateFilePath,
        IDictionary<string, object> values,
        WordDocumentProperties props,
        Dictionary<string, byte[]>? imageSelectors,
        string outputFilePath);
    
    
    //Task<Stream> GenerateWordStreamAsync<T>(T templateModel, string assemblyName) where T : IWordTemplateModel;
}
