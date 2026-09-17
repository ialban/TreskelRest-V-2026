using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LibFacturacionElectronica.cls
{
    [XmlRoot(ElementName = "totalConImpuestos")]
    public class TotalConImpuestos
    {
        [XmlElement(ElementName = "totalImpuesto")]
        public List<TotalImpuesto> TotalImpuesto { get; set; }
    }
}
