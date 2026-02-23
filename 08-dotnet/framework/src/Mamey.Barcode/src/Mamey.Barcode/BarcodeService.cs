
using Mamey.Barcode.Http;
using Mamey.Barcode.Requests;


namespace Mamey.Barcode;

public class BarcodeService : IBarcodeService
{
    private readonly IMameyBarcodeApiClient _mameyBarcodeApiClient;

    public BarcodeService(IMameyBarcodeApiClient mameyBarcodeApiClient)
    {
        _mameyBarcodeApiClient = mameyBarcodeApiClient;
    }

    // public void GeneratePDF417BarCode(string data)
    // {
    //     var myBarcode = BarcodeWriter.CreateBarcode(data, BarcodeWriterEncoding.PDF417, 500, 75);
    //
    //     myBarcode.StampToExistingPdfPage($"{Environment.CurrentDirectory}/AdoptionCertificateTemplate.pdf", 864, 576);
    //     myBarcode.SaveAsPng($"{Environment.CurrentDirectory}/AdoptionCertificateQR.png");
    // }

    public async Task<byte[]?> GenerateQRCodeAsync(string data, int maxWidth = 500, int maxHeight = 500)
    {
        var response =
            await _mameyBarcodeApiClient.GenerateBarcodeAsync(new BarcodeRequest(data,
                BarCodeType.QRCode.GetEnumShortNameDisplayAttribute()));
        if (!response.Succeeded)
        {
            return null;
        }

        return response.BarcodeBytes;
    }

    public async Task<byte[]?> GeneratePDF417Async(
        string data,
        int columns = 20,
        int securityLevel = 5,
        int scale = 2,
        int ratio = 3,
        int padding = 5)
    {
        var request = new PDF417Request(
            data: data,
            columns: columns,
            securityLevel: securityLevel,
            scale: scale,
            ratio: ratio,
            padding: padding);

        var response = await _mameyBarcodeApiClient.GenerateBarcodeAsync(request);
        if (!response.Succeeded)
        {
            return null;
        }

        return response.BarcodeBytes;
    }
    // public void GenerateQRCode(string data)
    // {   
    //     var myBarcode = BarcodeWriter.CreateBarcode(data, BarcodeWriterEncoding.PDF417, 500, 200);
    //     myBarcode.AddBarcodeValueTextBelowBarcode();
    //     var pdfPath = $"{Environment.CurrentDirectory}/AdoptionCertificateTemplate.pdf";
    //     var imagePath = $"{Environment.CurrentDirectory}/AdoptionCertificateQR.png";
    //     myBarcode.SaveAsPng(imagePath);
    //     //ManipulatePdf(pdfPath, imagePath);
    //
    // }
}