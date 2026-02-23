using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category3;

public class MT364
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("30")]
    public string ResetDate { get; set; }

    [FieldTag("36")]
    public string FloatingRateIndex { get; set; }

    [FieldTag("32B")]
    [RegularExpression(@"^[A-Z]{3}[0-9,]{1,15}$")]
    public string CurrencyAmount { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}