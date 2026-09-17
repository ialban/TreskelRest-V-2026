using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LibFacturacionElectronica.cls
{
    [XmlRoot(ElementName = "factura", Namespace = "")]
    public class Factura
    {
        [XmlElement(ElementName = "infoTributaria")]
        public InfoTributaria InfoTributaria { get; set; }
        [XmlElement(ElementName = "infoFactura")]
        public InfoFactura InfoFactura { get; set; }
        [XmlElement(ElementName = "detalles")]
        public Detalles Detalles { get; set; }
        [XmlElement(ElementName = "infoAdicional")]
        public InfoAdicional InfoAdicional { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }

    [XmlRoot(ElementName = "totalImpuesto")]
    public class TotalImpuesto
    {
        [XmlElement(ElementName = "codigo")]
        public string Codigo { get; set; }
        [XmlElement(ElementName = "codigoPorcentaje")]
        public string CodigoPorcentaje { get; set; }
        [XmlElement(ElementName = "descuentoAdicional")]
        public string DescuentoAdicional { get; set; }
        [XmlElement(ElementName = "baseImponible")]
        public string BaseImponible { get; set; }
        [XmlElement(ElementName = "tarifa")]
        public string Tarifa { get; set; }
        [XmlElement(ElementName = "valor")]
        public string Valor { get; set; }
    }

    //[XmlRoot(ElementName = "totalConImpuestos")]
    //public class TotalConImpuestos
    //{
    //    [XmlElement(ElementName = "totalImpuesto")]
    //    public List<TotalImpuesto> TotalImpuesto { get; set; }
    //}

    [XmlRoot(ElementName = "pago")]
    public class Pago
    {
        [XmlElement(ElementName = "formaPago")]
        public string FormaPago { get; set; }
        [XmlElement(ElementName = "total")]
        public string Total { get; set; }
        [XmlElement(ElementName = "plazo")]
        public string Plazo { get; set; }
        [XmlElement(ElementName = "unidadTiempo")]
        public string UnidadTiempo { get; set; }
    }

    [XmlRoot(ElementName = "pagos")]
    public class Pagos
    {
        [XmlElement(ElementName = "pago")]
        public List<Pago> Pago { get; set; }
    }

    [XmlRoot(ElementName = "infoFactura")]
    public class InfoFactura
    {
        [XmlElement(ElementName = "fechaEmision")]
        public string FechaEmision { get; set; }
        [XmlElement(ElementName = "dirEstablecimiento")]
        public string DirEstablecimiento { get; set; }
        [XmlElement(ElementName = "obligadoContabilidad")]
        public string ObligadoContabilidad { get; set; }
        [XmlElement(ElementName = "tipoIdentificacionComprador")]
        public string TipoIdentificacionComprador { get; set; }
        [XmlElement(ElementName = "guiaRemision")]
        public string GuiaRemision { get; set; }
        [XmlElement(ElementName = "razonSocialComprador")]
        public string RazonSocialComprador { get; set; }
        [XmlElement(ElementName = "identificacionComprador")]
        public string IdentificacionComprador { get; set; }
        [XmlElement(ElementName = "direccionComprador")]
        public string DireccionComprador { get; set; }
        [XmlElement(ElementName = "totalSinImpuestos")]
        public string TotalSinImpuestos { get; set; }
        [XmlElement(ElementName = "totalDescuento")]
        public string TotalDescuento { get; set; }
        [XmlElement(ElementName = "totalConImpuestos")]
        public TotalConImpuestos TotalConImpuestos { get; set; }
        [XmlElement(ElementName = "propina")]
        public string Propina { get; set; }
        [XmlElement(ElementName = "importeTotal")]
        public string ImporteTotal { get; set; }
        [XmlElement(ElementName = "moneda")]
        public string Moneda { get; set; }
        [XmlElement(ElementName = "pagos")]
        public Pagos Pagos { get; set; }
        [XmlElement(ElementName = "valorRetIva")]
        public string ValorRetIva { get; set; }
        [XmlElement(ElementName = "valorRetRenta")]
        public string ValorRetRenta { get; set; }
    }

    [XmlRoot(ElementName = "detAdicional")]
    public class DetAdicional
    {
        [XmlAttribute(AttributeName = "nombre")]
        public string Nombre { get; set; }
        [XmlAttribute(AttributeName = "valor")]
        public string Valor { get; set; }
    }

    [XmlRoot(ElementName = "detallesAdicionales")]
    public class DetallesAdicionales
    {
        [XmlElement(ElementName = "detAdicional")]
        public List<DetAdicional> DetAdicional { get; set; }
    }

    [XmlRoot(ElementName = "impuesto")]
    public class Impuesto
    {
        [XmlElement(ElementName = "codigo")]
        public string Codigo { get; set; }
        [XmlElement(ElementName = "codigoPorcentaje")]
        public string CodigoPorcentaje { get; set; }
        [XmlElement(ElementName = "tarifa")]
        public string Tarifa { get; set; }
        [XmlElement(ElementName = "baseImponible")]
        public string BaseImponible { get; set; }
        [XmlElement(ElementName = "valor")]
        public string Valor { get; set; }
    }

    [XmlRoot(ElementName = "impuestos")]
    public class Impuestos
    {
        [XmlElement(ElementName = "impuesto")]
        public List<Impuesto> Impuesto { get; set; }
    }

    [XmlRoot(ElementName = "detalle")]
    public class Detalle
    {
        [XmlElement(ElementName = "codigoPrincipal")]
        public string CodigoPrincipal { get; set; }
        [XmlElement(ElementName = "codigoAuxiliar")]
        public string CodigoAuxiliar { get; set; }
        [XmlElement(ElementName = "descripcion")]
        public string Descripcion { get; set; }
        [XmlElement(ElementName = "cantidad")]
        public string Cantidad { get; set; }
        [XmlElement(ElementName = "precioUnitario")]
        public string PrecioUnitario { get; set; }
        [XmlElement(ElementName = "descuento")]
        public string Descuento { get; set; }
        [XmlElement(ElementName = "precioTotalSinImpuesto")]
        public string PrecioTotalSinImpuesto { get; set; }
        [XmlElement(ElementName = "detallesAdicionales")]
        public DetallesAdicionales DetallesAdicionales { get; set; }
        [XmlElement(ElementName = "impuestos")]
        public Impuestos Impuestos { get; set; }
    }

    [XmlRoot(ElementName = "detalles")]
    public class Detalles
    {
        [XmlElement(ElementName = "detalle")]
        public List<Detalle> Detalle { get; set; }
    }

    [XmlRoot(ElementName = "campoAdicional")]
    public class CampoAdicional
    {
        [XmlAttribute(AttributeName = "nombre")]
        public string Nombre { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public List<Reference> Reference { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureValue
    {
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class RSAKeyValue
    {
        [XmlElement(ElementName = "Modulus", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string Modulus { get; set; }
        [XmlElement(ElementName = "Exponent", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string Exponent { get; set; }
    }

    [XmlRoot(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyValue
    {
        [XmlElement(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public RSAKeyValue RSAKeyValue { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
        [XmlElement(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyValue KeyValue { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "CertDigest", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
    public class CertDigest
    {
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
    }

    [XmlRoot(ElementName = "IssuerSerial", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
    public class IssuerSerial
    {
        [XmlElement(ElementName = "X509IssuerName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509IssuerName { get; set; }
        [XmlElement(ElementName = "X509SerialNumber", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509SerialNumber { get; set; }
    }

    [XmlRoot(ElementName = "Cert", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
    public class Cert
    {
        [XmlElement(ElementName = "CertDigest", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
        public CertDigest CertDigest { get; set; }
        [XmlElement(ElementName = "IssuerSerial", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
        public IssuerSerial IssuerSerial { get; set; }
    }

    [XmlRoot(ElementName = "SigningCertificate", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
    public class SigningCertificate
    {
        [XmlElement(ElementName = "Cert", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
        public Cert Cert { get; set; }
    }

    [XmlRoot(ElementName = "SignedSignatureProperties", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
    public class SignedSignatureProperties
    {
        [XmlElement(ElementName = "SigningTime", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
        public string SigningTime { get; set; }
        [XmlElement(ElementName = "SigningCertificate", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
        public SigningCertificate SigningCertificate { get; set; }
    }

    [XmlRoot(ElementName = "SignedProperties", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
    public class SignedProperties
    {
        [XmlElement(ElementName = "SignedSignatureProperties", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
        public SignedSignatureProperties SignedSignatureProperties { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "QualifyingProperties", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
    public class QualifyingProperties
    {
        [XmlElement(ElementName = "SignedProperties", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
        public SignedProperties SignedProperties { get; set; }
        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }
    }

    [XmlRoot(ElementName = "Object", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Object
    {
        [XmlElement(ElementName = "QualifyingProperties", Namespace = "http://uri.etsi.org/01903/v1.3.2#")]
        public QualifyingProperties QualifyingProperties { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureValue SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlElement(ElementName = "Object", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Object Object { get; set; }
        [XmlAttribute(AttributeName = "ds", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ds { get; set; }
        [XmlAttribute(AttributeName = "etsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Etsi { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }
}
