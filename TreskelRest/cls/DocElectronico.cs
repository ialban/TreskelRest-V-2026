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

namespace TreskelRest.cls
{
    public class DocElectronico
    {
        private TiposDatos.TipoIdentificadorComprador getTipoComprador(string tipo, string valor)
        {
            TiposDatos.TipoIdentificadorComprador vtipo = TiposDatos.TipoIdentificadorComprador.Cedula;
            switch (tipo)
            {
                case "C":
                    vtipo = TiposDatos.TipoIdentificadorComprador.Cedula;
                    break;
                case "R":
                    if (valor == "9999999999999")
                    {
                        vtipo = TiposDatos.TipoIdentificadorComprador.CosumidorFinal;
                    }
                    else
                    {
                        vtipo = TiposDatos.TipoIdentificadorComprador.Ruc;
                    }

                    break;
                case "P":
                    vtipo = TiposDatos.TipoIdentificadorComprador.Pasaporte;
                    break;
            }

            return vtipo;
        }

        public delegate void retornaEstado(string estado);
        public event retornaEstado _retornaEstado;

        public delegate void Terminado();
        public event Terminado _Finalizado;

        public async Task Procesar(mFacturasSRI row)
        {
            bool SecuencialFacGenerado = false;
            bool SecuencialRetGenerado = false;
            bool SecuencialNcrGenerado = false;
            // Logear al WS
            HttpClienteAPI client = new HttpClienteAPI();

            System.Windows.Forms.Application.DoEvents();

            try
            {
                if (row.errCode == "0101")
                {
                    return;
                }

                client.TrsklUrl = General.URL_WS;

                System.Windows.Forms.Application.DoEvents();
                _retornaEstado("");
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1000);
                System.Windows.Forms.Application.DoEvents();

                var resp = await client.Login(General.USUARIO_WS, General.CLAVE_WS, General.RUC_EMPRESA, General.EMPRESA_GUID);

                System.Windows.Forms.Application.DoEvents();
                _retornaEstado("C");
                Thread.Sleep(1000);

                System.Windows.Forms.Application.DoEvents();

                Thread.Sleep(1000);

                _retornaEstado("G");
                GeneraFactura gf = new GeneraFactura();

                gf.firmaFile = General.RUTA_FIRMA_ELECTRONICA;
                gf.CertificadoClave = General.CLAVE_FIRMA_ELECTRONICA;
                gf.xmlFolder = General.RUTA_XML_GENERADOS;
                gf.numeroBlock = General.NUMERO_FACTURERO;
                gf.esRimpe = General.ES_TIPO_RIMPE;

                gf.ambiente = resp.environment == "DEV" ? TiposDatos.TipoAmbiente.Pruebas : TiposDatos.TipoAmbiente.Produccion;
                gf.emision = TiposDatos.TipoEmision.EmisionNormal;
                gf.ruc = General.RUC_EMPRESA;
                gf.establecimiento = General.CODIGO_EMPRESA;
                gf.nombreComercial = General.EMPRESA;
                gf.razonSocial = General.RAZON_SOCIAL;
                gf.direccionMatriz = General.DIRECCION_MATRIZ.Trim();
                gf.direccionEstablecimiento = General.DIRECCION_LOCAL.Trim();
                gf.codigoNumerico = gf.generaCodigo();
                gf.puntoEmision = General.NUMERO_FACTURERO;

                gf.agenteRetencion = General.USA_RETENCIONES ? TiposDatos.AgenteRetencion.SI : TiposDatos.AgenteRetencion.NO;
                gf.llevaContabilidad = General.LLEVA_CONTABILIDAD ? TiposDatos.LLevaContabilidad.SI : TiposDatos.LLevaContabilidad.NO;
                gf.serie = General.CODIGO_EMPRESA + "-" + General.NUMERO_FACTURERO;
                gf.fechaEmision = DateTime.Parse(row.ventfch);
                
                string archivo = "";

                if (row.docutip.ToUpper() == "R")
                {
                    Entidades.Retencion retencion = new Entidades.Retencion(General.connectionString);

                    gf.RucProveedor = row.nidnid;
                    gf.DireccionProveedor = row.niddir.Trim();
                    gf.RazonSocialProveedor = row.nidnbr.Trim();
                    gf.tipoIdentificadorProveedor = TiposDatos.TipoIdentificadorComprador.Ruc;
                    gf.EmailProveedor = row.nideml.Trim();

                    // Verificar si existe ventscl con estado = N
                    string sec = row.secuencial ?? "";
                    string autorizacion = row.autosri ?? "";

                    if (sec != "")
                    {
                        gf.secuencial = int.Parse(sec);
                    }
                    else
                    {
                        gf.secuencial = retencion.obtenerSecuencialRetencion();
                        SecuencialRetGenerado = true;
                    }

                    gf.impuestos = new List<ImpuestoRetencion>();

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
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "Email",
                            Descripcion = row.nideml
                        },
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "Dirección",
                            Descripcion = row.niddir
                        },
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "Retención ID",
                            Descripcion = row.ventscl
                        },
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "RUC Proveedor",
                            Descripcion = row.rucDev
                        }
                    };

                    string path = gf.generarRetencion();

                    if (gf.ArchivoFirmado == false)
                    {
                        ObjComportamiento.showMsgError("El Archivo no pudo ser firmado");
                        return;
                    }

                    retencion.registraRetencionElect(row.ventscl, gf.secuencial.ToString().PadLeft(9, '0'), row.docuTipRet, gf.claveAcceso);

                    row.autosri = gf.claveAcceso;

                    var (xmlData, xmlData1) = General.GetXmlParaEnvio(path);

                    // Envio de factura generada al web service
                    client.Token = resp.Token;

                    // Proceso de Envío de Retenciones al SRI.
                    RetencionModel fm = new RetencionModel
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

                    _retornaEstado("W");
                    var rete = await client.sendRetencion(fm);

                    System.Windows.Forms.Application.DoEvents();

                    var retencionRespuesta = await client.obtieneRetencion(fm);
                    Thread.Sleep(1000);
                    if (retencionRespuesta != null)
                    {
                        _retornaEstado(retencionRespuesta.estado);

                        if (retencionRespuesta.estado == "A")
                        {
                            row.etdofac = "AUTORIZADO";

                            System.Windows.Forms.Application.DoEvents();
                        }
                        else
                        {
                            if (retencionRespuesta.estado == "P") row.etdofac = "PENDIENTE";
                            if (retencionRespuesta.estado == "D") row.etdofac = "DEVUELTA";
                            if (retencionRespuesta.estado == "N") row.etdofac = "NO PROCESADO";
                            if (retencionRespuesta.estado == "V") row.etdofac = "VALIDADO";

                            System.Windows.Forms.Application.DoEvents();
                        }

                        retencion.actualizaReteElect(retencionRespuesta.reteslc ?? row.ventscl,
                            gf.secuencial.ToString().PadLeft(9, '0'),
                            gf.claveAcceso,
                            retencionRespuesta.estado,
                            retencionRespuesta.estadoMail,
                            retencionRespuesta.mensaje);
                        System.Windows.Forms.Application.DoEvents();
                    }

                    string[] valores = { "N", "P", "V", "D" };

                    if (valores.Contains(retencionRespuesta.estado))
                    {
                        row.etdofac = "VERIFICANDO";
                        System.Windows.Forms.Application.DoEvents();

                        var retencionChecked = await client.VerificaRetencion(fm);
                        if (retencionChecked != null)
                        {
                            if (retencionChecked.estado != retencionRespuesta.estado)
                            {
                                if (retencionChecked.estado == "A") row.etdofac = "AUTORIZADO";
                                if (retencionChecked.estado == "P") row.etdofac = "PENDIENTE";
                                if (retencionChecked.estado == "N") row.etdofac = "NO PROCESADO";
                                if (retencionChecked.estado == "D") row.etdofac = "DEVUELTA";
                                if (retencionChecked.estado == "V") row.etdofac = "VALIDADO";

                                retencionRespuesta.estado = retencionChecked.estado;
                                retencionRespuesta.estadoMail = retencionChecked.estadoMail;
                                retencionRespuesta.mensaje = retencionChecked.mensaje;
                            }
                            else
                            {
                                if (retencionRespuesta.estado == "A") row.etdofac = "VERIFICANDO";
                                if (retencionRespuesta.estado == "P") row.etdofac = "PENDIENTE";
                                if (retencionRespuesta.estado == "N") row.etdofac = "NO PROCESADO";
                                if (retencionRespuesta.estado == "D") row.etdofac = "DEVUELTA";
                                if (retencionRespuesta.estado == "V") row.etdofac = "VALIDADO";    
                            }
                            System.Windows.Forms.Application.DoEvents();

                            retencion.actualizaReteElect(retencionRespuesta.reteslc ?? row.ventscl,
                                                        gf.secuencial.ToString().PadLeft(9, '0'),
                                                        gf.claveAcceso,
                            retencionRespuesta.estado,
                            retencionRespuesta.estadoMail,
                            retencionRespuesta.mensaje);
                            System.Windows.Forms.Application.DoEvents();
                        }
                        else
                        {
                            if (retencionRespuesta.estado == "N") row.etdofac = "NO PROCESADO";
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                }
                else if (row.docutip.ToUpper() == "NOTADECREDITO")
                {
                    Entidades.NotaCredito notaCredito = new Entidades.NotaCredito(General.connectionString);

                    gf.RucProveedor = row.nidnid;
                    gf.DireccionProveedor = row.niddir.Trim();
                    gf.RazonSocialProveedor = row.nidnbr.Trim();
                    gf.tipoIdentificadorProveedor = TiposDatos.TipoIdentificadorComprador.Ruc;
                    gf.EmailProveedor = row.nideml.Trim();

                    // Verificar si existe ventscl con estado = N
                    string sec = row.secuencial ?? "";
                    string autorizacion = row.autosri ?? "";

                    if (sec != "")
                    {
                        gf.secuencial = int.Parse(sec);
                    }
                    else
                    {
                        gf.secuencial = notaCredito.obtenerSecuencialNotaCredito();
                        SecuencialRetGenerado = true;
                    }

                    gf.claveAcceso = autorizacion != "" ? autorizacion : "";
                    gf.tipoIdentificadorComprador = getTipoComprador(row.nidtip, row.nidnid);

                    gf.infoNotaCredito = new InfoNotaCredito();
                    gf.infoNotaCredito.IdentificacionComprador = row.nidnid;
                    gf.infoNotaCredito.FechaEmision = string.Format("{0:dd-MM-yyyy}", DateTime.Parse(row.ventfch));
                    gf.infoNotaCredito.DirEstablecimiento = General.DIRECCION_LOCAL; //row.niddir == "" ? null : row.niddir.Trim();
                    gf.infoNotaCredito.RazonSocialComprador = row.nidnbr == "CLIENTE FINAL" ? "CONSUMIDOR FINAL" : row.nidnbr.Trim();
                    gf.infoNotaCredito.CodDocModificado = "01"; // Por el momento porque sólo se permitirá modificar facturas
                    gf.infoNotaCredito.NumDocModificado = row.numefac; // Número de la factura a modificar
                    gf.infoNotaCredito.FechaEmisionDocSustento = string.Format("{0:dd/MM/yyyy}", row.fechemi);
                    gf.infoNotaCredito.ObligadoContabilidad = General.LLEVA_CONTABILIDAD ? "SI" : "NO";
                    gf.infoNotaCredito.TipoIdentificacionComprador = ((int)gf.tipoIdentificadorComprador).ToString();
                    gf.infoNotaCredito.Moneda = TiposDatos.Moneda.DOLAR.ToString();

                    var detalles = notaCredito.getDetalleNotaCredito(row.ventscl);

                    var tieneIva = detalles.Where(x => x.itempiv > 0).ToList();
                    var NoTieneIva = detalles.Where(x => x.itempiv == 0).ToList();

                    var SubPrecioConIva = (decimal)tieneIva.Select(o => o.itemprc * o.itemcnt).Sum();
                    var SubPrecioSinIva = (decimal)NoTieneIva.Select(o => o.itemprc * o.itemcnt).Sum();

                    gf.totalSinImpuesto = Math.Round((decimal)SubPrecioConIva + (decimal)SubPrecioSinIva, 2);
                    gf.totalDescuento = Math.Round((decimal)row.ventdsc, 2);
                    gf.importeTotal = Math.Round((decimal)row.venttot, 2);

                    gf.infoNotaCredito.TotalSinImpuestos = string.Format("{0:0.00}", gf.totalSinImpuesto);

                    if (row.ventsbt > 0)
                        gf.porcentajeIva = (int)Math.Round((decimal)row.ventiva * 100 / (decimal)row.ventsbt, 0);
                    else
                        gf.porcentajeIva = 0;

                    int cantTotImpuestos = 0;
                    if (tieneIva.Count > 0) cantTotImpuestos++;
                    if (NoTieneIva.Count > 0) cantTotImpuestos++;

                    if (cantTotImpuestos > 1)
                    {
                        gf.totalImpuestos = new List<TiposDatos.TotalImpuesto>() {
                            new TiposDatos.TotalImpuesto
                            {
                                codigo = TiposDatos.TipoDeImpuesto.IVA,
                                CodigoPorcentaje = gf.getTipoTarifaIVA(),
                                BaseImponible = Math.Round((decimal)tieneIva.Select(o =>  o.itemprc*o.itemcnt).Sum(),2),
                                Valor = Math.Round((decimal) tieneIva.Select(o => o.itempiv*o.itemcnt).Sum(),2)
                            },
                            new TiposDatos.TotalImpuesto
                            {
                                codigo = TiposDatos.TipoDeImpuesto.IVA,
                                CodigoPorcentaje = TiposDatos.TarifaIVA.Cero,
                                BaseImponible = Math.Round((decimal) NoTieneIva.Select(o => o.itempto).Sum(),2),
                                Valor = 0
                            }
                        };
                    }
                    else if (cantTotImpuestos == 1)
                    {
                        if (tieneIva.Count > 0 && NoTieneIva.Count == 0)
                        {
                            gf.totalImpuestos = new List<TiposDatos.TotalImpuesto>() {
                                new TiposDatos.TotalImpuesto
                                {
                                    codigo = TiposDatos.TipoDeImpuesto.IVA,
                                    CodigoPorcentaje = gf.getTipoTarifaIVA(),
                                    BaseImponible = SubPrecioConIva,
                                    Valor = (decimal) tieneIva.Select(o => o.itempiv*o.itemcnt).Sum()
                                }
                            };
                        }
                        if (NoTieneIva.Count > 0 && tieneIva.Count == 0)
                        {
                            gf.totalImpuestos = new List<TiposDatos.TotalImpuesto>() {
                            new TiposDatos.TotalImpuesto
                                {
                                    codigo = TiposDatos.TipoDeImpuesto.IVA,
                                    CodigoPorcentaje = TiposDatos.TarifaIVA.Cero,
                                    BaseImponible = (decimal) NoTieneIva.Select(o => o.itemprc*o.itemcnt).Sum(),
                                    Valor = 0
                                }
                            };
                        }
                    }
                    else
                    {

                    }

                    if (gf.totalImpuestos != null && gf.totalImpuestos.Count > 0)
                    {
                        gf.infoNotaCredito.TotalConImpuestos = new TotalConImpuestos();
                        gf.infoNotaCredito.TotalConImpuestos.TotalImpuesto = new List<TotalImpuesto>();

                        foreach (var totalImpuesto in gf.totalImpuestos)
                        {
                            gf.infoNotaCredito.TotalConImpuestos.TotalImpuesto.Add(new TotalImpuesto
                            {
                                Codigo = ((int)totalImpuesto.codigo).ToString().Replace(",", "."),
                                CodigoPorcentaje = ((int)totalImpuesto.CodigoPorcentaje).ToString().Replace(",", "."),
                                Valor = string.Format("{0:0.00}", totalImpuesto.Valor),
                                BaseImponible = string.Format("{0:0.00}", totalImpuesto.BaseImponible)
                            });
                        }
                    }

                    gf.infoNotaCredito.ValorModificacion = string.Format("{0:0.00}", (gf.totalSinImpuesto + gf.totalImpuestos.Sum(x => x.Valor)));
                    gf.infoNotaCredito.Motivo = row.docuTipRet.ToUpper();

                    // Generar detalle de la nota de crédito
                    if (detalles != null && detalles.Count > 0)
                    {
                        gf.detalles = new List<TiposDatos.Detalle>();

                        foreach (var detalle in detalles)
                        {
                            var det = new TiposDatos.Detalle
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
                                        BaseImponible = (decimal) detalle.itemprc,
                                        Valor = (decimal) detalle.itempiv
                                    }
                                }
                            };

                            gf.detalles.Add(det);
                        }
                    }


                    gf.informacionAdicionales = new List<TiposDatos.InformacionAdicional>()
                    {
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "Email",
                            Descripcion = row.nideml
                        },
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "Dirección",
                            Descripcion = row.niddir
                        },
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "Nota de Crédito ID",
                            Descripcion = row.ventscl
                        },
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "RUC Proveedor",
                            Descripcion = row.rucDev
                        }
                    };

                    string path = gf.generarNotaCredito();

                    if (gf.ArchivoFirmado == false)
                    {
                        ObjComportamiento.showMsgError("El Archivo no pudo ser firmado");
                        return;
                    }

                    System.Windows.Forms.Application.DoEvents();
                    notaCredito.registraNotaCreditoElect(row.ventscl, gf.secuencial.ToString().PadLeft(9, '0'), gf.claveAcceso);
                    System.Windows.Forms.Application.DoEvents();

                    row.autosri = gf.claveAcceso;

                    var (xmlData, xmlData1) = General.GetXmlParaEnvio(path);

                    // Envio de factura generada al web service
                    client.Token = resp.Token;

                    row.etdofac = "ESPERA SRI";
                    System.Windows.Forms.Application.DoEvents();

                    // Proceso de Envío de Notas de Crédito al SRI.
                    NotaCreditoModel fm = new NotaCreditoModel
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

                    row.etdofac = "RECIVIENDO";
                    
                    System.Windows.Forms.Application.DoEvents();

                    var notaCreditoRespuesta = await client.obtieneNotaCredito(fm);
                    Thread.Sleep(5000);
                    if (notaCreditoRespuesta != null)
                    {
                        if (notaCreditoRespuesta.estado == "A")
                        {
                            row.etdofac = "AUTORIZADO";

                            System.Windows.Forms.Application.DoEvents();
                            // Grabar los archivos recibidos
                            //creaArchivosRecividos(rete, gf.claveAcceso);
                        }
                        else
                        {
                            if (notaCreditoRespuesta.estado == "P") row.etdofac = "PENDIENTE";
                            if (notaCreditoRespuesta.estado == "D") row.etdofac = "DEVUELTA";
                            if (notaCreditoRespuesta.estado == "N") row.etdofac = "NO PROCESADO";
                            if (notaCreditoRespuesta.estado == "V") row.etdofac = "VALIDADO";

                            System.Windows.Forms.Application.DoEvents();
                        }

                        notaCredito.actualizaNotaCreditoElect(notaCreditoRespuesta.ntcrslc ?? row.ventscl,
                            gf.secuencial.ToString().PadLeft(9, '0'),
                            gf.claveAcceso,
                            notaCreditoRespuesta.estado,
                            notaCreditoRespuesta.estadoMail,
                            notaCreditoRespuesta.mensaje);
                        System.Windows.Forms.Application.DoEvents();
                    }

                    if (notaCreditoRespuesta.estado == "N")
                    {
                        row.etdofac = "VERIFICANDO";
                        System.Windows.Forms.Application.DoEvents();

                        var notaCreditoChecked = await client.VerificaNotaCredito(fm);
                        if (notaCreditoChecked != null)
                        {
                            if (notaCreditoChecked.estado != notaCreditoRespuesta.estado)
                            {
                                if (notaCreditoChecked.estado == "A") row.etdofac = "AUTORIZADO";
                                if (notaCreditoChecked.estado == "P") row.etdofac = "PENDIENTE";
                                if (notaCreditoChecked.estado == "N") row.etdofac = "NO PROCESADO";
                                if (notaCreditoChecked.estado == "D") row.etdofac = "DEVUELTA";
                                if (notaCreditoChecked.estado == "V") row.etdofac = "VALIDADO";

                                notaCreditoRespuesta.estado = notaCreditoChecked.estado;
                                notaCreditoRespuesta.estadoMail = notaCreditoChecked.estadoMail;
                                notaCreditoRespuesta.mensaje = notaCreditoChecked.mensaje;
                            }
                            else
                            {
                                if (notaCreditoRespuesta.estado == "A") row.etdofac = "VERIFICANDO";
                                if (notaCreditoRespuesta.estado == "P") row.etdofac = "PENDIENTE";
                                if (notaCreditoRespuesta.estado == "N") row.etdofac = "NO PROCESADO";
                                if (notaCreditoRespuesta.estado == "D") row.etdofac = "DEVUELTA";
                                if (notaCreditoRespuesta.estado == "V") row.etdofac = "VALIDADO";
                            }

                            System.Windows.Forms.Application.DoEvents();
                            notaCredito.actualizaNotaCreditoElect(notaCreditoRespuesta.ntcrslc ?? row.ventscl,
                                                                    gf.secuencial.ToString().PadLeft(9, '0'),
                                                                    gf.claveAcceso,
                                                                    notaCreditoRespuesta.estado,
                                                                    notaCreditoRespuesta.estadoMail,
                                                                    notaCreditoRespuesta.mensaje);
                            System.Windows.Forms.Application.DoEvents();
                        }
                        else
                        {
                            if (notaCreditoRespuesta.estado == "N") row.etdofac = "NO PROCESADO";
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                }
                else
                {
                    Ventas ventas = new Ventas(General.connectionString);

                    System.Windows.Forms.Application.DoEvents();
                    if (ventas.checkIfVentWasProcess(row.ventscl))
                    {
                        ObjComportamiento.showMsgError("Esta venta ya fue procesada y autorizada");
                        return;
                    }

                    // Verificar si existe ventscl con estado = N
                    string sec = row.secuencial ?? "";
                    string autorizacion = row.autosri ?? "";

                    if (sec != "" && sec != "0")
                    {
                        gf.secuencial = int.Parse(sec);
                    }
                    else
                    {
                        gf.secuencial = ventas.obtenerSecuencialFactura();
                        SecuencialFacGenerado = true;
                    }

                    gf.claveAcceso = autorizacion != "" ? autorizacion : "";

                    gf.razonSocialComprador = row.nidnbr == "CLIENTE FINAL" ? "CONSUMIDOR FINAL" : row.nidnbr.Trim();
                    gf.direccionComprador = row.niddir == "" ? null : row.niddir.Trim();
                    gf.IdComprador = row.nidnid;
                    gf.tipoIdentificadorComprador = getTipoComprador(row.nidtip, row.nidnid);

                    var detalles = ventas.getDetalleVenta(row.ventscl);

                    var tieneIva = detalles.Where(x => x.itemiva > 0).ToList();
                    var NoTieneIva = detalles.Where(x => x.itemiva == 0).ToList();

                    var SubPrecioConIva = (decimal)tieneIva.Select(o => o.itemprc * o.itemcnt).Sum();
                    var SubPrecioSinIva = (decimal)NoTieneIva.Select(o => o.itemprc * o.itemcnt).Sum();

                    gf.totalSinImpuesto = Math.Round((decimal)SubPrecioConIva + (decimal)SubPrecioSinIva, 2);
                    gf.totalDescuento = Math.Round((decimal)row.ventdsc, 2);
                    gf.importeTotal = Math.Round((decimal)row.venttot, 2);

                    if (row.ventsbt > 0)
                        gf.porcentajeIva = (int)Math.Round((decimal)row.ventiva * 100 / (decimal)row.ventsbt, 0);
                    else
                        gf.porcentajeIva = 0;

                    int cantTotImpuestos = 0;
                    if (tieneIva.Count > 0) cantTotImpuestos++;
                    if (NoTieneIva.Count > 0) cantTotImpuestos++;

                    if (cantTotImpuestos == 1)
                    {
                        if (tieneIva.Count > 0 && NoTieneIva.Count == 0)
                        {
                            gf.totalImpuestos = new List<TiposDatos.TotalImpuesto>() {
                                new TiposDatos.TotalImpuesto
                                {
                                    codigo = TiposDatos.TipoDeImpuesto.IVA,
                                    CodigoPorcentaje = gf.getTipoTarifaIVA(),
                                    BaseImponible = SubPrecioConIva,
                                    Valor = (decimal) tieneIva.Select(o => o.itemiva*o.itemcnt).Sum()
                                }
                            };
                        }
                        if (NoTieneIva.Count > 0 && tieneIva.Count == 0)
                        {
                            gf.totalImpuestos = new List<TiposDatos.TotalImpuesto>() {
                            new TiposDatos.TotalImpuesto
                                {
                                    codigo = TiposDatos.TipoDeImpuesto.IVA,
                                    CodigoPorcentaje = TiposDatos.TarifaIVA.Cero,
                                    BaseImponible = (decimal) NoTieneIva.Select(o => o.itemprc*o.itemcnt).Sum(),
                                    Valor = 0
                                }
                            };
                        }
                    }
                    else if (cantTotImpuestos > 1)
                    {
                        gf.totalImpuestos = new List<TiposDatos.TotalImpuesto>() {
                            new TiposDatos.TotalImpuesto
                            {
                                codigo = TiposDatos.TipoDeImpuesto.IVA,
                                CodigoPorcentaje = gf.getTipoTarifaIVA(),
                                BaseImponible = Math.Round((decimal)tieneIva.Select(o =>  o.itemprc*o.itemcnt).Sum(),2),
                                Valor = Math.Round((decimal) tieneIva.Select(o => o.itemiva*o.itemcnt).Sum(),2)
                            },
                            new TiposDatos.TotalImpuesto
                            {
                                codigo = TiposDatos.TipoDeImpuesto.IVA,
                                CodigoPorcentaje = TiposDatos.TarifaIVA.Cero,
                                BaseImponible = Math.Round((decimal) NoTieneIva.Select(o => o.itempto).Sum(),2),
                                Valor = 0
                            }
                        };
                    }
                    else
                    {

                    }

                    gf.propina = 0;
                    gf.moneda = TiposDatos.Moneda.DOLAR;

                    gf.pagos = new List<TiposDatos.Pago>();

                    // Traer datos de pagos
                    System.Windows.Forms.Application.DoEvents();
                    var pagos = ventas.getPagosVenta(row.ventscl);
                    System.Windows.Forms.Application.DoEvents();

                    if (pagos.Count > 0)
                    {
                        foreach (var pago in pagos)
                        {
                            gf.pagos.Add(new TiposDatos.Pago
                            {
                                formaDePago = pago.isdiferido || pago.iscorriente ? TiposDatos.FormasDePago.TarjetaCredito :
                                              pago.ischeque ? TiposDatos.FormasDePago.OtrosConSistemaFinanciero :
                                              TiposDatos.FormasDePago.SinSistemaFinanciero,
                                Total = Math.Round((decimal)(pago.valor - pago.vuelto), 2),
                                Plazo = pago.meses,
                                TiempoPlazo = pago.meses > 0 ? TiposDatos.TiempoPlazo.Meses : TiposDatos.TiempoPlazo.Ninguno
                            });
                        }
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

                    // Traer detalle de los items de la factura
                    gf.detalles = new List<TiposDatos.Detalle>();

                    System.Windows.Forms.Application.DoEvents();

                    foreach (var detalle in detalles)
                    {
                        gf.detalles.Add(new TiposDatos.Detalle
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
                                    BaseImponible = Math.Round((decimal) detalle.itemprc * detalle.itemcnt,2),
                                    Valor = Math.Round((decimal) detalle.itemiva*detalle.itemcnt,2)
                                }
                            }
                        });
                    }

                    gf.informacionAdicionales = new List<TiposDatos.InformacionAdicional>()
                    {
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "Mesa",
                            Descripcion = row.ventmsa ?? "0000"
                        },
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "Pedido",
                            Descripcion = row.peddscl
                        },
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "Venta",
                            Descripcion = row.ventscl
                        },
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "Email Cliente",
                            Descripcion = row.nideml.Trim() ?? "Sin Email"
                        },
                        new TiposDatos.InformacionAdicional
                        {
                            Nombre = "RUC Proveedor",
                            Descripcion = row.rucDev
                        }
                    };

                    row.etdofac = "GENERANDO";
                    System.Windows.Forms.Application.DoEvents();

                    archivo = gf.generarFactura();

                    if (gf.ArchivoFirmado == false)
                    {
                        ObjComportamiento.showMsgError("El Archivo no pudo ser firmado");
                        return;
                    }

                    System.Windows.Forms.Application.DoEvents();
                    ventas.registraFactElect(row.ventscl, gf.secuencial.ToString().PadLeft(9, '0'), gf.claveAcceso, gf.fechaEmision,
                        "1", ((int)gf.ambiente).ToString(), gf.serie, gf.ruc, ((int)gf.codDoc).ToString(), ((int)gf.emision).ToString());
                    System.Windows.Forms.Application.DoEvents();

                    row.autosri = gf.claveAcceso;

                    var (xmlData, xmlData1) = General.GetXmlParaEnvio(archivo);

                    // Envio de factura generada al web service
                    client.Token = resp.Token;

                    row.etdofac = "ESPERA SRI";
                    System.Windows.Forms.Application.DoEvents();

                    FacturaModel fm = new FacturaModel
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
                        claveDownload = row.claveDownload
                    };

                    var factura = await client.sendFactura(fm);
                    System.Windows.Forms.Application.DoEvents();

                    bool recivido = false;
                    int timeTrans = 0;

                    while (recivido == false)
                    {
                        row.etdofac = "VERIFICANDO";
                        System.Windows.Forms.Application.DoEvents();

                        var facturaChecked = await client.VerificaFactura(fm);
                        System.Windows.Forms.Application.DoEvents();
                        if (facturaChecked != null)
                        {
                            if (facturaChecked.estado == "A") row.etdofac = "AUTORIZADO";
                            if (facturaChecked.estado == "P") row.etdofac = "PENDIENTE";
                            if (facturaChecked.estado == "N") row.etdofac = "NO PROCESADO";
                            if (facturaChecked.estado == "D") row.etdofac = "DEVUELTA";
                            if (facturaChecked.estado == "V") row.etdofac = "VALIDADO";

                            System.Windows.Forms.Application.DoEvents();

                            ventas.actualizaFactElect(facturaChecked.ventslc ?? row.ventscl,
                                                    gf.secuencial.ToString().PadLeft(9, '0'),
                                                    gf.claveAcceso,
                                                    facturaChecked.estado,
                                                    facturaChecked.estadoMail,
                                                    facturaChecked.mensaje);

                            System.Windows.Forms.Application.DoEvents();
                            _retornaEstado(facturaChecked.estado);
                            System.Windows.Forms.Application.DoEvents();
                            recivido = true;
                            Thread.Sleep(3000);
                            break;
                        }
                        else
                        {
                            _retornaEstado("W");
                        }

                        if (timeTrans >= 120)
                        {
                            ObjComportamiento.showMsgInfo("La Factura no pudo ser procesada en el SRI, vaya a la pantalla de \n\"Facturación Electrónica Manual\" y reintente procesarla");
                            break;
                        }

                        Thread.Sleep(1000);
                        timeTrans++;
                    }
                }

                _Finalizado();
            }
            catch (Exception err)
            {
                ObjComportamiento.showMsgError(err.Message);

                if (SecuencialFacGenerado)
                {
                    Ventas ventas = new Ventas(General.connectionString);

                    ventas.undoSecuencialFactura();
                }

                if (SecuencialRetGenerado)
                {
                    Entidades.Retencion retencion = new Entidades.Retencion(General.connectionString);

                    retencion.undoSecuencialRetencion();
                }

                if (SecuencialNcrGenerado)
                {
                    Entidades.NotaCredito notacredito = new Entidades.NotaCredito(General.connectionString);

                    //notacredito.undoSecuencialNotaCredito();
                }
            }
        }
    }
}
