using LibFacturacionElectronica;
using LibFacturacionElectronica.cls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using Entidades.web;
using AccessDatos;
using AccessDatos.Model;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlTestFactElect.xaml
    /// </summary>
    public partial class ctlTestFactElect : UserControl
    {
        public ctlTestFactElect()
        {
            InitializeComponent();
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            GeneraFactura gf = new GeneraFactura();
            gf.xmlFolder = General.RUTA_XML_GENERADOS;
            gf.ambiente = TiposDatos.TipoAmbiente.Pruebas;
            gf.emision = TiposDatos.TipoEmision.EmisionNormal;
            gf.codigoNumerico = 1003;
            gf.razonSocial = "Test de razon social";
            gf.nombreComercial = "Test nombre comercial";
            gf.ruc = "0914556634001";
            gf.serie = "001-001";
            gf.secuencial = 107;
            gf.puntoEmision = "001";
            gf.direccionMatriz = "Por ahi y la otra";
            gf.agenteRetencion = TiposDatos.AgenteRetencion.NO;
            gf.establecimiento = "001";
            
            gf.fechaEmision = DateTime.Now;
            gf.llevaContabilidad = TiposDatos.LLevaContabilidad.SI;
            gf.direccionEstablecimiento = "cruzando la calle";
            gf.tipoIdentificadorComprador = TiposDatos.TipoIdentificadorComprador.Cedula;
            gf.razonSocialComprador = "Juan de los palotes";
            gf.direccionComprador = "en la esquina de allá";
            gf.IdComprador = "0914556634";
            gf.totalSinImpuesto = 100;
            gf.totalDescuento = 0;
            gf.importeTotal = 112;

            gf.totalImpuestos = new List<TiposDatos.TotalImpuesto>();
            gf.totalImpuestos.Add(new TiposDatos.TotalImpuesto
            {
                 codigo = TiposDatos.TipoDeImpuesto.IVA,
                 CodigoPorcentaje = TiposDatos.TarifaIVA.Doce,
                 BaseImponible = 100,
                 Valor = 12
            });
            gf.propina = 0;
            gf.moneda = TiposDatos.Moneda.DOLAR;

            gf.pagos = new List<TiposDatos.Pago>();
            gf.pagos.Add(new TiposDatos.Pago
            {
                formaDePago = TiposDatos.FormasDePago.TarjetaCredito,
                Total = 100,
                Plazo = 3,
                TiempoPlazo = TiposDatos.TiempoPlazo.Meses
            });

            gf.pagos.Add(new TiposDatos.Pago
            {
                formaDePago = TiposDatos.FormasDePago.SinSistemaFinanciero,
                Total = 12,
                Plazo = 0,
                TiempoPlazo = TiposDatos.TiempoPlazo.Ninguno
            });

            gf.detalles = new List<TiposDatos.Detalle>();
            gf.detalles.Add(new TiposDatos.Detalle
            {
                CodigoPrincipal = "1001",
                CodigoAuxiliar = null,
                Descripcion = "Chaulafán Especial",
                Cantidad = 1,
                PrecioUnitario = 5,
                Descuento = 0,
                PrecioTotalSinImpuesto = 5,
                Impuesto = new List<TiposDatos.Impuesto>()
                {
                    new TiposDatos.Impuesto
                    {
                        codigo= TiposDatos.TipoDeImpuesto.IVA,
                        CodigoPorcentaje = TiposDatos.TarifaIVA.Doce,
                        BaseImponible = 5,
                        Valor = decimal.Parse("0,6")
                    }
                }
            });
            gf.detalles.Add(new TiposDatos.Detalle
            {
                CodigoPrincipal = "1002",
                CodigoAuxiliar = null,
                Descripcion = "Tallarin Especial",
                Cantidad = 1,
                PrecioUnitario = 5,
                Descuento = 0,
                PrecioTotalSinImpuesto = 5,
                Impuesto = new List<TiposDatos.Impuesto>()
                {
                    new TiposDatos.Impuesto
                    {
                        codigo= TiposDatos.TipoDeImpuesto.IVA,
                        CodigoPorcentaje = TiposDatos.TarifaIVA.Doce,
                        BaseImponible = 5,
                        Valor = decimal.Parse("0,6")
                    }
                }
            });

            gf.informacionAdicionales = new List<TiposDatos.InformacionAdicional>()
            {
                new TiposDatos.InformacionAdicional
                {
                    Nombre = "Mesa",
                    Descripcion = "02"
                },
                new TiposDatos.InformacionAdicional
                {
                    Nombre = "Pedido",
                    Descripcion = "20025"
                }
            };

            string archivo = gf.generarFactura();

            //string xmlData = General.GetXmlString(archivo)
            

            //var respuesta = new HttpClienteAPI();
            //respuesta.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJydWMiOiIwOTE0NTU2NjM0MDAxIiwidXN1YXJpbyI6ImlhbGJhbiIsImVtcHJlc2FpZCI6IjVDMTA3MDcwLTQyMkMtNEE0NS05QjRFLThENTU2NUI2NkQwMSIsImNvbm5lY3Rpb25kYXRlIjoiMTAvMjIvMjAyMiA3OjExOjA3IFBNIiwiZ3VpZCI6ImJlMWViNWM5LTc4NWEtNDljYi04MjY5LWUxNTFiZjczNThjMSIsImFwcGxpY2F0aW9uIjoiVHJlc2tlbEFQSSIsIm5iZiI6MTY2NjQ2NTg2NywiZXhwIjoxNjY3MDcwNjY3LCJpYXQiOjE2NjY0NjU4Njd9.VFXnouJq4bErMyX0KCc14iSpZOgYNP6StdwVAJdwCrk";
            //respuesta.TrsklUrl = "https://localhost:44351";
            //var factura = await respuesta.sendFactura(
            //    new FacturaModel
            //    {
            //        ruc = gf.ruc,
            //        ventasec = "0000001234",
            //        empresaid = "5C107070-422C-4A45-9B4E-8D5565B66D01",
            //        codigoLocal = gf.codigoNumerico.ToString(),
            //        nombreLocal = gf.nombreComercial,
            //        idCliente = gf.IdComprador,
            //        nombreCliente = gf.razonSocialComprador,
            //        emailCliente = "ignacio.alban@gmail.com",
            //        data = xmlData
            //    }
            //);
        }

        private void btnTestRetencion_Click(object sender, RoutedEventArgs e)
        {
            GeneraFactura gf = new GeneraFactura();
            gf.xmlFolder = General.RUTA_XML_GENERADOS;
            gf.ambiente = TiposDatos.TipoAmbiente.Pruebas;
            gf.emision = TiposDatos.TipoEmision.EmisionNormal;
            gf.codigoNumerico = 1003;
            gf.razonSocial = "Test de razon social";
            gf.nombreComercial = "Test nombre comercial";
            gf.ruc = "0914556634001";
            gf.serie = "001001";
            gf.secuencial = 107;
            gf.puntoEmision = "001";
            gf.direccionMatriz = "Por ahi y la otra";
            gf.agenteRetencion = TiposDatos.AgenteRetencion.NO;
            gf.establecimiento = "001";

            gf.fechaEmision = DateTime.Now;
            gf.llevaContabilidad = TiposDatos.LLevaContabilidad.SI;
            gf.direccionEstablecimiento = "cruzando la calle";

            gf.RucProveedor = "0914556634001";
            gf.DireccionProveedor = "Una esquina y la otra";
            gf.PeriodoFiscal = "01/2023";
            gf.RazonSocialProveedor = "IGNACIO S.A";
            gf.tipoIdentificadorProveedor = TiposDatos.TipoIdentificadorComprador.Ruc;

            gf.impuestos = new List<ImpuestoRetencion>
            {
                new ImpuestoRetencion
                {
                    Codigo = "0000000001",
                    CodigoRetencion = "0001",
                    BaseImponible = 100.ToString(),
                    PorcentajeRetener = "10",
                    Valor = "10",
                    CodDocSustento = "01",
                    NumDocSustento = "0010010000000001",
                    FechaEmisionSustento = DateTime.Now.ToString("dd/MM/yyyy")
                },
                new ImpuestoRetencion
                {
                    Codigo = "0000000002",
                    CodigoRetencion = "0002",
                    BaseImponible = 100.ToString(),
                    PorcentajeRetener = "10",
                    Valor = "10",
                    CodDocSustento = "01",
                    NumDocSustento = "001-001-0000000001",
                    FechaEmisionSustento = DateTime.Now.ToString("dd/MM/yyyy")
                }
            };

            //gf.tipoIdentificadorComprador = TiposDatos.TipoIdentificadorComprador.Cedula;
            //gf.razonSocialComprador = "Juan de los palotes";
            //gf.direccionComprador = "en la esquina de allá";
            //gf.IdComprador = "0914556634";
            //gf.totalSinImpuesto = 100;
            //gf.totalDescuento = 0;
            //gf.importeTotal = 112;

            //gf.totalImpuestos = new List<TiposDatos.TotalImpuesto>();
            //gf.totalImpuestos.Add(new TiposDatos.TotalImpuesto
            //{
            //    codigo = TiposDatos.TipoDeImpuesto.IVA,
            //    CodigoPorcentaje = TiposDatos.TarifaIVA.Doce,
            //    BaseImponible = 100,
            //    Valor = 12
            //});
            //gf.propina = 0;
            //gf.moneda = TiposDatos.Moneda.DOLAR;

            //gf.pagos = new List<TiposDatos.Pago>();
            //gf.pagos.Add(new TiposDatos.Pago
            //{
            //    formaDePago = TiposDatos.FormasDePago.TarjetaCredito,
            //    Total = 100,
            //    Plazo = 3,
            //    TiempoPlazo = TiposDatos.TiempoPlazo.Meses
            //});

            //gf.pagos.Add(new TiposDatos.Pago
            //{
            //    formaDePago = TiposDatos.FormasDePago.SinSistemaFinanciero,
            //    Total = 12,
            //    Plazo = 0,
            //    TiempoPlazo = TiposDatos.TiempoPlazo.Ninguno
            //});

            //gf.detalles = new List<TiposDatos.Detalle>();
            //gf.detalles.Add(new TiposDatos.Detalle
            //{
            //    CodigoPrincipal = "1001",
            //    CodigoAuxiliar = null,
            //    Descripcion = "Chaulafán Especial",
            //    Cantidad = 1,
            //    PrecioUnitario = 5,
            //    Descuento = 0,
            //    PrecioTotalSinImpuesto = 5,
            //    Impuesto = new List<TiposDatos.Impuesto>()
            //    {
            //        new TiposDatos.Impuesto
            //        {
            //            codigo= TiposDatos.TipoDeImpuesto.IVA,
            //            CodigoPorcentaje = TiposDatos.TarifaIVA.Doce,
            //            BaseImponible = 5,
            //            Valor = decimal.Parse("0,6")
            //        }
            //    }
            //});
            //gf.detalles.Add(new TiposDatos.Detalle
            //{
            //    CodigoPrincipal = "1002",
            //    CodigoAuxiliar = null,
            //    Descripcion = "Tallarin Especial",
            //    Cantidad = 1,
            //    PrecioUnitario = 5,
            //    Descuento = 0,
            //    PrecioTotalSinImpuesto = 5,
            //    Impuesto = new List<TiposDatos.Impuesto>()
            //    {
            //        new TiposDatos.Impuesto
            //        {
            //            codigo= TiposDatos.TipoDeImpuesto.IVA,
            //            CodigoPorcentaje = TiposDatos.TarifaIVA.Doce,
            //            BaseImponible = 5,
            //            Valor = decimal.Parse("0,6")
            //        }
            //    }
            //});

            gf.informacionAdicionales = new List<TiposDatos.InformacionAdicional>()
            {
                new TiposDatos.InformacionAdicional
                {
                    Nombre = "TELEFONO",
                    Descripcion = "12345678"
                },
                new TiposDatos.InformacionAdicional
                {
                    Nombre = "EMAIL",
                    Descripcion = "ignacio.alban@gmail.com"
                },
            };

            string archivo = gf.generarRetencion();

            //string xmlData = General.GetXmlString(archivo);

            /*var respuesta = new HttpClienteAPI();
            respuesta.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJydWMiOiIwOTE0NTU2NjM0MDAxIiwidXN1YXJpbyI6ImlhbGJhbiIsImVtcHJlc2FpZCI6IjVDMTA3MDcwLTQyMkMtNEE0NS05QjRFLThENTU2NUI2NkQwMSIsImNvbm5lY3Rpb25kYXRlIjoiMTAvMjIvMjAyMiA3OjExOjA3IFBNIiwiZ3VpZCI6ImJlMWViNWM5LTc4NWEtNDljYi04MjY5LWUxNTFiZjczNThjMSIsImFwcGxpY2F0aW9uIjoiVHJlc2tlbEFQSSIsIm5iZiI6MTY2NjQ2NTg2NywiZXhwIjoxNjY3MDcwNjY3LCJpYXQiOjE2NjY0NjU4Njd9.VFXnouJq4bErMyX0KCc14iSpZOgYNP6StdwVAJdwCrk";
            respuesta.TrsklUrl = "https://localhost:44351";
            var factura = await respuesta.sendFactura(
                new FacturaModel
                {
                    ruc = gf.ruc,
                    ventasec = "0000001234",
                    empresaid = "5C107070-422C-4A45-9B4E-8D5565B66D01",
                    codigoLocal = gf.codigoNumerico.ToString(),
                    nombreLocal = gf.nombreComercial,
                    idCliente = gf.IdComprador,
                    nombreCliente = gf.razonSocialComprador,
                    emailCliente = "ignacio.alban@gmail.com",
                    data = xmlData
                }
            );*/
        }
    }
}
