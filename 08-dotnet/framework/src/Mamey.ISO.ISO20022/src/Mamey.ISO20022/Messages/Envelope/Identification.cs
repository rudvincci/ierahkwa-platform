using System.Xml.Serialization;

namespace Mamey.ISO20022.Messages.Envelope;

public class Identification
{
    public Identification(OrganisationIdentification organisationIdentification)
    {
        OrganisationIdentification = organisationIdentification;
    }
    public Identification(PrivateIdentification privateIdentification)
    {
        PrivateIdentification = privateIdentification;
    }

    [XmlElement("OrgId")]
    public OrganisationIdentification? OrganisationIdentification { get; set; } = default!;
    [XmlElement("PrvtId")]
    public PrivateIdentification? PrivateIdentification { get; set; } = default!;
}