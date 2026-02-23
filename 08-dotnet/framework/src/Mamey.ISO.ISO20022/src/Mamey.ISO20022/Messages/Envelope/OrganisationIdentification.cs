using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Mamey.ISO20022.Messages.Envelope;

public class OrganisationIdentification
{
    [XmlElement("AnyBIC")]
    [MinLength(8), MaxLength(11)]
    public string AnyBIC { get; set; }
    
    [XmlElement("LEI")]
    [RegularExpression(@"^[A-Z0-9]{18}[0-9]{2}$")]
    public string? LEI { get; set; } = default!;
    
}