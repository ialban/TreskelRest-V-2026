using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LibFacturacionElectronica.cls
{
	[XmlRoot(ElementName = "maquinaFiscal")]
	public class MaquinaFiscal
	{
		[XmlElement(ElementName = "marca")]
		public string Marca { get; set; }
		[XmlElement(ElementName = "modelo")]
		public string Modelo { get; set; }
		[XmlElement(ElementName = "serie")]
		public string Serie { get; set; }
	}
}
