namespace Mamey.Barcode.Requests;

public class BarcodeRequest : IBarcodeRequest
{
    public BarcodeRequest(string data, string type)
    {
        Data = data;
        Type = type;
    }

    public string Data { get; set; }
    public string Type { get; set; }
}
