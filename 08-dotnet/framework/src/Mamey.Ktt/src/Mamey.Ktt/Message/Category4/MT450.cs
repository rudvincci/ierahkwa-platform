using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category4;

public class MT450
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("21")]
    public string RelatedReference { get; set; }

    [FieldTag("32A")]
    public string ValueDateCurrencyAmount { get; set; }
}