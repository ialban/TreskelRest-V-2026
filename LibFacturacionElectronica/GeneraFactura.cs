using LibElectSign.model;
using LibFacturacionElectronica.cls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibFacturacionElectronica
{
    public class GeneraFactura : TiposDatos
    {
        public TipoAmbiente ambiente { get; set; } = TipoAmbiente.Produccion;
        public TipoEmision emision { get; set; } = TipoEmision.EmisionNormal;
        public int codigoNumerico { get; set; }
        public string razonSocial { get; set; }
        public string nombreComercial { get; set; }
        public string claveAcceso { get; set; }
        public string ruc { get; set; }
        public string direccionMatriz { get; set; }
        public string serie { get; set; }
        public int secuencial { get; set; }
        public TipoComprobante codDoc { get; set; } = TipoComprobante.Factura;
        public string establecimiento { get; set; } = "001";
        public string numeroBlock { get; set; } = "001";
        public string direccionEstablecimiento { get; set; }
        public string puntoEmision { get; set; } = "001";
        public AgenteRetencion agenteRetencion {get; set;}
        public bool esRimpe { get; set; }
        public LLevaContabilidad llevaContabilidad { get; set; }
        public string IdComprador { get; set; }
        public DateTime fechaEmision { get; set; }
        public TipoIdentificadorComprador tipoIdentificadorComprador { get; set; }
        public string razonSocialComprador { get; set; }
        public string direccionComprador { get; set; }
        public decimal totalSinImpuesto { get; set; }
        public decimal totalDescuento { get; set; }
        public decimal importeTotal { get; set; }
        public int porcentajeIva { get; set; }
        public List<TotalImpuesto> totalImpuestos { get; set; }
        public decimal propina { get; set; } = 0;
        public Moneda moneda { get; set; } = Moneda.DOLAR;
        public List<Pago> pagos { get; set; }
        public decimal? valorRetencionIva { get; set; }
        public decimal? valorRetencionRenta { get; set; }
        public List<Detalle> detalles { get; set; }
        public List<InformacionAdicional> informacionAdicionales { get; set; }

        public string CertificadoClave { get; set; }
        public string firmaFile { get; set; }
        public string xmlPath { get; set; }
        public string xmlFolder { get; set; }

        public bool ArchivoFirmado { get; set; }
        public string MensajeError { get; set; } = string.Empty;

        /* Para Retenciones */
        public string PeriodoFiscal { get; set; } // MM/YYYY
        #region Propiedades para Retenciones
        public string RucProveedor { get; set; }
        public string DireccionProveedor { get; set; }
        public string RazonSocialProveedor { get; set; }
        public string EmailProveedor { get; set; }
        public TipoIdentificadorComprador tipoIdentificadorProveedor { get; set; }
        public List<ImpuestoRetencion> impuestos { get; set; }
        #endregion

        #region Propiedades para Nota de Crédito
        public string FechaEmision { get; set; }
        public InfoNotaCredito infoNotaCredito { get; set; }
        #endregion

        private static readonly XmlSerializerNamespaces XmlNamespaces = CrearNamespaces();

        private static readonly XmlSerializer FacturaSerializer = new(typeof(Factura));

        private static readonly XmlSerializer RetencionSerializer = new(typeof(Retencion));

        private static readonly XmlSerializer NotaCreditoSerializer = new(typeof(NotaCredito));

        //private static readonly XmlSerializer NotaDebitoSerializer = new(typeof(NotaDebito)); // TODO : Nota Debito no existe aún

        private static XmlSerializerNamespaces CrearNamespaces()
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            return ns;
        }

        // ----------------------------------------------------------------------------------

        public TiposDatos.TarifaIVA getTipoTarifaIVA()
        {
            TiposDatos.TarifaIVA tipo = TarifaIVA.Doce;

            if (porcentajeIva >= 8 && porcentajeIva < 12)
            {
                return TarifaIVA.IvaDiferenciado;
            }

            switch (porcentajeIva)
            {
                case 12:
                    tipo = TarifaIVA.Doce;
                    break;
                case 13:
                    tipo = TarifaIVA.Trece;
                    break;
                case 14:
                    tipo = TarifaIVA.Catorce;
                    break;
                case 15:
                    tipo = TarifaIVA.Quince;
                    break;
                case 0:
                    tipo = TarifaIVA.Cero;
                    break;
            }
            return tipo;
        }

        private string getTarifaIVA(TarifaIVA tarifa, int porcentaje) =>
        tarifa switch
        {
            TarifaIVA.Cinco => "5",
            TarifaIVA.Doce => "12",
            TarifaIVA.Trece => "13",
            TarifaIVA.Catorce => "14",
            TarifaIVA.Quince => "15",
            TarifaIVA.IvaDiferenciado => porcentaje.ToString(),
            _ => "0"
        };

        public int generaCodigo()
        {
            int suma = 0;
            foreach(char numero in ruc)
            {
                suma += int.Parse(numero.ToString());
            }

            foreach (char numero in establecimiento)
            {
                suma += int.Parse(numero.ToString());
            }

            return suma;
        }

        private string creaDirectorio(string tipo)
        {
            var rutaFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//";

            if (xmlFolder != "")
            {
                if (!Directory.Exists(xmlFolder))
                {
                    Directory.CreateDirectory(xmlFolder);
                }

                rutaFolder = xmlFolder + "\\Enviados\\";

                if (!Directory.Exists(rutaFolder))
                {
                    Directory.CreateDirectory(rutaFolder);
                }

                switch (tipo)
                {
                    case "F":
                        rutaFolder = Path.Combine(rutaFolder, "Factura");
                        break;
                    case "R":
                        rutaFolder = Path.Combine(rutaFolder, "Retencion");
                        break;
                    case "C":
                        rutaFolder = Path.Combine(rutaFolder, "NotaCredito");
                        break;
                    case "D":
                        rutaFolder = Path.Combine(rutaFolder, "NotaDebito");
                        break;
                }

                if (!Directory.Exists(rutaFolder))
                {
                    Directory.CreateDirectory(rutaFolder);
                }

                string recibidos = xmlFolder + "\\Recividos\\";

                if (!Directory.Exists(recibidos))
                {
                    Directory.CreateDirectory(recibidos);
                }
            }

            return rutaFolder;
        }

        /*private string creaXML<T>(string rutaFolder, string FileName, T documento)
        {
            var xns = new XmlSerializerNamespaces();
            XmlSerializer xmlFactura = new XmlSerializer(documento.GetType());
            xns.Add(string.Empty, string.Empty);

            var path = rutaFolder + FileName;

            using var file = File.Create(path);
            xmlFactura.Serialize(file, documento, xns);
            file.Close();

            return path;
        }*/

        private string creaXML<T>(XmlSerializer serializer, string ruta, string nombre, T documento)
        {
            var path = Path.Combine(ruta, nombre);
            using var file = File.Create(path);

            serializer.Serialize(file, documento, XmlNamespaces);
            file.Close();

            return path;
        }

        public string generarRetencion()
        {
            var tipoIdentificadorProveedor = ((int)this.tipoIdentificadorProveedor).ToString().PadLeft(2, '0');

            var tipoComprobante = ((int)TipoComprobante.ComprobanteRetencion).ToString().PadLeft(2, '0');
            var tipoAmbiente = ((int)ambiente).ToString();
            var codigoNumerico = this.codigoNumerico.ToString().PadLeft(8, '0');
            var tipoEmision = ((int)emision).ToString();
            var secuencial = this.secuencial.ToString().PadLeft(9, '0');

            GeneraClaveAcceso claveAcceso = new GeneraClaveAcceso();

            Retencion retencion = new Retencion();
            retencion.Id = "comprobante";
            retencion.Version = "1.0.0";

            retencion.InfoTributaria = new InfoTributaria();
            retencion.InfoTributaria.Ambiente = tipoAmbiente;
            retencion.InfoTributaria.TipoEmision = tipoEmision;
            retencion.InfoTributaria.RazonSocial = razonSocial;
            retencion.InfoTributaria.NombreComercial = nombreComercial;
            retencion.InfoTributaria.Ruc = ruc;
            retencion.InfoTributaria.DirMatriz = direccionMatriz;
            retencion.InfoTributaria.Secuencial = secuencial;
            retencion.InfoTributaria.CodDoc = tipoComprobante;
            retencion.InfoTributaria.PtoEmi = puntoEmision;
            retencion.InfoTributaria.Estab = establecimiento;

            retencion.InfoTributaria.ClaveAcceso =
                claveAcceso.getClaveAcceso(ruc, fechaEmision, tipoComprobante, tipoAmbiente, serie, secuencial, codigoNumerico, tipoEmision);
            this.claveAcceso = retencion.InfoTributaria.ClaveAcceso;

            retencion.InfoCompRetencion = new InfoCompRetencion();

            retencion.InfoCompRetencion.FechaEmision = fechaEmision.ToString("dd/MM/yyyy");
            retencion.InfoCompRetencion.DirEstablecimiento = direccionEstablecimiento;
            retencion.InfoCompRetencion.ObligadoContabilidad = llevaContabilidad.ToString();
            retencion.InfoCompRetencion.TipoIdentificacionSujetoRetenido = tipoIdentificadorProveedor;
            retencion.InfoCompRetencion.IdentificacionSujetoRetenido = RucProveedor;
            retencion.InfoCompRetencion.PeriodoFiscal = PeriodoFiscal.ToString();
            retencion.InfoCompRetencion.RazonSocialSujetoRetenido = RazonSocialProveedor;

            retencion.Impuestos = new ImpuestosRetencion();
            retencion.Impuestos.Impuesto = impuestos.Select(c => new cls.ImpuestoRetencion
            {
                Codigo = c.Codigo,
                CodigoRetencion = c.CodigoRetencion,
                BaseImponible = c.BaseImponible,
                FechaEmisionSustento = c.FechaEmisionSustento,
                CodDocSustento = c.CodDocSustento,
                NumDocSustento = c.NumDocSustento,
                PorcentajeRetener = c.PorcentajeRetener,
                Valor = c.Valor
            }).ToList() ;

            retencion.InfoAdicional = CrearInformacionAdicional();

            string FileName = "R-" + retencion.InfoTributaria.ClaveAcceso + ".xml";

            string rutaFolder = creaDirectorio("R");

            string path = creaXML<Retencion>(RetencionSerializer, rutaFolder, FileName, retencion);

            var respFirma = Firmar(path);

            ArchivoFirmado = respFirma.success;
            MensajeError = respFirma.message;

            return path;
        }

        public string generarFactura()
        {
            var serie = this.serie.Replace("-", "");
            var secuencial = this.secuencial.ToString().PadLeft(9, '0');
            var tipoComprobante = ((int)codDoc).ToString().PadLeft(2, '0');
            var tipoAmbiente = ((int)ambiente).ToString();
            var tipoEmision = ((int)emision).ToString();
            var codigoNumerico = this.codigoNumerico.ToString().PadLeft(8, '0');
            var tipoIdentificadorComprador = ((int)this.tipoIdentificadorComprador).ToString().PadLeft(2, '0');

            GeneraClaveAcceso claveAcceso = new GeneraClaveAcceso();

            Factura factura = new Factura();
            factura.Id = "comprobante";
            factura.Version = "1.1.0";
            //---------------------------------------------------------------------------------------------

            factura.InfoTributaria = new InfoTributaria();
            factura.InfoTributaria.Ambiente = tipoAmbiente;
            factura.InfoTributaria.TipoEmision = tipoEmision;
            factura.InfoTributaria.RazonSocial = razonSocial;
            factura.InfoTributaria.NombreComercial = nombreComercial;
            factura.InfoTributaria.Ruc = ruc;
            factura.InfoTributaria.DirMatriz = direccionMatriz;
            factura.InfoTributaria.Secuencial = secuencial;
            factura.InfoTributaria.CodDoc = tipoComprobante;
            factura.InfoTributaria.PtoEmi = puntoEmision;
            factura.InfoTributaria.Estab = establecimiento;
            if (esRimpe)
            {
                factura.InfoTributaria.ContribuyenteRimpe = "CONTRIBUYENTE RÉGIMEN RIMPE";
            }

            if (agenteRetencion != AgenteRetencion.NO)
            {
                factura.InfoTributaria.AgenteRetencion = "1";//agenteRetencion == AgenteRetencion.SI ? "1" : "2";
            }
            
            factura.InfoTributaria.ClaveAcceso = 
                claveAcceso.getClaveAcceso(ruc, fechaEmision, tipoComprobante, tipoAmbiente, serie, secuencial, codigoNumerico, tipoEmision) ;
            this.claveAcceso = factura.InfoTributaria.ClaveAcceso;
            //----------------------------------------------------------------------------------------------
            factura.InfoFactura = new InfoFactura();

            factura.InfoFactura.ObligadoContabilidad = llevaContabilidad.ToString();
            factura.InfoFactura.IdentificacionComprador = IdComprador;
            factura.InfoFactura.FechaEmision = fechaEmision.ToString("dd/MM/yyyy");
            factura.InfoFactura.DirEstablecimiento = direccionEstablecimiento;
            factura.InfoFactura.RazonSocialComprador = razonSocialComprador;
            factura.InfoFactura.TipoIdentificacionComprador = tipoIdentificadorComprador;
            factura.InfoFactura.DireccionComprador = direccionComprador;
            factura.InfoFactura.TotalSinImpuestos = totalSinImpuesto.ToString(CultureInfo.InvariantCulture);//.Replace(",",".");
            factura.InfoFactura.TotalDescuento = totalDescuento.ToString(CultureInfo.InvariantCulture);//.Replace(",", ".");
            factura.InfoFactura.TotalConImpuestos = new TotalConImpuestos();
            factura.InfoFactura.TotalConImpuestos.TotalImpuesto = new List<cls.TotalImpuesto>();

            // Agregando detalle de totales con impuesto
            factura.InfoFactura.TotalConImpuestos.TotalImpuesto = totalImpuestos.Select(c => new cls.TotalImpuesto
            {
                Codigo = ((int)c.codigo).ToString(),
                CodigoPorcentaje = ((int)c.CodigoPorcentaje).ToString(CultureInfo.InvariantCulture),//.Replace(",", "."),
                DescuentoAdicional = "0",
                BaseImponible = (Math.Round(c.BaseImponible, 2)).ToString(CultureInfo.InvariantCulture),//.Replace(",", "."),
                Tarifa = getTarifaIVA(c.CodigoPorcentaje, porcentajeIva),
                Valor = Math.Round(c.Valor, 2).ToString(CultureInfo.InvariantCulture)//.Replace(",", ".")
            }).ToList();

            factura.InfoFactura.ImporteTotal = importeTotal.ToString(CultureInfo.InvariantCulture);//.Replace(",",".");
            factura.InfoFactura.Propina = propina.ToString(CultureInfo.InvariantCulture);//.Replace(",", ".");
            factura.InfoFactura.Moneda = moneda.ToString(CultureInfo.InvariantCulture);//.Replace(",", ".");

            factura.InfoFactura.Pagos = new Pagos();
            factura.InfoFactura.Pagos.Pago = new List<cls.Pago>();

            factura.InfoFactura.Pagos.Pago = pagos.Select(c => new cls.Pago() { 
                FormaPago = ((int)c.formaDePago).ToString().PadLeft(2, '0'),
                Total = c.Total.ToString("0.00").Replace(",", "."),
                Plazo = c.Plazo == 0 ? null : c.Plazo.ToString(),
                UnidadTiempo = ((int)c.TiempoPlazo) == 0 ? null : ((int)c.TiempoPlazo).ToString()
            }).ToList();
            
            if (valorRetencionIva.HasValue)
            {
                factura.InfoFactura.ValorRetIva = valorRetencionIva.Value.ToString("0.00######", CultureInfo.InvariantCulture);//.Replace(",", ".");
            }

            if (valorRetencionRenta.HasValue)
            {
                factura.InfoFactura.ValorRetRenta = valorRetencionRenta.Value.ToString(CultureInfo.InvariantCulture);//.Replace(",", ".");
            }

            factura.Detalles = new Detalles();
            factura.Detalles.Detalle = new List<cls.Detalle>();
            foreach (Detalle detalle in detalles)
            {
                var cantidad = detalle.Cantidad;
                var impuestos = new Impuestos();
                impuestos.Impuesto = detalle.Impuesto.Select(c => new cls.Impuesto
                {
                    Codigo = ((int)c.codigo).ToString(),
                    CodigoPorcentaje = ((int)c.CodigoPorcentaje).ToString(),
                    BaseImponible = c.BaseImponible.ToString("0.00######",CultureInfo.InvariantCulture),//.Replace(",", "."),
                    Valor = (c.Valor).ToString("0.00######", CultureInfo.InvariantCulture),//.Replace(",", "."),
                    Tarifa = getTarifaIVA(c.CodigoPorcentaje, porcentajeIva)
                }).ToList();

                factura.Detalles.Detalle.Add(new cls.Detalle
                {
                    CodigoPrincipal = detalle.CodigoPrincipal,
                    CodigoAuxiliar = detalle.CodigoAuxiliar,
                    Cantidad = detalle.Cantidad.ToString(),
                    Descripcion = detalle.Descripcion,
                    Descuento = detalle.Descuento.ToString("0.00######", CultureInfo.InvariantCulture),//.Replace(",", "."),
                    PrecioUnitario = detalle.PrecioUnitario.ToString("0.00######", CultureInfo.InvariantCulture),//.Replace(",", "."),
                    PrecioTotalSinImpuesto = detalle.PrecioTotalSinImpuesto.ToString("0.00######", CultureInfo.InvariantCulture),//.Replace(",", "."),
                    Impuestos = impuestos
                }); ;
            }

            factura.InfoAdicional = CrearInformacionAdicional();

            //----------------------------------------------------------------------------------------------
            DateTime fecha = DateTime.Now;
            string anio = fecha.Year.ToString();
            string mes = fecha.Month.ToString().PadLeft(2, '0');
            string dia = fecha.Day.ToString().PadLeft(2, '0');
            string hora = fecha.Hour.ToString().PadLeft(2, '0');
            string minuto = fecha.Minute.ToString().PadLeft(2, '0');
            string segundo = fecha.Second.ToString().PadLeft(2, '0');

            string FileName = anio + "-" + mes + "-" + dia + "_" + hora + "-" + minuto + "-" + segundo;
            FileName = factura.InfoTributaria.ClaveAcceso + ".xml";

            string rutaFolder = creaDirectorio("F");

            var path = creaXML<Factura>(FacturaSerializer, rutaFolder, FileName, factura);

            var respFirma = Firmar(path);

            ArchivoFirmado = respFirma.success;
            MensajeError = respFirma.message;

            return path;
        }

        public string generarNotaCredito()
        {
            var tipoIdentificadorProveedor = ((int)this.tipoIdentificadorProveedor).ToString().PadLeft(2, '0');

            var tipoComprobante = ((int)TipoComprobante.ComprobanteRetencion).ToString().PadLeft(2, '0');
            var tipoAmbiente = ((int)ambiente).ToString();
            var codigoNumerico = this.codigoNumerico.ToString().PadLeft(8, '0');
            var tipoEmision = ((int)emision).ToString();
            var secuencial = this.secuencial.ToString().PadLeft(9, '0');

            GeneraClaveAcceso claveAcceso = new GeneraClaveAcceso();

            NotaCredito notaCredito = new NotaCredito();
            notaCredito.Id = "comprobante";
            notaCredito.Version = "1.0.0";

            notaCredito.InfoTributaria = new InfoTributaria();
            notaCredito.InfoTributaria.Ambiente = tipoAmbiente;
            notaCredito.InfoTributaria.TipoEmision = tipoEmision;
            notaCredito.InfoTributaria.RazonSocial = razonSocial;
            notaCredito.InfoTributaria.NombreComercial = nombreComercial;
            notaCredito.InfoTributaria.Ruc = ruc;
            notaCredito.InfoTributaria.DirMatriz = direccionMatriz;
            notaCredito.InfoTributaria.Secuencial = secuencial;
            notaCredito.InfoTributaria.CodDoc = tipoComprobante;
            notaCredito.InfoTributaria.PtoEmi = puntoEmision;
            notaCredito.InfoTributaria.Estab = establecimiento;

            if (this.claveAcceso == "")
            {
                // Cuando la Clave de Acceso no existe, debe ser generada
                //----------------------------------------------------------------------
                notaCredito.InfoTributaria.ClaveAcceso =
                    claveAcceso.getClaveAcceso(ruc, fechaEmision, tipoComprobante, tipoAmbiente, serie, secuencial, codigoNumerico, tipoEmision);
                this.claveAcceso = notaCredito.InfoTributaria.ClaveAcceso;
            }
            else
            {
                notaCredito.InfoTributaria.ClaveAcceso = this.claveAcceso;
            }

            notaCredito.InfoNotaCredito = infoNotaCredito;


            //notaCredito.Detalles = new NotaCreditoDetalle();

            if (informacionAdicionales != null && informacionAdicionales.Count > 0)
            {
                notaCredito.InfoAdicional = CrearInformacionAdicional();
            }

            if (detalles != null && detalles.Count > 0)
            {
                notaCredito.Detalles = new NotaCreditoDetalle();
                notaCredito.Detalles.detalle = new List<DetalleNotaCredito>();

                foreach (var detalle in detalles)
                {
                    var cantidad = detalle.Cantidad;
                    var impuestos = new Impuestos();
                    impuestos.Impuesto = detalle.Impuesto.Select(c => new cls.Impuesto
                    {
                        Codigo = ((int)c.codigo).ToString(),
                        CodigoPorcentaje = ((int)c.CodigoPorcentaje).ToString(),
                        BaseImponible = Math.Round(c.BaseImponible,6).ToString("0.00######", CultureInfo.InvariantCulture),//.Replace(",", "."),
                        Valor = (Math.Round(c.Valor,6)).ToString("0.00######", CultureInfo.InvariantCulture),//.Replace(",", "."),
                        Tarifa = getTarifaIVA(c.CodigoPorcentaje, porcentajeIva)
                    }).ToList();

                    notaCredito.Detalles.detalle.Add(new DetalleNotaCredito { 
                        CodigoInterno = detalle.CodigoPrincipal,
                        CodigoAuxiliar = detalle.CodigoAuxiliar,
                        Descripcion = detalle.Descripcion,
                        Cantidad = string.Format("{0:0.00}",detalle.Cantidad),
                        PrecioUnitario = string.Format("{0:0.000000}", detalle.PrecioUnitario),
                        Descuento = string.Format("{0:0.000000}", detalle.Descuento),
                        PrecioTotalSinImpuesto = string.Format("{0:0.000000}", detalle.PrecioTotalSinImpuesto),
                        Impuestos = impuestos
                    });
                }
            }

            string FileName = "NCR-" + notaCredito.InfoTributaria.ClaveAcceso + ".xml";

            string rutaFolder = creaDirectorio("C");

            string path = creaXML<NotaCredito>(NotaCreditoSerializer, rutaFolder, FileName, notaCredito);

            var respFirma = Firmar(path);

            ArchivoFirmado = respFirma.success;
            MensajeError = respFirma.message;

            return path;
        }

        public SignResponse Firmar(string xmlfile)
        {
            SignResponse resp = new SignResponse();
            try
            {
                LibElectSign.FirmarXML firmarXml = new LibElectSign.FirmarXML();

                firmarXml.CertificadoClave = this.CertificadoClave;
                firmarXml.firmaPath = firmaFile;
                firmarXml.xmlPath = xmlfile;

                //var res = firmarXml.SignXMLAsync().GetAwaiter().GetResult();
                resp = Task.Run(
                    async() => await firmarXml.SignXMLAsync().ConfigureAwait(false)    
                ).GetAwaiter().GetResult();
                if (resp.success) {
                    return resp;
                };

                return resp;
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.Message);
                System.Console.WriteLine(e.Message);
                return resp = new SignResponse
                {
                    success = false,
                    message = e.Message
                };
            }
        }

        private InfoAdicional CrearInformacionAdicional()
        {
            if (informacionAdicionales == null || !informacionAdicionales.Any())
                return null;

            return new InfoAdicional
            {
                CampoAdicional = informacionAdicionales
                    .Where(i => !string.IsNullOrWhiteSpace(i.Descripcion))
                    .Select(i => new CampoAdicional
                    {
                        Nombre = i.Nombre,
                        Text = i.Descripcion
                    })
                    .ToList()
            };
        }
    }
}
