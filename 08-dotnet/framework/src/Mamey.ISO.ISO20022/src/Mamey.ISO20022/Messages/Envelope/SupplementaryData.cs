using System.Xml.Serialization;

namespace Mamey.ISO20022.Messages.Envelope;

/// <summary>
/// Represents additional supplementary data.
/// </summary>
public class SupplementaryData
{
    /// <summary>
    /// Unambiguous reference to the location where the supplementary data must be inserted in the message instance.
    /// </summary>
    [XmlElement("PlcAndNm")]
    public string? PlaceAndName { get; set; }

    /// <summary>
    /// Technical element wrapping the supplementary data.
    /// </summary>
    [XmlElement("Envlp")]
    public object Envelope { get; set; }
}