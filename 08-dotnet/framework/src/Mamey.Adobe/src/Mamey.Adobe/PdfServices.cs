using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.exception;
using Microsoft.Extensions.Logging;
using Adobe.PDFServicesSDK.pdfjobs.jobs;
using Adobe.PDFServicesSDK.pdfjobs.results;
using Adobe.PDFServicesSDK.pdfjobs.parameters.createpdf;
using Adobe.PDFServicesSDK.pdfjobs.parameters.createpdf.word;

namespace Mamey.Adobe;

public class PdfServices : IPdfServices
{
    private readonly AdobePdfServicesOptions _adobeOptions;
    private readonly ILogger<PdfServices> _logger;
    public PdfServices(AdobePdfServicesOptions adobeOptions, ILogger<PdfServices> logger)
    {
        _adobeOptions = adobeOptions;
        if (string.IsNullOrEmpty(_adobeOptions.ClientId))
        {
            throw new Exception("clientId is null");
        }
        if (string.IsNullOrEmpty(_adobeOptions.ClientSecret))
        {
            throw new Exception("clientSecret is null");
        }
        _logger = logger;
    }

    public Task<Stream?> ConvertWordToPdfAsync(string filePath)
    {
        Stream? outputStream = null;
        try
        {

            // Initial setup, create credentials instance.
            ICredentials credentials = new ServicePrincipalCredentials(_adobeOptions.ClientId, _adobeOptions.ClientSecret);

            // Creates a PDF Services instance
            PDFServices pdfServices = PDFServiceFactory.GeneratePDFService(_adobeOptions.ClientId, _adobeOptions.ClientSecret);

            using Stream inputStream = File.OpenRead(filePath);
            IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.DOCX.GetMIMETypeValue());

            // Create parameters for the job
            CreatePDFParams createPDFParams = CreatePDFParams.WordParamsBuilder()
                .WithDocumentLanguage(DocumentLanguage.EN_US)
                .Build();

            // Creates a new job instance
            CreatePDFJob createPDFJob = new CreatePDFJob(asset).SetParams(createPDFParams);

            // Submits the job and gets the job result
            String location = pdfServices.Submit(createPDFJob);
            PDFServicesResponse<CreatePDFResult> pdfServicesResponse =
                pdfServices.GetJobResult<CreatePDFResult>(location, typeof(CreatePDFResult));

            // Get content from the resulting asset(s)
            IAsset resultAsset = pdfServicesResponse.Result.Asset;
            StreamAsset streamAsset = pdfServices.GetContent(resultAsset);

            // Creating output streams and copying stream asset's content to it
            var dirInfo = new FileInfo(filePath);

            //new FileInfo(CreateOutputFilePath()).Directory.Create();
            outputStream = File.Open(dirInfo.FullName.Replace(".docx", ".pdf"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            
            streamAsset.Stream.CopyTo(outputStream);
            

        }
        catch (ServiceUsageException ex)
        {
            _logger.LogError("Exception encountered while executing operation", ex);
        }
        catch (ServiceApiException ex)
        {
            _logger.LogError("Exception encountered while executing operation", ex);
        }
        catch (SDKException ex)
        {
            _logger.LogError("Exception encountered while executing operation", ex);
        }
        catch (IOException ex)
        {
            _logger.LogError("Exception encountered while executing operation", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception encountered while executing operation", ex);
        }
        finally
        {
            //if (outputStream is not null)
            //{
            //    outputStream?.Close();
            //}

        }
        return Task.FromResult(outputStream);
    }

    public Task ExportPdfToWord()
    {
        throw new NotImplementedException();
    }

    public Task GetPdfProperties()
    {
        throw new NotImplementedException();
    }

    public Task MergeDocumentToPdf()
    {
        throw new NotImplementedException();
    }

    public Task SealPdf()
    {
        throw new NotImplementedException();
    }

    //Generates a string containing a directory structure and file name for the output file.
    public static string CreateOutputFilePath(string path)
    {
        String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
        return ("/files/adoptions" + timeStamp + ".pdf");
    }
}
