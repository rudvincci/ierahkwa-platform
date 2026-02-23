using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Mamey.ISO20022.Messages.Envelope;

/// <summary>
/// Represents additional information for a reference.
/// </summary>
public class OtherReference
{
    /// <summary>
    /// Type of the reference.
    /// </summary>
    [XmlElement("Tp")]
    public ReferenceType Type { get; set; }

    /// <summary>
    /// Value of the reference.
    /// </summary>
    [XmlElement("Val")]
    [MinLength(1), MaxLength(256)]
    public string Value { get; set; }
}