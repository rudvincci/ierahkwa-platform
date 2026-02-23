using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category1;

public class MT112
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("21")]
    [RegularExpression(@"^[A-Z0-9]{0,16}$")]
    public string RelatedReference { get; set; }

    [FieldTag("32A")]
    [RegularExpression(@"^\d{6}[A-Z]{3}[0-9,]{1,15}$")]
    public string ValueDateCurrencyAmount { get; set; }

    [FieldTag("25")]
    [RegularExpression(@"^(ACCEPTED|REJECTED)$")]
    public string Status { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}