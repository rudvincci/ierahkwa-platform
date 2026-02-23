using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category4;

public class MT499
{
    [FieldTag("20")]
    public string TransactionReference { get; set; }

    [FieldTag("79")]
    public string Narrative { get; set; }
}