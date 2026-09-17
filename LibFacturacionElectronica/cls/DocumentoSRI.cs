using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LibFacturacionElectronica.cls
{
    public class DocumentoSRI
    {
        [XmlElement(ElementName = "infoTributaria")]
        public InfoTributaria InfoTributaria { get; set; }

        [XmlElement(ElementName = "infoAdicional")]
        public InfoAdicional InfoAdicional { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }
}
