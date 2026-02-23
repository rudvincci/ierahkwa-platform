using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category3;

public class MT304
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("21")]
    [RegularExpression(@"^[A-Z0-9]{0,16}$")]
    public string RelatedReference { get; set; }

    [FieldTag("22A")]
    [RegularExpression(@"^(FXSWAP|FXFWD)$")]
    public string TypeOfOperation { get; set; }

    [FieldTag("82A")]
    public string PartyA { get; set; }

    [FieldTag("87A")]
    public string PartyB { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}