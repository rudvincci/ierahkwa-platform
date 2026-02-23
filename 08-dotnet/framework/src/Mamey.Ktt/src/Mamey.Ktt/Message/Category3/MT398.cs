using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category3;

public class MT398
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("79")]
    public string Narrative { get; set; }
}