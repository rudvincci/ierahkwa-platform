using System.ComponentModel.DataAnnotations;

namespace Mamey.Barcode.Requests;

public enum ErrorCorrection
{
    [Display(Name="Low", ShortName = "L", Description = "Recovers 7% of data.")]
    Low,
    [Display(Name = "Medium", ShortName = "M", Description = "Recovers 15% of data.")]
    Medium,
    [Display(Name = "Quartile", ShortName = "Q", Description = "Recovers 25% of data.")]
    Quartile,
    [Display(Name = "High", ShortName = "H", Description = "Recovers 30% of data.")]
    High
}

