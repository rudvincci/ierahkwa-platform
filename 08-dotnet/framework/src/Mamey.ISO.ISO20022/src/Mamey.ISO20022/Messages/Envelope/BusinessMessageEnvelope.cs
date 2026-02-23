using System.Xml.Serialization;
using Mamey.ISO20022.Messages.Header;

namespace Mamey.ISO20022.Messages.Envelope;

/// <summary>
/// Represents the ISO 20022 Business Message Envelope (BME).
/// </summary>
[XmlRoot("BizMsgEnvlp", Namespace = "urn:iso:std:iso:20022:tech:xsd:nvlp.001.001.01")]
[XmlInclude(typeof(BusinessApplicationHeader)), XmlInclude(typeof(Reference22)), XmlInclude(typeof(SupplementaryData))]
public class BusinessMessageEnvelope
{
    public BusinessMessageEnvelope(object document, BusinessApplicationHeader? Header = default!)
    {
        
    }

    /// <summary>
    /// The Business Application Header instance.
    /// </summary>
    [XmlElement("Hdr")]
    public BusinessApplicationHeader? Header { get; set; }

    /// <summary>
    /// The ISO 20022 Message Definition instance.
    /// </summary>
    [XmlElement("Doc")]
    public required object Document { get; set; }

    /// <summary>
    /// Reference22 information related to the delivery of the business message.
    /// </summary>
    // [XmlElement("Ref")]
    [XmlArray("Ref")]
    // [XmlArrayItem("PersonObjekt")]
    public List<Reference22>? References { get; set; } = new();

    /// <summary>
    /// Additional supplementary data for the envelope.
    /// </summary>
    [XmlElement("SplmtryData")]
    [XmlArray("PersonenArray")]
    [XmlArrayItem("PersonObjekt")]
    public List<SupplementaryData> SupplementaryData { get; set; } = new();
}