using System.Text.Json.Serialization;
using Mamey.Emails.ACS;

namespace Mamey.Emails;

public class EmailOptions
{
    
    public bool Enabled { get; set; }
    public string Type { get; set; }
    public string EmailId { get; set; }
    public string Name { get; set; }
    public MailKitOptions Mailkit { get; set; }
    [JsonPropertyName("acs")]
    public ACSEmailOptions ACS { get; set; }
}
