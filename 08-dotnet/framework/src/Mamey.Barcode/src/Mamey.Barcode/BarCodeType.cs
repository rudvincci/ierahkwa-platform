using System.ComponentModel.DataAnnotations;
namespace Mamey.Barcode;

public enum BarCodeType
{
    [Display(Name = "PDF417", ShortName = "pdf417")]
    PDF417,
    [Display(Name = "DataMatrix", ShortName = "datamatrix")]
    DataMatrix,
    [Display(Name = "Code128", ShortName = "code128")]
    Code128,
    [Display(Name = "QRCode", ShortName = "qrcode")]
    QRCode,
    [Display(Name = "Code39", ShortName = "code39")]
    Code39,
    [Display(Name = "EAN13", ShortName = "ean13")]
    EAN13
}