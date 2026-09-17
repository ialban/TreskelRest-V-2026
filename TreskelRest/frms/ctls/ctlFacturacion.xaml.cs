using Entidades;
using Entidades.modelos;
using LibControls.buttons;
using LibControls.contenedor;
using LibKeys;
using LibPrint;
using LibPrint.dataset;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    public class Mesero
    {
        public string Content;
        public string Tag;
    }
    /// <summary>
    /// Lógica de interacción para ctlFacturacion.xaml
    /// </summary>
    public partial class ctlFacturacion : UserControl, IDisposable
    {
        private DataTable dtTipPedd = new datasets.dsPedidos.tiposDataTable();
        private ctlGrpProd gProd = new ctlGrpProd();
        //private Ventas ventas = new Ventas(General.connectionString);
        private VentasMarket ventas = new VentasMarket(General.connectionString);
        
        private List<ProdPedd> cocina = new List<ProdPedd>();
        private List<mPago> pagos = new List<mPago>();
        private NumKeyboard nKeyboard = new NumKeyboard();

        private Boolean PUEDE_DAR_DESCUENTO = false;
        private Boolean PUEDE_DAR_REGALO = false;
        private Boolean PUEDE_ANULAR_FACTURA = false;
        private Boolean PUEDE_ELIMINAR_PEDIDO = false;

        private Boolean DELIVERY_PUSH = false;

        private Mesero mesero = new Mesero();
        private string _barcode = "";
        private bool PorBusqueda = false;
        private string codigoUsuario = "";
        private string codigoUsuarioElimina = "";

        public ctlFacturacion()
        {
            InitializeComponent();
            
            ventas.cajacod = General.CAJA;

            DataRow dr = dtTipPedd.NewRow();
            dr["codigo"] = "L";
            dr["descripcion"] = "LOCAL";

            dtTipPedd.Rows.Add(dr);

            dr = dtTipPedd.NewRow();
            dr["codigo"] = "D";
            dr["descripcion"] = "DOMICILIO";

            dtTipPedd.Rows.Add(dr);

            dr = dtTipPedd.NewRow();
            dr["codigo"] = "C";
            dr["descripcion"] = "LLEVAR";

            dtTipPedd.Rows.Add(dr);

            dr = dtTipPedd.NewRow();
            dr["codigo"] = "E";
            dr["descripcion"] = "CARRIER";

            dtTipPedd.Rows.Add(dr);

            dr = dtTipPedd.NewRow();
            dr["codigo"] = "R";
            dr["descripcion"] = "RETIRAR";

            dtTipPedd.Rows.Add(dr);

            //btnImpFact.IsEnabled = false;

            sbLimpiar();
            ActivaCrearPedido(false);

            nKeyboard.retornaTexto += new LibKeys.NumKeyboard.RetornaTexto(TextoNumerico_Retorna);
            SetTamanioMenuConfig();
            barcode.Visibility = Visibility.Hidden;
            buscaItem.Visibility = Visibility.Hidden;
            btnBscCls.Visibility = Visibility.Hidden;
            btnBscItem.Visibility = Visibility.Hidden;
            this.KeyUp += ctlFacturacion_KeyUp;
        }

        private void SetTamanioMenuConfig()
        {
            if (General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal)
            {
                setTamanioBotones(btnSalir, 70, 14, 30);
                setTamanioBotones(btnCrear, 70, 14, 30);
                setTamanioBotones(btnBscPedd, 70, 14, 30);
                setTamanioBotones(btnFacturar, 70, 14, 30);
                setTamanioBotones(btnAnlFact, 70, 14, 30);
                setTamanioBotones(btnDelFact, 70, 14, 30);
                setTamanioBotones(btnCancelar, 70, 14, 30);
                //setTamanioBotones(btngift, 70, 14, 30);
                setTamanioBotones(btnDescto, 70, 14, 30);
                setTamanioBotones(btnToPay, 70, 14, 30);
                setTamanioBotones(btnCajero, 70, 14, 30);
                //Mesero.FontSize = 14;
                //Mesero.HorizontalContentAlignment = HorizontalAlignment.Right;
            }
            else
            {
                setTamanioBotones(btnSalir, 50, 10, 25);
                setTamanioBotones(btnCrear, 50, 10, 25);
                setTamanioBotones(btnBscPedd, 50, 10, 25);
                setTamanioBotones(btnFacturar, 50, 10, 25);
                setTamanioBotones(btnAnlFact, 50, 10, 25);
                setTamanioBotones(btnDelFact, 50, 10, 25);
                setTamanioBotones(btnCancelar, 50, 10, 25);
                //setTamanioBotones(btngift, 50, 10, 25);
                setTamanioBotones(btnDescto, 50, 10, 25);
                setTamanioBotones(btnToPay, 50, 10, 25);
                setTamanioBotones(btnCajero, 50, 10, 25);
                //Mesero.FontSize = 10;
                //Mesero.HorizontalContentAlignment = HorizontalAlignment.Right;
            }
        }

        private void setTamanioBotones(object obj, int size, int fontSize, int imgSize)
        {
            if (obj.GetType() == typeof(Button))
            {
                var btn = (Button)obj;
                btn.Height = size;
                btn.Width = size;
                ((Image)((StackPanel)btn.Content).Children[0]).Width = imgSize;
                ((Image)((StackPanel)btn.Content).Children[0]).Height = imgSize;
                ((Label)((StackPanel)btn.Content).Children[1]).FontSize = fontSize;
            }
            else
            {
                var btn = (ToggleButton)obj;
                btn.Height = size;
                btn.Width = size;
                ((Image)((StackPanel)btn.Content).Children[0]).Width = imgSize;
                ((Image)((StackPanel)btn.Content).Children[0]).Height = imgSize;
                ((Label)((StackPanel)btn.Content).Children[1]).FontSize = fontSize;
            }
        }

        private void sbLimpiar()
        {
            txtTotal.Text = "0,00";
            txtSubTot.Text = "0,00";
            txtSubTot0.Text = "0,00";
            txtDscto.Text = "0,00";
            txtIva.Text = "0,00";
            txtRecibido.Text = "0,00";
            txtCambio.Text = "0,00";

            txtCedula.Text = "";
            txtCliente.Text = "";

            ventas.nidnid = "";
            ventas.cliDir = "";
            ventas.cliNbr = "";
            ventas.cliNid = "";
            ventas.cliTlf = "";
            ventas.cliRef = "";

            ventas.ventmsa = "";
            ventas.peddscl = "";

            ventas.ventcrr = "";
            ventas.ventdpc = 0;
            ventas.ventdsc = 0;
            ventas.venttot = 0;
            ventas.ventsbt = 0;
            ventas.ventiva = 0;

            ventas.ventlta = "A";
            ventas.venttip = "L";
            ventas.ventfac = false;

            ventas.ventcrr = "0000";
            ventas.detVenta.Clear();
            ventas.detPago.Clear();

            pedidoContainer.Children.Clear();
            if (dgvPedd.ItemsSource != null)
                dgvPedd.ItemsSource = null;
            else
                dgvPedd.Items.Clear();

            this.DELIVERY_PUSH = false;
            SetTamanioMenuConfig();
            barcode.Visibility = Visibility.Hidden;
            buscaItem.Visibility = Visibility.Hidden;
            btnBscCls.Visibility = Visibility.Hidden;
            btnBscItem.Visibility = Visibility.Hidden;

            btnCdtoDir.IsEnabled = false;
            btnCdtoDir.Visibility = Visibility.Hidden;
            btnCdtoDir.IsChecked = false;
            btnCdtoDir.Width = 0;
            txtCliente.Width = 255;
        }

        private void ActivaCrearPedido(Boolean valor)
        {
            btnCrear.IsEnabled = valor;
            btnBscPedd.IsEnabled = valor;
            btnCancelar.IsEnabled = valor;
        }

        private void BuscaPorBarcode()
        {
            if (barcode.Text == "") return;

            this.sbActivaBtnPedd(true);
            Items itemsSel = new Items(General.connectionString);

            DataTable dti = itemsSel.getById(barcode.Text, this.ventas.ventcrr);

            if (!(General.StringToDouble(dti.Rows[0]["precio"].ToString()) > 0))
            {
                ObjComportamiento.showMsgError("Producto no encontrado");
                barcode.Text = "";
                return;
            }
            // Buscar si existe el item agregado
            Boolean existe = false;
            for (int i = 0; i < dgvPedd.Items.Count; i++)
            {
                if (((ProdPedd)dgvPedd.Items[i]).codigo.Equals(dti.Rows[0]["codigo"].ToString()) &&
                    ((ProdPedd)dgvPedd.Items[i]).peso == General.StringToDouble(dti.Rows[0]["peso"].ToString()))
                {
                    ((ProdPedd)dgvPedd.Items[i]).cantidad++;
                    dgvPedd.Items.Refresh();
                    existe = true;
                }
            }

            // Buscar si existe el item en control
            Boolean existeControl = false;
            for (int i = 0; i < cocina.Count; i++)
            {
                if (cocina[i].codigo.Equals(dti.Rows[0]["codigo"].ToString()) || cocina[i].noIngredientes.Count > 0)
                {
                    cocina[i].cantidad++;
                    existeControl = true;
                    break;
                }
            }

            ProdPedd prodPedd = new ProdPedd
            {
                codigo = dti.Rows[0]["codigo"].ToString(),
                producto = dti.Rows[0]["producto"].ToString(),
                cantidad = 1,
                usaIva = (Boolean)dti.Rows[0]["usaIva"],
                gravaIva = (Boolean)dti.Rows[0]["gravaIva"],
                precio = Math.Round(General.StringToDouble(dti.Rows[0]["Precio"].ToString()), 6),
                iva = Math.Round(Math.Abs(General.StringToDouble(dti.Rows[0]["IVA"].ToString()) - General.StringToDouble(dti.Rows[0]["precio"].ToString())), 6),
                precioPorPeso = (Boolean)dti.Rows[0]["porpeso"],
                peso = General.StringToDouble(dti.Rows[0]["peso"].ToString())
            };

            ProdPedd prodCocina = null;
            if ((Boolean)dti.Rows[0]["itemCoc"] == true)
            {
                prodCocina = new ProdPedd
                {
                    codigo = dti.Rows[0]["codigo"].ToString(),
                    producto = dti.Rows[0]["producto"].ToString(),
                    cantidad = 1,
                    usaIva = (Boolean)dti.Rows[0]["usaIva"],
                    gravaIva = (Boolean)dti.Rows[0]["gravaIva"],
                    precio = Math.Round(General.StringToDouble(dti.Rows[0]["Precio"].ToString()), 6),
                    iva = Math.Round(Math.Abs(General.StringToDouble(dti.Rows[0]["IVA"].ToString()) - General.StringToDouble(dti.Rows[0]["precio"].ToString())), 6)
                };
            }

            if (!existe)
            {
                dgvPedd.Items.Add(prodPedd);
            }

            if (!existeControl && prodCocina != null)
            {
                cocina.Add(prodCocina);
            }

            // Calculamos el total
            this.calcularTotalMostrar();

            // Se limpia el contenedor
            _barcode = "";
            barcode.Text = "";
        }

        private void ctlFacturacion_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

            _barcode += e.Key;

            if (e.Key == Key.Enter)
            {
                BuscaPorBarcode();
            }

            if (_barcode.EndsWith("J"))
            {
                BuscaPorBarcode();
            }
        }

        private void TextoNumerico_Retorna(Object txt, String texto)
        {
            if (texto.Equals(""))
            {
                ObjComportamiento.showMsgError("Debe ingresar una clave para poder realizar el pedido/facturación");
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

                PUEDE_ANULAR_FACTURA = permisos.Find(X => X.tag == "AFCT").estado;
                PUEDE_DAR_DESCUENTO = permisos.Find(X => X.tag == "DSCT").estado;
                PUEDE_DAR_REGALO = permisos.Find(X => X.tag == "GIFT").estado;

                if (codigo.Equals(""))
                {
                    ObjComportamiento.showMsgError("Clave ingresada incorrecta");
                    btnCajero.IsChecked = false;
                    return;
                }

                UsrEmpleado usr = usuario.getEmpleadoMesero(codigo);
                if (usr.usrusr == null)
                {
                    mesero.Content = "NID";
                    mesero.Tag = "NID";
                }
                else
                {
                    mesero.Content = usr.usrusr.ToLower();
                    mesero.Tag = usr.usrnbr.ToUpper();
                }

                ventas.empdnid = codigo;
                ventas.celEmpl = cellphone;
                ventas.empdNbr = nombre;
                codigoUsuario = codigo;

                ActivaCrearPedido(true);
            }
            else
            {
                ObjComportamiento.showMsgError("Clave ingresada no válida");
            }
        }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            txtCedula.Text = "";
            txtCliente.Text = "";

            this.ventas.nidnid = txtCedula.Text;
            this.ventas.nidnbr = "";
            this.ventas.niddir = "";
            this.ventas.nidtlf = "";

            txtCedula.Focus();
        }

        private void btnBscCli_Click(object sender, RoutedEventArgs e)
        {
            Clientes cliente = new Clientes(General.connectionString);

            if (string.IsNullOrEmpty(txtCedula.Text))
            {
                ObjComportamiento.showMsgError("Debe ingresar un criterio de búsqueda del cliente");
                return;
            }

            //cliente.nidNid = txtCedula.Text.ToString();
            var lClientes = cliente.getByIdOrName(txtCedula.Text.ToString());
            if (lClientes.Count == 1)
            {
                txtCedula.Text = lClientes.First().nidNid;
                txtCliente.Text = lClientes.First().nidNbr;
                this.ventas.nidnbr = lClientes.First().nidNbr;
                this.ventas.nidnid = lClientes.First().nidNid;
                this.ventas.niddir = lClientes.First().nidDir;
                this.ventas.nideml = lClientes.First().nidEml;
                this.ventas.nidtlf = lClientes.First().nidTlf;
                this.ventas.nidcdto = lClientes.First().cliecdto;
                this.ventas.emprruc = lClientes.First().nidEmpr;
                this.ventas.emprnbr = lClientes.First().emprNbr;
                this.ventas.cdtopem = lClientes.First().emprPaga;

                PermiteCreditoDirecto(lClientes.First().cliecdto, lClientes.First().emprPaga);
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

            barcode.Focus();
        }

        private void PermiteCreditoDirecto(bool credito, bool emprPaga)
        {
            btnCdtoDir.IsChecked = false;
            if (credito && emprPaga)
            {
                btnCdtoDir.IsEnabled = true;
                btnCdtoDir.Visibility = Visibility.Visible;
                btnCdtoDir.IsChecked = false;
                btnCdtoDir.Width = 40;
                txtCliente.Width = 213;
            }
            else
            {
                btnCdtoDir.IsEnabled = false;
                btnCdtoDir.Visibility = Visibility.Hidden;
                btnCdtoDir.IsChecked = false;
                btnCdtoDir.Width = 0;
                txtCliente.Width = 255;
            }
        }

        private void cliente_Seleccionado(string codigo, string nombre, string direccion, string telefono, string email, string tipo, bool credito, string emprruc, string emprnbr, bool emprPaga)
        {
            txtCliente.Text = nombre;
            txtCedula.Text = codigo;

            this.ventas.nidnid = codigo;
            this.ventas.nidnbr = nombre;
            this.ventas.niddir = direccion;
            this.ventas.nidtlf = telefono;
            this.ventas.nideml = email;
            this.ventas.nidcdto = credito;
            this.ventas.emprruc = emprruc;
            this.ventas.emprnbr = emprnbr;
            this.ventas.cdtopem = emprPaga;

            PermiteCreditoDirecto(credito, emprPaga);
        }

        private void Cliente_Creado(string cliente, string cedula, string direccion, string telefono, string tipo, string email, bool clieCdto, string emprruc, string emprnbr, bool emprPaga)
        {
            txtCliente.Text = cliente;
            txtCedula.Text = cedula;

            this.ventas.nidnid = cedula;
            this.ventas.nidnbr = cliente;
            this.ventas.niddir = direccion;
            this.ventas.nidtlf = telefono;
            this.ventas.nideml = email;
            this.ventas.nidcdto = clieCdto;
            this.ventas.emprruc = emprruc;
            this.ventas.emprnbr = emprnbr;
            this.ventas.cdtopem = emprPaga;

            PermiteCreditoDirecto(clieCdto, emprPaga);
        }

        private void btnCliFnl_Click(object sender, RoutedEventArgs e)
        {
            if (General.StringToDouble(txtTotal.Text) > 50)
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

            barcode.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AceptaEliminacionProducto(int cantidad, ProdPedd row)
        {
            if (cantidad <= 1 && row.cantidad == 1)
            {
                dgvPedd.Items.Remove(row);
            }
            else
            {
                row.cantidad = row.cantidad - 1;
            }
        }

        private void muestraControlElimina(string pedscl, ref List<ProdPedd> prods, string codigo, bool memoria, int cantidad, ProdPedd row)
        {
            winGeneral win = new winGeneral();
            win.Title = "Eliminación de productos";
            win.Width = 720;
            win.Height = 470;

            ctlEliminaProd ctl = new ctlEliminaProd(pedscl, ref prods, codigo, memoria);
            ctl.cantidad = cantidad;
            ctl.row = row;
            ctl.contenedor = win;
            ctl.aceptaEliminacion += new ctlEliminaProd.AceptaEliminacionProducto(AceptaEliminacionProducto);
            ctl.generar();

            win.Contenido.Children.Add(ctl);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private int BuscarIndiceCocina(string codigo, List<ProdPedd> obj)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                if (obj[i].codigo == codigo) return i;
            }

            return -1;
        }

        private void AceptaEliminacionProducto(string peddscl, int cantidad, ProdPedd row, int index, bool memoria)
        {
            if (index >= 0)
            {
                if (memoria)
                {
                    if (Principal.pedidosCocina[peddscl][index].cantidad > 1)
                    {
                        Principal.pedidosCocina[peddscl][index].cantidad--;
                    }
                    else
                    {
                        Principal.pedidosCocina[peddscl].RemoveAt(index);
                    }
                }
                else
                {
                    if (this.cocina[index].cantidad > 1)
                    {
                        this.cocina[index].cantidad--;
                    }
                    else
                    {
                        this.cocina.RemoveAt(index);
                    }

                }

                if (cantidad <= 1 && row.cantidad == 1)
                {
                    dgvPedd.Items.Remove(row);
                }
                else
                {
                    row.cantidad = row.cantidad - 1;
                }
            }
        }

        private void btnDelItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProdPedd row = (ProdPedd)((FrameworkElement)sender).DataContext;
                String codigo = row.codigo;
                int cantidad = row.cantidad;

                var prodsToDel = cocina.Where(x => x.codigo == codigo).ToList();

                int cantidadProd = prodsToDel.Count;

                if (General.IMPRIME_COMANDAS_MARKET)
                {
                    if (cantidadProd > 0)
                    {
                        if (cantidadProd > 1 && !row.DelFromCoc)
                        {
                            if (!row.DelFromCoc)
                            {
                                muestraControlElimina("", ref cocina, codigo, false, cantidad, row);
                            }
                            else
                            {
                                ObjComportamiento.showMsgError("Este item debe ser eliminado desde Cocina.");
                            }
                        }
                        else
                        {
                            if (!row.DelFromCoc)
                            {
                                var prodToDel = prodsToDel.FirstOrDefault();
                                AceptaEliminacionProducto("", prodToDel.cantidad, row, BuscarIndiceCocina(row.codigo, cocina), false);
                            }
                            else
                            {
                                ObjComportamiento.showMsgError("Este item debe ser eliminado desde Cocina.");
                            }
                        }
                    }
                    else
                    {
                        string peddscl = this.ventas.peddscl.ToString().PadLeft(10, '0');
                        if (Principal.pedidosCocina.ContainsKey(peddscl))
                        {

                            cantidadProd = Principal.pedidosCocina[peddscl].Where(x => x.codigo == codigo).ToList().Count;

                            cantidadProd = this.cocina.Where(x => x.codigo == codigo).ToList().Count;

                            if (cantidadProd > 1 && !row.DelFromCoc)
                            {
                                if (!row.DelFromCoc)
                                {
                                    List<ProdPedd> data = Principal.pedidosCocina[peddscl];
                                    muestraControlElimina(peddscl, ref data, codigo, true, cantidad, row);
                                }
                                else
                                {
                                    ObjComportamiento.showMsgError("Este item debe ser eliminado desde Cocina.");
                                }
                            }
                            else
                            {
                                if (!row.DelFromCoc)
                                {
                                    var prd = Principal.pedidosCocina[peddscl].Where(x => x.codigo == codigo).FirstOrDefault();
                                    if (prd != null)
                                    {
                                        AceptaEliminacionProducto(peddscl, prd.cantidad, row, BuscarIndiceCocina(row.codigo, Principal.pedidosCocina[peddscl]), true);
                                    }
                                    else
                                    {
                                        if (cantidad <= 1 && row.cantidad == 1)
                                        {
                                            dgvPedd.Items.Remove(row);
                                        }
                                        else
                                        {
                                            row.cantidad = row.cantidad - 1;
                                        }
                                    }
                                }
                                else
                                {
                                    ObjComportamiento.showMsgError("Este item debe ser eliminado desde Cocina.");
                                }
                            }
                        }
                        else
                        {
                            Items itemDel = new Items(General.connectionString);
                            itemDel.itemCod = row.codigo;
                            if (itemDel.getById())
                            {
                                if (!itemDel.itemDelCoc)
                                {
                                    if (cantidad <= 1)
                                    {
                                        dgvPedd.Items.Remove(row);
                                    }
                                    else
                                    {
                                        row.cantidad = cantidad - 1;
                                    }
                                }
                                else
                                {
                                    ObjComportamiento.showMsgError("Este item debe ser eliminado desde Cocina.");
                                }
                            }
                        }
                    }
                }

                //AceptaEliminacionProducto(cantidad, row);
               
                dgvPedd.Items.Refresh();

                this.calcularTotalMostrar();
                
                sbActivaBtnPedd(dgvPedd.Items.Count >= 1 ? true : false);

                //dgvPedd.Columns[0].Visibility = Visibility.Hidden;
            }
            catch (Exception er)
            {

                
            }
        }

        private void btnDelItems_Click(object sender, RoutedEventArgs e)
        {
            dgvPedd.Items.Clear();
            dgvPedd.Items.Refresh();
            dgvPedd.Items.Refresh();
            this.calcularTotalMostrar();
            sbActivaBtnPedd(dgvPedd.Items.Count >= 1 ? true : false);
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            sbLimpiar();

            btnCrear.IsEnabled = false;
            btnCancelar.IsEnabled = true;
            btnBscPedd.IsEnabled = false;
            btnCajero.IsEnabled = false;
            cocina.Clear();

            if (General.SOLO_VENTAS_LOCALES)
            {
                if (General.MUESTRA_MESAS_MARKET)
                {
                    creaMesas();
                }
                else
                {
                    creaGrupos();
                }

                if (General.PERMITE_USO_BARCODE)
                {
                    barcode.Visibility = Visibility.Visible;
                    barcode.Focus();
                }
            }
            else
            {
                foreach (DataRow dr in this.dtTipPedd.Rows)
                {
                    ctlButton btn = new ctlButton();

                    btn.Codigo = dr["codigo"].ToString();
                    btn.Titulo = dr["descripcion"].ToString();

                    btn.Imagen = General.sbDefineImagen(btn.Codigo);

                    btn._ctlButtonClick += new ctlButton.ctlButton_Click(ctlButton_Click);

                    pedidoContainer.Children.Add(btn);
                }
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private void btnBscPedd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                winGeneral win = new winGeneral();
                win.Title = "Buscar Pedido";
                win.Width = 760;
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
            catch (Exception er)
            {

            }
        }

        private void Busqueda_Cancelada()
        {
            btnBscPedd.IsEnabled = true;
            btnAnlFact.IsEnabled = false;
        }

        private void Pedido_Seleccionado(String pedidoScl, Boolean facturado)
        {
            try
            {
                btnFacturar.IsEnabled = false;

                //btnImpFact.IsEnabled = true;

                getPedido(pedidoScl, true);

                txtCedula.Text = this.ventas.nidnid;
                txtCliente.Text = this.ventas.nidnbr;

                txtRecibido.Text = String.Format("{0:0.00}", this.ventas.ventrcb);
                txtCambio.Text = String.Format("{0:0.00}", this.ventas.ventvto);

                if (facturado)
                {
                    btnToPay.IsEnabled = false;
                    btnDescto.IsEnabled = false;
                    //btngift.IsEnabled = false;
                    //btnImpFact.IsEnabled = true;
                }
                else
                {
                    btnToPay.IsEnabled = true;
                    if (this.PUEDE_DAR_DESCUENTO) btnDescto.IsEnabled = true;
                    //if (this.PUEDE_DAR_REGALO && this.ventas.ventStd != "E") btngift.IsEnabled = true;
                }

                if (this.PUEDE_ANULAR_FACTURA && (this.ventas.ventStd != "A" && this.ventas.ventStd != "E")) btnAnlFact.IsEnabled = true;

                btnBscPedd.IsEnabled = false;

                dgvPedd.Columns[0].Visibility = Visibility.Hidden;
                PorBusqueda = true;

                if (codigoUsuarioElimina != ventas.empdnid)
                    ventas.empdnid = codigoUsuarioElimina;
            }
            catch (Exception ex)
            {

            }
        }

        private void getPedido(String peddScl, Boolean vendido)
        {
            sbLimpiar();

            if (vendido)
            {
                this.ventas.getVendido(peddScl);

                if (this.ventas.ventStd == "A")
                {
                    this.ventas.ventscl = -1;
                    this.ventas.peddscl = "";
                }
                else
                {
                    txtCedula.Text = this.ventas.cliNid;
                    txtCliente.Text = this.ventas.cliNbr;

                    txtRecibido.Text = General.StringFormatMoney(this.ventas.ventrcb);
                    txtCambio.Text = General.StringFormatMoney(this.ventas.ventvto);

                    this.ventas.ventrcb = this.ventas.ventrcb;
                    this.ventas.ventvto = this.ventas.ventvto;
                }
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

        private void fillData()
        {
            ActivaCrearPedido(true);

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
                        this.btnFacturar.IsEnabled = false;
                    }
                }
            }

            if (this.ventas.venttip.Equals("D"))
            {
                //btnToDelivery.IsEnabled = true;
            }


            double subTotalNoIva = 0;
            double SubTotalConIva = 0;
            double subTotal = 0;
            Double descuento = 0;
            Double iva = 0;
            Double total = 0;

            subTotalNoIva = ventas.detVenta.Where(x => x.itemiva == 0).Select(o => o.itemprc).Sum();
            SubTotalConIva = ventas.detVenta.Where(x => x.itemiva > 0).Select(o => o.itemprc).Sum();

            dgvPedd.ItemsSource = ventas.detVenta.Select(o => new ProdPedd()
            {
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
            //sbDesactivaPedidos(false);

            //Double subTotal = 0;
            //Double descuento = 0;
            //Double iva = 0;
            //Double total = 0;

            //foreach (VentaDetalle detalle in this.ventas.detVenta)
            //{
            //    ProdPedd prod = new ProdPedd();
            //    prod.codigo = detalle.itemcod;
            //    prod.producto = detalle.nbrprd;
            //    prod.cantidad = detalle.itemcnt;
            //    prod.precio = detalle.itemprc;
            //    prod.iva = detalle.itemiva;
            //    prod.total = detalle.itempto;

            //    dgvPedd.Items.Add(prod);

            //    subTotal += prod.cantidad * prod.precio;
            //}

            //dgvPedd.Columns[0].Visibility = Visibility.Hidden;

            //if (this.ventas.ventStd == "")
            //{
            //    iva = subTotal * (General.IVA / 100);
            //    total = subTotal + iva;
            //    txtSubTot.Text = General.StringFormatMoney(subTotal);
            //    txtIva.Text = General.StringFormatMoney(iva);
            //    txtTotal.Text = General.StringFormatMoney(total);

            //    this.ventas.ventsbt = subTotal;
            //    this.ventas.ventdsc = descuento;
            //    this.ventas.ventiva = iva;
            //    this.ventas.venttot = total;
            //}
            //else
            //{
            //    iva = this.ventas.ventiva;
            //    subTotal = this.ventas.ventsbt;
            //    descuento = this.ventas.ventdsc;
            //    total = this.ventas.venttot;
            //    txtSubTot.Text = General.StringFormatMoney(subTotal);
            //    txtDscto.Text = General.StringFormatMoney(descuento);
            //    txtIva.Text = General.StringFormatMoney(iva);
            //    txtTotal.Text = General.StringFormatMoney(total);
            //}

            //this.ventas.peddscl = peddScl;
            if (this.ventas.ventfac && this.ventas.ventpgd)
            {
                btnFacturar.IsEnabled = true;
                //btnImpFact.IsEnabled = false;
            }
            else
            {
                if (this.ventas.ventStd != "E")
                {
                    btnFacturar.IsEnabled = true;
                }
                else
                {
                    //btngift.IsEnabled = false;
                }
                //btnImpFact.IsEnabled = true;
            }

            // En caso de hacer uso de un carrier.
            if (this.ventas.venttip == "E")
            {
                btnToPay.IsEnabled = false;
                btnDescto.IsEnabled = false;
                //btngift.IsEnabled = false;
                this.txtRecibido.Text = General.StringFormatMoney(total);
                this.ventas.ventrcb = General.StringToDouble(this.txtRecibido.Text.Replace('.', ','));
                this.ventas.ventvto = 0.0;
            }
            else
            {
                btnToPay.IsEnabled = true;
            }
        }

        private void btnAnlFact_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Listado de Motivos para anular una factura";
            win.Width = 500;
            win.Height = 400;

            ctlMotivosAnula crea = new ctlMotivosAnula();
            crea.secuencial = this.ventas.ventscl.ToString().PadLeft(10, '0');
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

        private void Anular_Factura(String ventScl, String ventObv, string numFact)
        {
            if (!ventScl.Equals(""))
            {
                Respuesta res = this.ventas.anulaFactura(ventScl, ventObv, numFact);
                ObjComportamiento.showMuestraMensaje(res);

                if (res.resultado)
                {
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

        private void btnFacturar_Click(object sender, RoutedEventArgs e)
        {
            this.llenaDetalleYPago();

            //if (this.ventas.detVenta.Count > General.CANTIDAD_MAXIMA_ARTICULOS_FACTURAR)
            //{
            //    ObjComportamiento.showMsgError("Existen más de " + General.CANTIDAD_MAXIMA_ARTICULOS_FACTURAR + " artículos por facturar, seleccione SEPARAR FACTURA");
            //    return;
            //}

            // Mandamos a grabar el pedido
            if (txtCedula.Text.Equals(""))
            {
                ObjComportamiento.showMsgError("No ha ingresado un cliente válido");
                return;
            }

            if (this.ventas.detPago.Count == 0 && !PorBusqueda)
            {
                if (!General.PERMITE_DAR_CREDITO || !ventas.nidcdto)
                {
                    ObjComportamiento.showMsgError("Debe ingresar el pago respectivo del elemento a facturar.");
                    return;
                }
            }

            if (!PorBusqueda)
            {
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
                        this.ventas.ventsep = "C";
                        this.ventas.clave_download = General.contraseniaDownload(10);

                        if (this.ventas.empdnid != codigoUsuario && !string.IsNullOrEmpty(codigoUsuario))
                        {
                            this.ventas.empdnid = codigoUsuario;
                        }

                        Respuesta res = this.ventas.grabarPedidoFactura();

                        if (General.IMPRIME_COMANDAS_MARKET)
                        {
                            if (ObjComportamiento.showMsgYesNo("¿Desea imprimir la comanda?") == MessageBoxResult.Yes)
                            {
                                paraCocinar(this.ventas.peddscl);
                                //paraBar(sec);

                                this.cocina.Clear();
                            }
                        }

                        if (res.resultado)
                        {
                            // Obtenemos el secuencial de la venta
                            var sec = res.mensaje.Split(' ').LastOrDefault();
                            this.ventas.ventscl = int.Parse(sec);

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

                            if (General.ENVIO_AUTO_AL_SRI)
                            {
                                mFacturasSRI data = new mFacturasSRI();
                                data = ventas.getFacturasToSRIByVentcls(ventas.ventscl.ToString().PadLeft(10, '0'));

                                winGeneral win = new winGeneral();
                                win.Width = 500;
                                win.Height = 300;

                                ctlProgressFact prgFact = new ctlProgressFact();
                                prgFact.contenedor = win;

                                win.Contenido.Children.Add(prgFact);
                                win.ResizeMode = ResizeMode.NoResize;
                                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                win.WindowStyle = WindowStyle.None;

                                prgFact.Iniciar(data);

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
            else
            {
                // Reimpresión de factura
                if (ObjComportamiento.showMsgYesNo("¿Desea imprimir la factura?") == MessageBoxResult.Yes)
                {
                    this.ToPrint("F");
                }

                btnCancelar_Click(sender, e);
            }
        }

        private void paraCocinar(String secuencial, string secuencialTemp = "")
        {
            //Imprimir cls = new Imprimir();
            if (this.cocina.Count == 0)
            {
                ObjComportamiento.showMsgError("No existen items para enviar a imprimir a Cocina");
                return;
            }

            try
            {
                //this.ventas.ltaPedd = this.pedido.getLetraPedido(secuencial);
                EposPrint cls = new EposPrint();
                EposPrint clsPar = new EposPrint();
                cls.TipoImpresora = General.TIPO_IMPRESORAS;
                clsPar.TipoImpresora = General.TIPO_IMPRESORAS;
                cls.imprimeSecTemporal = General.IMPRIME_SECUENCIAL_TEMPORAL;
                clsPar.imprimeSecTemporal = General.IMPRIME_SECUENCIAL_TEMPORAL;

                Respuesta resp = cls.setPrinter(General.PRINTER_COCINA);
                bool sendToParrilla = false;

                if (General.PRINTER_PARRILLA != "" && General.PRINTER_PARRILLA != "Ninguna" && General.ENVIA_IMPRESORA_PARRILLA == true)
                {
                    clsPar.setPrinter(General.PRINTER_PARRILLA);
                    clsPar.cantidadColumnas = General.CANTIDAD_COLUMNAS_IMPRESION;
                    clsPar.printCajaIzq = General.TICKET_COCINA_CANTIDAD_IZQ;
                    clsPar.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;
                    clsPar.ImprimeCabeceraComandas = General.IMPRIME_CABECERA_COMANDAS;
                    sendToParrilla = true;
                }

                if (resp.resultado == false)
                {
                    ObjComportamiento.showMsgError(resp.mensaje);
                    cls.Dispose();
                    clsPar.Dispose();
                    return;
                }

                cls.cantidadColumnas = General.CANTIDAD_COLUMNAS_IMPRESION;
                cls.printCajaIzq = General.TICKET_COCINA_CANTIDAD_IZQ;
                cls.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;
                cls.ImprimeCabeceraComandas = General.IMPRIME_CABECERA_COMANDAS;
                cls.SaltoLineaCabeceraComandas = General.SALTOLINEA_CABECERA_COMANDAS;
                cls.ImprimeClienteComanda = General.IMPRIME_CLIENTE_COMANDA;


                dsPrinter ds = new dsPrinter();
                ds.DataSetName = "cfa";
                DateTime fecha = DateTime.Now;
                String sTipo = this.ventas.venttip;
                String sMesa = this.ventas.ventmsa;
                String sScl = secuencial;
                String sSclTmp = secuencialTemp;
                String sFha = fecha.ToString("yyyy/MM/dd");
                
                for (int i = 0; i < dgvPedd.Items.Count; i++)
                {
                    ds.tbCocina.Rows.Add();
                    ds.tbCocina.Rows[i]["itemNbr"] = ((ProdPedd)dgvPedd.Items[i]).producto;
                    ds.tbCocina.Rows[i]["itemCtd"] = ((ProdPedd)dgvPedd.Items[i]).cantidad;
                }

                if (ds.tbCocina.Rows.Count > 0)
                {
                    //Se pasa a reducir el inventario
                    //Items items = new Items(General.connectionString);
                    //items.itemsInventario(sScl, this.cocina);

                    String tipo = "";
                    if (sTipo == "L" || sTipo == "") tipo = "LOCAL";
                    if (sTipo == "D") tipo = "DOMICILIO";
                    if (sTipo == "C") tipo = "LLEVAR";
                    if (sTipo == "E") tipo = "CARRIER";
                    if (sTipo == "R") tipo = "RETIRAR";

                    string sLetra = this.ventas.ltaPedd;

                    cls.Comanda("COCINA",
                                this.cocina,
                                sMesa + " " + sLetra, sFha,
                                tipo,
                                sScl,
                                General.IMPRIME_HORA_COCINA == 1 ? true : false,
                                this.mesero.Content.ToString(),
                                ventas.vtaCliDoNom, //this.pedido.nbrCrri,
                                this.ventas.ventcrr, //this.pedido.crrCodi,
                                this.ventas.nidnbr, "",
                                sSclTmp);

                    cls.sbIpr();

                    if (sendToParrilla)
                    {
                        clsPar.cantidadColumnas = General.CANTIDAD_COLUMNAS_IMPRESION;

                        clsPar.Comanda("PARRILLA",
                               this.cocina,
                               sMesa + " " + sLetra, sFha,
                               tipo,
                               sScl,
                               General.IMPRIME_HORA_COCINA == 1 ? true : false,
                               this.mesero.Content.ToString(),
                               ventas.vtaCliDoNom, //this.pedido.nbrCrri,
                               this.ventas.ventccd, //this.pedido.crrCodi,
                               this.ventas.nidnbr);
                    }
                    cls.Dispose();
                    clsPar.Dispose();
                }
            }
            catch (Exception e)
            {
                ObjComportamiento.showMsgError(e.Message);
            }
        }

        private void llenaDetalleYPago()
        {
            this.ventas.detVenta.Clear();
            this.ventas.detPago.Clear();

            for (int i = 0; i < dgvPedd.Items.Count; i++)
            {
                var prodPedd = (ProdPedd)dgvPedd.Items[i];

                this.ventas.detVenta.Add(new VentaDetalle
                {
                    itemcod = prodPedd.codigo,
                    itemcnt = prodPedd.cantidad,
                    itemiva = prodPedd.iva,
                    itemprc = prodPedd.precio,
                    //itempto = prodPedd.total,
                    itempto = (prodPedd.precio + prodPedd.iva) * prodPedd.cantidad,
                    nbrprd = prodPedd.producto,
                    usaiva = prodPedd.usaIva
                });
            }

            this.ventas.detPago = this.pagos;
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
                dr["pgoValor"] = this.ventas.detPago.Sum(x => x.valor);
                dr["pgoCbio"] = this.ventas.detPago.Sum(x => x.valor) - this.ventas.venttot;//this.ventas.detPago[i].vuelto;

                ds.tbPagoVta.Rows.Add(dr);
            }

            try
            {
                var copias = General.CANTIDAD_COPIAS_FACTURA;

                for (int i = 0; i < copias; i++)
                {
                    EposPrint cls = new EposPrint();
                    cls.TipoImpresora = General.TIPO_IMPRESORAS;
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
                        cls.imprimeClaveDownload = General.IMPRIME_CLAVE_DOWNLOAD;
                        cls.UrlDescarga = General.URL_SITIO_WEB;
                        

                        cls.Factura(ds.tbPeddVta, ds.tbPagoVta, General.EMPRESA, General.DIRECCION_LOCAL,
                            General.RUC_EMPRESA, this.ventas.nidnid, this.ventas.nidnbr, this.ventas.niddir,
                                this.ventas.nidtlf, this.ventas.nideml, this.ventas.venttip, this.ventas.peddscl,
                                this.mesero.Content,
                                this.ventas.ventmsa + " " + this.ventas.ventlta,
                                String.Format("{0:0.00}", this.ventas.ventsbt),
                                String.Format("{0:0.00}", this.ventas.ventsbn),
                                String.Format("{0:0.00}", this.ventas.ventdsc),
                                String.Format("{0:0.00}", this.ventas.ventdpc),
                                String.Format("{0:0.00}", this.ventas.ventiva),
                                String.Format("{0:0.00}", this.ventas.venttot),
                                String.Format("{0:0.00}", this.ventas.ventrcb),
                                String.Format("{0:0.00}", this.ventas.ventvto),
                                this.ventas.clave_download, // Para la clave de descarga para el cliente.
                                this.ventas.ventscl.ToString().PadRight(10, '0'),
                                this.ventas.nidcdto, this.ventas.emprnbr, this.ventas.emprruc,
                                this.ventas.cdtopem);
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
                                this.DeliveryTicket(this);
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
            catch (Exception e)
            {
                ObjComportamiento.showMsgError("Error al imprimir la " + (tipo.Equals("F") == true ? "Factura" : "Proforma"));
            }
        }

        private void DeliveryTicket(object sender)
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

                this.DELIVERY_PUSH = true;
                if (this.ventas.venttip.Equals("D"))
                {
                    cls.fnDataPeddDmclo(this.ventas.peddscl, this.ventas.cliNbr, this.ventas.cliTlf,
                                                   this.ventas.cliDir, this.ventas.cliRef);
                    cls.sbIpr();
                }
                
                cls.Dispose();
            }
            catch (Exception ex)
            {
                ObjComportamiento.showMsgError(ex.Message);
            }
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
            //Double desc = 0;

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

            if (this.ventas.nidnid == "") this.ventas.nidnid = "9999999999999";
            if (this.ventas.nidnbr == "") this.ventas.nidnbr = "CLIENTE FINAL";

            // Grabar regalo
            Respuesta res = this.ventas.grabarRegaloFP();
            ObjComportamiento.showMuestraMensaje(res);
            if (res.resultado)
            {
                btnCancelar_Click(btnCancelar, new RoutedEventArgs());
            }
        }

        private void btnToPay_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Pago de Factura";
            win.Width = 1010;
            win.Height = 550;

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
            Double valor = 0;
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

            this.pagos = pagos;
            this.ventas.ventrcb = valor;
            this.ventas.ventvto = valor - total;
        }

        private void btnDelFact_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Listado de Motivos para eliminar una factura";
            win.Width = 500;
            win.Height = 400;

            ctlMotivosAnula crea = new ctlMotivosAnula();
            crea.secuencial = this.ventas.ventscl.ToString().PadLeft(10, '0');
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

        private void Eliminar_Factura(String ventScl, String ventObv, string numFact)
        {
            if (!ventScl.Equals(""))
            {
                Respuesta res = this.ventas.eliminaFactura(ventScl, ventObv, numFact);
                ObjComportamiento.showMuestraMensaje(res);

                if (res.resultado)
                {
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

        private void btnDescto_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Listado de Descuentos a aplicar";
            win.Width = 500;
            win.Height = 400;

            //ctlMotivos crea = new ctlMotivos();

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

        private void Crea_aceptaDescuento(String secuencial, String mensaje)
        {
            Double desc = 0;
            Double subTotal = 0;
            Double descuento = 0;

            if (!mensaje.Equals("SIN DESCUENTO"))
            {
                desc = General.StringToDouble(mensaje.Substring(mensaje.Length - 4, 3)) / 100;
            }

            if (txtSubTot.Text != "")
                subTotal = General.StringToDouble(txtSubTot.Text.Replace(".", ","));

            descuento = subTotal * desc;

            txtDscto.Text = General.StringFormatMoney(descuento);

            this.calcularTotalMostrar();
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
            ActivaCrearPedido(false);
            //cambiaTipoPedido.IsEnabled = false;
            mesero.Content = "";
            mesero.Tag = "";
        }

        private void cambiaTipoPedido_Click(object sender, RoutedEventArgs e)
        {

        }

        /*--------------------------------------------------------------------------------*/
        private void setTipoPedido(string tipo)
        {
            if (tipo.Equals("L"))
            {
                //imgTipoPedd.Source = new BitmapImage(new Uri("pack://application:,,,/images/toserv.png"));
                //lblTipo.Content = "Local";
                //this.pedido.tipPedd = tipo;
                this.ventas.venttip = tipo;
            }
            else if (tipo.Equals("D"))
            {
                //imgTipoPedd.Source = new BitmapImage(new Uri("pack://application:,,,/images/delivery.png"));
                //lblTipo.Content = "Domicilio";
                this.ventas.venttip = tipo;
            }
            else if (tipo.Equals("C"))
            {
                //imgTipoPedd.Source = new BitmapImage(new Uri("pack://application:,,,/images/tocarry.png"));
                //lblTipo.Content = "Llevar";
                //this.pedido.tipPedd = tipo;
                this.ventas.venttip = tipo;
            }
            else if (tipo.Equals("E"))
            {
                //imgTipoPedd.Source = new BitmapImage(new Uri("pack://application:,,,/images/uber.png"));
                //lblTipo.Content = "Carrier";
                //this.pedido.tipPedd = tipo;
                this.ventas.venttip = tipo;
            }
            else if (tipo.Equals("R"))
            {
                //imgTipoPedd.Source = new BitmapImage(new Uri("pack://application:,,,/images/clock-color.png"));
                //lblTipo.Content = "Retirar";
                //this.pedido.tipPedd = tipo;
                this.ventas.venttip = tipo;
            }
        }

        public void creaGrupos()
        {
            pedidoContainer.Children.Clear();

            pedidoContainer.Children.Add(gProd);

            Grupos grupos = new Grupos(General.connectionString);

            DataTable dt = grupos.listar();

            gProd.clearGroup();
            gProd.clearProduct();

            gProd.SetTamanioGroups = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal ?
                ctlGrpProd.TamanioGroups.Normal : ctlGrpProd.TamanioGroups.Small;
            gProd.Organizacion = General.ORGANIZA_ITEMS_PEDIDO;

            foreach (DataRow dr in dt.Rows)
            {
                ctlButtonNI btn = new ctlButtonNI();

                if (General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal)
                {
                    btn.Alto = 60;
                    btn.Ancho = 100;
                    btn.SetTamanioButton = ctlButtonNI.tamanioButton.Normal;
                }
                else
                {
                    btn.Alto = 50;
                    btn.Ancho = 80;
                    btn.SetTamanioButton = ctlButtonNI.tamanioButton.Small;
                }

                btn.ShowCodigo = false;
                btn.CentrarTexto = true;

                btn.Codigo = "G";
                btn.CodigoAlterno = dr["codigo"].ToString();
                btn.Titulo = dr["descripcion"].ToString();

                btn.TipoBotones = "ST";

                btn._ctlButtonClick += new ctlButtonNI.ctlButton_Click(ctlButton_Click);

                gProd.addToGroup(btn);
            }
        }

        private void sbActivaBtnPedd(Boolean accion)
        {
            btnFacturar.IsEnabled = accion;
            btnToPay.IsEnabled = accion;
            btnDescto.IsEnabled = accion;

            if (this.PUEDE_DAR_DESCUENTO && accion) btnDescto.IsEnabled = accion;
            //if (this.PUEDE_DAR_REGALO && accion) btngift.IsEnabled = accion;
        }

        public void calcularTotalMostrar()
        {
            // Calculamos el total
            double lDescuento = 0;
            double ltotal = 0;

            // Calculamos el total
            List<ProdPedd> items = new List<ProdPedd>();
            items.AddRange(dgvPedd.Items.OfType<ProdPedd>());

            var StotNoGravaIva = items.Where(x => x.gravaIva == false).Select(c => c.precio * c.cantidad).Sum();
            var StotConIva = items.Where(x => x.gravaIva == true && x.usaIva == true).Select(c => c.precio * c.cantidad).Sum();
            var StotGravaIva = items.Where(x => x.gravaIva == true && x.usaIva == false).Select(c => c.precio * c.cantidad).Sum();

            double STotConIva = StotConIva + StotGravaIva;
            double Subtotal = StotNoGravaIva + STotConIva;

            var PctjeIva = items.Where(x => x.PrctjIva > 0).Select(c => c.PrctjIva).FirstOrDefault();
            var iva = STotConIva * (PctjeIva / 100);
            //var iva = STotConIva * (General.IVA / 100);

            lDescuento = General.StringToDouble(txtDscto.Text.Replace(".", ","));

            ltotal = Subtotal + iva - lDescuento;

            txtSubTot.Text = String.Format("{0:0.00}", STotConIva);
            txtSubTot0.Text = String.Format("{0:0.00}", StotNoGravaIva);
            txtIva.Text = String.Format("{0:0.00}", iva);
            txtTotal.Text = String.Format("{0:0.00}", ltotal);

            this.ventas.ventsbt = STotConIva;
            this.ventas.ventsbn = StotNoGravaIva;
            this.ventas.ventdsc = lDescuento;
            this.ventas.ventiva = iva;
            this.ventas.venttot = ltotal;
        }

        private void GeneraCarriers()
        {
            if (this.ventas.venttip == "E")
            {
                pedidoContainer.Children.Clear();
                Carriers carriers = new Carriers(General.connectionString);
                List<mCarrier> dt = carriers.buscar("", "A");

                for (int i = 0; i < dt.Count; i++)
                {
                    ctlButton btn = new ctlButton();

                    btn.Codigo = "CR";
                    string icono = dt[i].icono;
                    btn.CodigoAlterno = dt[i].codigo;
                    btn.Titulo = dt[i].carrier;

                    if (dt[i].imagen != null)
                    {
                        btn.Icono = dt[i].imagen;
                    }
                    else
                    {
                        switch (icono)
                        {
                            case "CBF":
                                btn.Imagen = "pack://application:,,,/images/cabify.png";
                                break;
                            case "GLV":
                                btn.Imagen = "pack://application:,,,/images/glovo.png";
                                break;
                            case "RPI":
                                btn.Imagen = "pack://application:,,,/images/rappi.png";
                                break;
                            case "SES":
                                btn.Imagen = "pack://application:,,,/images/super-easy.png";
                                break;
                            case "TPT":
                                btn.Imagen = "pack://application:,,,/images/tipti.png";
                                break;
                            case "UBR":
                                btn.Imagen = "pack://application:,,,/images/uber-eats.png";
                                break;
                            case "PEY":
                                btn.Imagen = "pack://application:,,,/images/pedidosya.png";
                                break;
                            case "DID":
                                btn.Imagen = "pack://application:,,,/images/DiDiFood.png";
                                break;
                        }
                    }

                    btn._ctlButtonClick += new ctlButton.ctlButton_Click(ctlButton_Click);

                    pedidoContainer.Children.Add(btn);
                }
            }
        }

        public void Genera_Carrier(String cliNid, String cliObv)
        {
            this.ventas.ventccd = cliNid;
            this.ventas.ventcro = cliObv;

            GeneraCarriers();
        }

        public void Genera_Pedido(String cliNid, String cliNbr, String cliDir, String cliTlf, String cliRef)
        {
            this.ventas.cliDir = cliDir;
            this.ventas.cliNbr = cliNbr;
            this.ventas.cliNid = cliNid;
            this.ventas.cliRef = cliRef;
            this.ventas.cliTlf = cliTlf;

            if (this.ventas.venttip == "D")
            {
                creaGrupos();
                if (General.PERMITE_USO_BARCODE)
                {
                    barcode.Visibility = Visibility.Visible;
                    barcode.Focus();
                }
            }

            GeneraCarriers();
        }

        public void MuestraOpcionesCarrier()
        {
            winGeneral win = new winGeneral();
            win.Title = "Datos del Carrier";
            win.Width = 620;
            win.Height = 520;

            ctlCarrierData ctl = new ctlCarrierData();
            ctl.contenedor = win;
            ctl.parent = this;
            ctl.pedidoCarrier += new ctlCarrierData.GeneraPedidoCarrier(Genera_Carrier);
            win.Contenido.Children.Add(ctl);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void MuestraBarcodeYBuscador()
        {
            if (General.PERMITE_BUSCAR_ITEMS && General.PERMITE_USO_BARCODE)
            {
                buscaItem.Margin = new Thickness(3, 3, 98, 3);
                barcode.Margin = new Thickness(3, 3, 300, 3);
            }
            else
            {
                if (General.PERMITE_USO_BARCODE)
                {
                    barcode.Margin = new Thickness(3, 3, 3, 3);
                }

                if (General.PERMITE_BUSCAR_ITEMS)
                {
                    buscaItem.Margin = new Thickness(3, 3, 98, 3);
                }
            }
        }

        public void ctlButton_Click(object sender, RoutedEventArgs e, string tipo)
        {
            try
            {
                setTipoPedido(tipo);

                switch (tipo)
                {
                    case "L":
                        // Cuando selecciona un tipo de pedido LOCAL
                        this.ventas.ventmsa = "";
                        if (General.MUESTRA_MESAS_MARKET)
                        {
                            creaMesas();
                        }
                        else
                        {
                            creaGrupos();
                        }

                        if (General.PERMITE_USO_BARCODE)
                        {
                            barcode.Visibility = Visibility.Visible;
                            barcode.Focus();
                        }
                        break;
                    case "M":
                        // Cuando selecciona una mesa
                        this.ventas.ventmsa = ((Button)sender).Tag.ToString();
                        //this.pedido.msaPedd = lblMesa.Content.ToString();
                        gProd.setTituloGrupo("Grupo");
                        gProd.setTituloProducto("Productos");
                        creaGrupos();
                        if (General.PERMITE_USO_BARCODE)
                        {
                            barcode.Visibility = Visibility.Visible;
                            barcode.Focus();
                        }
                        if (General.PERMITE_BUSCAR_ITEMS)
                        {
                            buscaItem.Visibility = Visibility.Visible;
                            btnBscItem.Visibility = Visibility.Visible;
                            btnBscCls.Visibility = Visibility.Visible;
                        }
                        break;
                    case "T":
                        // Cuando se selecciona un grupo de mesas (localidad)
                        int factor = 1;
                        int cantidad = 0;
                        string codAlterno = ((Button)sender).Tag.ToString();
                        gProd.clearProduct();
                        if (codAlterno == "PR") { factor = 100; cantidad = General.CANTIDAD_MESAS; }
                        if (codAlterno == "AL") { factor = 200; cantidad = General.CANTIDAD_MESAS_ALTERNA; }
                        if (codAlterno == "EX") { factor = 300; cantidad = General.CANTIDAD_MESAS_EXTERIOR; }
                        if (codAlterno == "TR") { factor = 400; cantidad = General.CANTIDAD_MESAS_TERRAZA; }
                        if (codAlterno == "SP") { factor = 500; cantidad = General.CANTIDAD_MESAS_2DO_PISO; }

                        for (int i = 1; i <= cantidad; i++)
                        {
                            ctlButton btn = new ctlButton();

                            btn.SetTamanioButton = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal ?
                                    ctlButton.TamanioButton.Normal : ctlButton.TamanioButton.Small;

                            btn.Codigo = "M";
                            btn.CodigoAlterno = (i + factor).ToString().PadLeft(3, '0');
                            btn.Titulo = (i + factor).ToString().PadLeft(3, '0');
                            btn.Imagen = "pack://application:,,,/images/table.png";

                            btn._ctlButtonClick += new ctlButton.ctlButton_Click(ctlButton_Click);

                            //pedidoContainer.Children.Add(btn);
                            gProd.addToProduct(btn);
                        }
                        break;
                    case "D":
                        // Cuando selecciona un tipo de pedido a DOMICILIO
                        MuestraOpcionesDomicilio(false);
                        break;
                    case "C":
                        // Cuando selecciona un tipo de pedido para LLEVAR
                        this.ventas.ventmsa = "";
                        creaGrupos();
                        if (General.PERMITE_USO_BARCODE)
                        {
                            barcode.Visibility = Visibility.Visible;
                            barcode.Focus();
                        }
                        if (General.PERMITE_BUSCAR_ITEMS)
                        {
                            buscaItem.Visibility = Visibility.Visible;
                            btnBscItem.Visibility = Visibility.Visible;
                            btnBscCls.Visibility = Visibility.Visible;
                        }

                        break;
                    case "E":
                        // Cuando selecciona un tipo de pedido por carrier
                        this.ventas.vtaCliDo = "";
                        this.ventas.ventcrr = "";
                        this.ventas.vtaCliDoObv = "";
                        this.ventas.vtaCliDoNom = "";

                        MuestraOpcionesCarrier();
                        break;
                    case "R":
                        // Define tipo de pedido para retiros
                        this.ventas.ventmsa = "";
                        creaGrupos();
                        //creaGrupos();
                        if (General.PERMITE_USO_BARCODE)
                        {
                            barcode.Visibility = Visibility.Visible;
                            barcode.Focus();
                        }
                        if (General.PERMITE_BUSCAR_ITEMS)
                        {
                            buscaItem.Visibility = Visibility.Visible;
                            btnBscItem.Visibility = Visibility.Visible;
                            btnBscCls.Visibility = Visibility.Visible;
                        }
                        break;
                    case "CR":
                        this.ventas.ventcrr = ((Button)sender).Tag.ToString();
                        this.ventas.vtaCliDoNom = ((TextBlock)((StackPanel)((StackPanel)((Button)sender).Content).Children[1]).Children[1]).Text;
                        this.ventas.ventmsa = "";
                        creaGrupos();

                        if (General.PERMITE_USO_BARCODE)
                        {
                            barcode.Visibility = Visibility.Visible;
                            barcode.Focus();
                        }
                        if (General.PERMITE_BUSCAR_ITEMS)
                        {
                            buscaItem.Visibility = Visibility.Visible;
                            btnBscItem.Visibility = Visibility.Visible;
                            btnBscCls.Visibility = Visibility.Visible;
                        }
                        break;
                    case "G":
                        // Cuando se selecciona un grupo
                        gProd.clearProduct();

                        MuestraBarcodeYBuscador();

                        Items items = new Items(General.connectionString);
                        DataTable dtp = null;

                        //DataTable dtp = items.getItemsByGroup(((Button)sender).Tag.ToString(), "M");

                        if (sender.GetType() == typeof(Button))
                        {
                            dtp = items.getItemsByGroup(((Button)sender).Tag.ToString(), "M");
                        }

                        if (sender.GetType() == typeof(ctlButtonNI))
                        {
                            dtp = items.getItemsByGroup(((ctlButtonNI)sender).CodigoAlterno.ToString(), "M");
                        }

                        foreach (DataRow dr in dtp.Rows)
                        {
                            ctlButtonNI btn = new ctlButtonNI();

                            if (General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal)
                            {
                                btn.Alto = 60;
                                btn.Ancho = 120;
                                btn.SetTamanioButton = ctlButtonNI.tamanioButton.Normal;
                            }
                            else
                            {
                                btn.Alto = 50;
                                btn.Ancho = 110;
                                btn.SetTamanioButton = ctlButtonNI.tamanioButton.Small;
                            }

                            btn.ShowCodigo = false;
                            btn.CentrarTexto = true;

                            btn.Codigo = "I";
                            btn.CodigoAlterno = dr["codigo"].ToString();
                            btn.Titulo = dr["descripcion"].ToString();

                            if (btn.Titulo.Contains(buscaItem.Text))
                            {
                                btn.setTituloBold(true);
                                btn.setTituloColor(true);
                            }
                            if (General.PERMITE_BUSCAR_ITEMS)
                            {
                                buscaItem.Visibility = Visibility.Visible;
                                btnBscItem.Visibility = Visibility.Visible;
                                btnBscCls.Visibility = Visibility.Visible;
                            }

                            btn._ctlButtonClick += new ctlButtonNI.ctlButton_Click(ctlButton_Click);
                            btn._ctlButtonDobleClick += new ctlButtonNI.ctlButton_DobleClick(ctlButton_DobleClick);
                            btn._ctlButtonEditClick += new ctlButtonNI.ctlButton_EditClick(ctlButton_EditClick);

                            btn.MuestraEdicion = (bool)dr["ParaCocina"];
                            btn.TipoBotones = General.TIPO_BOTONES;
                            btn.Imagen = dr["imagen"] != DBNull.Value ? (byte[])dr["imagen"] : null;
                            
                            gProd.addToProduct(btn);
                        }
                        break;
                    case "I":
                        // Cuando se selecciona un item o producto
                        this.sbActivaBtnPedd(true);
                        if (General.PERMITE_USO_BARCODE)
                        {
                            barcode.Visibility = Visibility.Visible;
                            barcode.Focus();
                        }

                        if (General.PERMITE_BUSCAR_ITEMS)
                        {
                            buscaItem.Visibility = Visibility.Visible;
                            btnBscItem.Visibility = Visibility.Visible;
                            btnBscCls.Visibility = Visibility.Visible;
                        }
                        MuestraBarcodeYBuscador();

                        Items itemsSel = new Items(General.connectionString);

                        DataTable dti = itemsSel.getById(((Button)sender).Tag.ToString(), this.ventas.ventcrr);

                        // Buscar si existe el item agregado
                        Boolean existe = false;
                        for (int i = 0; i < dgvPedd.Items.Count; i++)
                        {
                            if (((ProdPedd)dgvPedd.Items[i]).codigo.Equals(dti.Rows[0]["codigo"].ToString()))
                            {
                                ((ProdPedd)dgvPedd.Items[i]).cantidad++;
                                dgvPedd.Items.Refresh();
                                existe = true;
                            }
                        }

                        // Buscar si existe el item en control
                        Boolean existeControl = false;
                        for (int i = 0; i < cocina.Count; i++)
                        {
                            if (cocina[i].codigo.Equals(dti.Rows[0]["codigo"].ToString()) || cocina[i].noIngredientes.Count > 0)
                            {
                                cocina[i].cantidad++;
                                existeControl = true;
                                break;
                            }
                        }

                        ProdPedd prodPedd = new ProdPedd
                        {
                            codigo = dti.Rows[0]["codigo"].ToString(),
                            producto = dti.Rows[0]["producto"].ToString(),
                            cantidad = 1,
                            usaIva = (Boolean)dti.Rows[0]["usaIva"],
                            gravaIva = (Boolean)dti.Rows[0]["gravaIva"],
                            PrctjIva = Math.Round(General.StringToDouble(dti.Rows[0]["prctjIva"].ToString()),2),
                            precio = Math.Round(General.StringToDouble(dti.Rows[0]["Precio"].ToString()), 6),
                            iva = Math.Round(Math.Abs(General.StringToDouble(dti.Rows[0]["IVA"].ToString()) - General.StringToDouble(dti.Rows[0]["precio"].ToString())), 6)
                        };

                        ProdPedd prodCocina = null;
                        if ((Boolean)dti.Rows[0]["itemCoc"] == true)
                        {
                            prodCocina = new ProdPedd
                            {
                                codigo = dti.Rows[0]["codigo"].ToString(),
                                producto = dti.Rows[0]["producto"].ToString(),
                                cantidad = 1,
                                usaIva = (Boolean)dti.Rows[0]["usaIva"],
                                gravaIva = (Boolean)dti.Rows[0]["gravaIva"],
                                precio = Math.Round(General.StringToDouble(dti.Rows[0]["Precio"].ToString()), 6),
                                iva = Math.Round(Math.Abs(General.StringToDouble(dti.Rows[0]["IVA"].ToString()) - General.StringToDouble(dti.Rows[0]["precio"].ToString())), 6)
                            };
                        }

                        if (!existe)
                        {
                            dgvPedd.Items.Add(prodPedd);
                        }

                        if (!existeControl && prodCocina != null)
                        {
                            cocina.Add(prodCocina);
                        }

                        // Calculamos el total
                        this.calcularTotalMostrar();

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public void creaMesas()
        {
            // Cuando selecciona un tipo de pedido LOCAL
            // Se van a generar las mesas
            pedidoContainer.Children.Clear();

            if (General.CANTIDAD_MESAS_TERRAZA > 0 || General.CANTIDAD_MESAS_EXTERIOR > 0 ||
               General.CANTIDAD_MESAS_2DO_PISO > 0 || General.CANTIDAD_MESAS_ALTERNA > 0)
            {
                pedidoContainer.Children.Clear();

                pedidoContainer.Children.Add(gProd);

                gProd.clearGroup();
                gProd.clearProduct();

                gProd.Organizacion = General.ORGANIZA_ITEMS_PEDIDO;

                gProd.setTituloGrupo("Salones");
                gProd.setTituloProducto("Mesas");

                creaGrupoMesas("SALÓN PRINCIPAL", "PR");

                if (General.CANTIDAD_MESAS_ALTERNA > 0)
                {
                    creaGrupoMesas("SALÓN ALTERNO", "AL");
                }

                if (General.CANTIDAD_MESAS_EXTERIOR > 0)
                {
                    creaGrupoMesas("EXTERIORES", "EX");
                }

                if (General.CANTIDAD_MESAS_TERRAZA > 0)
                {
                    creaGrupoMesas("TERRAZA", "TR");
                }

                if (General.CANTIDAD_MESAS_2DO_PISO > 0)
                {
                    creaGrupoMesas("SEGUNDO PISO", "SP");
                }
            }
            else
            {
                for (int i = 1; i <= General.CANTIDAD_MESAS; i++)
                {
                    ctlButton btn = new ctlButton();

                    btn.SetTamanioButton = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal ?
                        ctlButton.TamanioButton.Normal : ctlButton.TamanioButton.Small;

                    btn.Codigo = "M";
                    btn.CodigoAlterno = i.ToString().PadLeft(3, '0');
                    btn.Titulo = i.ToString().PadLeft(3, '0');
                    btn.Imagen = "pack://application:,,,/images/table.png";

                    btn._ctlButtonClick += new ctlButton.ctlButton_Click(ctlButton_Click);

                    pedidoContainer.Children.Add(btn);
                }
            }
        }

        private void creaGrupoMesas(string titulo, string codalt)
        {
            ctlButtonNI btn = new ctlButtonNI();

            if (General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal)
            {
                btn.Alto = 60;
                btn.Ancho = 100;
                btn.SetTamanioButton = ctlButtonNI.tamanioButton.Normal;
            }
            else
            {
                btn.Alto = 40;
                btn.Ancho = 70;
                btn.SetTamanioButton = ctlButtonNI.tamanioButton.Small;
            }

            btn.ShowCodigo = false;
            btn.CentrarTexto = true;

            btn.Codigo = "T";
            btn.CodigoAlterno = codalt;
            btn.Titulo = titulo;

            btn.TipoBotones = "ST";

            btn._ctlButtonClick += new ctlButtonNI.ctlButton_Click(ctlButton_Click);

            gProd.addToGroup(btn);
        }

        public void MuestraOpcionesDomicilio(Boolean paraCarrier)
        {
            winGeneral win = new winGeneral();
            win.Title = "Datos de pedido a domicilio";
            win.Width = 750;
            win.Height = 530;

            ctlDelivery ctl = new ctlDelivery();
            ctl.Contenedor = win;
            ctl.parentFact = this;
            ctl.paraCarrier = paraCarrier;
            ctl.pedidoDomicilio += new ctlDelivery.GeneraPedidoDomicilio(Genera_Pedido);
            win.Contenido.Children.Add(ctl);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void LoadEditarItemProducto(object sender)
        {
            // Sección para indicar ingredientes a quitar para el item
            Items itemsSel = new Items(General.connectionString);

            String codigo = ((Button)sender).Tag.ToString();
            String item = ((TextBlock)((StackPanel)((Button)sender).Content).Children[2]).Text.ToString();

            DataTable dt = itemsSel.getRecetaDetalle(((Button)sender).Tag.ToString());

            winGeneral win = new winGeneral();
            win.Title = "Eliminación de ingredientes en el pedido";
            win.Width = 650;
            win.Height = 630;

            ctlQuitaIngredientes ctl = new ctlQuitaIngredientes(codigo, item, dt);
            ctl.contenedor = win;
            ctl.agregaItem += new ctlQuitaIngredientes.AgregaItemSinIngredientes(Quita_Ingredientes);
            win.Contenido.Children.Add(ctl);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        public void Quita_Ingredientes(String codigo, int cantidad, List<mIngredientes> lista, string observacion)
        {
            this.sbActivaBtnPedd(true);
            Items itemsSel = new Items(General.connectionString);

            DataTable dti = itemsSel.getById(codigo, this.ventas.ventcrr);

            // Buscar si existe el item agregado
            Boolean existe = false;
            for (int i = 0; i < dgvPedd.Items.Count; i++)
            {
                if (((ProdPedd)dgvPedd.Items[i]).codigo.Equals(dti.Rows[0]["codigo"].ToString()))
                {
                    ((ProdPedd)dgvPedd.Items[i]).cantidad += cantidad;
                    dgvPedd.Items.Refresh();
                    existe = true;
                }
            }

            // falta
            ProdPedd prodPedd = new ProdPedd
            {
                codigo = dti.Rows[0]["codigo"].ToString(),
                producto = dti.Rows[0]["producto"].ToString(),
                cantidad = cantidad,
                usaIva = (Boolean)dti.Rows[0]["usaIva"],
                gravaIva = (Boolean)dti.Rows[0]["gravaIva"],
                precio = Math.Round(General.StringToDouble(dti.Rows[0]["Precio"].ToString()), 6),
                PrctjIva = Math.Round(General.StringToDouble(dti.Rows[0]["prctjIva"].ToString()), 2),
                iva = Math.Round(Math.Abs(General.StringToDouble(dti.Rows[0]["IVA"].ToString()) - General.StringToDouble(dti.Rows[0]["precio"].ToString())), 6)
            };

            if ((Boolean)dti.Rows[0]["itemCoc"] == true)
            {
                // Eliminación del item en arreglo de cocina
                ProdPedd prodCocina = new ProdPedd
                {
                    codigo = dti.Rows[0]["codigo"].ToString(),
                    producto = dti.Rows[0]["producto"].ToString(),
                    cantidad = cantidad,
                    usaIva = (Boolean)dti.Rows[0]["usaIva"],
                    gravaIva = (Boolean)dti.Rows[0]["gravaIva"],
                    precio = Math.Round(
                                (Boolean)dti.Rows[0]["usaIva"] ?
                                General.StringToDouble(dti.Rows[0]["IVA"].ToString()) :
                                General.StringToDouble(dti.Rows[0]["precio"].ToString()), 6),
                    iva = Math.Round(Math.Abs(General.StringToDouble(dti.Rows[0]["IVA"].ToString()) - General.StringToDouble(dti.Rows[0]["precio"].ToString())), 6),
                    noIngredientes = lista,
                    observacion = observacion
                };

                if (!existe)
                {
                    dgvPedd.Items.Add(prodPedd);
                }

                cocina.Add(prodCocina);

            }

            // Calculamos el total
            this.calcularTotalMostrar();
        }

        public void ctlButton_DobleClick(object sender, MouseButtonEventArgs e, string tipo)
        {
            LoadEditarItemProducto(sender);
        }

        public void ctlButton_EditClick(object sender, RoutedEventArgs e)
        {
            LoadEditarItemProducto(sender);
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            gProd.altura = dgvPedd.ActualHeight + 175;
            gProd.ancho = General.getCurrentScreenWidth((Window)((DockPanel)((Grid)this.Parent).Parent).Parent) - 485;
            
            gProd.setHeight();
            gProd.setWidth();

            if (barcode.Visibility == Visibility.Visible)
            {
                //barcode.Focus();
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            sbLimpiar();
            btnCrear.IsEnabled = true;
            btnCancelar.IsEnabled = false;
            btnDelFact.IsEnabled = false;
            btnFacturar.IsEnabled = true;
            btnCajero.IsEnabled = true;
            btnBscPedd.IsEnabled = true;
            btnAnlFact.IsEnabled = false;
            //btngift.IsEnabled = false;
            btnDescto.IsEnabled = false;
            btnToPay.IsEnabled = false;
            this.sbActivaBtnPedd(false);
            cocina.Clear();
            this.ventas.ventscl = -1;
            PorBusqueda = false;

            dgvPedd.Columns[0].Visibility = Visibility.Visible;
        }

        private void btnBscItem_Click(object sender, RoutedEventArgs e)
        {
            if (buscaItem.Text.Trim().Length > 0)
            {
                Items itemsSel = new Items(General.connectionString);

                string orden = "CODIGO";
                switch (General.FORMA_ORDEN_ITEMS_GRPS)
                {
                    case "C":
                        orden = "CODIGO";
                        break;
                    case "A":
                        orden = "ALIAS";
                        break;
                    case "N":
                        orden = "PRODUCTO";
                        break;
                }

                var result = itemsSel.buscarPorNombre(buscaItem.Text, orden);

                gProd.resetFilter();

                // Iniciar proceso de filtrado.
                var grupos = result.Select(o => o.grupo).Distinct<string>();

                ctlButtonNI btnFirst = null;
                int i = 0;

                foreach (var itmsGrp in gProd.getGrupoContainer())
                {
                    var btn = (ctlButtonNI)itmsGrp;

                    foreach (var grp in grupos)
                    {
                        if (btn.Titulo == grp)
                        {
                            btn.setTituloBold(true);
                            btn.setTituloColor(true);

                            if (i == 0)
                            {
                                btnFirst = btn;
                            }

                            i++;
                        }
                    }
                }

                if (btnFirst != null)
                {
                    if (General.CLICK_GRUPO_BUSCAR_ITEMS)
                    {
                        ctlButton_Click(btnFirst, new RoutedEventArgs(), "G");
                    }
                }
                else
                {
                    //gProd.clearProductFilter();
                    gProd.clearProduct();
                }
            }
            if (General.PERMITE_USO_BARCODE)
            {
                barcode.Visibility = Visibility.Visible;
                barcode.Focus();
            }
        }

        private void btnBscCls_Click(object sender, RoutedEventArgs e)
        {
            gProd.resetFilter();
            barcode.Focus();
            buscaItem.Text = "";
            gProd.clearProduct();
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar llamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: elimine el estado administrado (objetos administrados).
                }

                // TODO: libere los recursos no administrados (objetos no administrados) y reemplace el siguiente finalizador.
                // TODO: configure los campos grandes en nulos.

                disposedValue = true;
            }
        }

        // TODO: reemplace un finalizador solo si el anterior Dispose(bool disposing) tiene código para liberar los recursos no administrados.
        // ~ctlFacturacion() {
        //   // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
        //   Dispose(false);
        // }

        // Este código se agrega para implementar correctamente el patrón descartable.
        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
            Dispose(true);
            // TODO: quite la marca de comentario de la siguiente línea si el finalizador se ha reemplazado antes.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}