namespace Mamey.Barcode.Requests;

public interface IBarcodeRequest
{
    public string Data { get; set; }
    public string Type { get; set; }
}
