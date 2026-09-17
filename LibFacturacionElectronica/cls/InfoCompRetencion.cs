using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LibFacturacionElectronica.cls
{
	[XmlRoot(ElementName = "infoCompRetencion")]
	public class InfoCompRetencion
	{
		[XmlElement(ElementName = "fechaEmision")]
		public string FechaEmision { get; set; }
		[XmlElement(ElementName = "dirEstablecimiento")]
		public string DirEstablecimiento { get; set; }
		[XmlElement(ElementName = "contribuyenteEspecial")]
		public string ContribuyenteEspecial { get; set; }
		[XmlElement(ElementName = "obligadoContabilidad")]
		public string ObligadoContabilidad { get; set; }
		[XmlElement(ElementName = "tipoIdentificacionSujetoRetenido")]
		public string TipoIdentificacionSujetoRetenido { get; set; }
		[XmlElement(ElementName = "razonSocialSujetoRetenido")]
		public string RazonSocialSujetoRetenido { get; set; }
		[XmlElement(ElementName = "identificacionSujetoRetenido")]
		public string IdentificacionSujetoRetenido { get; set; }
		[XmlElement(ElementName = "periodoFiscal")]
		public string PeriodoFiscal { get; set; }
	}
}
