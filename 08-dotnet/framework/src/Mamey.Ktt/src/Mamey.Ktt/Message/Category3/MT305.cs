using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category3;

public class MT305
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("22A")]
    [RegularExpression(@"^(CALL|PUT)$")]
    public string TypeOfOption { get; set; }

    [FieldTag("31G")]
    [RegularExpression(@"^[A-Z]{3}[0-9,]{1,15}$")]
    public string Premium { get; set; }

    [FieldTag("32B")]
    [RegularExpression(@"^[A-Z]{3}[0-9,]{1,15}$")]
    public string CurrencyAmount { get; set; }

    [FieldTag("36")]
    public string StrikePrice { get; set; }

    [FieldTag("57A")]
    public string BeneficiaryInstitution { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}