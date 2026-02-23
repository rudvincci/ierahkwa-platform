using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using QRCoder;
using Spire.Doc;

namespace Mamey.Word;

public static class WordProcessor
{
    /// <summary>
    /// Load an embedded DOCX resource from the given assembly.
    /// </summary>
    public static Document LoadDocument(string resourceName, string assemblyName)
    {
        if (string.IsNullOrWhiteSpace(resourceName))
            throw new ArgumentException("Resource name is required.", nameof(resourceName));
        if (string.IsNullOrWhiteSpace(assemblyName))
            throw new ArgumentException("Assembly name is required.", nameof(assemblyName));
        
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
                           .SingleOrDefault(a => a.GetName().Name == assemblyName)
                       ?? throw new FileNotFoundException($"Assembly '{assemblyName}' not loaded in the current AppDomain.");
      
        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new FileNotFoundException("Embedded resource not found.", resourceName);
        
        // Create a copy of the stream
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        // Open a WordprocessingDocument on the copied stream
        // Note: The WordprocessingDocument should be opened in read-only mode 
        // because the stream does not support writing
        Document document = new Document();
        document.LoadFromStream(memoryStream, FileFormat.Docx);
        return document;
    }
    /// <summary>
    /// Generic reflection-based replacement for {{PropertyName}} from the model.
    /// </summary>
    public static Document ReplaceText<T>(this Document document, T wordTemplateModel)
         where T : IWordTemplateModel
    {
        if (document is null) throw new ArgumentNullException(nameof(document));
        if (wordTemplateModel is null) throw new ArgumentNullException(nameof(wordTemplateModel));
        
        var replacements = new Dictionary<string, string>();
        // Use reflection to iterate through each property of the wordTemplateModel
        foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
        {
            // Get the property name and value
            string propertyName = propertyInfo.Name;
            string propertyValue = propertyInfo.GetValue(wordTemplateModel)?.ToString() ?? string.Empty;

            // Add the property name and value to the replacements dictionary
            replacements.Add($"{{{{{propertyName}}}}}", propertyValue);
        }
        foreach (var replacement in replacements)
        {
            document.Replace(replacement.Key, replacement.Value, true, true);
        }
        return document;

    }
    public static void SaveToPath(this Document document, string fullFilePath)
    {
        if (document is null) throw new ArgumentNullException(nameof(document));
        if (string.IsNullOrWhiteSpace(fullFilePath)) throw new ArgumentException("Output path required.", nameof(fullFilePath));
        var dir = Path.GetDirectoryName(fullFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        document.SaveToFile(fullFilePath, FileFormat.Docx);
    }
    /// <summary>
    /// Replace the first image found in main body, headers, and footers with a generated QR image.
    /// Writes result to <paramref name="destinationDocumentPath"/>.
    /// </summary>
    public static void ReplaceQR(string sourceDocumentPath, string destinationDocumentPath, string qrString)
    {
        if (string.IsNullOrWhiteSpace(sourceDocumentPath)) throw new ArgumentException("Required", nameof(sourceDocumentPath));
        if (string.IsNullOrWhiteSpace(destinationDocumentPath)) throw new ArgumentException("Required", nameof(destinationDocumentPath));
        
        var qrCodeImage = GetQRCodeBytes(qrString); // Assume this method returns a byte[] representing the QR code

        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(sourceDocumentPath, true))
        {
            var mainPart = wordDoc.MainDocumentPart;

            // Replace image in the main body
            var bodyImagePart = FindImagePart(mainPart);
            if (bodyImagePart != null)
            {
                ReplaceImage(mainPart, bodyImagePart, qrCodeImage);
            }

            // Replace image in headers
            foreach (var headerPart in mainPart.HeaderParts)
            {
                var headerImagePart = FindImagePart(headerPart);
                if (headerImagePart != null)
                {
                    ReplaceImage(headerPart, headerImagePart, qrCodeImage);
                }
            }

            // Replace image in footers
            foreach (var footerPart in mainPart.FooterParts)
            {
                var footerImagePart = FindImagePart(footerPart);
                if (footerImagePart != null)
                {
                    ReplaceImage(footerPart, footerImagePart, qrCodeImage);
                }
            }

            // Save the changes to the document
            wordDoc.Save();
        }
        
        // Copy the modified document to a new location
        File.Copy(sourceDocumentPath, destinationDocumentPath, overwrite: true);

    }
    private static byte[] GetQRCodeBytes(string qrString)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrString, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }
    private static ImagePart? FindImagePart(OpenXmlPart part)
    {
        // Implement logic to find the specific image part
        // This might involve checking for a unique name, alt text, or tag
        // For simplicity, let's assume there's only one image and return it
        return part.GetPartsOfType<ImagePart>()?.FirstOrDefault();
    }
    /// <summary>
    /// SAFE image swap: retain relationships and feed bytes into the existing ImagePart.
    /// </summary>
    private static void ReplaceImage(OpenXmlPart part, ImagePart oldImagePart, byte[] newImage)
    {
        using var s = new MemoryStream(newImage, writable: false);
        oldImagePart.FeedData(s);
    }
    // private static void ReplaceImage(OpenXmlPart part, ImagePart oldImagePart, byte[] newImage)
    // {
    //
    //     // Remove the old image part
    //     part.DeletePart(oldImagePart);
    //
    //     //// Add a new image part
    //     //ImagePart newImagePart = part.AddImagePart(ImagePartType.Png);
    //
    //     //// Write the new image data to the new image part
    //     //using (Stream stream = newImagePart.GetStream())
    //     //{
    //     //    stream.Write(newImage, 0, newImage.Length);
    //     //}
    // }
    
}
