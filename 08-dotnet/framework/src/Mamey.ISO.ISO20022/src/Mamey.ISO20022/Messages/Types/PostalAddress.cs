using System.Xml.Serialization;

namespace Mamey.ISO20022.Messages.Types;

public class PostalAddress
{
    [XmlElement("AdrTp")]
    public string? AddressType { get; set; }
    [XmlElement("Dept")]
    public string? Department { get; set; }
    [XmlElement("SubDept")]
    public string? SubDepartment { get; set; }
    [XmlElement("StrtNm")]
    public string? StreetName { get; set; }
    [XmlElement("BldgNb")]
    public string? BuildingNumber { get; set; }
    [XmlElement("Flr")]
    public string? Floor { get; set; }
    [XmlElement("PstBx")]
    public string? PostBox { get; set; }
    [XmlElement("Room")]
    public string? Room { get; set; }
    [XmlElement("PstCd")]
    public string? PostCode { get; set; }
    [XmlElement("TwnName")]
    public string? TownName { get; set; }
    [XmlElement("TwnLctnNm")]
    public string? TownLocationName { get; set; }
    [XmlElement("DstrctNm")]
    public string? DistrictName { get; set; }
    [XmlElement("CountrySubDivision")]
    public string? SubDivision { get; set; }
    [XmlElement("Ctry")]
    public string? Country { get; set; }
    [XmlElement("AdrLine")]
    public List<string> AddressLine { get; set; } = new();
}