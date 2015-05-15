using System.Xml.Serialization;

namespace Odes.Licence.Model
{
    public class DatabaseLicenseFragment
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlAttribute("expiration")]
        public string expiration { get; set; }
        [XmlAttribute("type")]
        public string type { get; set; }
        [XmlAttribute("LicenceType")]
        public string LicenceType { get; set; }
        [XmlAttribute("ClicksReqeusted")]
        public int ClicksReqeusted { get; set; }
        [XmlAttribute("Email")]
        public string Email { get; set; }
        [XmlAttribute("UserName")]
        public string UserName { get; set; }
        [XmlAttribute("CompanyName")]
        public string CompanyName { get; set; }
        [XmlAttribute("ServiceQueue")]
        public string ServiceQueue { get; set; }
        [XmlAttribute("SystemId")]
        public string SystemId { get; set; }
        [XmlAttribute("SystemDateTimeStamp")]
        public string SystemDateTimeStamp { get; set; }
        [XmlAttribute("SystemNetworkCredential")]
        public string SystemNetworkCredential { get; set; }
        [XmlAttribute("SystemMachineName")]
        public string SystemMachineName { get; set; }
        [XmlElement("name")]
        public string name { get; set; }
        [XmlElement("Signature")]
        public string Signature { get; set; }
    }
}