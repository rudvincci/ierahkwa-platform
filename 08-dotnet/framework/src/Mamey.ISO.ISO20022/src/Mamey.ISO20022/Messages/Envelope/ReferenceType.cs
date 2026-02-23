using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Mamey.ISO20022.Messages.Envelope;

/// <summary>
/// Represents the type of reference.
/// </summary>
public class ReferenceType
{
    public ReferenceType(string code, string proprietary)
    {
        if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(proprietary))
        {
            throw new ArgumentException("Code or Proprietary is required");
        }

        Code = code;
        Proprietary = proprietary;
    }
    /// <summary>
    /// Code identifying the type of reference.
    /// </summary>
    [XmlElement("Cd")]
    [MinLength(1), MaxLength(4)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Proprietary identification of the type of reference.
    /// </summary>
    [XmlElement("Prtry")]
    [MinLength(1), MaxLength(35)]
    public string Proprietary { get; set; } = string.Empty!;
}