using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LibFacturacionElectronica.cls
{
    [XmlRoot(ElementName = "notaCredito")]
    public class NotaCredito
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id;

        [XmlAttribute(AttributeName = "version")]
        public string Version;

        [XmlElement(ElementName = "infoTributaria", Order = 1)]
        public InfoTributaria InfoTributaria { get; set; }

        [XmlElement(ElementName = "infoNotaCredito", Order = 2)]
        public InfoNotaCredito InfoNotaCredito;

        [XmlElement(ElementName = "detalles", Order = 3)]
        public NotaCreditoDetalle Detalles { get; set; }

        //[XmlElement(ElementName = "maquinaFiscal")]
        //public NotaCreditoMaquinaFiscal MaquinaFiscal { get; set; }

        [XmlElement(ElementName = "infoAdicional", Order = 5)]
        public InfoAdicional InfoAdicional { get; set; }

    }

    [XmlRoot(ElementName = "detalles")]
    public class NotaCreditoDetalle
    {
        //[XmlArrayItemAttribute("detalle")]
        [XmlElement(ElementName = "detalle")]
        public List<DetalleNotaCredito> detalle { get; set; }
    }

    public class NotaCreditoMaquinaFiscal
    {
        [XmlElement(ElementName = "marca")]
        public string Marca { get; set; }
        [XmlElement(ElementName = "modelo")]
        public string Modelo { get; set; }
        [XmlElement(ElementName = "serie")]
        public string Serie { get; set; }
    }
}
