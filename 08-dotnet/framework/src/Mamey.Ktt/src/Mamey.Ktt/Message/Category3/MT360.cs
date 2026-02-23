using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category3;

public class MT360
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("22A")]
    public string TypeOfSwap { get; set; }

    [FieldTag("30")]
    [RegularExpression(@"^\d{6}$")]
    public string TradeDate { get; set; }

    [FieldTag("32B")]
    [RegularExpression(@"^[A-Z]{3}[0-9,]{1,15}$")]
    public string NotionalAmount { get; set; }

    [FieldTag("34A")]
    public string FixedInterestRate { get; set; }

    [FieldTag("36")]
    public string FloatingRateIndex { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}