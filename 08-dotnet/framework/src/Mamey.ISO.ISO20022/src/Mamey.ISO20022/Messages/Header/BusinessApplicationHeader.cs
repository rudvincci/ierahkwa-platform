using System.Xml.Serialization;

namespace Mamey.ISO20022.Messages.Header;

    /// <summary>
    /// Represents the Business Application Header (BAH) in an ISO 20022 message.
    /// </summary>
    [XmlRoot("AppHdr", Namespace = "urn:iso:std:iso:20022:tech:xsd:head.001.001.04")]
    public class BusinessApplicationHeader
    {
        /// <summary>
        /// Character set used in the message text elements.
        /// </summary>
        [XmlElement("CharSet")]
        public string CharacterSet { get; set; }

        /// <summary>
        /// Sender of the message.
        /// </summary>
        [XmlElement("Fr")]
        public Party From { get; set; }

        /// <summary>
        /// Recipient of the message.
        /// </summary>
        [XmlElement("To")]
        public Party To { get; set; }

        /// <summary>
        /// Unique identifier for the business message.
        /// </summary>
        [XmlElement("BizMsgIdr")]
        public string BusinessMessageIdentifier { get; set; }

        /// <summary>
        /// Identifier of the message definition.
        /// </summary>
        [XmlElement("MsgDefIdr")]
        public string MessageDefinitionIdentifier { get; set; }

        /// <summary>
        /// Specifies the business service under which the message is exchanged.
        /// </summary>
        [XmlElement("BizSvc")]
        public string BusinessService { get; set; }

        /// <summary>
        /// Creation date and time of the business message.
        /// </summary>
        [XmlElement("CreDt")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Processing date for the receiver of the business message.
        /// </summary>
        [XmlElement("BizPrcgDt")]
        public DateTime? BusinessProcessingDate { get; set; }

        /// <summary>
        /// Indicates whether the message is a copy, duplicate, or a copy of a duplicate.
        /// </summary>
        [XmlElement("CpyDplct")]
        public string CopyDuplicate { get; set; }

        /// <summary>
        /// Indicates if the message is potentially a duplicate.
        /// </summary>
        [XmlElement("PssblDplct")]
        public bool? PossibleDuplicate { get; set; }

        /// <summary>
        /// Relative priority of the message.
        /// </summary>
        [XmlElement("Prty")]
        public string Priority { get; set; }

        /// <summary>
        /// Digital signature of the message.
        /// </summary>
        [XmlElement("Sgntr")]
        public string Signature { get; set; }

        /// <summary>
        /// Related messages to this business message.
        /// </summary>
        [XmlArrayItem("Rltd")]
        public List<BusinessApplicationHeader> Related { get; set; }
    }

    /// <summary>
    /// Represents a party in the Business Application Header.
    /// </summary>
    public class Party
    {
        /// <summary>
        /// Organisation identification.
        /// </summary>
        [XmlElement("OrgId")]
        public OrganisationIdentification OrganisationIdentification { get; set; }

        /// <summary>
        /// Financial institution identification.
        /// </summary>
        [XmlElement("FIId")]
        public FinancialInstitutionIdentification FinancialInstitutionIdentification { get; set; }
    }

    /// <summary>
    /// Represents organisation identification details.
    /// </summary>
    public class OrganisationIdentification
    {
        /// <summary>
        /// Name of the organisation.
        /// </summary>
        [XmlElement("Nm")]
        public string Name { get; set; }

        /// <summary>
        /// Organisation identifier.
        /// </summary>
        [XmlElement("Id")]
        public string Identification { get; set; }
    }

    /// <summary>
    /// Represents financial institution identification details.
    /// </summary>
    public class FinancialInstitutionIdentification
    {
        /// <summary>
        /// Business Identifier Code (BIC) of the institution.
        /// </summary>
        [XmlElement("BICFI")]
        public string BICFI { get; set; }

        /// <summary>
        /// Name of the financial institution.
        /// </summary>
        [XmlElement("Nm")]
        public string Name { get; set; }
    }