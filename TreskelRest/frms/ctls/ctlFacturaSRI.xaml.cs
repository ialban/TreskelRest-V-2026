using Entidades;
using Entidades.modelos;
using LibFacturacionElectronica;
using LibFacturacionElectronica.cls;
using LibHttpClient;
using LibHttpClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlFacturaSRI.xaml
    /// </summary>
    public partial class ctlFacturaSRI : UserControl, IDisposable
    {
        #region Campos internos
 
        private bool disposedValue;
        private bool toProcessEnable = true;
        private bool autenticado = false;
        private string environment = "";
        private string token = "";
 
        private const string TIPO_RETENCION = "RETENCIÓN";
        private const string TIPO_NOTA_CREDITO = "NOTADECREDITO";
 
        private const string ESTADO_AUTORIZADO = "A";
        private const string ESTADO_PENDIENTE = "P";
        private const string ESTADO_NO_PROCESADO = "N";
        private const string ESTADO_DEVUELTA = "D";
        private const string ESTADO_VALIDADO = "V";

        #endregion


        #region Mapas de estado

        private static readonly IReadOnlyDictionary<string, (string Texto, string Imagen)> MapaEstadoVerificar =
            new Dictionary<string, (string, string)>
            {
                [ESTADO_AUTORIZADO] = ("AUTORIZADO", "../../images/authorize2.png"),
                [ESTADO_PENDIENTE] = ("PENDIENTE", "../../images/pending.png"),
                [ESTADO_NO_PROCESADO] = ("NO PROCESADO", "../../images/toprocess.png"),
                [ESTADO_DEVUELTA] = ("DEVUELTA", "../../images/returned.png"),
            };

        private static readonly IReadOnlyDictionary<string, string> MapaTextoCambiado =
            new Dictionary<string, string>
            {
                [ESTADO_AUTORIZADO] = "AUTORIZADO",
                [ESTADO_PENDIENTE] = "PENDIENTE",
                [ESTADO_NO_PROCESADO] = "NO PROCESADO",
                [ESTADO_DEVUELTA] = "DEVUELTA",
                [ESTADO_VALIDADO] = "VALIDADO",
            };

        private static readonly IReadOnlyDictionary<string, string> MapaTextoSinCambio =
            new Dictionary<string, string>
            {
                [ESTADO_AUTORIZADO] = "VERIFICANDO",
                [ESTADO_PENDIENTE] = "PENDIENTE",
                [ESTADO_NO_PROCESADO] = "NO PROCESADO",
                [ESTADO_DEVUELTA] = "DEVUELTA",
                [ESTADO_VALIDADO] = "VALIDADO",
            };

        #endregion

        public ctlFacturaSRI()
        {
            InitializeComponent();
            fecha.SelectedDate = DateTime.Now;

            autenticado = Task.Run(async () => await LoginAsync()).GetAwaiter().GetResult();
        }

        private async Task<bool> LoginAsync()
        {
            try
            {
                var client = new HttpClienteAPI { TrsklUrl = General.URL_WS };

                var resp = await client.Login(General.USUARIO_WS, General.CLAVE_WS, General.RUC_EMPRESA, General.EMPRESA_GUID);

                if (resp != null && !string.IsNullOrEmpty(resp.Token))
                {
                    environment = resp.environment;
                    token = resp.Token;
                    return true;
                }
            } catch (Exception ex) { 
                
            }
            return false;
        }

        private static TiposDatos.TipoIdentificadorComprador GetTipoComprador(string tipo, string valor) => tipo switch
        {
            "C" => TiposDatos.TipoIdentificadorComprador.Cedula,
            "R" => valor == "9999999999999"
                ? TiposDatos.TipoIdentificadorComprador.CosumidorFinal
                : TiposDatos.TipoIdentificadorComprador.Ruc,
            "P" => TiposDatos.TipoIdentificadorComprador.Pasaporte,
            _ => TiposDatos.TipoIdentificadorComprador.Cedula
        };

        private void ActualizarFilaUI(mFacturasSRI row, string etdofac, string imagen = null, string detalle = null)
        {
            row.etdofac = etdofac;
            if (imagen != null) row.etdofacImg = imagen;
            if (detalle != null) row.etdofacdet = detalle;
            Resultado.Items.Refresh();
        }

        private void AplicarEstadoDesdeMapa(mFacturasSRI row, string estado,
            IReadOnlyDictionary<string, (string Texto, string Imagen)> mapa, string mensaje = null)
        {
            if (!mapa.TryGetValue(estado, out var info)) return;

            bool esPendienteODevuelta = estado == ESTADO_PENDIENTE || estado == ESTADO_DEVUELTA;
            ActualizarFilaUI(row, info.Texto, info.Imagen, esPendienteODevuelta ? mensaje : null);
        }

        private void AplicarSoloTexto(mFacturasSRI row, string estado, IReadOnlyDictionary<string, string> mapa)
        {
            if (mapa.TryGetValue(estado, out var texto))
            {
                ActualizarFilaUI(row, texto);
            }
        }

        #region Construcción de GeneraFactura y cálculo de impuestos (compartido entre ramas)
        private GeneraFactura CrearGeneraFacturaBase(mFacturasSRI row)
        {
            var gf = new GeneraFactura
            {
                firmaFile = General.RUTA_FIRMA_ELECTRONICA,
                CertificadoClave = General.CLAVE_FIRMA_ELECTRONICA,
                xmlFolder = General.RUTA_XML_GENERADOS,
                numeroBlock = General.NUMERO_FACTURERO,
                esRimpe = General.ES_TIPO_RIMPE,

                ambiente = environment == "DEV" ? TiposDatos.TipoAmbiente.Pruebas : TiposDatos.TipoAmbiente.Produccion,
                emision = TiposDatos.TipoEmision.EmisionNormal,
                ruc = General.RUC_EMPRESA,
                establecimiento = General.CODIGO_EMPRESA,
                nombreComercial = General.EMPRESA,
                razonSocial = General.RAZON_SOCIAL,
                direccionMatriz = General.DIRECCION_MATRIZ.Trim(),
                direccionEstablecimiento = General.DIRECCION_LOCAL.Trim(),
                puntoEmision = General.NUMERO_FACTURERO, //row.cajacod.Substring(row.cajacod.Length - 3, 3);

                agenteRetencion = General.USA_RETENCIONES ? TiposDatos.AgenteRetencion.SI : TiposDatos.AgenteRetencion.NO,
                llevaContabilidad = General.LLEVA_CONTABILIDAD ? TiposDatos.LLevaContabilidad.SI : TiposDatos.LLevaContabilidad.NO,
                serie = General.CODIGO_EMPRESA + "-" + General.NUMERO_FACTURERO,
                fechaEmision = DateTime.Parse(row.ventfch)
            };

            gf.codigoNumerico = gf.generaCodigo();

            return gf;
        }

        private static (List<T> conIva, List<T> sinIva, decimal totalConIva, decimal totalSinIva) SepararPorIva<T>(
            IEnumerable<T> detalles,
            Func<T, decimal> ivaSelector,
            Func<T, decimal> precioSelector,
            Func<T, decimal> cantidadSelector)
        {
            var lista = detalles.ToList();
            var conIva = lista.Where(x => ivaSelector(x) > 0).ToList();
            var sinIva = lista.Where(x => ivaSelector(x) == 0).ToList();

            decimal totalConIva = conIva.Select(o => precioSelector(o) * cantidadSelector(o)).Sum();
            decimal totalSinIva = sinIva.Select(o => precioSelector(o) * cantidadSelector(o)).Sum();

            return (conIva, sinIva, totalConIva, totalSinIva);
        }

        private static List<TiposDatos.TotalImpuesto> ConstruirTotalesImpuesto<T>(
            List<T> conIva, List<T> sinIva,
            Func<T, decimal> ivaSelector,
            Func<T, decimal> precioSelector,
            Func<T, decimal> cantidadSelector,
            Func<T, decimal> totalItemSelector,
            decimal totalConIvaPrecio,
            GeneraFactura gf)
        {
            int cantidadTiposImpuesto = (conIva.Count > 0 ? 1 : 0) + (sinIva.Count > 0 ? 1 : 0);

            if (cantidadTiposImpuesto > 1)
            {
                return new List<TiposDatos.TotalImpuesto>
                {
                    new TiposDatos.TotalImpuesto
                    {
                        codigo = TiposDatos.TipoDeImpuesto.IVA,
                        CodigoPorcentaje = gf.getTipoTarifaIVA(),
                        BaseImponible = Math.Round(conIva.Select(o => precioSelector(o) * cantidadSelector(o)).Sum(), 2),
                        Valor = Math.Round(conIva.Select(o => ivaSelector(o) * cantidadSelector(o)).Sum(), 2)
                    },
                    new TiposDatos.TotalImpuesto
                    {
                        codigo = TiposDatos.TipoDeImpuesto.IVA,
                        CodigoPorcentaje = TiposDatos.TarifaIVA.Cero,
                        BaseImponible = Math.Round(sinIva.Select(totalItemSelector).Sum(), 2),
                        Valor = 0
                    }
                };
            }

            if (conIva.Count > 0 && sinIva.Count == 0)
            {
                return new List<TiposDatos.TotalImpuesto>
                {
                    new TiposDatos.TotalImpuesto
                    {
                        codigo = TiposDatos.TipoDeImpuesto.IVA,
                        CodigoPorcentaje = gf.getTipoTarifaIVA(),
                        BaseImponible = totalConIvaPrecio,
                        Valor = conIva.Select(o => ivaSelector(o) * cantidadSelector(o)).Sum()
                    }
                };
            }

            if (sinIva.Count > 0 && conIva.Count == 0)
            {
                return new List<TiposDatos.TotalImpuesto>
                {
                    new TiposDatos.TotalImpuesto
                    {
                        codigo = TiposDatos.TipoDeImpuesto.IVA,
                        CodigoPorcentaje = TiposDatos.TarifaIVA.Cero,
                        BaseImponible = sinIva.Select(o => precioSelector(o) * cantidadSelector(o)).Sum(),
                        Valor = 0
                    }
                };
            }

            return null;
        }
        #endregion

        #region Guardado de archivos recibidos (pdf/xml)
        private void GuardarArchivosRecibidos(byte[] pdf, byte[] xml, string claveAcceso,
            string subCarpeta = "", string prefijo = "")
        {
            string carpeta = General.RUTA_XML_GENERADOS + "\\Recibidos\\" + subCarpeta;

            if (!System.IO.Directory.Exists(carpeta))
            {
                System.IO.Directory.CreateDirectory(carpeta);
            }

            if (pdf != null)
                System.IO.File.WriteAllBytes(carpeta + prefijo + claveAcceso + ".pdf", pdf);

            if (xml != null)
                System.IO.File.WriteAllBytes(carpeta + prefijo + claveAcceso + ".xml", xml);
        }

        private void creaArchivosRecividos(FacturaRequest factura, string claveAcceso) =>
            GuardarArchivosRecibidos(factura.pdf, factura.xml, claveAcceso);

        private void creaArchivosRecividos(RetencionRequest factura, string claveAcceso) =>
            GuardarArchivosRecibidos(factura.pdf, factura.xml, claveAcceso, "Retencion\\", "RET-");

        private void creaArchivosRecividos(NotaCreditoRequest factura, string claveAcceso) =>
            GuardarArchivosRecibidos(factura.pdf, factura.xml, claveAcceso);
        #endregion

        #region Procesamiento y envío al SRI
        private async void btnProcesar_Click(object sender, RoutedEventArgs e)
        {
            if (toProcessEnable)
            {
                await Procesar(sender);
            }
            else
            {
                ObjComportamiento.showMsgInfo("No es posible procesar esta factura o retención porque se encuentra Autorizada o Anulada");
            }
        }

        private async Task Procesar(object data)
        {
            bool secuencialFacGenerado = false;
            bool secuencialRetGenerado = false;
            bool secuencialNCrGenerado = false;

            if (!autenticado)
            {
                ObjComportamiento.showMsgError("No se ha podido autenticar, por lo cual no puede procesarse las facturas." + Environment.NewLine +
                    "Cierre la pantalla e inténtelo más tarde");
                return;
            }

            var client = new HttpClienteAPI { TrsklUrl = General.URL_WS };

            mFacturasSRI row;
            try
            {
                row = data is mFacturasSRI directRow ? directRow : (mFacturasSRI)((FrameworkElement)data).DataContext;

                if (row.errCode == "0101" && ObjComportamiento.showMsgYesNo("¿Desea mandar a procesar?") == MessageBoxResult.No)
                {
                    return;
                }

                ActualizarFilaUI(row, "CONECTANDO");

                GeneraFactura gf = CrearGeneraFacturaBase(row);
                string tipoDoc = row.docutip.ToUpper();

                if (tipoDoc == TIPO_RETENCION)
                {
                    secuencialRetGenerado = await ProcesarRetencionAsync(client, gf, row);
                }
                else if (tipoDoc == TIPO_NOTA_CREDITO)
                {
                    secuencialNCrGenerado = await ProcesarNotaCreditoAsync(client, gf, row);
                }
                else
                {
                    secuencialFacGenerado = await ProcesarFacturaAsync(client, gf, row);
                }
            }
            catch (Exception err)
            {
                ObjComportamiento.showMsgError(err.Message);

                if (secuencialFacGenerado)
                {
                    new Ventas(General.connectionString).undoSecuencialFactura();
                }

                if (secuencialRetGenerado)
                {
                    new Entidades.Retencion(General.connectionString).undoSecuencialRetencion();
                }

            }
        }

        private async Task<bool> ProcesarRetencionAsync(HttpClienteAPI client, GeneraFactura gf, mFacturasSRI row)
        {
            bool secuencialGenerado = false;
            var retencion = new Entidades.Retencion(General.connectionString);

            gf.RucProveedor = row.nidnid; //"0914556634001";
            gf.DireccionProveedor = row.niddir.Trim(); //"Una esquina y la otra";
            gf.RazonSocialProveedor = row.nidnbr.Trim(); // "IGNACIO S.A";
            gf.tipoIdentificadorProveedor = TiposDatos.TipoIdentificadorComprador.Ruc;
            gf.EmailProveedor = row.nideml.Trim();

            string sec = row.secuencial ?? "";

            if (sec != "")
            {
                gf.secuencial = int.Parse(sec);
            }
            else
            {
                gf.secuencial = retencion.obtenerSecuencialRetencion();
                secuencialGenerado = true;
            }

            var detalles = retencion.getDetalleRetencion(row.ventscl);

            gf.PeriodoFiscal = detalles.FirstOrDefault().aniofisc;

            gf.impuestos = detalles.Select(x => new ImpuestoRetencion
            {
                Codigo = x.tipoimpo == "R" ? "1" : "2",
                CodigoRetencion = x.codsri,
                BaseImponible = string.Format("{0:0.00}", Math.Round(x.baseimpo, 2)),
                PorcentajeRetener = x.pctrete.ToString(),
                Valor = string.Format("{0:0.00}", Math.Round(x.valorrete, 2)),
                CodDocSustento = ((int)TiposDatos.TipoComprobante.Factura).ToString().PadLeft(2, '0'),
                NumDocSustento = row.docuSri.Replace("-", ""),
                FechaEmisionSustento = DateTime.Parse(row.ventfch).ToString("dd/MM/yyyy")
            }).ToList();

            gf.informacionAdicionales = new List<TiposDatos.InformacionAdicional>()
            {
                new TiposDatos.InformacionAdicional { Nombre = "Email", Descripcion = row.nideml },
                new TiposDatos.InformacionAdicional { Nombre = "Dirección", Descripcion = row.niddir },
                new TiposDatos.InformacionAdicional { Nombre = "Retención ID", Descripcion = row.ventscl },
                new TiposDatos.InformacionAdicional { Nombre = "Ruc Proveedor", Descripcion = row.rucDev }
            };

            string path = gf.generarRetencion();

            if (gf.ArchivoFirmado == false)
            {
                ObjComportamiento.showMsgError(gf.MensajeError);
                ActualizarFilaUI(row, "NO PROCESADO", "../../images/toprocess.png",gf.MensajeError);
                return secuencialGenerado;
            }

            retencion.registraRetencionElect(row.ventscl, gf.secuencial.ToString().PadLeft(9, '0'), row.docuTipRet, gf.claveAcceso);

            row.autosri = gf.claveAcceso;

            var (xmlData, xmlData1) = General.GetXmlParaEnvio(path);

            client.Token = token;
            ActualizarFilaUI(row, "ESPERA SRI", "../../images/waiting.png");

            var fm = new RetencionModel
            {
                ruc = gf.ruc,
                retesec = row.ventscl,
                claveAcceso = gf.claveAcceso,
                empresaid = General.EMPRESA_GUID,
                serial = gf.secuencial.ToString().PadLeft(9, '0'),
                codigoLocal = gf.codigoNumerico.ToString(),
                nombreLocal = gf.nombreComercial,
                direccionLocal = gf.direccionEstablecimiento,

                idProveedor = gf.RucProveedor,
                nombreProveedor = gf.RazonSocialProveedor.Trim(),
                emailProveedor = gf.EmailProveedor.Trim(),
                data = xmlData,
                bData = Encoding.UTF8.GetBytes(xmlData1),
                pcName = General.PC_NAME,
                ipAddress = General.IP_ADDRESS,
                claveDownload = General.contraseniaDownload(10)
            };

            var rete = await client.sendRetencion(fm);

            ActualizarFilaUI(row, "RECIVIENDO", "../../images/getting.png");

            var retencionRespuesta = await client.obtieneRetencion(fm);
            await Task.Delay(5000);

            if (retencionRespuesta != null)
            {
                if (retencionRespuesta.estado == ESTADO_AUTORIZADO)
                {
                    ActualizarFilaUI(row, "AUTORIZADO", "../../images/authorize.png");
                    GuardarArchivosRecibidos(rete.pdf, rete.xml, gf.claveAcceso);
                }
                else
                {
                    if (retencionRespuesta.estado == ESTADO_PENDIENTE) ActualizarFilaUI(row, "PENDIENTE", "../../images/pending.png");
                    if (retencionRespuesta.estado == ESTADO_DEVUELTA) ActualizarFilaUI(row, "DEVUELTA", "../../images/returned.png");
                    if (retencionRespuesta.estado == ESTADO_NO_PROCESADO) ActualizarFilaUI(row, "NO PROCESADO", "../../images/toprocess.png");
                }

                retencion.actualizaReteElect(retencionRespuesta.reteslc ?? row.ventscl,
                                        gf.secuencial.ToString().PadLeft(9, '0'),
                                        gf.claveAcceso,
                                        retencionRespuesta.estado,
                                        retencionRespuesta.estadoMail,
                                        retencionRespuesta.mensaje);

                if (retencionRespuesta.estado == ESTADO_NO_PROCESADO)
                {
                    ActualizarFilaUI(row, "VERIFICANDO");

                    var facturaChecked = await client.VerificaRetencion(fm);
                    if (facturaChecked != null)
                    {
                        if (facturaChecked.estado != retencionRespuesta.estado)
                        {
                            GuardarArchivosRecibidos(facturaChecked.pdf, facturaChecked.xml, gf.claveAcceso);

                            retencion.actualizaReteElect(retencionRespuesta.reteslc ?? row.ventscl,
                                                    gf.secuencial.ToString().PadLeft(9, '0'),
                                                    gf.claveAcceso,
                                                    retencionRespuesta.estado,
                                                    retencionRespuesta.estadoMail,
                                                    retencionRespuesta.mensaje);

                            AplicarSoloTexto(row, facturaChecked.estado, MapaTextoCambiado);
                        }
                        else
                        {
                            AplicarSoloTexto(row, retencionRespuesta.estado, MapaTextoSinCambio);
                        }

                        retencion.actualizaReteElect(retencionRespuesta.reteslc ?? row.ventscl,
                                                gf.secuencial.ToString().PadLeft(9, '0'),
                                                gf.claveAcceso,
                                                retencionRespuesta.estado,
                                                retencionRespuesta.estadoMail,
                                                retencionRespuesta.mensaje);
                    }
                    else
                    {
                        if (retencionRespuesta.estado == ESTADO_NO_PROCESADO) ActualizarFilaUI(row, "NO PROCESADO");
                    }
                }
            }

            return secuencialGenerado;
        }

        private async Task<bool> ProcesarNotaCreditoAsync(HttpClienteAPI client, GeneraFactura gf, mFacturasSRI row)
        {
            bool secuencialGenerado = false;
            var notaCredito = new Entidades.NotaCredito(General.connectionString);

            gf.RucProveedor = row.nidnid;
            gf.DireccionProveedor = row.niddir.Trim();
            gf.RazonSocialProveedor = row.nidnbr.Trim();
            gf.tipoIdentificadorProveedor = TiposDatos.TipoIdentificadorComprador.Ruc;
            gf.EmailProveedor = row.nideml.Trim();

            string sec = row.secuencial ?? "";
            string autorizacion = row.autosri ?? "";

            if (sec != "")
            {
                gf.secuencial = int.Parse(sec);
            }
            else
            {
                gf.secuencial = notaCredito.obtenerSecuencialNotaCredito();
                secuencialGenerado = true;
            }

            gf.claveAcceso = autorizacion != "" ? autorizacion : "";
            gf.tipoIdentificadorComprador = GetTipoComprador(row.nidtip, row.nidnid);

            gf.infoNotaCredito = new InfoNotaCredito
            {
                IdentificacionComprador = row.nidnid,
                FechaEmision = string.Format("{0:dd-MM-yyyy}", DateTime.Parse(row.ventfch)),
                DirEstablecimiento = General.DIRECCION_LOCAL, 
                RazonSocialComprador = row.nidnbr == "CLIENTE FINAL" ? "CONSUMIDOR FINAL" : row.nidnbr.Trim(),
                CodDocModificado = "01", 
                NumDocModificado = row.numefac, 
                FechaEmisionDocSustento = string.Format("{0:dd/MM/yyyy}", row.fechemi),
                ObligadoContabilidad = General.LLEVA_CONTABILIDAD ? "SI" : "NO",
                TipoIdentificacionComprador = ((int)gf.tipoIdentificadorComprador).ToString(),
                Moneda = TiposDatos.Moneda.DOLAR.ToString()
            };

            var detalles = notaCredito.getDetalleNotaCredito(row.ventscl);

            var (tieneIva, noTieneIva, subPrecioConIva, subPrecioSinIva) = SepararPorIva(
                detalles, x => (decimal)x.itempiv, x => (decimal)x.itemprc, x => (decimal)x.itemcnt);

            gf.totalSinImpuesto = Math.Round(subPrecioConIva + subPrecioSinIva, 2);
            gf.totalDescuento = Math.Round((decimal)row.ventdsc, 2);
            gf.importeTotal = Math.Round((decimal)row.venttot, 2);

            gf.infoNotaCredito.TotalSinImpuestos = string.Format("{0:0.00}", gf.totalSinImpuesto);

            gf.porcentajeIva = row.ventsbt > 0
                ? (int)Math.Round((decimal)row.ventiva * 100 / (decimal)row.ventsbt, 0)
                : 0;

            gf.totalImpuestos = ConstruirTotalesImpuesto(
                tieneIva, noTieneIva,
                x => (decimal)x.itempiv, x => (decimal)x.itemprc, x => (decimal)x.itemcnt, x => (decimal)x.itempto,
                subPrecioConIva, gf);

            if (gf.totalImpuestos != null && gf.totalImpuestos.Count > 0)
            {
                gf.infoNotaCredito.TotalConImpuestos = new TotalConImpuestos
                {
                    TotalImpuesto = gf.totalImpuestos.Select(totalImpuesto => new TotalImpuesto
                    {
                        Codigo = ((int)totalImpuesto.codigo).ToString().Replace(",", "."),
                        CodigoPorcentaje = ((int)totalImpuesto.CodigoPorcentaje).ToString().Replace(",", "."),
                        Valor = string.Format("{0:0.00}", totalImpuesto.Valor),
                        BaseImponible = string.Format("{0:0.00}", totalImpuesto.BaseImponible)
                    }).ToList()
                };
            }

            gf.infoNotaCredito.ValorModificacion = string.Format("{0:0.00}", (gf.totalSinImpuesto + gf.totalImpuestos.Sum(x => x.Valor)));
            gf.infoNotaCredito.Motivo = row.docuTipRet.ToUpper();

            if (detalles != null && detalles.Count > 0)
            {
                gf.detalles = detalles.Select(detalle => new TiposDatos.Detalle
                {
                    CodigoPrincipal = detalle.itemcod,
                    CodigoAuxiliar = detalle.itemcod,
                    Descripcion = detalle.nbrprd,
                    Cantidad = detalle.itemcnt,
                    Descuento = (decimal)detalle.itemdto,
                    PrecioUnitario = (decimal)detalle.itemprc,
                    PrecioTotalSinImpuesto = (decimal)detalle.itemprc,
                    Impuesto = new List<TiposDatos.Impuesto>()
                    {
                        new TiposDatos.Impuesto
                        {
                            codigo = TiposDatos.TipoDeImpuesto.IVA,
                            CodigoPorcentaje = gf.getTipoTarifaIVA(),
                            BaseImponible = (decimal)detalle.itemprc,
                            Valor = (decimal)detalle.itempiv
                        }
                    }
                }).ToList();
            }

            gf.informacionAdicionales = new List<TiposDatos.InformacionAdicional>()
            {
                new TiposDatos.InformacionAdicional { Nombre = "Email", Descripcion = row.nideml },
                new TiposDatos.InformacionAdicional { Nombre = "Dirección", Descripcion = row.niddir },
                new TiposDatos.InformacionAdicional { Nombre = "Nota de Crédito ID", Descripcion = row.ventscl },
                new TiposDatos.InformacionAdicional { Nombre = "Ruc Proveedor", Descripcion = row.rucDev }
            };

            string path = gf.generarNotaCredito();

            if (gf.ArchivoFirmado == false)
            {
                ObjComportamiento.showMsgError(gf.MensajeError);
                ActualizarFilaUI(row, "NO PROCESADO", "../../images/toprocess.png", gf.MensajeError);
                return secuencialGenerado;
            }

            notaCredito.registraNotaCreditoElect(row.ventscl, gf.secuencial.ToString().PadLeft(9, '0'), gf.claveAcceso);

            row.autosri = gf.claveAcceso;

            var (xmlData, xmlData1) = General.GetXmlParaEnvio(path);

            client.Token = token;
            ActualizarFilaUI(row, "ESPERA SRI", "../../images/waiting.png");

            var fm = new NotaCreditoModel
            {
                ruc = gf.ruc,
                ntcrsec = row.ventscl,
                claveAcceso = gf.claveAcceso,
                empresaid = General.EMPRESA_GUID,
                serial = gf.secuencial.ToString().PadLeft(9, '0'),
                codigoLocal = gf.codigoNumerico.ToString(),
                nombreLocal = gf.nombreComercial,
                direccionLocal = gf.direccionEstablecimiento,

                idCliente = gf.RucProveedor,
                nombreCliente = gf.RazonSocialProveedor.Trim(),
                emailCliente = gf.EmailProveedor.Trim(),
                data = xmlData,
                bData = Encoding.UTF8.GetBytes(xmlData1),
                pcName = General.PC_NAME,
                ipAddress = General.IP_ADDRESS,
                claveDownload = General.contraseniaDownload(10)
            };

            var rete = await client.sendNotaCredito(fm);

            ActualizarFilaUI(row, "RECIVIENDO", "../../images/getting.png");

            var notaCreditoRespuesta = await client.obtieneNotaCredito(fm);
            await Task.Delay(5000);

            if (notaCreditoRespuesta != null)
            {
                if (notaCreditoRespuesta.estado == ESTADO_AUTORIZADO)
                {
                    ActualizarFilaUI(row, "AUTORIZADO", "../../images/authorize.png");
                    GuardarArchivosRecibidos(rete.pdf, rete.xml, gf.claveAcceso);
                }
                else
                {
                    if (notaCreditoRespuesta.estado == ESTADO_PENDIENTE) ActualizarFilaUI(row, "PENDIENTE", "../../images/pending.png");
                    if (notaCreditoRespuesta.estado == ESTADO_DEVUELTA) ActualizarFilaUI(row, "DEVUELTA", "../../images/returned.png");
                    if (notaCreditoRespuesta.estado == ESTADO_NO_PROCESADO) ActualizarFilaUI(row, "NO PROCESADO", "../../images/toprocess.png");
                    if (notaCreditoRespuesta.estado == ESTADO_VALIDADO) ActualizarFilaUI(row, "VALIDADO", "../../images/verified.png");
                }

                notaCredito.actualizaNotaCreditoElect(notaCreditoRespuesta.ntcrslc ?? row.ventscl,
                                                    gf.secuencial.ToString().PadLeft(9, '0'),
                                                    gf.claveAcceso,
                                                    notaCreditoRespuesta.estado,
                                                    notaCreditoRespuesta.estadoMail,
                                                    notaCreditoRespuesta.mensaje);

                if (notaCreditoRespuesta.estado == ESTADO_NO_PROCESADO)
                {
                    ActualizarFilaUI(row, "VERIFICANDO");

                    var notaCreditoChecked = await client.VerificaNotaCredito(fm);
                    if (notaCreditoChecked != null)
                    {
                        if (notaCreditoChecked.estado != notaCreditoRespuesta.estado)
                        {
                            notaCredito.actualizaNotaCreditoElect(notaCreditoRespuesta.ntcrslc ?? row.ventscl,
                                                                gf.secuencial.ToString().PadLeft(9, '0'),
                                                                gf.claveAcceso,
                                                                notaCreditoRespuesta.estado,
                                                                notaCreditoRespuesta.estadoMail,
                                                                notaCreditoRespuesta.mensaje);

                            AplicarSoloTexto(row, notaCreditoChecked.estado, MapaTextoCambiado);
                        }
                        else
                        {
                            AplicarSoloTexto(row, notaCreditoRespuesta.estado, MapaTextoSinCambio);
                        }

                        notaCredito.actualizaNotaCreditoElect(notaCreditoRespuesta.ntcrslc ?? row.ventscl,
                                                            gf.secuencial.ToString().PadLeft(9, '0'),
                                                            gf.claveAcceso,
                                                            notaCreditoRespuesta.estado,
                                                            notaCreditoRespuesta.estadoMail,
                                                            notaCreditoRespuesta.mensaje);
                    }
                    else
                    {
                        if (notaCreditoRespuesta.estado == ESTADO_NO_PROCESADO) ActualizarFilaUI(row, "NO PROCESADO");
                    }
                }
            }

            return secuencialGenerado;
        }

        private async Task<bool> ProcesarFacturaAsync(HttpClienteAPI client, GeneraFactura gf, mFacturasSRI row)
        {
            bool secuencialGenerado = false;
            var ventas = new Ventas(General.connectionString);

            if (ventas.checkIfVentWasProcess(row.ventscl))
            {
                ObjComportamiento.showMsgError("Esta venta ya fue procesada y autorizada");
                return secuencialGenerado;
            }

            string sec = row.secuencial ?? "";
            string autorizacion = row.autosri ?? "";

            if (sec != "")
            {
                gf.secuencial = int.Parse(sec);
            }
            else
            {
                gf.secuencial = ventas.obtenerSecuencialFactura();
                secuencialGenerado = true;
            }

            gf.claveAcceso = autorizacion != "" ? autorizacion : "";

            gf.razonSocialComprador = row.nidnbr == "CLIENTE FINAL" ? "CONSUMIDOR FINAL" : row.nidnbr.Trim();
            gf.direccionComprador = row.niddir == "" ? null : row.niddir.Trim();
            gf.IdComprador = row.nidnid;
            gf.tipoIdentificadorComprador = GetTipoComprador(row.nidtip, row.nidnid);

            var detalles = ventas.getDetalleVenta(row.ventscl);

            var (tieneIva, noTieneIva, subPrecioConIva, subPrecioSinIva) = SepararPorIva(
                detalles, x => (decimal)x.itemiva, x => (decimal)x.itemprc, x => (decimal)x.itemcnt);

            gf.totalSinImpuesto = Math.Round(subPrecioConIva + subPrecioSinIva, 2);
            gf.totalDescuento = Math.Round((decimal)row.ventdsc, 2);
            gf.importeTotal = Math.Round((decimal)row.venttot, 2);

            gf.porcentajeIva = row.ventsbt > 0
                ? (int)Math.Round((decimal)row.ventiva * 100 / (decimal)row.ventsbt, 0)
                : 0;

            gf.totalImpuestos = ConstruirTotalesImpuesto(
                tieneIva, noTieneIva,
                x => (decimal)x.itemiva, x => (decimal)x.itemprc, x => (decimal)x.itemcnt, x => (decimal)x.itempto,
                subPrecioConIva, gf);

            gf.propina = 0;
            gf.moneda = TiposDatos.Moneda.DOLAR;
            gf.pagos = new List<TiposDatos.Pago>();

            var pagos = ventas.getPagosVenta(row.ventscl);

            if (pagos.Count > 0)
            {
                gf.pagos.AddRange(pagos.Select(pago => new TiposDatos.Pago
                {
                    formaDePago = pago.isdiferido || pago.iscorriente ? TiposDatos.FormasDePago.TarjetaCredito :
                                  pago.ischeque ? TiposDatos.FormasDePago.OtrosConSistemaFinanciero :
                                  TiposDatos.FormasDePago.SinSistemaFinanciero,
                    Total = Math.Round((decimal)(pago.valor - pago.vuelto), 2),
                    Plazo = pago.meses,
                    TiempoPlazo = pago.meses > 0 ? TiposDatos.TiempoPlazo.Meses : TiposDatos.TiempoPlazo.Ninguno
                }));
            }
            else
            {
                gf.pagos.Add(new TiposDatos.Pago
                {
                    formaDePago = TiposDatos.FormasDePago.SinSistemaFinanciero,
                    Total = (decimal)row.venttot,
                    Plazo = 0,
                    TiempoPlazo = TiposDatos.TiempoPlazo.Ninguno
                });
            }

            gf.detalles = detalles.Select(detalle => new TiposDatos.Detalle
            {
                CodigoPrincipal = detalle.itemcod,
                CodigoAuxiliar = detalle.itemcod,
                Descripcion = detalle.itemnbr.Trim(),
                Cantidad = detalle.itemcnt,
                PrecioUnitario = Math.Round((decimal)detalle.itemprc, 2),
                Descuento = 0,
                PrecioTotalSinImpuesto = Math.Round((decimal)detalle.itemprc * detalle.itemcnt, 2),
                Impuesto = new List<TiposDatos.Impuesto>()
                {
                    new TiposDatos.Impuesto
                    {
                        codigo = TiposDatos.TipoDeImpuesto.IVA,
                        CodigoPorcentaje = detalle.itemiva == 0 ? TiposDatos.TarifaIVA.Cero : gf.getTipoTarifaIVA(),
                        BaseImponible = Math.Round((decimal)detalle.itemprc * detalle.itemcnt, 2),
                        Valor = Math.Round((decimal)detalle.itemiva * detalle.itemcnt, 2)
                    }
                }
            }).ToList();

            gf.informacionAdicionales = new List<TiposDatos.InformacionAdicional>()
            {
                new TiposDatos.InformacionAdicional { Nombre = "Mesa", Descripcion = row.ventmsa ?? "0000" },
                new TiposDatos.InformacionAdicional { Nombre = "Pedido", Descripcion = row.peddscl },
                new TiposDatos.InformacionAdicional { Nombre = "Venta", Descripcion = row.ventscl },
                new TiposDatos.InformacionAdicional { Nombre = "Email Cliente", Descripcion = row.nideml.Trim() ?? "Sin Email" },
                new TiposDatos.InformacionAdicional { Nombre = "Ruc Proveedor", Descripcion = row.rucDev }
            };

            ventas.registraFactElect(row.ventscl, gf.secuencial.ToString().PadLeft(9, '0'), gf.claveAcceso, gf.fechaEmision,
                ((int)gf.codDoc).ToString(), ((int)gf.ambiente).ToString(), gf.serie, gf.ruc, gf.codigoNumerico.ToString(), ((int)gf.emision).ToString());

            string secuencial = gf.secuencial.ToString().PadLeft(9, '0');
            string claveAcceso = gf.claveAcceso;

            ventas.verificaFactElect(row.ventscl, ref secuencial, ref claveAcceso);

            if (secuencial != gf.secuencial.ToString().PadLeft(9, '0'))
            {
                gf.secuencial = int.Parse(secuencial);
            }

            if (claveAcceso != gf.claveAcceso)
            {
                gf.claveAcceso = claveAcceso;
            }

            ActualizarFilaUI(row, "GENERANDO", "../../images/generando.png");

            string archivo = gf.generarFactura();

            if (gf.ArchivoFirmado == false)
            {
                ObjComportamiento.showMsgError(gf.MensajeError);
                ActualizarFilaUI(row, "NO PROCESADO", "../../images/toprocess.png", gf.MensajeError);
                return secuencialGenerado;
            }

            row.autosri = gf.claveAcceso;

            var (xmlData, xmlData1) = General.GetXmlParaEnvio(archivo);

            client.Token = token;
            ActualizarFilaUI(row, "ESPERA SRI", "../../images/waiting.png");

            var fm = new FacturaModel
            {
                ruc = gf.ruc,
                ventasec = row.ventscl,
                claveAcceso = gf.claveAcceso,
                empresaid = General.EMPRESA_GUID,
                serial = gf.secuencial.ToString().PadLeft(9, '0'),
                codigoLocal = gf.codigoNumerico.ToString(),
                nombreLocal = gf.nombreComercial,
                direccionLocal = gf.direccionEstablecimiento,
                idCliente = gf.IdComprador,
                nombreCliente = gf.razonSocialComprador,
                emailCliente = row.nideml,
                data = xmlData,
                bData = Encoding.UTF8.GetBytes(xmlData1),
                pcName = General.PC_NAME,
                ipAddress = General.IP_ADDRESS,
                claveDownload = row.claveDownload //General.contraseniaDownload(10)
            };

            var factura = await client.sendFactura(fm);

            ActualizarFilaUI(row, "RECIVIENDO", "../../images/getting.png");

            var facturaRespuesta = await client.obtieneFactura(fm);
            await Task.Delay(5000);

            if (facturaRespuesta != null)
            {
                if (facturaRespuesta.Estado == ESTADO_AUTORIZADO)
                {
                    ActualizarFilaUI(row, "AUTORIZADO", "../../images/authorize.png");
                    GuardarArchivosRecibidos(factura.pdf, factura.xml, gf.claveAcceso);
                }
                else
                {
                    if (facturaRespuesta.Estado == ESTADO_PENDIENTE) ActualizarFilaUI(row, "PENDIENTE", "../../images/pending.png");
                    if (facturaRespuesta.Estado == ESTADO_DEVUELTA) ActualizarFilaUI(row, "DEVUELTA", "../../images/returned.png");
                    if (facturaRespuesta.Estado == ESTADO_NO_PROCESADO) ActualizarFilaUI(row, "NO PROCESADO", "../../images/toprocess.png");
                    if (facturaRespuesta.Estado == ESTADO_VALIDADO) ActualizarFilaUI(row, "VALIDADO", "../../images/verified.png");
                }

                ventas.actualizaFactElect(facturaRespuesta.Secuencial ?? row.ventscl,
                                        gf.secuencial.ToString().PadLeft(9, '0'),
                                        gf.claveAcceso,
                                        facturaRespuesta.Estado,
                                        facturaRespuesta.EstadoMail == true ? "A" : "N",
                                        facturaRespuesta.Mensaje);

                if (facturaRespuesta.Estado == ESTADO_NO_PROCESADO)
                {
                    ActualizarFilaUI(row, "VERIFICANDO");

                    var facturaChecked = await client.VerificaFactura(fm);
                    if (facturaChecked != null)
                    {
                        if (facturaChecked.estado != facturaRespuesta.Estado)
                        {
                            GuardarArchivosRecibidos(facturaChecked.pdf, facturaChecked.xml, gf.claveAcceso);

                            ventas.actualizaFactElect(facturaRespuesta.Secuencial ?? row.ventscl,
                                gf.secuencial.ToString().PadLeft(9, '0'),
                                gf.claveAcceso,
                                facturaRespuesta.Estado,
                                facturaRespuesta.EstadoMail == true ? "A" : "N",
                                facturaRespuesta.Mensaje);

                            AplicarSoloTexto(row, facturaChecked.estado, MapaTextoCambiado);
                        }
                        else
                        {
                            AplicarSoloTexto(row, facturaRespuesta.Estado, MapaTextoSinCambio);
                        }

                        ventas.actualizaFactElect(facturaRespuesta.Secuencial ?? row.ventscl,
                                                gf.secuencial.ToString().PadLeft(9, '0'),
                                                gf.claveAcceso,
                                                facturaRespuesta.Estado,
                                                facturaRespuesta.EstadoMail == true ? "A" : "N",
                                                facturaRespuesta.Mensaje);
                    }
                    else
                    {
                        if (facturaRespuesta.Estado == ESTADO_NO_PROCESADO) ActualizarFilaUI(row, "NO PROCESADO");
                    }
                }
            }

            return secuencialGenerado;
        }

        #endregion

        #region Verificación manual
        private async void btnVerificar_Click(object sender, RoutedEventArgs e)
        {
            mFacturasSRI row = (mFacturasSRI)((FrameworkElement)sender).DataContext;
            string tipoDoc = row.docutip.ToUpper();

            if (tipoDoc == TIPO_RETENCION)
            {
                await VerificarRetencionAsync(row);
            }
            else if (tipoDoc == TIPO_NOTA_CREDITO)
            {
                await VerificarNotaCreditoAsync(row);
            }
            else
            {
                await VerificarFacturaAsync(row);
            }
        }

        private async Task VerificarRetencionAsync(mFacturasSRI row)
        {
            var retencion = new Entidades.Retencion(General.connectionString);
            var client = new HttpClienteAPI { TrsklUrl = General.URL_WS };

            ActualizarFilaUI(row, "CONECTANDO", "../../images/connecting.png");

            /*var resp = await client.Login(General.USUARIO_WS, General.CLAVE_WS, General.RUC_EMPRESA, General.EMPRESA_GUID);
            client.Token = resp.Token;*/

            string sec = retencion.verificaSclEnRetencionElect(row.ventscl);
            string claveAcceso = retencion.getClaveAcceso(row.ventscl);

            if (string.IsNullOrEmpty(sec))
            {
                ObjComportamiento.showMsgError("No existe una Retención con ese secuencial de retención");
                return;
            }

            string secuencial = sec.PadLeft(9, '0');

            var fm = new RetencionModel
            {
                ruc = General.RUC_EMPRESA,
                retesec = row.ventscl,
                claveAcceso = claveAcceso,
                empresaid = General.EMPRESA_GUID,
                serial = secuencial,
                codigoLocal = null,
                nombreLocal = null,
                direccionLocal = null,
                idProveedor = null,
                nombreProveedor = null,
                emailProveedor = null,
                data = null,
                bData = null
            };

            ActualizarFilaUI(row, "VERIFICANDO", "../../images/checking.png");

            var retencionChecked = await client.VerificaRetencion(fm);
            if (retencionChecked != null)
            {
                GuardarArchivosRecibidos(retencionChecked.pdf, retencionChecked.xml, claveAcceso, "Retencion\\", "RET-");

                retencion.actualizaReteElect(row.ventscl, secuencial, claveAcceso,
                    retencionChecked.estado, retencionChecked.estadoMail, retencionChecked.mensaje);

                AplicarEstadoDesdeMapa(row, retencionChecked.estado, MapaEstadoVerificar, retencionChecked.mensaje);
            }
            else
            {
                ActualizarFilaUI(row, "FINALIZADO", "../../images/cancelsend.png");
            }
        }

        private async Task VerificarNotaCreditoAsync(mFacturasSRI row)
        {
            var notaCredito = new Entidades.NotaCredito(General.connectionString);
            var client = new HttpClienteAPI { TrsklUrl = General.URL_WS };

            ActualizarFilaUI(row, "CONECTANDO", "../../images/connecting.png");

            /*var resp = await client.Login(General.USUARIO_WS, General.CLAVE_WS, General.RUC_EMPRESA, General.EMPRESA_GUID);
            client.Token = resp.Token;*/

            string sec = notaCredito.verificaSclEnNotaCreditoElect(row.ventscl);
            string claveAcceso = notaCredito.getClaveAcceso(row.ventscl);

            if (string.IsNullOrEmpty(sec))
            {
                ObjComportamiento.showMsgError("No existe una Nota de Crédito para ese secuencial de venta");
                return;
            }

            string secuencial = sec.PadLeft(9, '0');

            var fm = new NotaCreditoModel
            {
                ruc = General.RUC_EMPRESA,
                ntcrsec = row.ventscl,
                claveAcceso = claveAcceso,
                empresaid = General.EMPRESA_GUID,
                serial = secuencial,
                codigoLocal = null,
                nombreLocal = null,
                direccionLocal = null,
                idCliente = null,
                nombreCliente = null,
                emailCliente = null,
                data = null,
                bData = null
            };

            ActualizarFilaUI(row, "VERIFICANDO", "../../images/checking.png");

            var notaCreditoChecked = await client.VerificaNotaCredito(fm);

            if (notaCreditoChecked != null)
            {
                notaCredito.actualizaNotaCreditoElect(row.ventscl, secuencial, claveAcceso,
                    notaCreditoChecked.estado, notaCreditoChecked.estadoMail, notaCreditoChecked.mensaje);

                AplicarEstadoDesdeMapa(row, notaCreditoChecked.estado, MapaEstadoVerificar, notaCreditoChecked.mensaje);
            }
            else
            {
                ActualizarFilaUI(row, "FINALIZADO", "../../images/cancelsend.png");
            }
        }

        private async Task VerificarFacturaAsync(mFacturasSRI row)
        {
            var ventas = new Ventas(General.connectionString);
            var client = new HttpClienteAPI { TrsklUrl = General.URL_WS };

            ActualizarFilaUI(row, "CONECTANDO", "../../images/connecting.png");

            /*var resp = await client.Login(General.USUARIO_WS, General.CLAVE_WS, General.RUC_EMPRESA, General.EMPRESA_GUID);
            client.Token = resp.Token;*/

            var sec = ventas.verificaSclEnFacturaElect(row.ventscl).Split(':');

            if (sec.Length == 1 && sec[0] == "")
            {
                ActualizarFilaUI(row, "NO PROCESADO", "../../images/toprocess.png");
                ObjComportamiento.showMsgInfo("Esta transacción no ha sido procesada...");
                return;
            }

            string secuencial = sec[0];
            string claveAcceso = sec[1];
            string estado = sec[2];

            if (string.IsNullOrEmpty(secuencial))
            {
                ObjComportamiento.showMsgError("No existe una Factura con ese secuencial de venta");
                return;
            }

            if (estado == ESTADO_AUTORIZADO)
            {
                ActualizarFilaUI(row, "AUTORIZADO", "../../images/authorize2.png");
                return;
            }

            secuencial = secuencial.PadLeft(9, '0');

            var fm = new FacturaModel
            {
                ruc = General.RUC_EMPRESA,
                ventasec = row.ventscl,
                claveAcceso = claveAcceso,
                empresaid = General.EMPRESA_GUID,
                serial = secuencial,
                codigoLocal = null,
                nombreLocal = null,
                direccionLocal = null,
                idCliente = null,
                nombreCliente = null,
                emailCliente = null,
                data = null,
                bData = null
            };

            ActualizarFilaUI(row, "VERIFICANDO", "../../images/checking.png");

            var facturaChecked = await client.VerificaFactura(fm);
            if (facturaChecked != null)
            {
                GuardarArchivosRecibidos(facturaChecked.pdf, facturaChecked.xml, claveAcceso);

                ventas.actualizaFactElect(row.ventscl, secuencial, claveAcceso,
                    facturaChecked.estado, facturaChecked.estadoMail, facturaChecked.mensaje);

                AplicarEstadoDesdeMapa(row, facturaChecked.estado, MapaEstadoVerificar, facturaChecked.mensaje);
            }
            else
            {
                ActualizarFilaUI(row, "FINALIZADO", "../../images/cancelsend.png");
            }
        }

        #endregion

        #region Grilla: búsqueda, filtro, envío en lote y misceláneos

        private void btnMail_Click(object sender, RoutedEventArgs e)
        {
        }

        private void checkFact_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void checkFact_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void btnBscRsv_Click(object sender, RoutedEventArgs e)
        {
            var ventas = new Ventas(General.connectionString);

            var vFecha = string.Format("{0:yyyy-MM-dd}", fecha.SelectedDate);
            string filtro = ((ComboBoxItem)(Filtro.SelectedItem)).Tag.ToString();

            List<mFacturasSRI> resp = ventas.getFacturasToSRI(vFecha, filtro);

            if (resp.Count > 0)
            {
                btnGrabar.IsEnabled = true;
                toProcessEnable = true;

                AsignarImagenes(resp, x => x.docutip == "Factura" && x.venttip != "E", o => o.docuTipImg = "../../images/factura.png");
                AsignarImagenes(resp, x => x.docutip == "Factura" && x.venttip == "E", o => o.docuTipImg = "../../images/uber.png");
                AsignarImagenes(resp, x => x.docutip == "NotaDeCredito", o => o.docuTipImg = "../../images/notaCredito.png");
                AsignarImagenes(resp, x => x.docutip == "Retención", o => o.docuTipImg = "../../images/retencion.png");
                AsignarImagenes(resp, x => x.docuTipRet == "Compra", o => o.docuRetImg = "../../images/buy.png");
                AsignarImagenes(resp, x => x.docuTipRet == "Venta", o => o.docuRetImg = "../../images/sell.png");
                AsignarImagenes(resp, x => x.docuTipRet == "Anulación", o => o.docuRetImg = "../../images/anulaFactura.png");
                AsignarImagenes(resp, x => x.docuTipRet == "Devolución", o => o.docuRetImg = "../../images/devolucionCompra.png");

                if (filtro == "A" || filtro == "L")
                {
                    btnGrabar.IsEnabled = false;
                    toProcessEnable = false;
                }
            }
            else
            {
                btnGrabar.IsEnabled = false;
            }

            Resultado.ItemsSource = null;
            cntFacturas.Text = "Cantidad de Facturas: 0";

            if (resp.Count > 0)
            {
                cntFacturas.Text = $"Cantidad de Facturas: {resp.Count}";
                Resultado.ItemsSource = resp;
            }
        }

        private static void AsignarImagenes(List<mFacturasSRI> filas, Func<mFacturasSRI, bool> filtro, Action<mFacturasSRI> asignar)
        {
            foreach (var fila in filas.Where(filtro))
            {
                asignar(fila);
            }
        }

        private async void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            var filas = Resultado.Items.OfType<mFacturasSRI>().ToList();

            foreach (var row in filas)
            {
                await Procesar(row);
                await Task.Delay(1000);
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CheckAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (mFacturasSRI row in Resultado.Items)
            {
                row.isChecked = true;
            }
        }

        private void CheckAll_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void imgEstado_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mFacturasSRI row = (mFacturasSRI)((FrameworkElement)sender).DataContext;

            if (row.etdofac != "Devuelta" && row.etdofac != "Pendiente" && row.etdofac != "Anulado")
            {
                return;
            }

            var ventas = new Ventas(General.connectionString);

            string tipo = row.etdofac switch
            {
                "Devuelta" => "D",
                "Pendiente" => "P",
                "Anulado" => "L",
                _ => ""
            };

            string mensaje = ventas.getMensajeFacturacion(row.ventscl, row.docutip == "Factura" ? "F" : "R", tipo);

            mensaje = mensaje == "" ? "No se ha registrado un motivo" : mensaje;

            ObjComportamiento.showMsgError(mensaje);
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminar el estado administrado (objetos administrados)
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion        
    }
}
