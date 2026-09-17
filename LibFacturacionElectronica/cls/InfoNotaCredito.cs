using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LibFacturacionElectronica.cls
{
	[XmlRoot(ElementName = "infoNotaCredito")]
	public class InfoNotaCredito
	{
		[XmlElement(ElementName = "fechaEmision")]
		public string FechaEmision { get; set; }

		[XmlElement(ElementName = "dirEstablecimiento")]
		public string DirEstablecimiento { get; set; }

		[XmlElement(ElementName = "tipoIdentificacionComprador")]
		public string TipoIdentificacionComprador { get; set; }

		[XmlElement(ElementName = "razonSocialComprador")]
		public string RazonSocialComprador { get; set; }

		[XmlElement(ElementName = "identificacionComprador")]
		public string IdentificacionComprador { get; set; }

		[XmlElement(ElementName = "contribuyenteEspecial")]
		public string ContribuyenteEspecial { get; set; }

		[XmlElement(ElementName = "obligadoContabilidad")]
		public string ObligadoContabilidad { get; set; }

		[XmlElement(ElementName = "rise")]
		public string Rise { get; set; }

		[XmlElement(ElementName = "codDocModificado")]
		public string CodDocModificado { get; set; }

		[XmlElement(ElementName = "numDocModificado")]
		public string NumDocModificado { get; set; }

		[XmlElement(ElementName = "fechaEmisionDocSustento")]
		public string FechaEmisionDocSustento { get; set; }

		[XmlElement(ElementName = "totalSinImpuestos")]
		public string TotalSinImpuestos { get; set; }

		//[XmlElement(ElementName = "compensaciones", IsNullable =false)]
		//public List<Compensacion> Compensaciones { get; set; }

		[XmlElement(ElementName = "valorModificacion")]
		public string ValorModificacion { get; set; }

		[XmlElement(ElementName = "moneda")]
		public string Moneda { get; set; }

		[XmlElement(ElementName = "totalConImpuestos")]
		//public List<TotalConImpuesto> TotalConImpuestos { get; set; }
		public TotalConImpuestos TotalConImpuestos { get; set; }

		[XmlElement(ElementName = "motivo")]
		public string Motivo { get; set; }
	}
}
