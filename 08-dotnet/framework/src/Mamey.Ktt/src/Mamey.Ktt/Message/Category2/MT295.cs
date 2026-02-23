using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category2;

public class MT295
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("21")]
    [RegularExpression(@"^[A-Z0-9]{0,16}$")]
    public string RelatedReference { get; set; }

    [FieldTag("79")]
    public string Narrative { get; set; }
}