using Entidades;
using Entidades.modelos;
using LibControls.buttons;
using LibKeys;
using LibPrint;
using LibPrint.dataset;
using LibWhatsapp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlVenta.xaml
    /// </summary>
    public partial class ctlVenta : UserControl, IDisposable
    {
        private Ventas ventas = new Ventas(General.connectionString);
        private List<mPago> pagos = new List<mPago>();

        private NumKeyboard nKeyboard = new NumKeyboard();

        private bool PUEDE_DAR_DESCUENTO = false;
        private bool PUEDE_DAR_REGALO = false;
        private bool PUEDE_ANULAR_FACTURA = false;
        private bool ConfirmacionAceptada = false;

        private bool DELIVERY_PUSH = false;
        private string codigoUsuario = "";

        private clsTextBox clsNid;
        private string codigoUsuarioElimina = "";

        public void ActivaElementosCajero(bool valor)
        {
            pedidoContainer.IsEnabled = valor;
            btnBscPedd.IsEnabled = valor;
        }

        public ctlVenta()
        {
            InitializeComponent();

            General.TipoVentana = "V";
            ventas.cajacod = General.CAJA;

            clsNid = new clsTextBox(txtCedula, General.USA_TECLADO_PANTALLA);

            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsNid.permiteEspacios = true;
            clsNid.setMaxLength(20);
            
            getPedidosFacturar();
            
            ActivaElementosCajero(false);

            nKeyboard.retornaTexto += new LibKeys.NumKeyboard.RetornaTexto(TextoNumerico_Retorna);

            SetTamanioMenuConfig();
        }

        private void SetTamanioMenuConfig()
        {
            if (General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal)
            {
                setTamanioBotones(btnSalir, 75, 65, 12, 30);
                setTamanioBotones(btnBscPedd, 75, 65, 12, 30);
                setTamanioBotones(btnCancelar, 75, 65, 12, 30);
                setTamanioBotones(btnFacturar, 75, 65, 12, 30);
                setTamanioBotones(btnAnlFact, 75, 65, 12, 30);
                setTamanioBotones(btnDivFact, 75, 65, 12, 30);
                setTamanioBotones(btnImpFact, 75, 65, 12, 30);
                setTamanioBotones(btnActualizar, 75, 65, 12, 30);
                setTamanioBotones(btnToDelivery, 75, 65, 12, 30);
                setTamanioBotones(btnToPay, 75, 65, 12, 30);
                setTamanioBotones(btnDescto, 75, 65, 12, 30);
                setTamanioBotones(btngift, 75, 65, 12, 30);
                setTamanioBotones(btnDelFact, 75, 65, 12, 30);
                setTamanioBotones(btnFactElec, 75, 65, 12, 30);
                setTamanioBotones(btnCajero, 75, 65, 12, 30);
                Mesero.FontSize = 12;
                lNormal.Visibility = Visibility.Visible;
                lSmall.Visibility = Visibility.Hidden;
            }
            else
            {
                setTamanioBotones(btnSalir, 58, 53, 10, 25);
                setTamanioBotones(btnBscPedd, 58, 53, 10, 25);
                setTamanioBotones(btnCancelar, 58, 53, 10, 25);
                setTamanioBotones(btnFacturar, 58, 53, 10, 25);
                setTamanioBotones(btnAnlFact, 58, 53, 10, 25);
                setTamanioBotones(btnDivFact, 58, 53, 10, 25);
                setTamanioBotones(btnImpFact, 58, 53, 10, 25);
                setTamanioBotones(btnActualizar, 58, 53, 10, 25);
                setTamanioBotones(btnToDelivery, 58, 53, 10, 25);
                setTamanioBotones(btnToPay, 58, 53, 10, 25);
                setTamanioBotones(btnDescto, 58, 53, 10, 25);
                setTamanioBotones(btngift, 58, 53, 10, 25);
                setTamanioBotones(btnDelFact, 58, 53, 10, 25);
                setTamanioBotones(btnFactElec, 58, 53, 10, 25);
                setTamanioBotones(btnCajero, 58, 53, 10, 25);
                Mesero.FontSize = 10;
                lNormal.Visibility = Visibility.Hidden;
                lSmall.Visibility = Visibility.Visible;
            }
        }

        private void setTamanioBotones(object obj, int width, int height, int fontSize, int imgSize)
        {
            if (obj.GetType() == typeof(Button))
            {
                var btn = (Button)obj;

                if (btn.Name == "btnCajero")
                {
                    btn.Height = height;
                    btn.Width = width;
                }
                else
                {
                    btn.Height = height;
                    btn.Width = width;
                }
                
                ((Image)((StackPanel)btn.Content).Children[0]).Width = imgSize;
                ((Image)((StackPanel)btn.Content).Children[0]).Height = imgSize;
                ((Label)((StackPanel)btn.Content).Children[1]).FontSize = fontSize;
            }
            else
            {
                var btn = (ToggleButton)obj;
                if(btn.Name == "btnCajero")
                {
                    btn.Height = height-8;
                    btn.Width = width;
                }
                else
                {
                    btn.Height = height;
                    btn.Width = width;
                }
                ((Image)((StackPanel)btn.Content).Children[0]).Width = imgSize;
                ((Image)((StackPanel)btn.Content).Children[0]).Height = imgSize;
                ((Label)((StackPanel)btn.Content).Children[1]).FontSize = fontSize;
            }
        }

        private void TextoNumerico_Retorna(Object txt, String texto)
        {
            if (texto.Equals(""))
            {
                ObjComportamiento.showMsgError("Debe ingresar una clave para poder realizar el pedido");
                btnCajero.IsChecked = false;
                return;
            }

            Usuarios usuario = new Usuarios(General.connectionString);

            string codigo = usuario.getCodMesero(texto, General.USA_CLAVE_NUMERICA);

            if (codigo != "")
            {
                var rcodigo = codigo.Split(';');
                string cellphone = rcodigo[1];
                string nombre = rcodigo[2];
                codigo = rcodigo[0];

                codigoUsuarioElimina = codigo;

                List<mPermiso> permisos = usuario.getPermisoUsuario(codigo);

                if (permisos.Count > 0)
                {
                    this.PUEDE_ANULAR_FACTURA = permisos.Find(X => X.tag == "AFCT").estado;
                    this.PUEDE_DAR_DESCUENTO = permisos.Find(X => X.tag == "DSCT").estado;
                    this.PUEDE_DAR_REGALO = permisos.Find(X => X.tag == "GIFT").estado;

                    if (codigo.Equals(""))
                    {
                        ObjComportamiento.showMsgError("Clave ingresada incorrecta");
                        btnCajero.IsChecked = false;
                        return;
                    }

                    UsrEmpleado usr = usuario.getEmpleadoMesero(codigo);

                    if (usr.usrusr == null)
                    {
                        Mesero.Text = "NID";
                        Mesero.Tag = "NID";
                    }
                    else
                    {
                        Mesero.Text = usr.usrusr.ToLower();
                        Mesero.Tag = usr.usrnbr.ToUpper();
                    }

                    ventas.empdnid = codigo;
                    ventas.celEmpl = cellphone;
                    ventas.empdNbr = nombre;
                    codigoUsuario = codigo;

                    ActivaElementosCajero(true);
                }
                else
                {
                    ObjComportamiento.showMsgError("Clave del cajero no válida.");
                }
            }
            else
            {
                ObjComportamiento.showMsgError("Clave ingresada no válida");
            }
        }

        private void sbLimpiar()
        {
            txtCliente.Text = "";
            txtCedula.Text = "";
            txtCambio.Text = "0.00";
            txtDscto.Text = "0.00";
            txtSubTot.Text = "0.00";
            txtSubTot0.Text = "0.00";
            txtRecibido.Text = "0,00";
            txtIva.Text = "0.00";
            txtTotal.Text = "0.00";

            this.ventas.ventsbt = 0;
            this.ventas.ventsbn = 0;
            this.ventas.ventiva = 0;
            this.ventas.venttot = 0;
            this.ventas.ventdpc = 0;
            this.ventas.ventdsc = 0;

            this.DELIVERY_PUSH = false;

            if (dgvPedd.ItemsSource != null)
                dgvPedd.ItemsSource = null;
            else
                dgvPedd.Items.Clear();
        }

        private void sbActivaVenta(bool value)
        {
            btnCancelar.IsEnabled = value;
            btnToPay.IsEnabled = value;
            if (this.PUEDE_DAR_DESCUENTO && value) btnDescto.IsEnabled = value;
            if (this.PUEDE_DAR_REGALO && value) btngift.IsEnabled = value;
            btnDivFact.IsEnabled = value;            
            btnActualizar.IsEnabled = !value;
        }

        private void sbDesactivaPedidos(bool value)
        {
            foreach(UIElement btn in pedidoContainer.Children)
            {
                ((ctlButton)btn).IsEnabled = value;
            }
        }

        private void getPedidosFacturar()
        {
            sbLimpiar();
            Ventas ventas = new Ventas(General.connectionString);

            DataTable dt = ventas.getPedidosFacturar();

            pedidoContainer.Children.Clear();

            foreach (DataRow dr in dt.Rows)
            {
                ctlButton btn = new ctlButton();

                btn.Imagen = General.sbDefineImagen(dr["peddTip"].ToString());
                bool separado = dr["ventsep"].ToString() == "S" ? true : false;
                bool anulado = int.Parse(dr["ventanl"].ToString()) == 1 ? true : false;
                if (dr["nidnid"].ToString() != "" && separado)
                {
                    btn.Codigo = dr["peddscl"].ToString() + "\n" + dr["nidnid"];
                }
                else
                {
                    btn.Codigo = dr["peddscl"].ToString();
                }
                
                btn.CodigoAlterno = dr["peddscl"].ToString();
                btn.CodigoSubAlterno = dr.IsNull("ventscl") ? "" : dr["ventscl"].ToString() + ":" + (anulado == true ? "A": "");
                btn.Titulo = dr["peddmsa"].ToString() + " " + dr["peddlta"].ToString();
                string facturado = dr["ventfac"].ToString();
                
                bool cobrado = dr["ventsep"].ToString() == "" ? false : true;
                if (facturado == "F" && cobrado)
                {
                    btn.setBackgroundColor(Brushes.LightSalmon);
                }
                if (separado)
                {
                    btn.setBackgroundColor(Brushes.LightSkyBlue);
                    btn.CodigoSubAlterno = dr["ventscl"].ToString();
                }
                if (anulado)
                {
                    btn.setBackgroundColor(Brushes.LightGreen);
                }
                
                btn.Ancho = 160;
                btn.Alto = 70;

                btn._ctlButtonClick += new ctlButton.ctlButton_Click(ctlButton_Click);

                pedidoContainer.Children.Add(btn);
            }
        }

        private void btnCliFnl_Click(object sender, RoutedEventArgs e)
        {
            if (General.StringToDouble(txtTotal.Text) > 50 && this.ventas.venttip != "E")
            {
                ObjComportamiento.showMsgError("La compra supera los $50, por cuanto no puede ser definida como CLIENTE FINAL");
                return;
            }

            txtCedula.Text = "9999999999999";
            txtCliente.Text = "CLIENTE FINAL";

            this.ventas.nidnid = txtCedula.Text;
            this.ventas.nidnbr = "CLIENTE FINAL";
            this.ventas.niddir = "";
            this.ventas.nidtlf = "";
            this.ventas.nideml = "";
            this.ventas.nidcdto = false;
            this.ventas.emprruc = "";
            this.ventas.emprnbr = "";
            this.ventas.cdtopem = false;
        }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            txtCedula.Text = "";
            txtCliente.Text = "";
            txtEmail.Text = "";

            this.ventas.nidnid = txtCedula.Text;
            this.ventas.nidnbr = "";
            this.ventas.niddir = "";
            this.ventas.nidtlf = "";
            this.ventas.nideml = "";
            this.ventas.nidcdto = false;
            this.ventas.emprruc = "";
            this.ventas.emprnbr = "";
            this.ventas.cdtopem = false;
        }

        private void Cliente_Creado(string cliente, string cedula, string direccion, string telefono, string tipo, string email, bool clieCdto, string emprruc, string emprnbr, bool emprpaga)
        {
            txtCliente.Text = cliente;
            txtCedula.Text = cedula;
            txtEmail.Text = email;

            this.ventas.nidnid = cedula;
            this.ventas.nidnbr = cliente;
            this.ventas.niddir = direccion;
            this.ventas.nidtlf = telefono;
            this.ventas.nideml = email;
            this.ventas.nidcdto = clieCdto;
            this.ventas.emprruc = emprruc;
            this.ventas.emprnbr = emprnbr;
            this.ventas.cdtopem = emprpaga;
        }

        private void btnBscCli_Click(object sender, RoutedEventArgs e)
        {
            Clientes cliente = new Clientes(General.connectionString);

            if (string.IsNullOrEmpty(txtCedula.Text))
            {
                ObjComportamiento.showMsgError("Debe ingresar un criterio de búsqueda del cliente");
                return;
            }

            var lClientes = cliente.getByIdOrName(txtCedula.Text.ToString());

            if (lClientes.Count == 1)
            {
                txtCedula.Text = lClientes.First().nidNid;
                txtCliente.Text = lClientes.First().nidNbr;
                txtEmail.Text = lClientes.First().nidEml;
                this.ventas.nidnbr = lClientes.First().nidNbr;
                this.ventas.nidnid = lClientes.First().nidNid;
                this.ventas.niddir = lClientes.First().nidDir;
                this.ventas.nidtlf = lClientes.First().nidTlf;
                this.ventas.nideml = lClientes.First().nidEml;
                this.ventas.nidcdto = lClientes.First().cliecdto;
                this.ventas.emprruc = lClientes.First().nidEmpr;
                this.ventas.emprnbr = lClientes.First().nidEmpr;
                this.ventas.cdtopem = lClientes.First().emprPaga;
            }
            else if (lClientes.Count > 1)
            {
                // Muestra listado de clientes que cumplen con el filtro ingresado
                winGeneral win = new winGeneral();
                win.Title = "Selección de cliente";
                win.Width = 810;
                win.Height = 480;

                ctlBscClientes crea = new ctlBscClientes(lClientes);
                crea.contenedor = win;
                crea.aceptaCliente += new ctlBscClientes.AceptaCliente(cliente_Seleccionado);

                win.Contenido.Children.Add(crea);

                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
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
                crea.setNid(txtCedula.Text.ToString());
                win.Contenido.Children.Add(crea);

                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
            }
        }

        private void btnEditClie_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Modificación de cliente";
            win.Width = 710;
            win.Height = 430;

            ctlCreaCliente crea = new ctlCreaCliente();
            crea.contenedor = win;
            crea.callFromFact = true;
            crea.accion = "M";
            crea.creacionCliente += new ctlCreaCliente.CreacionCliente(Cliente_Creado);
            crea.setNid(txtCedula.Text.ToString());
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }
        private void cliente_Seleccionado(string codigo, string nombre, string direccion, string telefono, string email, string tipo, bool credito, string emprruc, string emprnbr, bool emprpaga)
        {
            txtCliente.Text = nombre;
            txtCedula.Text = codigo;
            txtEmail.Text = email;

            this.ventas.nidnid = codigo;
            this.ventas.nidnbr = nombre;
            this.ventas.niddir = direccion;
            this.ventas.nidtlf = telefono;
            this.ventas.nideml = email;
            this.ventas.nidcdto = credito;
            this.ventas.emprruc = emprruc;
            this.ventas.emprnbr = emprnbr;
            this.ventas.cdtopem = emprpaga;
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void llenaDetalleYPago()
        {
            this.ventas.detVenta.Clear();
            for (int i = 0; i < dgvPedd.Items.Count; i++)
            {
                var prodPedd = (ProdPedd)dgvPedd.Items[i];

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

            this.ventas.detPago = this.pagos;
        }

        private void btnFacturar_Click(object sender, RoutedEventArgs e)
        {
            this.llenaDetalleYPago();

            // Mandamos a grabar el pedido
            if (txtCedula.Text.Equals(""))
            {
                ObjComportamiento.showMsgError("No ha ingresado un cliente válido");
                return;
            }

            if (this.ventas.ventfac)
            {
                if (this.ventas.detPago.Count == 0 && this.ventas.venttip != "E")
                {
                    ObjComportamiento.showMsgError("Debe ingresar el pago respectivo del elemento a facturar.");
                    return;
                }
            }

            if( ObjComportamiento.showMsgYesNo("¿Desea grabar la venta?")==MessageBoxResult.Yes)
            {
                if (this.ventas.detVenta.Count > 0)
                {
                    //Se pasa a reducir el inventario
                    List<ProdPedd> prds = new List<ProdPedd>();
                    foreach(ProdPedd prd in dgvPedd.Items)
                    {
                        prds.Add(prd);
                    }
                    Items items = new Items(General.connectionString);
                    items.itemsInventario(this.ventas.peddscl, prds);
                    this.ventas.ventsep = "C";
                    this.ventas.clave_download = General.contraseniaDownload(10);

                    if (this.ventas.empdnid != codigoUsuario && !string.IsNullOrEmpty(codigoUsuario))
                    {
                        this.ventas.empdnid = codigoUsuario;
                    }

                    Respuesta res = this.ventas.grabar();

                    if (res.resultado)
                    {
                        // Impresión de la factura
                        if (!this.ventas.ventfac)
                        {
                            this.ventas.ventfac = true;
                            if (ObjComportamiento.showMsgYesNo("¿Desea imprimir la factura?") == MessageBoxResult.Yes)
                            {
                                this.ToPrint("F");
                            }
                        }
                        
                        ObjComportamiento.showMsgInfo(res.mensaje);

                        // Enviar la factura al SRI cuando esté activo la facturación automática
                        if (General.ENVIO_AUTO_AL_SRI)
                        {
                            string codigoVenta = ventas.ventscl.ToString().PadLeft(10, '0');

                            if (ventas.ventscl == -1)
                            {
                                codigoVenta = res.mensaje.Substring(res.mensaje.Length - 10);
                            }

                            mFacturasSRI data = ventas.getFacturasToSRIByVentcls(codigoVenta);
                            data.docutip = "F";

                            winGeneral win = new winGeneral
                            {
                                Width = 500,
                                Height = 300,
                                ResizeMode = ResizeMode.NoResize,
                                WindowStyle = WindowStyle.None,
                                WindowStartupLocation = WindowStartupLocation.CenterScreen
                            };


                            ctlProgressFact prgFact = new ctlProgressFact
                            {
                                contenedor = win
                            };
                            
                            win.Contenido.Children.Add(prgFact);

                            RoutedEventHandler loadedHandler = null;
                            loadedHandler = (sender, e) =>
                            {
                                win.Loaded -= loadedHandler;
                                prgFact.Iniciar(data);
                            };
                            win.Loaded += loadedHandler;

                            win.ShowDialog();
                        }

                        btnCancelar_Click(sender, e);
                        this.ventas.detVenta.Clear();
                        this.ventas.detPago.Clear();

                    }
                    else
                    {
                        ObjComportamiento.showMsgError(res.mensaje);
                    }
                }
                else
                {
                    if (this.ventas.detVenta.Count == 0)
                        ObjComportamiento.showMsgError("El pedido a facturar no contiene items.");
                    if (this.ventas.detPago.Count == 0)
                        ObjComportamiento.showMsgError("No se ha ingresado el pago respectivo del pedido a facturar");
                }
            }            
        }

        private void btnProforma_Click(object sender, RoutedEventArgs e)
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
                    {
                        ObjComportamiento.showMsgError("El pedido a proformar no contiene items.");
                    }
                }
            }
        }

        private void btnBscPedd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                winGeneral win = new winGeneral();
                win.Title = "Buscar Pedido";
                win.Width = 860;
                win.Height = 450;

                ctlBscPedido crea = new ctlBscPedido();
                crea.contenedor = win;
                crea.pedidoSeleccionado += new ctlBscPedido.PedidoSeleccionado(Pedido_Seleccionado);
                crea.cancelaBusqueda += new ctlBscPedido.CancelaBusqueda(Busqueda_Cancelada);
                win.Contenido.Children.Add(crea);

                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
            }
            catch(Exception er)
            {

            }
        }

        private void Busqueda_Cancelada()
        {
            btnBscPedd.IsEnabled = true;
            btnAnlFact.IsEnabled = false;
        }

        private void Pedido_Seleccionado(string pedidoScl, bool facturado)
        {
            try
            {
                btnFacturar.IsEnabled = false;
                btnProforma.IsEnabled = false;

                btnImpPro.IsEnabled = true;
                btnImpFact.IsEnabled = true;

                getPedido(pedidoScl, true);

                txtCedula.Text = this.ventas.nidnid;
                txtCliente.Text = this.ventas.nidnbr;

                txtRecibido.Text = String.Format("{0:0.00}", this.ventas.ventrcb);
                txtCambio.Text = String.Format("{0:0.00}", this.ventas.ventvto);

                if (facturado)
                {
                    btnToPay.IsEnabled = false;
                    btnDescto.IsEnabled = false;
                    btngift.IsEnabled = false;
                    btnImpFact.IsEnabled = true;
                }
                else
                {
                    btnToPay.IsEnabled = true;
                    if (this.PUEDE_DAR_DESCUENTO) btnDescto.IsEnabled = true;
                    if (this.PUEDE_DAR_REGALO && this.ventas.ventStd != "E") btngift.IsEnabled = true;
                }

                if (this.PUEDE_ANULAR_FACTURA && (this.ventas.ventStd != "A" && this.ventas.ventStd != "E")) btnAnlFact.IsEnabled = true;

                btnBscPedd.IsEnabled = false;
                btnDivFact.IsEnabled = false;

                if (codigoUsuarioElimina != ventas.empdnid)
                    ventas.empdnid = codigoUsuarioElimina;

            }
            catch (Exception ex)
            {

            }
        }

        private void btnAnlFact_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Listado de Motivos para anular una factura";
            win.Width = 500;
            win.Height = 530;

            ctlMotivosAnula crea = new ctlMotivosAnula();
            crea.secuencial = this.ventas.ventscl.ToString().PadLeft(10,'0');
            crea.numeroFactura = General.CODIGO_EMPRESA + "-" + General.NUMERO_FACTURERO + "-" + this.ventas.secuencial;
            crea.contenedor = win;
            crea.tipo = "A";
            crea.data = ventas.getMotivosDescuento("A");
            crea.generar();

            crea.aceptaMotivo += new ctlMotivosAnula.AceptaMotivo(Anular_Factura);

            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();

        }

        private void btnDelFact_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Listado de Motivos para eliminar una factura";
            win.Width = 500;
            win.Height = 440;

            ctlMotivosAnula crea = new ctlMotivosAnula();
            crea.secuencial = this.ventas.ventscl.ToString().PadLeft(10, '0');
            crea.numeroFactura = General.CODIGO_EMPRESA + "-" + General.NUMERO_FACTURERO + "-" + this.ventas.secuencial;
            crea.contenedor = win;
            crea.tipo = "R";
            crea.data = ventas.getMotivosDescuento("R");
            crea.generar();

            crea.aceptaMotivo += new ctlMotivosAnula.AceptaMotivo(Eliminar_Factura);

            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void Anular_Factura(string ventScl, string ventObv, string numFact)
        {
            if (!ventScl.Equals(""))
            {
                if (General.MODO_VALIDACION_ANULA_ELIMINA == "P")
                {
                    if (PcNetworkInfo.InternetConnectionStatus())
                    {
                        if (string.IsNullOrEmpty(ventas.celEmpl))
                        {
                            ObjComportamiento.showMsgError("El empleado no tiene asignado un número de celular, VERIFIQUE Y ASIGNE");
                            btnCancelar_Click(btnCancelar, new RoutedEventArgs());
                            return;
                        }

                        muestraIngresoPin("A");
                    }
                    else
                    {
                        muestraIngresoCoord("A");
                    }
                }

                if (General.MODO_VALIDACION_ANULA_ELIMINA == "G")
                {
                    muestraIngresoCoord("A");
                }

                if (General.MODO_VALIDACION_ANULA_ELIMINA == "H")
                {
                    // Falta desarrollar el uso de huella digital
                }

                if (General.MODO_VALIDACION_ANULA_ELIMINA == "C")
                {
                    // Falta desarrollar el uso de tarjeta contactless
                }

                if (General.MODO_VALIDACION_ANULA_ELIMINA != "N" && !ConfirmacionAceptada)
                {
                    ObjComportamiento.showMsgError("La anulación de la factura no pudo ser realizada, porque la CONFIRMACIÓN no fue VÁLIDA");
                    return;
                }

                Respuesta res = this.ventas.anulaFactura(ventScl, ventObv, numFact);
                ObjComportamiento.showMuestraMensaje(res);

                if (res.resultado)
                {
                    // Enviar correo de eliminación de pedido
                    mClientAuth client = new mClientAuth
                    {
                        usuario = General.USUARIO_WS,
                        clave = General.CLAVE_WS,
                        empresaId = General.EMPRESA_GUID,
                        ruc = General.RUC_EMPRESA,
                        token = General.TOKEN_MAIL
                    };

                    string msgAuth = JsonConvert.SerializeObject(client);

                    General.enviarCorreo(General.NOMBRE_USUARIO, "Se ha eliminado una Factura", ventObv, usaMailTipo.Pedido, usaTipoAccion.Eliminacion, ventScl, msgAuth, ventas.empdNbr);

                    List<ProdPedd> prds = new List<ProdPedd>();
                    foreach (ProdPedd prd in dgvPedd.Items)
                    {
                        prds.Add(prd);
                    }
                    Items items = new Items(General.connectionString);
                    items.itemsInventarioReversar(this.ventas.peddscl, prds);
                }

                btnCancelar_Click(btnCancelar, new RoutedEventArgs());
            }
            
            btnAnlFact.IsEnabled = false;            
        }

        private void muestraIngresoPin(string tipoAccion, bool usaWhatsapp = true)
        {
            // Genera PIN y guarda en tabla
            ConfirmaPin cls = new ConfirmaPin(General.connectionString);
            cls.msgpin = General.generaPin();
            cls.msgatv = "A";
            cls.msgtip = tipoAccion;
            cls.tiporg = "V";
            cls.usucod = this.ventas.empdnid;
            cls.secorg = this.ventas.ventscl.ToString().PadLeft(10, '0');

            cls.grabar("N");
            // envía el PIN al usuario vía Whatsapp
            if (usaWhatsapp)
            {
                General.enviarWhatsapp("Ingrese el PIN #" + cls.msgpin + " Para confirmar la acción de " +
                                    (tipoAccion == "A" ? "anulación " : "eliminación") + " de la factura del pedido" +
                                    ventas.peddscl + " con secuencial " + ventas.ventscl + ".", ventas.celEmpl);
            }
            else
            {
                // Envío vía Email
                mClientAuth client = new mClientAuth
                {
                    usuario = General.USUARIO_WS,
                    clave = General.CLAVE_WS,
                    empresaId = General.EMPRESA_GUID,
                    ruc = General.RUC_EMPRESA,
                    token = General.TOKEN_MAIL
                };

                string msgAuth = JsonConvert.SerializeObject(client);

                General.enviarCorreo(codigoUsuario, "Pin de Confirmación", "Ingrese el PIN #" + cls.msgpin + " Para confirmar la acción de " +
                                    (tipoAccion == "A" ? "anulación " : "eliminación") + " de la factura del pedido" +
                                    ventas.peddscl + " con secuencial " + ventas.ventscl + ".", usaMailTipo.Factura, 
                                    tipoAccion == "A" ?usaTipoAccion.Anulacion : usaTipoAccion.Eliminacion, 
                                    ventas.ventscl.ToString().PadLeft(10,'0'), msgAuth, ventas.empdnid);
            }
            
            // Abre la ventana para ingreso del PIN
            ctlIngresaPIN wPin = new ctlIngresaPIN();
            wPin.Width = 320;
            wPin.Height = 450;
            wPin.SetearPin = cls.msgpin;
            wPin.SetearPinSecuencial = cls.msgscl;
            wPin.codigo = cls.secorg;
            wPin.aceptaPin += new ctlIngresaPIN.AceptaPIN(acepta_confirmacion);
            wPin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wPin.ShowDialog();
            wPin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void muestraIngresoCoord(string tipoAccion)
        {
            // Generar la coordenada
            Coordenada newCoord = new Coordenada(General.connectionString);

            mCoordenada valor = newCoord.Generar("N", 0, "E", "P", this.ventas.ventscl.ToString().PadLeft(10, '0'), ventas.empdnid, "");

            // Abre la ventana para ingreso de las coordenadas
            ctlIngresaCoord wPin = new ctlIngresaCoord();
            wPin.Width = 320;
            wPin.Height = 450;
            wPin.SetearOrigen = "V";
            wPin.SetearTipo = tipoAccion;
            wPin.codigoEmpleado = this.ventas.empdnid;
            wPin.SetearCoordenada = valor;
            wPin.SetearCoordSecuencial = valor.coordscl;
            wPin.SetearCodigoOrigen = this.ventas.ventscl.ToString().PadLeft(10, '0');
            wPin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wPin.aceptaCoord += new ctlIngresaCoord.AceptaCoord(acepta_confirmacion);
            wPin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wPin.ShowDialog();
        }

        private void acepta_confirmacion(bool acepta)
        {
            ConfirmacionAceptada = acepta;
        }

        private void Eliminar_Factura(String ventScl, String ventObv, string numFact)
        {
            if (!ventScl.Equals(""))
            {
                if (General.MODO_VALIDACION_ANULA_ELIMINA == "P")
                {
                    if (PcNetworkInfo.InternetConnectionStatus())
                    {
                        if (string.IsNullOrEmpty(ventas.celEmpl))
                        {
                            ObjComportamiento.showMsgError("El empleado no tiene asignado un número de celular, VERIFIQUE Y ASIGNE");
                            btnCancelar_Click(btnCancelar, new RoutedEventArgs());
                            return;
                        }

                        muestraIngresoPin("E");
                    }
                    else
                    {
                        muestraIngresoCoord("E");
                    }
                }

                if (General.MODO_VALIDACION_ANULA_ELIMINA == "G")
                {
                    muestraIngresoCoord("E");
                }

                if (General.MODO_VALIDACION_ANULA_ELIMINA == "H")
                {
                    // Falta desarrollar el uso de huella digital
                }

                if (General.MODO_VALIDACION_ANULA_ELIMINA == "C")
                {
                    // Falta desarrollar el uso de tarjeta contactless
                }

                if (General.MODO_VALIDACION_ANULA_ELIMINA != "N" && !ConfirmacionAceptada)
                {
                    ObjComportamiento.showMsgError("La eliminación de la factura no pudo ser realizada, porque la CONFIRMACIÓN no fue VÁLIDA");
                    return;
                }

                Respuesta res = this.ventas.eliminaFactura(ventScl, ventObv, numFact);
                ObjComportamiento.showMuestraMensaje(res);

                if (res.resultado)
                {
                    // Enviar correo de eliminación de pedido
                    mClientAuth client = new mClientAuth
                    {
                        usuario = General.USUARIO_WS,
                        clave = General.CLAVE_WS,
                        empresaId = General.EMPRESA_GUID,
                        ruc = General.RUC_EMPRESA,
                        token = General.TOKEN_MAIL
                    };

                    string msgAuth = JsonConvert.SerializeObject(client);

                    General.enviarCorreo(General.NOMBRE_USUARIO, "Se ha eliminado una Factura", ventObv, usaMailTipo.Pedido, usaTipoAccion.Eliminacion, ventScl, msgAuth, ventas.empdNbr);

                    List<ProdPedd> prds = new List<ProdPedd>();
                    foreach (ProdPedd prd in dgvPedd.Items)
                    {
                        prds.Add(prd);
                    }
                    Items items = new Items(General.connectionString);
                    items.itemsInventarioReversar(this.ventas.peddscl, prds);
                }

                btnCancelar_Click(btnCancelar, new RoutedEventArgs());
            }

            btnAnlFact.IsEnabled = false;
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

                dr["pgoForm"] = this.ventas.detPago[i].isefectivo == true ? "E" : (this.ventas.detPago[i].ischeque==true? "C" : "T" );
                dr["pgoValor"] = this.ventas.detPago[i].valor;

                ds.tbPagoVta.Rows.Add(dr);
            }

            try
            {
                var copias = General.CANTIDAD_COPIAS_FACTURA;

                for (int i = 0; i < copias; i++)
                {
                    EposPrint cls = new EposPrint();
                    cls.TipoImpresora = General.TIPO_IMPRESORAS;
                    cls.imprimeSecTemporal = General.IMPRIME_SECUENCIAL_TEMPORAL;
                    Respuesta resp = cls.setPrinter(General.PRINTER_FACTURA);
                    if (resp.resultado == false)
                    {
                        ObjComportamiento.showMsgError(resp.mensaje);
                        cls.Dispose();
                        return;
                    }

                    cls.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;
                    cls.tipo = tipo;
                    cls.impresionOriginal = i > 0 ? false : true;

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
                        cls.usaCabeceraExpandida = General.MODO_EXPANDIDO_CABECERA_FACT;
                        cls.imprimeClaveDownload = General.IMPRIME_CLAVE_DOWNLOAD;
                        cls.UrlDescarga = General.URL_SITIO_WEB;

                        cls.Factura(ds.tbPeddVta, ds.tbPagoVta, General.EMPRESA, General.DIRECCION_LOCAL,
                            General.RUC_EMPRESA, this.ventas.nidnid, this.ventas.nidnbr, this.ventas.niddir,
                                this.ventas.nidtlf, this.ventas.nideml, this.ventas.venttip, this.ventas.peddscl,
                                this.Mesero.Text,
                                this.ventas.ventmsa + " " + this.ventas.ventlta,
                                String.Format("{0:0.00}", this.ventas.ventsbt),
                                String.Format("{0:0.00}", this.ventas.ventsbn),
                                String.Format("{0:0.00}", this.ventas.ventdsc),
                                String.Format("{0:0.00}", this.ventas.ventdpc),
                                String.Format("{0:0.00}", this.ventas.ventiva),
                                String.Format("{0:0.00}", this.ventas.venttot),
                                String.Format("{0:0.00}", this.ventas.ventrcb),
                                String.Format("{0:0.00}", this.ventas.ventvto),
                                this.ventas.clave_download,
                                this.ventas.ventscl.ToString().PadRight(10, '0'),
                                this.ventas.nidcdto, this.ventas.emprnbr, this.ventas.emprruc,
                                false,
                                this.ventas.peddSclTmp);
                    }
                    else
                    {
                        cls.fnFacturaPrnt(ds.tbPeddVta, ds.tbPagoVta, this.ventas.peddscl,
                                this.ventas.ventmsa + " " + this.ventas.ventlta,
                                String.Format("{0:0.00}", this.ventas.ventsbt),
                                String.Format("{0:0.00}", this.ventas.ventsbn),
                                String.Format("{0:0.00}", this.ventas.ventdsc),
                                String.Format("{0:0.00}", this.ventas.ventdpc),
                                String.Format("{0:0.00}", this.ventas.ventiva),
                                String.Format("{0:0.00}", this.ventas.venttot),
                                String.Format("{0:0.00}", "_____________________________"),
                                "_____________________________", "_____________________________",
                                "_____________________________",
                                String.Format("{0:0.00}", this.ventas.ventrcb),
                                String.Format("{0:0.00}", this.ventas.ventvto),
                                tipo);
                    }

                    if (tipo.Equals("F"))
                    {
                        cls.sbIpr();

                        if (this.ventas.venttip == "D" || this.ventas.venttip == "E")
                        {
                            if (General.IMPRIME_TICKET_CARRIER_AUT && !this.DELIVERY_PUSH)
                            {
                                this.btnToDelivery_Click(this, new RoutedEventArgs());
                            }
                        }
                    }
                    else
                    {
                        cls.sbIpr();
                    }

                    cls.Dispose();
                }
            }
            catch(Exception e)
            {
                ObjComportamiento.showMsgError("Error al imprimir la " + (tipo.Equals("F")==true ? "Factura" : "Proforma"));
            }            
        }

        private void btnImpFact_Click(object sender, RoutedEventArgs e)
        {
            if (!this.ventas.ventfac)
            {
                // Imprime la pre-factura
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

                    ds.tbPagoVta.Rows.Add(dr);
                }

                try
                {
                    EposPrint cls = new EposPrint();
                    cls.TipoImpresora = General.TIPO_IMPRESORAS;
                    Respuesta resp = cls.setPrinter(General.PRINTER_FACTURA);
                    cls.CabeceraNegrita = true;
                    if (resp.resultado == false)
                    {
                        ObjComportamiento.showMsgError(resp.mensaje);
                        cls.Dispose();
                        return;
                    }
                    
                    cls.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;

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

                    cls.PreFactura(ds.tbPeddVta, General.EMPRESA,
                                General.DIRECCION_LOCAL, General.RUC_EMPRESA,
                                this.ventas.venttip,
                                this.ventas.peddscl,
                                this.Mesero.Text,
                                this.ventas.ventmsa + " " + this.ventas.ventlta,
                                string.Format("{0:0.00}", this.ventas.ventsbt),
                                string.Format("{0:0.00}", this.ventas.ventsbn),
                                string.Format("{0:0.00}", this.ventas.ventdsc),
                                string.Format("{0:0.00}", this.ventas.ventdpc),
                                string.Format("{0:0.00}", this.ventas.ventiva),
                                string.Format("{0:0.00}", this.ventas.venttot));
                    cls.sbIpr();
                    
                    cls.Dispose();
                }
                catch (Exception err)
                {
                    ObjComportamiento.showMsgError("Error al imprimir la Pre-factura");
                }
            }
            else
            {
                this.ToPrint("F");
            }
        }

        private void btnFact_Click(object sender, RoutedEventArgs e)
        {
            this.ToPrint("F");
        }

        private void btnImpPro_Click(object sender, RoutedEventArgs e)
        {
            this.ToPrint("P");
        }

        private void btnToPay_Click(object sender, RoutedEventArgs e)
        {
            this.ventas.detPago.Clear();

            winGeneral win = new winGeneral();
            win.Title = "Pago de Factura";
            win.Width = 1010;//940;
            win.Height = 550;//450;

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
            //Double tot = 0;
            Double valor = 0;// pagos[0].valor;
            foreach (mPago pago in pagos)
            {
                valor += pago.valor;
            }
            
            this.pagos.Clear();
            //this.ventas.detPago.Clear();

            txtRecibido.Text = String.Format("{0:0.00}", valor);
            Double total = General.StringToDouble(txtTotal.Text.ToString());
            if (valor > total)
            {
                txtCambio.Text = String.Format("{0:0.00}", valor - total);
            }
            else
            {
                txtCambio.Text = "0.00";
            }
            //btnFacturar.IsEnabled = true;
            this.pagos = pagos;
            //this.ventas.detPago = pagos;
            this.ventas.ventrcb = valor;
            this.ventas.ventvto = valor - total;
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            getPedidosFacturar();
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDivFact_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Separación de Factura";
            win.Width = 1100;
            win.Height = 650;

            ctlSepFact crea = new ctlSepFact(this.ventas.peddscl);
            crea.contenedor = win;
            crea.PUEDE_DAR_DESCUENTO = this.PUEDE_DAR_DESCUENTO;
            crea.PUEDE_DAR_REGALO = this.PUEDE_DAR_REGALO;
            crea.finalizaSeparacion += new ctlSepFact.FinalizaSeparacion(finalizaSeparacion);
            
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void finalizaSeparacion()
        {
            btnCancelar_Click(this, new RoutedEventArgs());
        }
        
        private void btnToDelivery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EposPrint cls = new EposPrint();
                cls.TipoImpresora = General.TIPO_IMPRESORAS;
                Respuesta resp = cls.setPrinter(General.PRINTER_PROFORMA);
                if (resp.resultado == false)
                {
                    ObjComportamiento.showMsgError(resp.mensaje);
                    cls.Dispose();
                    return;
                }

                cls.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;

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

                this.DELIVERY_PUSH = true;
                if (this.ventas.venttip.Equals("D"))
                {
                    cls.fnDataPeddDmclo(this.ventas.peddscl, this.ventas.cliNbr, this.ventas.cliTlf,
                                        this.ventas.cliDir, this.ventas.cliRef);
                    cls.sbIpr();
                }

                if (this.ventas.venttip.Equals("E"))
                {
                    double totve = this.ventas.venttot;
                    double valor = 0;
                    double total = 0;

                    if (this.ventas.crrpct)
                    {
                        valor = totve * this.ventas.crrprc / 100;
                        total = totve + valor;
                    }
                    else
                    {
                        total = totve + valor;
                    }
                }
                cls.Dispose();
            }
            catch(Exception ex)
            {
                ObjComportamiento.showMsgError(ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            sbLimpiar();
            sbActivaVenta(false);
            sbDesactivaPedidos(true);
            getPedidosFacturar();
            btnFacturar.IsEnabled = false;
            btnBscPedd.IsEnabled = true;
            btnAnlFact.IsEnabled = false;
            btnImpFact.IsEnabled = false;
            btnImpPro.IsEnabled = false;
            btngift.IsEnabled = false;
            btnDescto.IsEnabled = false;
            btnDelFact.IsEnabled = false;
        }

        private void btnDescto_Click(object sender, RoutedEventArgs e)
        {
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
            
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void btngift_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Listado de Motivos para aplicar Regalos";
            win.Width = 500;
            win.Height = 400;

            ctlMotivos crea = new ctlMotivos();
            crea.secuencial = this.ventas.peddscl;
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

            double iva = 0;
            double total = 0;
            double descuento = 0;

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

            if (this.ventas.nidnid == "") this.ventas.nidnid = "9999999999999";
            if (this.ventas.nidnbr == "") this.ventas.nidnbr = "CLIENTE FINAL";

            // Grabar regalo
            Respuesta res = this.ventas.grabarRegalo();
            ObjComportamiento.showMuestraMensaje(res);
            if (res.resultado)
            {
                btnCancelar_Click(btnCancelar, new RoutedEventArgs());                
            }            
        }

        private void Crea_aceptaDescuento(String secuencial, String mensaje)
        {
            double desc = 0;

            if(!mensaje.Equals("SIN DESCUENTO"))
            {
                desc= General.StringToDouble(mensaje.Substring(mensaje.Length - 4, 3)) / 100;
            }
                
            double subTotal = 0;

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

            double iva = 0;
            double total = 0;
            double descuento = subTotal * desc;

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

        private void btnCajero_Checked(object sender, RoutedEventArgs e)
        {
            nKeyboard.MaxLength = General.CANTIDAD_CARACTERES_PASSWORD;

            nKeyboard.setTextBox = new PasswordBox();

            nKeyboard.ShowDialog();
        }

        private void btnCajero_Unchecked(object sender, RoutedEventArgs e)
        {
            nKeyboard.Hide();
            ActivaElementosCajero(false);
            Mesero.Text = "";
        }

        //-------------------------------------------------------------------------------

        private void fillData()
        {
            sbActivaVenta(true);

            if (this.ventas.ventfac)
            {
                if (this.ventas.ventStd != "")
                {
                    this.btnAnlFact.IsEnabled = true;
                    this.btnDelFact.IsEnabled = true;
                }
            }
            else
            {
                if (this.ventas.ventStd == "A" || this.ventas.ventStd == "E")
                {
                    this.btnAnlFact.IsEnabled = false;
                    this.btnDelFact.IsEnabled = true;
                    if (this.ventas.ventStd == "E")
                    {
                        this.btnFact.IsEnabled = false;
                    }
                }
            }

            if (this.ventas.venttip.Equals("D"))
            {
                btnToDelivery.IsEnabled = true;
            }

            sbDesactivaPedidos(false);

            double subTotalNoIva = 0;
            double SubTotalConIva = 0;
            double subTotal = 0;
            double descuento = 0;
            double iva = 0;
            double total = 0;

            subTotalNoIva = ventas.detVenta.Where(x => x.itemiva == 0).Select(o => o.itemprc * o.itemcnt).Sum();
            SubTotalConIva = ventas.detVenta.Where(x => x.itemiva > 0).Select(o => o.itemprc * o.itemcnt).Sum();

            dgvPedd.ItemsSource = ventas.detVenta.Select(o => new ProdPedd() {
                codigo = o.itemcod,
                producto = o.nbrprd,
                cantidad = o.itemcnt,
                precio = o.itemprc,
                iva = o.itemiva,
                total = o.itempto
            });

            if (this.ventas.ventStd == "")
            {
                iva = SubTotalConIva * (General.IVA / 100);
                total = subTotalNoIva + SubTotalConIva + iva;
                txtSubTot0.Text = General.StringFormatMoney(subTotalNoIva);
                txtSubTot.Text = General.StringFormatMoney(SubTotalConIva);
                txtIva.Text = General.StringFormatMoney(iva);
                txtTotal.Text = General.StringFormatMoney(total);

                this.ventas.ventsbt = SubTotalConIva;
                this.ventas.ventsbn = subTotalNoIva;
                this.ventas.ventdsc = descuento;
                this.ventas.ventiva = iva;
                this.ventas.venttot = total;
            }
            else
            {
                iva = this.ventas.ventiva;
                subTotal = this.ventas.ventsbt;
                descuento = this.ventas.ventdsc;
                total = this.ventas.venttot;
                txtSubTot.Text = General.StringFormatMoney(subTotal);
                txtSubTot0.Text = General.StringFormatMoney(this.ventas.ventsbn);
                txtDscto.Text = General.StringFormatMoney(descuento);
                txtIva.Text = General.StringFormatMoney(iva);
                txtTotal.Text = General.StringFormatMoney(total);
            }
            
            if (this.ventas.ventfac && this.ventas.ventpgd)
            {
                btnFacturar.IsEnabled = true;
                btnImpFact.IsEnabled = false;
                btnFact.IsEnabled = false;
            }
            else
            {
                if (this.ventas.ventStd != "E")
                {
                    btnFacturar.IsEnabled = true;
                }
                else
                {
                    btngift.IsEnabled = false;
                }
                btnImpFact.IsEnabled = true;
                btnFact.IsEnabled = true;
            }

            // En caso de hacer uso de un carrier.
            if (this.ventas.venttip == "E")
            {
                btnToPay.IsEnabled = false;
                btnDescto.IsEnabled = false;
                btngift.IsEnabled = false;
                this.txtRecibido.Text = General.StringFormatMoney(total);
                this.ventas.ventrcb = General.StringToDouble(this.txtRecibido.Text.Replace('.', ','));
                this.ventas.ventvto = 0.0;
            }
            else
            {
                btnToPay.IsEnabled = true;
            }
        }

        private void getPeddidoByVentScl(string ventScl)
        {
            sbLimpiar();

            this.ventas.getPedidoByVentScl(ventScl);

            txtCedula.Text = this.ventas.nidnid;
            txtCliente.Text = this.ventas.nidnbr;

            txtRecibido.Text = General.StringFormatMoney(this.ventas.ventrcb);
            txtCambio.Text = General.StringFormatMoney(this.ventas.ventvto);

            this.ventas.ventrcb = this.ventas.ventrcb;
            this.ventas.ventvto = this.ventas.ventvto;
            

            fillData();
        }

        private void getPedido(string peddScl, bool vendido)
        {
            sbLimpiar();

            if (vendido)
            {
                this.ventas.getVendido(peddScl);

                txtCedula.Text = this.ventas.cliNid;
                txtCliente.Text = this.ventas.cliNbr;

                txtRecibido.Text = General.StringFormatMoney(this.ventas.ventrcb);
                txtCambio.Text = General.StringFormatMoney(this.ventas.ventvto);

                this.ventas.ventrcb = this.ventas.ventrcb;
                this.ventas.ventvto = this.ventas.ventvto; 

            }
            else
            {
                this.ventas.getPedido(peddScl);

                if (this.ventas.ventfac)
                {
                    txtCedula.Text = this.ventas.nidnid;
                    txtCliente.Text = this.ventas.nidnbr;
                }
            }

            fillData();
            
        }

        private void btnFactElec_Click(object sender, RoutedEventArgs e)
        {

        }

        public void ctlButton_Click(object sender, RoutedEventArgs e, string tipo)
        {
            Button btn = ((Button)sender);

            String peddScl = btn.Tag.ToString();
            string uid = btn.Uid;
            if (uid != "")
            {
                string[] data = uid.Split(':');

                try
                {
                    string ventscl = data[0];
                    if (data.Length > 1)
                    {
                        
                        if (data[1] == "A")
                        {
                            getPeddidoByVentScl(ventscl);
                            btnDivFact.IsEnabled = true;
                            btnAnlFact.IsEnabled = false;
                        }
                        else
                        {
                            if (ventscl != "" && ventscl != null)
                            {
                                getPeddidoByVentScl(ventscl);
                                btnDivFact.IsEnabled = false;
                            }
                            else
                            {
                                getPedido(peddScl, false);
                                btnDivFact.IsEnabled = true;
                            }
                        }
                    }
                    else
                    {
                        if (ventscl != "" && ventscl != null)
                        {
                            getPeddidoByVentScl(ventscl);
                            btnDivFact.IsEnabled = false;
                        }
                        else
                        {
                            getPedido(peddScl, false);
                            btnDivFact.IsEnabled = true;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                getPedido(peddScl, false);
                if (ventas.venttip == "E")
                {
                    btnDivFact.IsEnabled = false;
                }
                else
                {
                    btnDivFact.IsEnabled = true;
                }
            }
        }

        private void btnAutomata_Unchecked(object sender, RoutedEventArgs e)
        {
            General.ENVIO_AUTO_AL_SRI = false;
        }

        private void btnAutomata_Checked(object sender, RoutedEventArgs e)
        {
            General.ENVIO_AUTO_AL_SRI = true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ctlVenta() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
