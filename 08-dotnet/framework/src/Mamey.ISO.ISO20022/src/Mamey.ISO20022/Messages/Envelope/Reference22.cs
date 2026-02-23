using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Mamey.ISO20022.Messages.Envelope;

/// <summary>
/// Represents a reference related to the delivery of the business message.
/// </summary>
public class Reference22
{
    public Reference22(PartyIdentification issuer, string value, Guid uuid, string? name = null)
    {
        Value = value;
        UUID = uuid;
        Name = name;
        Issuer = issuer;
    }
    public Reference22(PartyIdentification issuer, string value, string iri, string? name = null)
    {
        Value = value;
        IRI = iri;
        Name = name;
        Issuer = issuer;
    }
    public Reference22(PartyIdentification issuer, string value, OtherReference other, string? name = null)
    {
        Value = value;
        Other = other;
        Name = name;
        Issuer = issuer;
    }
    /// <summary>
    /// Name of the reference.
    /// </summary>
    [XmlElement("Nm")]
    [MinLength(1), MaxLength(35)]
    public string? Name { get; }

    /// <summary>
    /// Issuer of the reference.
    /// </summary>
    [XmlElement("Issr")]
    [Required]
    public PartyIdentification Issuer { get; }

    /// <summary>
    /// Value of the reference.
    /// </summary>
    [XmlElement("Val")]
    [Required]
    public string Value { get; private set; }

    /// <summary>
    /// Universally Unique Identifier (UUID) version 4.
    /// </summary>
    [XmlElement("UUID")]
    public Guid? UUID { get; }

    /// <summary>
    /// Internationalized Resource Identifier (IRI) address.
    /// </summary>
    [XmlElement("IRI")]
    public string? IRI { get; }

    /// <summary>
    /// Other type of reference.
    /// </summary>
    [XmlElement("Othr")]
    public OtherReference? Other { get; }
}