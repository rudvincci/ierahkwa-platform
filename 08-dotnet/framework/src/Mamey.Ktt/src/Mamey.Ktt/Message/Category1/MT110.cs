using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category1;

public class MT110
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

    [FieldTag("50A")]
    public string OrderingCustomer { get; set; }

    [FieldTag("59")]
    public string BeneficiaryCustomer { get; set; }

    [FieldTag("70")]
    [RegularExpression(@"^.{0,35}$")]
    public string RemittanceInformation { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}