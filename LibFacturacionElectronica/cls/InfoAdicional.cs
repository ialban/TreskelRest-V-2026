using System.Collections.Generic;
using System.Xml.Serialization;

namespace LibFacturacionElectronica.cls
{
    [XmlRoot(ElementName = "infoAdicional")]
    public class InfoAdicional
    {
        [XmlElement(ElementName = "campoAdicional")]
        public List<CampoAdicional> CampoAdicional { get; set; }
    }
}
