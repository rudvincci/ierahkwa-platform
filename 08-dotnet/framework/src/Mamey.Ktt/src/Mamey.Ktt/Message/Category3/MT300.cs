using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category3;

public class MT300
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("22A")]
    [RegularExpression(@"^(BUY|SELL)$")]
    public string TypeOfOperation { get; set; }

    [FieldTag("32B")]
    [RegularExpression(@"^[A-Z]{3}[0-9,]{1,15}$")]
    public string CurrencyAmount { get; set; }

    [FieldTag("33B")]
    [RegularExpression(@"^[A-Z]{3}[0-9,]{1,15}$")]
    public string CounterCurrencyAmount { get; set; }

    [FieldTag("36")]
    [RegularExpression(@"^[0-9]{1,12}\.[0-9]{1,6}$")]
    public string ExchangeRate { get; set; }

    [FieldTag("57A")]
    public string AccountWithInstitution { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}