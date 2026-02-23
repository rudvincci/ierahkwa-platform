using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Mamey.ISO20022.Messages.Types;

namespace Mamey.ISO20022.Messages.Envelope;

/// <summary>
/// Represents information about a party in the message.
/// </summary>
public class PartyIdentification
{
    /// <summary>
    /// Name by which a party is known and which is usually used to identify that party.
    /// </summary>
    [XmlElement("Nm")]
    [MinLength(1), MaxLength(140)]
    public string? Name { get; set; } = string.Empty;

    /// <summary>
    /// Identification details of the party.
    /// </summary>
    [XmlElement("Id")]
    public Identification? Identification { get; set; }
    
    /// <summary>
    /// Information that locates and identifies a specific address, as defined by postal services.
    /// </summary>
    [XmlElement("PstlAdr")]
    public PostalAddress? PostalAddress { get; set; }
    
    [XmlElement("CtryOfRes")]
    public string? CountryOfResidence { get; set; }
    
    [XmlElement("CtctDtls")]
    public string? ContactDetails { get; set; }
}

