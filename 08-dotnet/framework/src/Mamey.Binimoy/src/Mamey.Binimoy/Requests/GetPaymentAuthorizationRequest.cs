using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Mamey.Binimoy.Requests;

public class GetPaymentAuthorizationRequest
{
    public GetPaymentAuthorizationRequest(string orgId, string senderVID,
        string receiverVID, string channelID, string referenceNo, decimal txnAmount)
        : this(orgId, senderVID, receiverVID, channelID, null, null, null, null, referenceNo, txnAmount)
    {
    }

    public GetPaymentAuthorizationRequest(string orgId, string senderVID,
        string receiverVID, string channelID, string? deviceId, string? mobileNo,
        string? location, string? iP, string referenceNo, decimal txnAmount)
    {
        OrgId = orgId;
        SenderVID = senderVID;
        ReceiverVID = receiverVID;
        ChannelID = channelID;
        DeviceId = deviceId;
        MobileNo = mobileNo;
        Location = location;
        IP = iP;
        ReferenceNo = referenceNo;
        TxnAmount = txnAmount;
    }
    /// <summary>
    /// Valid FI Routing No
    /// </summary>
    [MinLength(9), MaxLength(9)]
    public string OrgId { get; set; }
    /// <summary>
    /// Valid Virtual ID
    /// </summary>
    [MinLength(15), MaxLength(60)]
    public string SenderVID { get; set; }
    /// <summary>
    /// Valid Virtual ID
    /// </summary>
    [MinLength(15), MaxLength(60)]
    public string ReceiverVID { get; set; }
    /// <summary>
    /// Online/Mobile/Others
    /// </summary>
    [MinLength(5), MaxLength(6)]
    public string ChannelID { get; set; }
    [MaxLength(250)]
    public string? DeviceId { get; set; }
    /// <summary>
    /// Valid Mobile No
    /// </summary>
    [MinLength(11), MaxLength(11)]
    public string? MobileNo { get; set; }
    /// <summary>
    /// Valid Longitude & Latitude
    /// Example: 23.7805,90.4267
    /// </summary>
    [MaxLength(30)]
    public string? Location { get; set; }
    /// <summary>
    /// Valid IP
    /// </summary>
    [MaxLength(15)]
    public string? IP { get; set; }
    /// <summary>
    /// Valid Reference
    /// </summary>
    [MinLength(9), MaxLength(22)]
    public string ReferenceNo { get; set; }
    /// <summary>
    /// Valid Amount
    /// MaxLength 18,2
    /// </summary>
    public decimal TxnAmount { get; set; }

    public void Build()
    {

    }
}

[Serializable]
public class CreateRTPRequest
{
    [Required]
    [MinLength(9), MaxLength(9)]
    [RegularExpression("^[1-9]\\d{8}$")] //This ensures that the matched integer is a positive number with exactly 9 digits
    public string OrgId { get; set; }

    [Required]
    [MinLength(5), MaxLength(6)]
    [JsonPropertyName("channelID")]
    [XmlElement("ChannelID")]
    public ChannelId ChannelId { get; set; }

    [Required]
    [MinLength(15), MaxLength(60)]
    [JsonPropertyName("senderVID")]
    [XmlElement("SenderVID")]
    public string SenderVId { get; set; }

    [Required]
    [MinLength(15), MaxLength(60)]
    [JsonPropertyName("receiverVID")]
    [XmlElement("ReceiverVID")]
    public string ReceiverVId { get; set; }

    [MaxLength(250)]
    [JsonPropertyName("receiverVID")]
    [XmlElement("ReceiverVID")]
    public string DeviceId { get; set; }

    [Required]
    [MinLength(11), MaxLength(11)]  // TODO: is this a hard requirement??
    [JsonPropertyName("mobileNo")]
    [XmlElement("MobileNo")]
    public string MobileNumber { get; set; }

    [JsonPropertyName("location")]
    [MaxLength(30)]
    [XmlElement("Location")]
    public string Location { get; set; }

    [MaxLength(15)]
    [JsonPropertyName("ip")]
    [XmlElement("IP")]
    public string IPAddress { get; set; }

    [Required]
    [JsonPropertyName("referenceNo")]
    [XmlElement("ReferenceNo")]
    [MaxLength(22)]
    public string ReferenceNumber { get; set; }

    [MaxLength(22)]
    [JsonPropertyName("refNo_Transaction")]
    [XmlElement("RefNo_Transaction")]
    public string ReferenceNumberTransaction { get; set; }

    [JsonPropertyName("reqAmount")]
    [XmlElement("ReqAmount")]
    [DataType(DataType.Currency)]
    public decimal RequestAmount { get; set; }

    [Required]
    [MaxLength(35)]
    [JsonPropertyName("purpose")]
    [XmlElement("Purpose")]
    public string Purpose { get; set; }

    [Required]
    [MinLength(5), MaxLength(25)]
    [JsonPropertyName("call_From")]
    [XmlElement("Call_From")]
    public string CallFrom { get; set; }

    public string GenerateXmlPayload()
    {
        var xmlSerializer = new XmlSerializer(typeof(CreateRTPRequest));

        XmlDocument doc = new XmlDocument();
        using (XmlWriter writer = doc.CreateNavigator().AppendChild())
        {
            // Do this directly 
            //writer.WriteStartDocument();
            //writer.WriteStartElement("root");
            //writer.WriteElementString("foo", "bar");
            //writer.WriteEndElement();
            //writer.WriteEndDocument();

            // or anything else you want to with writer, like calling functions etc.
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };


            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("Binimoy", "http://Binimoy.gov.bd/xxx/schema/");

            xmlSerializer.Serialize(writer, this, namespaces);

            return writer.ToString();
        }
    }
}
public enum ChannelId
{
    Online,
    Mobile,
    Others
}