using Entidades;
using Entidades.modelos;
using LibKeys;
using LibPrint;
using LibPrint.dataset;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlSepFact.xaml
    /// </summary>
    public partial class ctlSepFact : UserControl
    {
        public Window contenedor { get; set; }
        public ctlVenta obj { get; set; }
        private Ventas ventas = new Ventas(General.connectionString);
        private List<mPago> pagos = new List<mPago>();

        public Boolean PUEDE_DAR_DESCUENTO= false;
        public Boolean PUEDE_DAR_REGALO = false;

        public event FinalizaSeparacion finalizaSeparacion;
        public delegate void FinalizaSeparacion();

        private struct Producto
        {
            public String codigo;
            public String nombre;
            public int cantidad;
            public Double precio;
            public Double iva;
            public Double total;
            public int row;
        };

        private Producto producto = new Producto();
        private Producto prodSell = new Producto();
        private String peddScl;

        private clsTextBox clsNid;

        public ctlSepFact(String peddScl)
        {
            InitializeComponent();

            clsNid = new clsTextBox(txtCedula, General.USA_TECLADO_PANTALLA);

            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsNid.setMaxLength(13);

            this.peddScl = peddScl;
            getPedido(peddScl);

            if (this.PUEDE_DAR_DESCUENTO)
            {
                btnDescto.IsEnabled = true;
            }

            if (this.PUEDE_DAR_REGALO)
            {
                btngift.IsEnabled = true;
            }     
        }

        private void sbLimpiar()
        {
            txtCliente.Text = "";
            txtCedula.Text = "";
            txtCambio.Text = "0,00";
            txtDscto.Text = "0,00";
            txtSubTot.Text = "0,00";
            txtRecibido.Text = "0,00";
            txtIva.Text = "0,00";
            txtTotal.Text = "0,00";

            btnToPay.IsEnabled = true;

            btnFact.IsEnabled = true;
            btnProform.IsEnabled = false;
            btnImpFact.IsEnabled = false;
            btnImpPro.IsEnabled = false;

            dgvFact.Items.Clear();
            //dgvPedd.Items.Clear();            
        }

        private void sbActivaVenta(Boolean value)
        {
            btnCancelar.IsEnabled = value;
            btnToPay.IsEnabled = value;
            if (this.PUEDE_DAR_DESCUENTO && value) btnDescto.IsEnabled = value;
            if (this.PUEDE_DAR_REGALO && value) btngift.IsEnabled = value;
            btnDescto.IsEnabled = value;
            btngift.IsEnabled = value;
        }

        private void getPedido(String peddScl)
        {
            sbLimpiar();    
            this.ventas.getPedido(peddScl);
            sbActivaVenta(true);

            foreach (VentaDetalle detalle in this.ventas.detVenta)
            {
                ProdPedd prod = new ProdPedd();
                prod.codigo = detalle.itemcod;
                prod.producto = detalle.nbrprd;
                prod.cantidad = detalle.itemcnt;
                prod.precio = detalle.itemprc;
                prod.iva = detalle.itemiva;
                prod.total = detalle.itempto;

                dgvPedd.Items.Add(prod);
            }

            this.ventas.peddscl = peddScl;  
            
            if(dgvPedd.Items.Count > 0)
            {
                btnFact.IsEnabled = true;
            }          
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
            finalizaSeparacion();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            sbLimpiar();
            dgvPedd.Items.Clear();
            getPedido(peddScl);
        }

        private void btnToPay_Click(object sender, RoutedEventArgs e)
        {
            if (dgvFact.Items.Count == 0)
            {
                ObjComportamiento.showMsgError("No se la seleccionado items a factuar");
                return;
            }    

            winGeneral win = new winGeneral();
            win.Title = "Pago de Factura";
            win.Width = 940;
            win.Height = 450;

            ctlPagos crea = new ctlPagos(txtTotal.Text.ToString());
            crea.contenedor = win;
            crea.pagoRealizado += new ctlPagos.PagoRealizado(Pago_Realizado);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void Pago_Realizado(List<mPago> pagos)
        {
            Double valor = 0;// pagos[0].valor;
            foreach (mPago pago in pagos)
            {
                valor += pago.valor;
            }

            this.pagos.Clear();

            txtRecibido.Text = String.Format("{0:0.00}", valor);
            Double total = General.StringToDouble(txtTotal.Text.ToString());
            if (valor > total)
            {
                txtCambio.Text = String.Format("{0:0.00}", valor - total);
            }
            btnFact.IsEnabled = true;
            btnImpFact.IsEnabled = true;
            this.pagos = pagos;
        }

        private void btnDescto_Click(object sender, RoutedEventArgs e)
        {
            if (dgvFact.Items.Count == 0)
            {
                ObjComportamiento.showMsgError("Debe seleccionar items para generar un descuento");
                return;
            }

            winGeneral win = new winGeneral();
            win.Title = "Listado de Descuentos a aplicar";
            win.Width = 500;
            win.Height = 400;

            if (General.USA_DESCUENTOS_PERSONALIZADOS)
            {
                ctlDescuentos crea = new ctlDescuentos();

                crea.secuencial = "";
                crea.contenedor = win;
                crea.tipo = "D";
                crea.data = ventas.getMotivosDescuento("D");
                crea.generar();

                crea.aceptaMotivo += new ctlDescuentos.AceptaMotivo(Crea_aceptaDescuento);
                win.Contenido.Children.Add(crea);
            }
            else
            {
                ctlMotivos crea = new ctlMotivos();
                crea.secuencial = "";
                crea.contenedor = win;
                crea.tipo = "D";
                crea.data = ventas.getMotivosDescuento("D");
                crea.generar();

                crea.aceptaMotivo += new ctlMotivos.AceptaMotivo(Crea_aceptaDescuento);
                win.Contenido.Children.Add(crea);
            }

            /*ctlMotivos crea = new ctlMotivos();
            crea.secuencial = "";
            crea.contenedor = win;
            crea.tipo = "D";
            crea.data = ventas.getMotivosDescuento("D");
            crea.generar();

            crea.aceptaMotivo += new ctlMotivos.AceptaMotivo(Crea_aceptaDescuento);

            win.Contenido.Children.Add(crea);*/

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void Crea_aceptaDescuento(String secuencial, String mensaje)
        {
            Double desc = 0;

            if (!mensaje.Equals("SIN DESCUENTO"))
            {
                desc = General.StringToDouble(mensaje.Substring(mensaje.Length - 4, 3)) / 100;
            }

            Double subTotal = 0;

            foreach (ProdPedd detalle in dgvFact.Items)
            {
                ProdPedd prod = new ProdPedd();
                prod.codigo = detalle.codigo;
                prod.cantidad = detalle.cantidad;
                prod.precio = detalle.precio;
                prod.iva = detalle.iva;
                prod.total = detalle.total;

                subTotal += prod.cantidad * prod.precio;
            }

            Double iva = 0;
            Double total = 0;
            Double descuento = subTotal * desc;

            iva = (subTotal - descuento) * (General.IVA / 100);
            total = subTotal - descuento + iva;
            txtSubTot.Text = General.StringFormatMoney(subTotal);
            txtDscto.Text = General.StringFormatMoney(descuento);
            txtIva.Text = General.StringFormatMoney(iva);
            txtTotal.Text = General.StringFormatMoney(total);

            this.ventas.ventsbt = subTotal;
            this.ventas.ventdsc = descuento;
            this.ventas.ventdpc = int.Parse(mensaje.Substring(mensaje.Length - 4, 3));
            this.ventas.ventiva = iva;
            this.ventas.venttot = total;
        }

        private void btngift_Click(object sender, RoutedEventArgs e)
        {
            if (dgvFact.Items.Count == 0)
            {
                ObjComportamiento.showMsgError("Debe seleccionar items para generar un regalo");
                return;
            }

            winGeneral win = new winGeneral();
            win.Title = "Listado de Motivos para aplicar Regalos";
            win.Width = 500;
            win.Height = 400;

            ctlMotivos crea = new ctlMotivos();
            crea.secuencial = "";
            crea.contenedor = win;
            crea.tipo = "G";
            crea.data = ventas.getMotivosDescuento("G");
            crea.generar();

            crea.aceptaMotivo += new ctlMotivos.AceptaMotivo(Crea_aceptaRegalo);

            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void Crea_aceptaRegalo(String secuencial, String mensaje)
        {
            Double desc = 0;

            if (mensaje.Equals("SIN REGALO"))
            {
                ObjComportamiento.showMsgInfo("Seleccionó no generar regalo, proceda a Facturar o Proformar");
                return;
            }

            Double subTotal = 0;

            foreach (ProdPedd detalle in dgvPedd.Items)
            {
                ProdPedd prod = new ProdPedd();
                prod.codigo = detalle.codigo;
                prod.cantidad = detalle.cantidad;
                prod.precio = detalle.precio;
                prod.iva = detalle.iva;
                prod.total = detalle.total;

                subTotal += prod.cantidad * prod.precio;
            }

            Double iva = 0;
            Double total = 0;
            Double descuento = 0;

            iva = (subTotal - descuento) * (General.IVA / 100);
            total = subTotal - descuento + iva;
            txtSubTot.Text = General.StringFormatMoney(subTotal);
            txtDscto.Text = General.StringFormatMoney(descuento);
            txtIva.Text = General.StringFormatMoney(iva);
            txtTotal.Text = General.StringFormatMoney(total);

            this.ventas.ventsbt = subTotal;
            this.ventas.ventdsc = descuento;
            this.ventas.ventiva = iva;
            this.ventas.venttot = total;
            this.ventas.ventobv = mensaje;

            // Grabar regalo
            Respuesta res = this.ventas.grabarRegalo();
            ObjComportamiento.showMuestraMensaje(res);
            if (res.resultado)
            {
                sbLimpiar();
            }
        }

        private void llenaDetalleYPago()
        {
            this.ventas.detVenta.Clear();
            this.ventas.detPago.Clear();

            for (int i = 0; i < dgvFact.Items.Count; i++)
            {
                var prodPedd = (ProdPedd)dgvFact.Items[i];

                this.ventas.detVenta.Add(new VentaDetalle
                {
                    itemcod = prodPedd.codigo,
                    itemcnt = prodPedd.cantidad,
                    itemiva = prodPedd.iva,
                    itemprc = prodPedd.precio,
                    itempto = prodPedd.total,
                    nbrprd = prodPedd.producto
                });
            }
            mPago[] lst = new mPago[pagos.Count];
            pagos.CopyTo(lst);

            this.ventas.detPago = lst.ToList();
        }

        private void btnFact_Click(object sender, RoutedEventArgs e)
        {
            if(dgvFact.Items.Count == 0)
            {
                ObjComportamiento.showMsgError("No se ha seleccionado items a facturar");
                return;
            }

            this.llenaDetalleYPago();

            if (this.ventas.detVenta.Count > General.CANTIDAD_MAXIMA_ARTICULOS_FACTURAR)
            {
                ObjComportamiento.showMsgError("Existen más de " + General.CANTIDAD_MAXIMA_ARTICULOS_FACTURAR + " artículos por facturar, seleccione SEPARAR FACTURA");
                return;
            }

            // Mandamos a grabar el pedido
            if (txtCedula.Text.Equals(""))
            {
                ObjComportamiento.showMsgError("No ha ingresado un cliente válido");
                return;
            }

            /*if (this.ventas.detPago.Count == 0)
            {
                ObjComportamiento.showMsgError("Debe ingresar el pago respectivo del elemento a facturar.");
                return;
            }*/

            if (ObjComportamiento.showMsgYesNo("¿Desea grabar la venta?") == MessageBoxResult.Yes)
            {
                if (this.ventas.detVenta.Count > 0)
                {
                    //Se pasa a reducir el inventario
                    List<ProdPedd> prds = new List<ProdPedd>();
                    foreach (ProdPedd prd in dgvPedd.Items)
                    {
                        prds.Add(prd);
                    }
                    Items items = new Items(General.connectionString);
                    items.itemsInventario(this.ventas.peddscl, prds);

                    this.ventas.ventfac = true;
                    this.ventas.ventsep = "S";
                    Respuesta res = this.ventas.grabar();

                    if (res.resultado)
                    {
                        // Impresión de la factura
                        if (ObjComportamiento.showMsgYesNo("¿Desea imprimir la factura?") == MessageBoxResult.Yes)
                        {
                            this.ToPrint("F");
                        }

                        //btnImpFact_Click(sender, e);

                        ObjComportamiento.showMsgInfo(res.mensaje);

                        //btnCancelar_Click(sender, e);
                        sbLimpiar();
                    }
                    else
                    {
                        ObjComportamiento.showMsgError(res.mensaje);
                    }
                }
                /*else
                {
                    if (this.ventas.detVenta.Count == 0)
                        ObjComportamiento.showMsgError("El pedido a facturar no contiene items.");
                    if (this.ventas.detPago.Count == 0)
                        ObjComportamiento.showMsgError("No se ha ingresado el pago respectivo del pedido a facturar");
                }*/
            }
        }

        private void ToPrint(String tipo)
        {
            dsPrinter ds = new dsPrinter();
            for (int i = 0; i < this.ventas.detVenta.Count; i++)
            {
                DataRow dr = ds.tbPeddVta.NewRow();

                dr["itemNbr"] = this.ventas.detVenta[i].nbrprd;
                dr["itemCnt"] = this.ventas.detVenta[i].itemcnt;
                dr["itemPrc"] = this.ventas.detVenta[i].itemprc;
                dr["itemIva"] = this.ventas.detVenta[i].itemiva;
                dr["itemTot"] = this.ventas.detVenta[i].itempto;

                ds.tbPeddVta.Rows.Add(dr);
            }

            for (int i = 0; i < this.ventas.detPago.Count; i++)
            {
                DataRow dr = ds.tbPagoVta.NewRow();
                
                dr["pgoForm"] = this.ventas.detPago[i].isefectivo == true ? "E" : (this.ventas.detPago[i].ischeque == true ? "C" : "T");
                dr["pgoValor"] = this.ventas.detPago.Sum(x => x.valor);//this.ventas.detPago[i].valor;
                dr["pgoCbio"] = this.ventas.detPago.Sum(x => x.valor) - this.ventas.venttot;//this.ventas.detPago[i].vuelto;
                //dr["pgoValor"] = this.ventas.detPago[i].valor;
                //dr["pgoCbio"] = this.ventas.detPago[i].vuelto;

                ds.tbPagoVta.Rows.Add(dr);
            }

            try
            {
                //Imprimir cls = new Imprimir();
                EposPrint cls = new EposPrint();
                cls.TipoImpresora = General.TIPO_IMPRESORAS;
                Respuesta resp = cls.setPrinter(General.PRINTER_FACTURA);
                if (resp.resultado == false)
                {
                    ObjComportamiento.showMsgError(resp.mensaje);
                    return;
                }

                cls.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;

                /*List<String> data = cls.fnFacturaPrnt(ds.tbPeddVta, ds.tbPagoVta, this.ventas.peddscl,
                            this.ventas.ventmsa + " " + this.ventas.ventlta,
                            String.Format("{0:0.00}", this.ventas.ventsbt),
                            String.Format("{0:0.00}", this.ventas.ventdsc),
                            String.Format("{0:0.00}", this.ventas.ventiva),
                            String.Format("{0:0.00}", this.ventas.venttot),
                            String.Format("{0:0.00}", this.ventas.nidnid),
                            this.ventas.nidnbr, this.ventas.niddir, this.ventas.nidtlf,
                            String.Format("{0:0.00}", this.ventas.ventrcb),
                            String.Format("{0:0.00}", this.ventas.ventvto),
                            tipo);*/

                if (tipo == "F")
                {
                    cls.DefineSeparador = char.Parse(General.SEPARADOR);
                    cls.cantidadColumnas = General.CANTIDAD_COLUMNAS_IMPRESION;
                    cls.ImprimeNombreEmpresa = General.IMPRIME_NOMBRE_EMPRESA;
                    cls.ImprimeDireccionEmpresa = General.IMPRIME_DIRECCION_EMPRESA;
                    cls.ImprimeRucEmpresa = General.IMPRIME_RUC_EMPRESA;
                    cls.CabeceraNegrita = General.NEGRITA_EN_CABECERA;
                    cls.AlineacionCabecera = General.ALINEACION_CABECERA;
                    cls.ImprimeSeparadorEndCabecera = General.IMPRIME_SEPARADO_FIN_CABECERA;
                    cls.ImprimeNegritaCabeceraTabla = General.IMPRIME_NEGRITA_CABECERA_CONTENIDO;
                    cls.CantidadLineasSaltarEndTabla = General.CANTIDAD_LINEAS_FINAL_ITEMS;
                    cls.ImprimeSeparadorTablas = General.IMPRIME_SEPARADOR_FIN_ITEMS;
                    cls.CantidadDecimalesItems = General.CANTIDAD_DECIMALES_EN_ITEMS;
                    cls.ImprimeFormaDePago = General.IMPRIME_FORMA_PAGO;
                    cls.IncluyeDataMesero = General.IMPRIME_NOMBRE_MESERO;
                    cls.IncluyeAgradecimiento = General.IMPRIME_AGRADECIMIENTO;
                    cls.TipoImpresora = General.TIPO_IMPRESORAS;

                    //cls.data = cls.fnFacturaPrnt(ds.tbPeddVta, ds.tbPagoVta, this.ventas.peddscl,
                    cls.Factura(ds.tbPeddVta, ds.tbPagoVta, General.EMPRESA, General.DIRECCION_LOCAL,
                        General.RUC_EMPRESA, this.ventas.nidnid, this.ventas.nidnbr, this.ventas.niddir,
                            this.ventas.nidtlf, this.ventas.nideml, this.ventas.venttip, this.ventas.peddscl,
                            "",
                            this.ventas.ventmsa + " " + this.ventas.ventlta,
                            String.Format("{0:0.00}", this.ventas.ventsbt),
                            String.Format("{0:0.00}", this.ventas.ventsbn),
                            String.Format("{0:0.00}", this.ventas.ventdsc),
                            String.Format("{0:0.00}", this.ventas.ventdpc),
                            String.Format("{0:0.00}", this.ventas.ventiva),
                            String.Format("{0:0.00}", this.ventas.venttot),
                            String.Format("{0:0.00}", this.ventas.ventrcb),
                            String.Format("{0:0.00}", this.ventas.ventvto),
                            "",
                            this.ventas.ventscl.ToString().PadRight(10, '0'),
                            this.ventas.nidcdto, this.ventas.emprnbr, this.ventas.emprruc);
                    //cls.data = cls.fnFacturaPrnt(ds.tbPeddVta, ds.tbPagoVta, this.ventas.peddscl,
                    //        this.ventas.ventmsa + " " + this.ventas.ventlta,
                    //        String.Format("{0:0.00}", this.ventas.ventsbt),
                    //        String.Format("{0:0.00}", this.ventas.ventdsc),
                    //        String.Format("{0:0.00}", this.ventas.ventiva),
                    //        String.Format("{0:0.00}", this.ventas.venttot),
                    //        String.Format("{0:0.00}", this.ventas.nidnid),
                    //        this.ventas.nidnbr, this.ventas.niddir, this.ventas.nidtlf,
                    //        String.Format("{0:0.00}", this.ventas.ventrcb),
                    //        String.Format("{0:0.00}", this.ventas.ventvto),
                    //        tipo);
                }
                else
                {
                    //cls.data = cls.fnFacturaPrnt(ds.tbPeddVta, ds.tbPagoVta, this.ventas.peddscl,
                    //        this.ventas.ventmsa + " " + this.ventas.ventlta,
                    //        String.Format("{0:0.00}", this.ventas.ventsbt),
                    //        String.Format("{0:0.00}", this.ventas.ventdsc),
                    //        String.Format("{0:0.00}", this.ventas.ventiva),
                    //        String.Format("{0:0.00}", this.ventas.venttot),
                    //        String.Format("{0:0.00}", "_____________________________"),
                    //        "_____________________________", "_____________________________",
                    //        "_____________________________",
                    //        String.Format("{0:0.00}", this.ventas.ventrcb),
                    //        String.Format("{0:0.00}", this.ventas.ventvto),
                    //        tipo);
                }

                if (tipo.Equals("F"))
                {
                    cls.sbIpr();
                    //cls.sbIpr(General.PRINTER_FACTURA, "Ipr:Impresión de Factura " + this.ventas.peddscl);
                }
                else
                {
                    //cls.sbIpr(General.PRINTER_PROFORMA, "Ipr:Impresión de Proforma " + this.ventas.peddscl);
                }
                
                cls.Dispose();                
            }
            catch (Exception e)
            {
                ObjComportamiento.showMsgError("Error al imprimir la " + (tipo.Equals("F") == true ? "Factura" : "Proforma"));
            }            
        }

        private void btnImpFact_Click(object sender, RoutedEventArgs e)
        {
            this.ToPrint("F");
        }

        private void btnProform_Click(object sender, RoutedEventArgs e)
        {
            this.llenaDetalleYPago();

            // Mandamos a grabar el pedido
            if (ObjComportamiento.showMsgYesNo("¿Desea generar la Proforma?") == MessageBoxResult.Yes)
            {
                if (this.ventas.detVenta.Count > 0)
                {
                    this.ventas.ventfac = false;
                    Respuesta res = this.ventas.grabar();

                    if (res.resultado)
                    {
                        // Impresión de la Proforma
                        //btnImpPro_Click(sender, e);
                        this.ToPrint("P");

                        ObjComportamiento.showMsgInfo(res.mensaje);

                        btnCancelar_Click(sender, e);
                    }
                    else
                    {
                        ObjComportamiento.showMsgError(res.mensaje);
                    }
                }
                else
                {
                    if (this.ventas.detVenta.Count == 0)
                        ObjComportamiento.showMsgError("El pedido a proformar no contiene items.");
                }
            }
        }

        private void btnImpPro_Click(object sender, RoutedEventArgs e)
        {
            this.ToPrint("P");
        }

        private void btnCliFnl_Click(object sender, RoutedEventArgs e)
        {
            txtCedula.Text = "9999999999999";
            txtCliente.Text = "CLIENTE FINAL";

            this.ventas.nidnid = txtCedula.Text;
        }

        private void btnBscCli_Click(object sender, RoutedEventArgs e)
        {
            Clientes cliente = new Clientes(General.connectionString);

            cliente.nidNid = txtCedula.Text.ToString();
            if (cliente.getById())
            {
                txtCliente.Text = cliente.nidNbr;
                this.ventas.nidnbr = cliente.nidNbr;
                this.ventas.nidnid = cliente.nidNid;
                this.ventas.niddir = cliente.nidDir;
                this.ventas.nidtlf = cliente.nidTlf;
                this.ventas.nideml = cliente.nidEml;
            }
            else
            {
                winGeneral win = new winGeneral();
                win.Title = "Creación de cliente";
                win.Width = 710;
                win.Height = 430;

                ctlCreaCliente crea = new ctlCreaCliente();
                crea.contenedor = win;
                crea.callFromFact = true;
                crea.accion = "N";
                crea.creacionCliente += new ctlCreaCliente.CreacionCliente(Cliente_Creado);
                crea.setNid(cliente.nidNid);
                win.Contenido.Children.Add(crea);

                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
            }
        }

        private void Cliente_Creado(string cliente, string cedula, string direccion, string telefono, string tipo, string email, bool clieCdto, string emprruc, string emprnbr, bool emprPaga)
        {
            txtCliente.Text = cliente;

            this.ventas.nidnid = cedula;
            this.ventas.nidnbr = cliente;
            this.ventas.niddir = direccion;
            this.ventas.nidtlf = telefono;
            this.ventas.nideml = email;
            this.ventas.nidcdto = clieCdto;
            this.ventas.emprruc = emprruc;
            this.ventas.emprnbr = emprnbr;
            this.ventas.cdtopem = emprPaga;
        }

        private void calcula(DataGrid dgvDestino)
        {
            // Calculamos el total
            Double subTotal = 0;
            Double subTotalNoIva = 0;
            Double descuento = 0;
            Double iva = 0;
            Double total = 0;

            List<ProdPedd> destino = dgvDestino.Items.OfType<ProdPedd>().ToList();

            subTotalNoIva = destino.Where(x => x.iva == 0).Select(o => o.precio * o.cantidad).Sum();
            subTotal = destino.Where(x => x.iva > 0).Select(o => o.precio * o.cantidad).Sum();

            /*for (int i = 0; i < dgvDestino.Items.Count; i++)
            {
                ProdPedd prod = new ProdPedd();
                prod.codigo = ((ProdPedd)dgvDestino.Items[i]).codigo;
                prod.producto = ((ProdPedd)dgvDestino.Items[i]).producto;
                prod.cantidad = ((ProdPedd)dgvDestino.Items[i]).cantidad;
                prod.precio = ((ProdPedd)dgvDestino.Items[i]).precio;
                prod.iva = ((ProdPedd)dgvDestino.Items[i]).iva;
                prod.total = ((ProdPedd)dgvDestino.Items[i]).total;

                subTotal += prod.cantidad * prod.precio;
            }*/

            iva = subTotal * (General.IVA / 100);
            total = subTotal + subTotalNoIva + iva;
            txtSubTot.Text = General.StringFormatMoney(subTotal);
            txtSubTot0.Text = General.StringFormatMoney(subTotalNoIva);
            txtDscto.Text = General.StringFormatMoney(descuento);
            txtIva.Text = General.StringFormatMoney(iva);
            txtTotal.Text = General.StringFormatMoney(total);

            this.ventas.ventsbt = subTotal;
            this.ventas.ventdsc = descuento;
            this.ventas.ventiva = iva;
            this.ventas.venttot = total;
            //this.ventas.peddscl = peddScl;            
        }

        private void AccionPasaItem(DataGrid dgvOrigen, DataGrid dgvDestino, ref Producto pOrig)
        {
            if((ProdPedd)dgvOrigen.SelectedItem == null)
            {
                return;
            }

            if (pOrig.row >= 0)
            {
                Boolean existe = false;

                for (int i = 0; i < dgvDestino.Items.Count; i++)
                {
                    if (((ProdPedd)dgvDestino.Items[i]).codigo.Equals(pOrig.codigo))
                    {
                        ((ProdPedd)dgvDestino.Items[i]).cantidad++;
                        ((ProdPedd)dgvDestino.Items[i]).total= ((ProdPedd)dgvDestino.Items[i]).cantidad *
                            ((ProdPedd)dgvDestino.Items[i]).precio +
                            ((ProdPedd)dgvDestino.Items[i]).iva * ((ProdPedd)dgvDestino.Items[i]).cantidad;
                        dgvDestino.Items.Refresh();
                        existe = true;
                    }
                }

                if (!existe)
                {
                    ProdPedd prodPedd = new ProdPedd
                    {
                        codigo = pOrig.codigo,
                        cantidad = 1,
                        precio = pOrig.precio,
                        iva = pOrig.iva,
                        total = pOrig.precio + pOrig.iva,
                        producto = pOrig.nombre
                    };

                    dgvDestino.Items.Add(prodPedd);
                }

                if (((ProdPedd)dgvOrigen.Items[pOrig.row]).cantidad > 1)
                {
                    ((ProdPedd)dgvOrigen.Items[pOrig.row]).cantidad--;
                }
                else
                {
                    dgvOrigen.Items.Remove((ProdPedd)dgvOrigen.Items[pOrig.row]);
                    pOrig.row--;
                }

                dgvOrigen.Items.Refresh();                
            }

            calcula(dgvFact);
        }

        private void pasaTodos(DataGrid dgvOrigen, DataGrid dgvDestino)
        {
            for (int i = 0; i < dgvOrigen.Items.Count; i++)
            {
                Boolean existe = false;
                String codigo = ((ProdPedd)dgvOrigen.Items[i]).codigo;
                int cantidad = ((ProdPedd)dgvOrigen.Items[i]).cantidad;

                for (int j=0; j < dgvDestino.Items.Count; j++)
                {
                    if (codigo.Equals(((ProdPedd)dgvDestino.Items[j]).codigo))
                    {
                        ((ProdPedd)dgvDestino.Items[j]).cantidad = ((ProdPedd)dgvDestino.Items[j]).cantidad + cantidad;
                        existe = true;
                    }
                }

                if (!existe)
                {
                    ProdPedd prodPedd = new ProdPedd
                    {
                        codigo = ((ProdPedd)dgvOrigen.Items[i]).codigo,
                        cantidad = ((ProdPedd)dgvOrigen.Items[i]).cantidad,
                        precio = ((ProdPedd)dgvOrigen.Items[i]).precio,
                        iva = ((ProdPedd)dgvOrigen.Items[i]).iva,
                        total = ((ProdPedd)dgvOrigen.Items[i]).total,
                        producto = ((ProdPedd)dgvOrigen.Items[i]).producto
                    };

                    dgvDestino.Items.Add(prodPedd);
                }                
            }

            dgvDestino.Items.Refresh();
            dgvOrigen.Items.Clear();

            calcula(dgvFact);
        }
        
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgvPedd.Items.Count > 0)
            {
                this.AccionPasaItem(dgvPedd, dgvFact, ref producto);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgvFact.Items.Count > 0)
            {
                this.AccionPasaItem(dgvFact, dgvPedd, ref prodSell);
            }
        }

        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            pasaTodos(dgvPedd, dgvFact);
        }

        private void btnNone_Click(object sender, RoutedEventArgs e)
        {
            pasaTodos(dgvFact, dgvPedd);
        }

        private void dgvPedd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ProdPedd pedido = (ProdPedd)e.AddedItems[0];

                producto.codigo = pedido.codigo;
                producto.nombre = pedido.producto;
                producto.cantidad = pedido.cantidad;
                producto.precio = pedido.precio;
                producto.iva = pedido.iva;
                producto.total = pedido.total;
                producto.row = ((DataGrid)sender).Items.IndexOf(((DataGrid)sender).CurrentItem);
            }
            catch(Exception ex)
            {

            }            
        }

        private void dgvFact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ProdPedd pedido = (ProdPedd)e.AddedItems[0];

                prodSell.codigo = pedido.codigo;
                prodSell.nombre = pedido.producto;
                prodSell.cantidad = pedido.cantidad;
                prodSell.precio = pedido.precio;
                prodSell.iva = pedido.iva;
                prodSell.total = pedido.total;
                prodSell.row = ((DataGrid)sender).Items.IndexOf(((DataGrid)sender).CurrentItem);

            }
            catch(Exception ex)
            {

            }
        }
    }
}
