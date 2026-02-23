using DocumentFormat.OpenXml.Packaging;
using System.Reflection;
using QRCoder;
using Spire.Doc;
using Spire.Pdf;
using Spire.Pdf.Security;
using System.Drawing;
using Spire.Doc.Documents;
using Spire.Doc.Fields;

namespace Mamey.Word;

public static class WordTemplateProcessor
{
    //public static void ProcessDocumentWithOpenXml(string docPath, object data, string newImagePath)
    //{
    //    using var doc = WordprocessingDocument.Open(docPath, true);
    //    var body = doc.MainDocumentPart.Document.Body;

    //    // Replace placeholders with property values
    //    foreach (var prop in data.GetType().GetProperties())
    //    {
    //        var placeholder = $"{{{{{prop.Name}}}}}";
    //        var value = prop.GetValue(data, null)?.ToString() ?? string.Empty;

    //        var texts = body.Descendants<Text>().Where(t => t.Text.Contains(placeholder));
    //        foreach (var text in texts)
    //        {
    //            text.Text = text.Text.Replace(placeholder, value);
    //        }
    //    }

    //    // Replace image in the footer
    //    var footerParts = doc.MainDocumentPart.FooterParts;
    //    foreach (var footerPart in footerParts)
    //    {
    //        var images = footerPart.ImagesPart;
    //        foreach (var imagePart in images)
    //        {
    //            using var stream = new System.IO.FileStream(newImagePath, System.IO.FileMode.Open);
    //            imagePart.FeedData(stream);
    //        }
    //    }

    //    doc.Save();
    //}
    public static void ProcessWordDocument(FileStream docStream, object data, byte[] qrImageBytes)
    {
        using var memoryStream = new MemoryStream();
        docStream.CopyTo(memoryStream); // Copy FileStream content to memory stream

        using var doc = WordprocessingDocument.Open(memoryStream, true);
        var body = doc?.MainDocumentPart?.Document.Body;

        // Replace placeholders with property values
        foreach (var prop in data.GetType().GetProperties())
        {
            var placeholder = $"{{{{{prop.Name}}}}}";
            var value = prop.GetValue(data, null)?.ToString() ?? string.Empty;

            var texts = body?.Descendants<DocumentFormat.OpenXml.Drawing.Text>().Where(t => t.Text.Contains(placeholder));
            if(texts is not null)
            {
                foreach (var text in texts)
                {
                    text.Text = text.Text.Replace(placeholder, value);
                }
            }
        }

        // Replace image in the footer
        var footerParts = doc?.MainDocumentPart?.FooterParts;
        if(footerParts is not null)
        {
            foreach (var footerPart in footerParts)
            {
                var imageParts = footerPart.ImageParts.ToList(); // Collect all the image parts
                if (imageParts.Any())
                {
                    var imagePart = imageParts.First(); // Take the first image part or adapt as needed
                    using var qrStream = new MemoryStream(qrImageBytes);
                    imagePart.FeedData(qrStream);
                }
            }
        }
        
        doc?.Save();
        var wordDocStream = doc?.MainDocumentPart?.GetStream();
        // Convert Word document in memoryStream to PDF using FreeSpire.Doc

        var pdfDocument = ConvertToPdf(ProcessDocumentWithSpire(wordDocStream, data,
            qrImageBytes));
        
        //// Now, you can save the pdfStream to a file, return it in a web response, etc.
        //var filename = $"{Environment.CurrentDirectory}POF.pdf";
        //using (var fileStream = new FileStream(filename, FileMode.Create))
        //{
        //    pdfStream.WriteTo(fileStream);
        //}

        //// Now, if you want to persist the changes, write back to the original FileStream
        //docStream.Position = 0;
        //memoryStream.WriteTo(docStream);
    }
    private static string ExtractResourceToTempFile(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new ArgumentException("No such resource", nameof(resourceName));

        var tempFilePath = Path.GetTempFileName();
        using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
        {
            stream.CopyTo(fileStream);
        }
        return tempFilePath;
    }
    private static byte[] GenerateQRCodeToTempFile(string qrData)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new QRCoder.PngByteQRCode(qrCodeData);
        var qrImage = qrCode.GetGraphic(20); // Adjust the number for size
        return qrImage;
    }

    //private MemoryStream ConvertDocxToPdfWithFreeSpire(MemoryStream docxStream)
    //{
    //    Document document = new Document();
    //    docxStream.Position = 0;  // Reset the stream's position
    //    document.LoadFromStream(docxStream, FileFormat.Docx);

    //    var pdfStream = new MemoryStream();
    //    document.SaveToStream(pdfStream, FileFormat.PDF);

    //    return pdfStream;  // This stream contains the PDF representation of the Word document
    //}
    //public static void ConvertAndSignDocxToPdf(string docxPath, string outputPath, string certFilePath, string certPassword)
    //{
    //    // Convert DOCX to PDF
    //    Spire.Doc.Document doc = new Spire.Doc.Document(docxPath);
    //    var pdfStream = new MemoryStream();
    //    doc.SaveToStream(pdfStream, Spire.Doc.FileFormat.PDF);

    //    // Load the generated PDF
    //    PdfDocument pdfDoc = new PdfDocument(pdfStream);

    //    // Digitally sign the PDF
    //    PdfCertificate cert = new PdfCertificate(certFilePath, certPassword);
    //    PdfSignature signature = new PdfSignature(pdfDoc, pdfDoc.Pages[0], cert, "signature");
    //    signature.Bounds = new RectangleF(new PointF(50, 690), new SizeF(200, 100)); // Adjust the rectangle as per your needs

    //    pdfDoc.SaveToFile(outputPath);
    //}
    //public static void ConvertAndSignDocxToPdf(WordprocessingDocument wordDocument, Stream pdfOutputStream,
    //    string certFilePath, string certPassword)
    //{
    //    using (var docxStream = wordDocument.Package.GetStream(FileMode.Open, FileAccess.Read))
    //    {
    //        ConvertAndSignDocxToPdf(docxStream, pdfOutputStream, certFilePath, certPassword);
    //    }

    //    // Convert DOCX to PDF
    //    Spire.Doc.Document doc = new Spire.Doc.Document();
    //    // Reset the stream's position if it's been read before
    //    wordDocument.Position = 0;  
    //    doc.LoadFromStream(wordDocument, Spire.Doc.FileFormat.Docx);

    //    var pdfTempStream = new MemoryStream();
    //    doc.SaveToStream(pdfTempStream, Spire.Doc.FileFormat.PDF);

    //    // Load the generated PDF
    //    PdfDocument pdfDoc = new PdfDocument(pdfTempStream);

    //    // Digitally sign the PDF
    //    PdfCertificate cert = new PdfCertificate(certFilePath, certPassword);
    //    PdfSignature signature = new PdfSignature(pdfDoc, pdfDoc.Pages[0],
    //        cert, "signature");
    //    signature.Bounds = new RectangleF(new PointF(50, 690),
    //        new SizeF(200, 100)); // Adjust the rectangle as per your needs

    //    pdfDoc.SaveToStream(pdfOutputStream, Spire.Pdf.FileFormat.PDF);
    //    pdfDoc.SaveToFile($"{Environment.CurrentDirectory}/PDF/POF{DateTime.Today.Year}{DateTime.Today.Month}{DateTime.Today.Day}.pdf");
    //}

    public static Stream ProcessDocumentWithSpire<T>(Stream docStream, T data, byte[] newImageBytes)
    {
        if (docStream is null)
        {
            throw new ArgumentNullException(nameof(docStream));
        }

        if (newImageBytes is null)
        {
            throw new ArgumentNullException(nameof(newImageBytes));
        }

        Spire.Doc.Document document = new Spire.Doc.Document();
        docStream.Position = 0; // Reset position in case it was changed
        document.LoadFromStream(docStream, Spire.Doc.FileFormat.Docx);

        // Replace placeholders with property values
        foreach (var prop in data.GetType().GetProperties())
        {
            string placeholder = "{{" + prop.Name + "}}";
            string value = prop.GetValue(data)?.ToString() ?? string.Empty;
            document.Replace(placeholder, value, true, true);
        }

        // Replace image in the footer
        foreach (Section section in document.Sections)
        {
            foreach (HeaderFooter footer in section.HeadersFooters)
            {
                if (footer.DocumentObjectType == DocumentObjectType.HeaderFooter)
                {
                    foreach (DocumentObject obj in footer.ChildObjects)
                    {
                        if (obj.DocumentObjectType == DocumentObjectType.Picture)
                        {
                            DocPicture pic = obj as DocPicture;
                            using (var ms = new MemoryStream(newImageBytes))
                            {
                                pic.LoadImage(ms);
                            }
                        }
                    }
                }
            }
        }
        try
        {
            var doc = new Spire.Doc.Document();
            docStream.Position = 0; // Reset position in case it was changed
            doc.LoadFromStream(docStream, Spire.Doc.FileFormat.Docx);

            MemoryStream newStream = new MemoryStream();
            document.SaveToStream(newStream, Spire.Doc.FileFormat.Docx);
            return newStream;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        // Save the modified document back to the original stream
        
    }
    public static PdfDocument ConvertToPdf(Stream docStream)
    {
        // Convert DOCX to PDF
        var doc = new Spire.Doc.Document();
        docStream.Position = 0;  // Reset the stream's position if it's been read before
        doc.LoadFromStream(docStream, Spire.Doc.FileFormat.Docx);

        var pdfTempStream = new MemoryStream();
        try
        {
            //Create a ToPdfParameterList object
            ToPdfParameterList parameters = new ToPdfParameterList();

            //Embed all the fonts used in Word in the generated PDF
            parameters.IsEmbeddedAllFonts = true;
            doc.SaveToStream(pdfTempStream, Spire.Doc.FileFormat.PDF);

        }
        catch (Exception ex)
        {
            throw ex;
        }
        doc.Dispose();

        // Load the generated PDF
        return new PdfDocument(pdfTempStream);
    }
    public static PdfDocument DigitallySignPdf(PdfDocument pdfDocument, string certFilePath, string certPassword)
    {
        // Digitally sign the PDF
        PdfCertificate cert = new PdfCertificate(certFilePath, certPassword);
        PdfSignature signature = new PdfSignature(pdfDocument, pdfDocument.Pages[0],
            cert, "signature");
        signature.Bounds = new RectangleF(new PointF(50, 690),
            new SizeF(200, 100)); // Adjust the rectangle as per your needs

        //pdfDocument.SaveToStream(pdfOutputStream, Spire.Pdf.FileFormat.PDF);
        pdfDocument.SaveToFile($"{Environment.CurrentDirectory}/PDF/POF{DateTime.Today.Year}{DateTime.Today.Month}{DateTime.Today.Day}.pdf");
        return pdfDocument;
    }
}
/// <summary>
/// High-level templating façade for DOCX generation only.
/// Keeps concerns focused: load template (embedded/file/bytes), inject values/images/props,
/// and write a DOCX to disk. Optional QR stamping convenience.
/// </summary>
public interface IWordTemplateProcessor
{
    /// <summary>
    /// Render from an embedded resource in a given assembly using a values map.
    /// Writes a DOCX to <paramref name="outputFilePath"/>.
    /// </summary>
    Task<string> RenderFromEmbeddedAsync(
        string resource,
        string assemblyName,
        IDictionary<string, object> values,
        WordDocumentProperties props,
        Dictionary<string, byte[]>? imageSelectors,
        string outputFilePath,
        CancellationToken ct = default);

    /// <summary>
    /// Render from a template file on disk using a values map.
    /// Writes a DOCX to <paramref name="outputFilePath"/>.
    /// </summary>
    Task<string> RenderFromFileAsync(
        string templateFilePath,
        IDictionary<string, object> values,
        WordDocumentProperties props,
        Dictionary<string, byte[]>? imageSelectors,
        string outputFilePath,
        CancellationToken ct = default);

    /// <summary>
    /// Render directly from template bytes (e.g., fetched from GridFS/S3) using a values map.
    /// Writes a DOCX to <paramref name="outputFilePath"/>.
    /// </summary>
    Task<string> RenderFromBytesAsync(
        byte[] templateBytes,
        IDictionary<string, object> values,
        WordDocumentProperties props,
        Dictionary<string, byte[]>? imageSelectors,
        string outputFilePath,
        CancellationToken ct = default);

    /// <summary>
    /// Convenience: apply a QR code replacement into a DOCX by reusing an existing image placeholder.
    /// Writes a new DOCX to <paramref name="destinationDocumentPath"/>.
    /// </summary>
    Task<string> ApplyQrAsync(
        string sourceDocumentPath,
        string destinationDocumentPath,
        string qrData,
        CancellationToken ct = default);
}
