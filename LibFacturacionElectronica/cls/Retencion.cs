using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace LibFacturacionElectronica.cls
{
	[XmlRoot(ElementName = "impuestos")]
	public class ImpuestosRetencion
	{
		[XmlElement(ElementName = "impuesto")]
		public List<ImpuestoRetencion> Impuesto { get; set; }
	}

	[XmlRoot(ElementName = "impuesto")]
	public class ImpuestoRetencion
	{
		[XmlElement(ElementName = "codigo")]
		[Description("Código asociado a la retención")]
		public string Codigo { get; set; }
		[XmlElement(ElementName = "codigoRetencion")]
		[Description("Código del SRI para la retención")]
		public string CodigoRetencion { get; set; }
		[Description("Valor de la Base Imponible de la retención")]
		[XmlElement(ElementName = "baseImponible")]
		public string BaseImponible { get; set; }
		[XmlElement(ElementName = "porcentajeRetener")]
		public string PorcentajeRetener { get; set; }
		[XmlElement(ElementName = "valorRetenido")]
		public string Valor { get; set; }
		[XmlElement(ElementName = "codDocSustento")]
		public string CodDocSustento { get; set; }
		[XmlElement(ElementName = "numDocSustento")]
		public string NumDocSustento { get; set; }
		[XmlElement(ElementName = "fechaEmisionDocSustento")]
		public string FechaEmisionSustento { get; set; }
		//[XmlElement(ElementName = "tarifa")]
		//public string Tarifa { get; set; }
	}

	[XmlRoot(ElementName = "comprobanteRetencion")]
	public class Retencion
    {
		[XmlElement(ElementName = "infoTributaria")]
		public InfoTributaria InfoTributaria { get; set; }
		[XmlElement(ElementName = "infoCompRetencion")]
		public InfoCompRetencion InfoCompRetencion { get; set; }
		[XmlElement(ElementName = "impuestos")]
		public ImpuestosRetencion Impuestos { get; set; }
		[XmlElement(ElementName = "maquinaFiscal")]
		public MaquinaFiscal MaquinaFiscal { get; set; }
		[XmlElement(ElementName = "infoAdicional")]
		public InfoAdicional InfoAdicional { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "version")]
		public string Version { get; set; }
	}
}
