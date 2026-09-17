using Entidades;
using Entidades.modelos;
using LibControls.buttons;
using LibControls.contenedor;
using LibEncrypt;
using AccessDatos.Model;
using LibKeys;
using LibPrint;
using LibPrint.dataset;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TreskelRest.cls;
using LibHttpClient;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlPedidos.xaml
    /// </summary>
    public partial class ctlPedidos : UserControl, IDisposable
    {
        private DataTable dtTipPedd = new datasets.dsPedidos.tiposDataTable();
        private ctlGrpProd gProd = new ctlGrpProd();
        private Pedido pedido = new Pedido(General.connectionString);
        private List<ProdPedd> cocina = new List<ProdPedd>();
        private List<ProdPedd> bar = new List<ProdPedd>();

        private NumKeyboard nKeyboard = new NumKeyboard();

        private bool PUEDE_ELIMINAR_PEDIDO = false;
        private string _barcode = "";
        private bool ConfirmacionAceptada = false;

        private double valorServicio = 0;

        private void sbLimpiar()
        {
            txtTotal.Content = "0,00";
            pedidoContainer.Children.Clear();
            dgvPedd.Items.Clear();
            txtPedido.Content = "";
            lblMesa.Content = "";
            imgTipoPedd.Source = new BitmapImage(new Uri("pack://application:,,,/images/toserv.png"));
            lblTipo.Content = "Local";
            cambiaTipoPedido.IsEnabled = false;

            pedido.cliDir = "";
            pedido.cliNbr = "";
            pedido.cliNid = "";
            pedido.cliTlf = "";
            pedido.cliRef = "";
            pedido.msaPedd = "";
            pedido.sclPedd = -1;
            pedido.ltaPedd = "A";
            pedido.tipPedd = "L";

            pedido.codCrri = "0000";

            pedido.detPedd.Clear();
            SetTamanioMenuConfig();
            barcode.Visibility = Visibility.Hidden;
            buscaItem.Visibility = Visibility.Hidden;
            btnBscCls.Visibility = Visibility.Hidden;
            btnBscItem.Visibility = Visibility.Hidden;

            ScrollPedidos.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void SetTamanioMenuConfig()
        {
            if (General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal)
            {
                setTamanioBotones(btnSalir, 70, 14, 30);
                setTamanioBotones(btnCrear, 70, 14, 30);
                setTamanioBotones(btnModificar, 70, 14, 30);
                setTamanioBotones(btnSave, 70, 14, 30);
                setTamanioBotones(btnEliminar, 70, 14, 30);
                setTamanioBotones(btnCocinar, 70, 14, 30);
                setTamanioBotones(btnCancelar, 70, 14, 30);
                setTamanioBotones(btnToBill, 70, 14, 30);
                setTamanioBotones(btnUndoBill, 70, 14, 30);
                setTamanioBotones(btnImpFact, 70, 14, 30);
                setTamanioBotones(btnMesero, 70, 14, 30);
                Mesero.FontSize = 14;
            }
            else
            {
                setTamanioBotones(btnSalir, 50, 10, 25);
                setTamanioBotones(btnCrear, 50, 10, 25);
                setTamanioBotones(btnModificar, 50, 10, 25);
                setTamanioBotones(btnSave, 50, 10, 25);
                setTamanioBotones(btnEliminar, 50, 10, 25);
                setTamanioBotones(btnCocinar, 50, 10, 25);
                setTamanioBotones(btnCancelar, 50, 10, 25);
                setTamanioBotones(btnToBill, 50, 10, 25);
                setTamanioBotones(btnUndoBill, 50, 10, 25);
                setTamanioBotones(btnImpFact, 50, 10, 25);
                setTamanioBotones(btnMesero, 50, 10, 25);
                Mesero.FontSize = 10;
            }
        }

        private void setTamanioBotones(object obj, int size, int fontSize, int imgSize)
        {
            if (obj is ButtonBase btn)
            {
                btn.Height = size;
                btn.Width = size;

                if (btn.Content is StackPanel panel && panel.Children.Count >= 2)
                {
                    if (panel.Children[0] is Image imagen)
                    {
                        imagen.Width = imgSize;
                        imagen.Height = imgSize;
                    }

                    if (panel.Children[1] is Label etiqueta)
                    {
                        etiqueta.FontSize = fontSize;
                    }
                }
            }
        }

        private void sbActivaBtnPedd(Boolean accion)
        {
            btnSave.IsEnabled = accion;
            btnChgMesa.IsEnabled = accion;
            btnCocinar.IsEnabled = false;

            if (!txtPedido.Content.ToString().Equals(""))
            {
                btnCocinar.IsEnabled = accion;
            }         
        }

        private void sbActivaOpcionesPedido(Boolean activo)
        {
            btnCancelar.IsEnabled = activo;
            btnModificar.IsEnabled = !activo;
            btnToBill.IsEnabled = !activo;
            btnUndoBill.IsEnabled = !activo;
        }        

        private void ActivaCrearPedido(Boolean valor)
        {
            btnCrear.IsEnabled = valor;
            btnModificar.IsEnabled = valor;
            btnToBill.IsEnabled = valor;
            btnUndoBill.IsEnabled = valor;
        }

        public ctlPedidos()
        {
            InitializeComponent();

            General.TipoVentana = "P";

            var tiposPedido = new[]
            {
                new { Codigo = "L", Descripcion = "LOCAL" },
                new { Codigo = "D", Descripcion = "DOMICILIO" },
                new { Codigo = "C", Descripcion = "LLEVAR" },
                new { Codigo = "E", Descripcion = "CARRIER" },
                new { Codigo = "R", Descripcion = "RETIRAR" }
            };

            foreach (var tipo in tiposPedido)
            {
                DataRow dr = dtTipPedd.NewRow();
                dr["codigo"] = tipo.Codigo;
                dr["descripcion"] = tipo.Descripcion;
                dtTipPedd.Rows.Add(dr);
            }

            sbLimpiar();
            btnImpFact.IsEnabled = false;
            ActivaCrearPedido(false);

            nKeyboard.retornaTexto += TextoNumerico_Retorna;
            KeyUp += ctlPedidos_KeyUp;

            barcode.Visibility = Visibility.Hidden;
            buscaItem.Visibility = Visibility.Hidden;
            btnBscCls.Visibility = Visibility.Hidden;
            btnBscItem.Visibility = Visibility.Hidden;
        }


        private void ctlPedidos_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            _barcode += e.Key;

            if (_barcode.EndsWith("J"))
            {
                sbActivaBtnPedd(true);

                Items itemsSel = new Items(General.connectionString);
                DataTable dti = itemsSel.getById(barcode.Text, pedido.codCrri);

                if (dti == null || dti.Rows.Count == 0)
                {
                    ObjComportamiento.showMsgError("Producto no encontrado");
                    _barcode = "";
                    barcode.Text = "";
                    return;
                }

                DataRow filaItem = dti.Rows[0];
                double precioOriginal = General.StringToDouble(filaItem["precio"].ToString());

                if (!(precioOriginal > 0))
                {
                    ObjComportamiento.showMsgError("Producto no encontrado");
                    _barcode = "";
                    barcode.Text = "";
                    return;
                }

                string codigoArticulo = filaItem["codigo"].ToString();

                var itemExistente = dgvPedd.Items.OfType<ProdPedd>().FirstOrDefault(i => i.codigo == codigoArticulo);
                bool existe = itemExistente != null;

                if (existe)
                {
                    itemExistente.cantidad++;
                    dgvPedd.Items.Refresh();
                }

                var itemCocina = cocina.FirstOrDefault(c => c.codigo == codigoArticulo || c.noIngredientes.Count > 0);
                bool existeControl = itemCocina != null;

                if (existeControl)
                {
                    itemCocina.cantidad++;
                }

                double precioUnidad = Math.Round(precioOriginal, 6);
                double ivaUnidad = Math.Round(Math.Abs(General.StringToDouble(filaItem["IVA"].ToString()) - precioOriginal), 6);
                bool usaIva = (bool)filaItem["usaIva"];
                bool gravaIva = (bool)filaItem["gravaIva"];
                string nombreProducto = filaItem["producto"].ToString();

                ProdPedd prodPedd = existe ? null : new ProdPedd
                {
                    codigo = codigoArticulo,
                    producto = nombreProducto,
                    cantidad = 1,
                    usaIva = usaIva,
                    gravaIva = gravaIva,
                    precio = precioUnidad,
                    iva = ivaUnidad
                };

                ProdPedd prodCocina = null;
                if ((bool)filaItem["itemCoc"])
                {
                    prodCocina = existeControl ? null : new ProdPedd
                    {
                        codigo = codigoArticulo,
                        producto = nombreProducto,
                        cantidad = 1,
                        usaIva = usaIva,
                        gravaIva = gravaIva,
                        precio = precioUnidad,
                        iva = ivaUnidad
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

                calcularTotalMostrar();

                _barcode = "";
                barcode.Text = "";
            }
        }

        private void TextoNumerico_Retorna(Object txt, String texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                ObjComportamiento.showMsgError("Debe ingresar una clave para poder realizar el pedido");
                btnMesero.IsChecked = false;
                return;
            }

            Usuarios usuario = new Usuarios(General.connectionString);
            string respuestaCodigo = usuario.getCodMesero(texto, General.USA_CLAVE_NUMERICA);

            if (string.IsNullOrEmpty(respuestaCodigo))
            {
                ObjComportamiento.showMsgError("Clave ingresada no válida");
                return;
            }

            var rcodigo = respuestaCodigo.Split(';', 3);
            if (rcodigo.Length < 3 || string.IsNullOrEmpty(rcodigo[0]))
            {
                ObjComportamiento.showMsgError("Clave ingresada incorrecta");
                btnMesero.IsChecked = false;
                return;
            }

            string codigo = rcodigo[0];

            var permisos = usuario.getPermisoUsuario(codigo);
            PUEDE_ELIMINAR_PEDIDO = permisos?.FirstOrDefault(x => x.tag == "DPED")?.estado ?? false;

            pedido.codEmpl = codigo;
            pedido.celEmpl = rcodigo[1];
            pedido.empdNbr = rcodigo[2];

            UsrEmpleado usr = usuario.getEmpleadoMesero(codigo);
            if (usr?.usrusr == null)
            {
                Mesero.Text = "NID";
                Mesero.Tag = "NID";
            }
            else
            {
                Mesero.Text = usr.usrusr.ToLowerInvariant();
                Mesero.Tag = usr.usrnbr.ToUpperInvariant();
            }

            ActivaCrearPedido(true);
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            sbLimpiar();
            sbActivaOpcionesPedido(true);
            btnCrear.IsEnabled = false;
            cocina.Clear();
            bar.Clear();

            pedidoContainer.Visibility = Visibility.Collapsed;
            pedidoContainer.Children.Clear();
            pedidoContainer.Columns = 6;

            var tamanioBoton = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal
                ? ctlButton.TamanioButton.Normal
                : ctlButton.TamanioButton.Small;

            foreach (DataRow dr in this.dtTipPedd.Rows)
            {
                string codigo = dr["codigo"].ToString();

                ctlButton btn = new ctlButton
                {
                    SetTamanioButton = tamanioBoton,
                    Codigo = codigo,
                    Titulo = dr["descripcion"].ToString(),
                    Imagen = General.sbDefineImagen(codigo)
                };

                btn._ctlButtonClick += ctlButton_Click;

                pedidoContainer.Children.Add(btn);
            }

            pedidoContainer.Visibility = Visibility.Visible;
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            sbLimpiar();

            btnCrear.IsEnabled = false;
            sbActivaOpcionesPedido(true);

            pedidoContainer.Children.Clear();
            cocina.Clear();
            bar.Clear();

            DataTable dt = new Pedido(General.connectionString).getPedidosVigentes();

            var tamanioBoton = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal
                ? ctlButton.TamanioButton.Normal
                : ctlButton.TamanioButton.Small;

            foreach (DataRow dr in dt.Rows)
            {
                string secuencial = dr["peddscl"].ToString();

                ctlButton btn = new ctlButton
                {
                    Codigo = "P",
                    CodigoAlterno = secuencial,
                    Titulo = $"{dr["peddmsa"]} {dr["peddlta"]} : {secuencial}",
                    Imagen = General.sbDefineImagen(dr["peddtip"].ToString()),
                    SetTamanioButton = tamanioBoton
                };

                btn._ctlButtonClick += ctlButton_Click;

                pedidoContainer.Children.Add(btn);
            }

            pedidoContainer.Columns = 1;
            gProd.SetColumnasGrid = 11;
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            cocina.Clear();
            bar.Clear();

            winGeneral win = new winGeneral
            {
                Title = "Definir motivo de eliminación de pedido",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            ctlMotivos crea = new ctlMotivos
            {
                secuencial = txtPedido.Content.ToString(),
                contenedor = win,
                tipo = "M",
                data = pedido.getMotivosEliminacion()
            };

            crea.aceptaMotivo += Crea_aceptaMotivo;
            crea.generar();

            win.Contenido.Children.Add(crea);
            win.ShowDialog();
        }

        private void muestraIngresoCoord()
        {
            string secuencialPedido = this.pedido.sclPedd.ToString().PadLeft(10, '0');
            string codigoEmpleado = this.pedido.codEmpl;

            // Generar la coordenada
            Coordenada newCoord = new Coordenada(General.connectionString);
            mCoordenada valor = newCoord.Generar("N", 0, "E", "P", secuencialPedido, codigoEmpleado, "");

            ctlIngresaCoord wPin = new ctlIngresaCoord
            {
                Width = 320,
                Height = 450,
                SetearOrigen = "P",
                SetearTipo = "E",
                codigoEmpleado = codigoEmpleado,
                SetearCoordenada = valor,
                SetearCoordSecuencial = valor.coordscl,
                SetearCodigoOrigen = secuencialPedido,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            wPin.aceptaCoord += acepta_confirmacion;

            wPin.ShowDialog();
        }

        private void Crea_aceptaMotivo(String secuencial, String mensaje)
        {
            if (this.pedido == null) return;

            string modoValidacion = General.MODO_VALIDACION_ANULA_ELIMINA;

            if (modoValidacion == "P")
            {
                if (PcNetworkInfo.InternetConnectionStatus())
                {
                    if (string.IsNullOrEmpty(pedido.celEmpl))
                    {
                        ObjComportamiento.showMsgError("El empleado no tiene asignado un número de celular, VERIFIQUE Y ASIGNE");
                        btnCancelar_Click(btnCancelar, new RoutedEventArgs());
                        return;
                    }

                    ConfirmaPin cls = new ConfirmaPin(General.connectionString)
                    {
                        msgpin = General.generaPin(),
                        msgatv = "A",
                        msgtip = "E",
                        tiporg = "P",
                        usucod = this.pedido.codEmpl,
                        secorg = this.pedido.sclPedd.ToString().PadLeft(10, '0')
                    };

                    cls.grabar("N");

                    General.enviarWhatsapp($"Ingrese el PIN #{cls.msgpin} Para confirmar la acción de eliminacion del Pedido.", pedido.celEmpl);
                    HttpClienteAPI client = new HttpClienteAPI(); 

                    ctlIngresaPIN wPin = new ctlIngresaPIN
                    {
                        Width = 320,
                        Height = 450,
                        SetearPin = cls.msgpin,
                        SetearPinSecuencial = cls.msgscl,
                        codigo = cls.secorg,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    wPin.aceptaPin += acepta_confirmacion;
                    wPin.ShowDialog();
                }
                else
                {
                    muestraIngresoCoord();
                }
            }
            else if (modoValidacion == "G")
            {
                muestraIngresoCoord();
            }
            else if (modoValidacion == "H")
            {
                // TODO: Desarrollar el uso de huella digital
            }
            else if (modoValidacion == "C")
            {
                // TODO: Desarrollar el uso de tarjeta contactless
            }

            if (modoValidacion != "N" && !ConfirmacionAceptada)
            {
                ObjComportamiento.showMsgError("La Eliminación del pedido no pudo ser realizada, porque la CONFIRMACIÓN no fue VÁLIDA");
                return;
            }

            Respuesta respuesta = this.pedido.EliminacionPedido(secuencial, mensaje);

            if (respuesta.resultado)
            {
                mClientAuth clientAuth = new mClientAuth
                {
                    usuario = General.USUARIO_WS,
                    clave = General.CLAVE_WS,
                    empresaId = General.EMPRESA_GUID,
                    ruc = General.RUC_EMPRESA,
                    token = General.TOKEN_MAIL
                };

                string msgAuth = JsonConvert.SerializeObject(clientAuth);

                General.enviarCorreo(
                    General.NOMBRE_USUARIO,
                    "Se ha eliminado una Factura",
                    mensaje,
                    usaMailTipo.Pedido,
                    usaTipoAccion.Eliminacion,
                    secuencial,
                    msgAuth,
                    pedido.empdNbr);

                sbLimpiar();
                btnCancelar_Click(btnCancelar, new RoutedEventArgs());
            }

            ObjComportamiento.showMuestraMensaje(respuesta);
        }

        private void acepta_confirmacion(bool acepta)
        {
            ConfirmacionAceptada = acepta;
        }

        private void btnToBill_Click(object sender, RoutedEventArgs e)
        {
            sbLimpiar();

            btnCrear.IsEnabled = false;
            btnModificar.IsEnabled = false;
            btnCancelar.IsEnabled = true;
            btnToBill.IsEnabled = false;
            btnUndoBill.IsEnabled = false;

            pedidoContainer.Children.Clear();
            
            cocina.Clear();
            bar.Clear();

            DataTable dt = new Pedido(General.connectionString).getPedidosVigentes();

            var tamanioBoton = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal
                ? ctlButton.TamanioButton.Normal
                : ctlButton.TamanioButton.Small;

            double anchoScreen = General.getCurrentScreeResolution();

            pedidoContainer.Columns = 6;

            foreach (DataRow dr in dt.Rows)
            {
                string secuencial = dr["peddscl"].ToString();

                ctlButton btn = new ctlButton
                {
                    Codigo = "F",
                    CodigoAlterno = secuencial,
                    Titulo = $"{dr["peddmsa"]} {dr["peddlta"]} : {secuencial}",
                    Imagen = General.sbDefineImagen(dr["peddTip"].ToString()),
                    SetTamanioButton = tamanioBoton 
                };

                btn.setBackgroundColor(Brushes.Bisque);
                btn._ctlButtonClick += ctlButton_Click;

                pedidoContainer.Children.Add(btn);
            }
        }

        private void btnUndoBill_Click(object sender, RoutedEventArgs e)
        {
            sbLimpiar();

            btnCrear.IsEnabled = false;
            btnModificar.IsEnabled = false;
            btnCancelar.IsEnabled = true;
            btnToBill.IsEnabled = false;
            btnUndoBill.IsEnabled = false;

            pedidoContainer.Children.Clear();
            cocina.Clear();
            bar.Clear();

            DataTable dt = new Pedido(General.connectionString).getPedidosFinalizados();

            var tamanioBoton = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal
                ? ctlButton.TamanioButton.Normal
                : ctlButton.TamanioButton.Small;

            foreach (DataRow dr in dt.Rows)
            {
                string secuencial = dr["peddscl"].ToString();

                ctlButton btn = new ctlButton
                {
                    Codigo = "U",
                    CodigoAlterno = secuencial,
                    Titulo = $"{dr["peddmsa"]} {dr["peddlta"]} : {secuencial}",
                    Imagen = General.sbDefineImagen(dr["peddtip"].ToString()),
                    SetTamanioButton = tamanioBoton
                };

                btn.setBackgroundColor(Brushes.Beige);
                btn._ctlButtonClick += ctlButton_Click;

                pedidoContainer.Children.Add(btn);
            }

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            sbLimpiar();
            btnCrear.IsEnabled = true;
            btnCancelar.IsEnabled = false;
            btnModificar.IsEnabled = true;
            btnEliminar.IsEnabled = false;
            btnToBill.IsEnabled = true;
            btnUndoBill.IsEnabled = true;
            btnChgMesa.IsEnabled = false;
            btnImpFact.IsEnabled = false;            
            sbActivaBtnPedd(false);
            cocina.Clear();
            bar.Clear();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private void llenaFromMemoria(string peddscl)
        {
            try
            {
                this.cocina = Principal.pedidosCocina[peddscl];
            }
            catch(Exception e)
            {
                this.cocina.Clear();
            }
            
        }

        /*
        * Param acction: string A=Agregar (cuando se va a agregar un nuevo producto)
                                E=Eliminar (cuando se va a eliminar un producto existente)
                                C=Cargar (cuando se va a cargar el pedido en memoria)
        */
        private void existeEnCocina(string peddscl, List<ProdPedd> lista, string accion = "A")
        {
            try
            {
                if (lista == null) return;

                if (Principal.pedidosCocina.TryGetValue(peddscl, out List<ProdPedd> pedidoActual))
                {
                    switch (accion)
                    {
                        case "C":
                            Principal.pedidosCocina[peddscl] = lista.ToList();
                            break;

                        case "E":
                        default:
                            foreach (ProdPedd prd in lista)
                            {
                                string observacionPrd = prd.observacion?.ToUpper() ?? string.Empty;

                                ProdPedd prodEncontrado = pedidoActual.FirstOrDefault(p =>
                                    p.codigo == prd.codigo &&
                                    (p.observacion?.ToUpper() ?? string.Empty) == observacionPrd);

                                if (prodEncontrado != null)
                                {
                                    if (accion == "A")
                                    {
                                        prodEncontrado.cantidad++;
                                    }
                                    else
                                    {
                                        if (prodEncontrado.cantidad > 1)
                                        {
                                            prodEncontrado.cantidad--;
                                        }
                                        else
                                        {
                                            pedidoActual.Remove(prodEncontrado);
                                        }
                                    }
                                }
                                else
                                {
                                    pedidoActual.Add(prd);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    Principal.pedidosCocina.Add(peddscl, lista.ToList());
                    Principal.secPedds.Add(peddscl);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al sincronizar pedido de cocina: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dgvPedd.Items.Count == 0)
            {
                ObjComportamiento.showMsgError("Debe ingresar algún producto para poder generar el pedido");
                return;
            }

            if (ObjComportamiento.showMsgYesNo("¿Desea grabar el pedido?") == MessageBoxResult.No)
            {
                return;
            }

            this.pedido.detPedd.Clear();

            var detallesMapeados = dgvPedd.Items.OfType<ProdPedd>().Select(prod => new PedidoDetalle
            {
                codprd = prod.codigo,
                cntprd = prod.cantidad,
                prcprd = prod.precio,
                ivaprd = prod.iva,
                usaiva = prod.usaIva,
                ptoprd = Math.Round((prod.precio + prod.iva) * prod.cantidad, 2),
                nbrprd = prod.producto
            });

            this.pedido.detPedd.AddRange(detallesMapeados);

            Respuesta res = this.pedido.grabar();

            if (!res.resultado)
            {
                ObjComportamiento.showMsgError(res.mensaje);
                return;
            }

            MessageBoxResult deseaImprimirComandas = ObjComportamiento.showMsgYesNo("¿Desea imprimir COMANDAS?");

            string mensajeServidor = res.mensaje ?? string.Empty;
            string sec = string.Empty;
            string secTmp = string.Empty;

            if (mensajeServidor.Contains("-") && mensajeServidor.Length >= 21)
            {
                sec = mensajeServidor.Substring(mensajeServidor.Length - 21, 10);
                secTmp = mensajeServidor.Substring(mensajeServidor.Length - 10);
            }
            else if (mensajeServidor.Length >= 10)
            {
                sec = mensajeServidor.Substring(mensajeServidor.Length - 10);
            }

            if (this.cocina.Count > 0)
            {
                existeEnCocina(sec, this.cocina, "A");
            }
            else
            {
                llenaFromMemoria(sec);
            }

            if (Principal.pedidosCocina.ContainsKey(sec))
            {
                pedido.grabarPedidoCocina(sec, Principal.pedidosCocina[sec], this.pedido.codEmpl);
            }

            if (deseaImprimirComandas == MessageBoxResult.Yes)
            {
                paraCocinar(sec, secTmp);
                paraBar(sec, secTmp);
            }

            ObjComportamiento.showMsgInfo(res.mensaje);
            btnCancelar_Click(sender, e);
        }

        private string getTipoLaber(string sTipo)
        {
            return sTipo switch
            {
                "D" => "DOMICILIO",
                "C" => "LLEVAR",
                "E" => "CARRIER",
                "R" => "RETIRAR",
                _ => "LOCAL"
            };
        }

        private void paraBar(string secuencial, string secuencialTmp = "")
        {
            if (this.bar.Count == 0)
            {
                return;
            }

            try
            {
                pedido.ltaPedd = pedido.getLetraPedido(secuencial);

                using (EposPrint cls = new EposPrint())
                {
                    cls.TipoImpresora = General.TIPO_IMPRESORAS;
                    Respuesta resp = cls.setPrinter(General.PRINTER_BAR);

                    if (!resp.resultado)
                    {
                        ObjComportamiento.showMsgError(resp.mensaje);
                        return;
                    }

                    cls.printCajaIzq = General.TICKET_COCINA_CANTIDAD_IZQ;
                    cls.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;

                    dsPrinter ds = new dsPrinter { DataSetName = "cfa" };
                    string sTipo = pedido.tipPedd;
                    string sMesa = lblMesa.Content.ToString();
                    string sScl = string.IsNullOrEmpty(secuencial) ? txtPedido.Content.ToString() : secuencial;

                    if (txtPedido.Content.ToString() == "")
                    {
                        var filasDataGrid = dgvPedd.Items.OfType<ProdPedd>().Select(item =>
                        {
                            var row = ds.tbCocina.NewRow();
                            row["itemNbr"] = item.producto;
                            row["itemCtd"] = item.cantidad;
                            return row;
                        });
                        foreach (var row in filasDataGrid) ds.tbCocina.Rows.Add(row);
                    }
                    else
                    {
                        var filasCocina = cocina.Select(item =>
                        {
                            var row = ds.tbCocina.NewRow();
                            row["itemNbr"] = item.producto;
                            row["itemCtd"] = item.cantidad;
                            return row;
                        });
                        foreach (var row in filasCocina) ds.tbCocina.Rows.Add(row);
                    }

                    if (ds.tbCocina.Rows.Count > 0)
                    {
                        string tipoLabel = getTipoLaber(sTipo);
                        
                        cls.DefineSeparador = char.TryParse(General.SEPARADOR, out char sep) ? sep : '-';
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

                        cls.Comanda("BAR",
                                bar,
                                $"{sMesa} {pedido.ltaPedd}",
                                DateTime.Now.ToString("yyyy/MM/dd"),
                                tipoLabel,
                                sScl,
                                General.IMPRIME_HORA_COCINA == 1,
                                Mesero.Tag.ToString(),
                                pedido.nbrCrri,
                                pedido.crrCodi);

                        cls.sbIpr();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error de hardware en impresora BAR: {ex.Message}");
            }
        }

        private void paraCocinar(String secuencial, string secuencialTemp = "")
        {
            if (cocina.Count == 0)
            {
                ObjComportamiento.showMsgError("No existen items para enviar a imprimir a Cocina");
                return;
            }

            try
            {
                pedido.ltaPedd = pedido.getLetraPedido(secuencial);

                using (EposPrint cls = new EposPrint())
                using (EposPrint clsPar = new EposPrint())
                {
                    cls.TipoImpresora = General.TIPO_IMPRESORAS;
                    clsPar.TipoImpresora = General.TIPO_IMPRESORAS;
                    cls.imprimeSecTemporal = General.IMPRIME_SECUENCIAL_TEMPORAL;
                    clsPar.imprimeSecTemporal = General.IMPRIME_SECUENCIAL_TEMPORAL;

                    Respuesta resp = cls.setPrinter(General.PRINTER_COCINA);
                    bool sendToParrilla = false;

                    if (!string.IsNullOrEmpty(General.PRINTER_PARRILLA) &&
                        General.PRINTER_PARRILLA != "Ninguna" &&
                        General.ENVIA_IMPRESORA_PARRILLA)
                    {
                        clsPar.setPrinter(General.PRINTER_PARRILLA);
                        clsPar.cantidadColumnas = General.CANTIDAD_COLUMNAS_IMPRESION;
                        clsPar.printCajaIzq = General.TICKET_COCINA_CANTIDAD_IZQ;
                        clsPar.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;
                        clsPar.ImprimeCabeceraComandas = General.IMPRIME_CABECERA_COMANDAS;
                        sendToParrilla = true;
                    }

                    if (!resp.resultado)
                    {
                        ObjComportamiento.showMsgError(resp.mensaje);
                        return;
                    }

                    cls.cantidadColumnas = General.CANTIDAD_COLUMNAS_IMPRESION;
                    cls.printCajaIzq = General.TICKET_COCINA_CANTIDAD_IZQ;
                    cls.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;
                    cls.ImprimeCabeceraComandas = General.IMPRIME_CABECERA_COMANDAS;
                    cls.SaltoLineaCabeceraComandas = General.SALTOLINEA_CABECERA_COMANDAS;

                    dsPrinter ds = new dsPrinter { DataSetName = "cfa" };
                    string sTipo = pedido.tipPedd;
                    string sMesa = lblMesa.Content.ToString();
                    string sScl = string.IsNullOrEmpty(secuencial) ? txtPedido.Content.ToString() : secuencial;

                    if (txtPedido.Content.ToString() == "")
                    {
                        var filasDataGrid = dgvPedd.Items.OfType<ProdPedd>().Select(item => {
                            var row = ds.tbCocina.NewRow();
                            row["itemNbr"] = item.producto;
                            row["itemCtd"] = item.cantidad;
                            return row;
                        });
                        foreach (var row in filasDataGrid) ds.tbCocina.Rows.Add(row);
                    }
                    else
                    {
                        var filasCocina = cocina.Select(item => {
                            var row = ds.tbCocina.NewRow();
                            row["itemNbr"] = item.producto;
                            row["itemCtd"] = item.cantidad;
                            return row;
                        });
                        foreach (var row in filasCocina) ds.tbCocina.Rows.Add(row);
                    }

                    if (ds.tbCocina.Rows.Count > 0)
                    {
                        string tipoLabel = getTipoLaber(sTipo);

                        bool imprimeHora = General.IMPRIME_HORA_COCINA == 1;
                        string nombreMesaLetra = $"{sMesa} {this.pedido.ltaPedd}";
                        string fechaFormateada = DateTime.Now.ToString("yyyy/MM/dd");
                        string tagMesero = Mesero.Tag.ToString();

                        cls.Comanda("COCINA", this.cocina, nombreMesaLetra, fechaFormateada, tipoLabel, sScl,
                                    imprimeHora, tagMesero, this.pedido.nbrCrri, this.pedido.crrCodi,
                                    "", "", secuencialTemp);
                        cls.sbIpr();

                        if (sendToParrilla)
                        {
                            clsPar.cantidadColumnas = General.CANTIDAD_COLUMNAS_IMPRESION;
                            clsPar.Comanda("PARRILLA", this.cocina, nombreMesaLetra, fechaFormateada, tipoLabel, sScl,
                                           imprimeHora, tagMesero, this.pedido.nbrCrri, this.pedido.crrCodi, "", "", secuencialTemp);
                            clsPar.sbIpr();
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                ObjComportamiento.showMsgError(ex.Message);
            }
        }

        private void btnCocinar_Click(object sender, RoutedEventArgs e)
        {
            paraCocinar("");
        }

        public void calcularTotalMostrar()
        {
            Double ltotal = 0;

            // Calculamos el total
            List<ProdPedd> items = new List<ProdPedd>();
            items.AddRange(dgvPedd.Items.OfType<ProdPedd>());

            var totNoGravaIva = items.Where(x => x.gravaIva == false && x.codigo != "9999").Select(c => c.precio * c.cantidad).Sum();
            var totConIva = items.Where(x => x.gravaIva == true && x.usaIva == true ).Select(c => (c.precio + c.iva) * c.cantidad).Sum();
            var totGravaIva = items.Where(x => x.gravaIva == true && x.usaIva == false).Select(c => (c.precio + c.iva) * c.cantidad).Sum();

            ltotal = Math.Round(totNoGravaIva + totConIva + totGravaIva, 2);
            txtTotal.Content = String.Format("{0:0.00}", ltotal);
        }

        private void muestraControlElimina(string pedscl, ref List<ProdPedd> prods, string codigo, bool memoria, int cantidad, ProdPedd row)
        {
            winGeneral win = new winGeneral
            {
                Title = "Eliminación de productos",
                Width = 720,
                Height = 470,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            ctlEliminaProd ctl = new ctlEliminaProd(pedscl, ref prods, codigo, memoria)
            {
                cantidad = cantidad,
                row = row,
                contenedor = win
            };

            ctl.aceptaEliminacion += AceptaEliminacionProducto;

            ctl.generar();
            win.Contenido.Children.Add(ctl);
            win.ShowDialog();
        }

        private void AceptaEliminacionProducto(string peddscl, int cantidad, ProdPedd row, int index, bool memoria)
        {
            if (index >= 0)
            {
                List<ProdPedd> listaCocina = memoria
                            ? Principal.pedidosCocina[peddscl]
                            : this.cocina;

                var itemCocina = listaCocina[index];
                if (itemCocina.cantidad > 1)
                {
                    itemCocina.cantidad--;
                }
                else
                {
                    listaCocina.RemoveAt(index);
                }

                if (cantidad <= 1 && row.cantidad == 1)
                {
                    dgvPedd.Items.Remove(row);
                }
                else
                {
                    row.cantidad--; 
                }
            }
        }

        private int BuscarIndiceCocina(string codigo, List<ProdPedd> obj)
        {
            return obj?.FindIndex(item => item.codigo == codigo) ?? -1;
        }

        private void btnDelItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProdPedd row = (ProdPedd)((FrameworkElement)sender).DataContext;
                String codigo = row.codigo;
                int cantidad = row.cantidad;
                

                var prodsToDel = cocina.Where(x => x.codigo == codigo).ToList();
                var prodBarToDel = bar.Where(x => x.codigo == codigo).ToList();
                int cantidadProd = prodsToDel.Count;
                int cantidadBar = prodBarToDel.Count;

                if (cantidadBar > 1)
                {
                    int index = 0;
                    for (var i = 0; i < bar.Count; i++)
                    {
                        if (bar[i].codigo == codigo)
                            index = i;
                    }
                    bar[index].cantidad--;
                }
                else if (cantidadBar == 1)
                {
                    int index = 0;
                    for (var i = 0; i < bar.Count; i++)
                    {
                        if (bar[i].codigo == codigo)
                            index = i;
                    }

                    bar[index].cantidad--;

                   
                }else
                {

                }

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
                    string peddscl = this.pedido.sclPedd.ToString().PadLeft(10, '0');
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
                
                dgvPedd.Items.Refresh();

                var valorTotal = dgvPedd.Items.OfType<ProdPedd>().Where(c => c.codigo != "9999").Sum(o => o.precio * o.cantidad);
                double Precio = General.FORMA_PAGO_SERVICIO == "P" ?
                                        (valorTotal * (valorServicio / 100)) :
                                        (valorTotal + valorServicio);

                if (General.AGREGA_SERVICIO)
                {
                    var pedds = dgvPedd.Items.OfType<ProdPedd>().ToList();
                    var serv = pedds.Where(x => x.codigo == "9999").FirstOrDefault();
                    if (serv != null)
                        serv.precio = Precio;
                }

                this.calcularTotalMostrar();

                btnSave.IsEnabled = true;
            }
            catch(Exception er)
            {

            }
        }

        private void btnChgMesa_Click(object sender, RoutedEventArgs e)
        {
            if (this.pedido.sclPedd <= 0)
            {
                ObjComportamiento.showMsgError("El pedido aún no ha sido creado, por lo cual no es posible cambiar la mesa.");
                return;
            }

            winGeneral win = new winGeneral
            {
                Title = "Cambio de mesa",
                Width = 720,
                Height = 470,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            ctlChgMesa ctl = new ctlChgMesa
            {
                contenedor = win
            };

            ctl.cambiaMesa += Cambia_Mesa;

            win.Contenido.Children.Add(ctl);
            win.ShowDialog();
        }

        private void Cambia_Mesa(object sender, RoutedEventArgs e, String mesa)
        {
            this.pedido.msaPedd = mesa;

            Respuesta res = this.pedido.grabar();

            if (res.resultado)
            {
                ObjComportamiento.showMsgInfo("Cambio de mesa realizado correctamente");
                btnCancelar_Click(sender, e);
            }
            else
            {
                ObjComportamiento.showMsgError("El Cambio de mesa no pudo ser realizado");
            }
        }

        private void btnMesero_Click(object sender, RoutedEventArgs e)
        {
            
        }
        
        private void btnMesero_Checked(object sender, RoutedEventArgs e)
        {
            nKeyboard.MaxLength = General.CANTIDAD_CARACTERES_PASSWORD;

            nKeyboard.setTextBox = new PasswordBox();

            nKeyboard.ShowDialog();
        }

        private void btnMesero_Unchecked(object sender, RoutedEventArgs e)
        {
            nKeyboard.Hide();
            ActivaCrearPedido(false);
            cambiaTipoPedido.IsEnabled = false;
            Mesero.Text = "";
            Mesero.Tag = "";
        }

        private void cambiaTipoPedido_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral
            {
                Title = "Cambio de Tipo de Pedido",
                Width = 530,
                Height = 310,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            ctlChgTipPedd ctl = new ctlChgTipPedd
            {
                contenedor = win
            };

            ctl.aceptaCambioTipo += Cambia_TipoPedido;

            win.Contenido.Children.Add(ctl);
            win.ShowDialog();
        }

        public void Cambia_TipoPedido(String tipo)
        {
            this.pedido.tipPedd = tipo;
            setTipoPedido(tipo);            
        }


        /*-------------------------------------------------------------------------*/

        public void creaGrupos()
        {
            pedidoContainer.Children.Clear();
            pedidoContainer.Children.Add(gProd);

            Grupos grupos = new Grupos(General.connectionString);
            DataTable dt = grupos.listar(1);

            gProd.clearGroup();
            gProd.clearProduct();

            bool esTamanioNormal = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal;

            gProd.anchoContenedor = General.getCurrentScreeResolution() - 300;
            gProd.cntCols = General.CANTIDAD_COLUMNAS_GRUPOS;
            gProd.SetTamanioGroups = esTamanioNormal ? ctlGrpProd.TamanioGroups.Normal : ctlGrpProd.TamanioGroups.Small;
            gProd.Organizacion = General.ORGANIZA_ITEMS_PEDIDO;

            double altoBoton = esTamanioNormal ? 60 : 50;
            gProd.Width = gProd.anchoContenedor;
            double anchoBoton = esTamanioNormal ? 100 : 80;
            var tipoTamanioBoton = esTamanioNormal ? ctlButtonNI.tamanioButton.Normal : ctlButtonNI.tamanioButton.Small;

            foreach (DataRow dr in dt.Rows)
            {
                ctlButtonNI btn = new ctlButtonNI
                {
                    Alto = altoBoton,
                    Ancho = anchoBoton,
                    SetTamanioButton = tipoTamanioBoton,
                    ShowCodigo = false,
                    CentrarTexto = true,
                    Codigo = "G",
                    CodigoAlterno = dr["codigo"].ToString(),
                    Titulo = dr["descripcion"].ToString(),
                    TipoBotones = "ST"
                };

                btn._ctlButtonClick += ctlButton_Click;

                gProd.addToGroup(btn);
            }

            ScrollPedidos.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        private void setTipoPedido(string tipo)
        {
            var (rutaImagen, textoEtiqueta) = tipo switch
            {
                "L" => ("pack://application:,,,/images/toserv.png", "Local"),
                "D" => ("pack://application:,,,/images/delivery.png", "Domicilio"),
                "C" => ("pack://application:,,,/images/tocarry.png", "Llevar"),
                "E" => ("pack://application:,,,/images/uber.png", "Carrier"),
                "R" => ("pack://application:,,,/images/clock-color.png", "Retirar"),
                _ => (string.Empty, string.Empty) 
            };

            if (!string.IsNullOrEmpty(rutaImagen))
            {
                this.pedido.tipPedd = tipo;
                imgTipoPedd.Source = new BitmapImage(new Uri(rutaImagen));
                lblTipo.Content = textoEtiqueta;
            }
        }

        public void ctlButton_DobleClick(object sender, MouseButtonEventArgs e, string tipo)
        {
            LoadEditarItemProducto(sender);
        }

        public void ctlButton_EditClick(object sender, RoutedEventArgs e)
        {
            LoadEditarItemProducto(sender);
        }

        private void LoadEditarItemProducto(object sender)
        {
            // Sección para indicar ingredientes a quitar para el item
            Items itemsSel = new Items(General.connectionString);

            string codigo = string.Empty;
            string item = string.Empty;

            if (sender is Button btnSender)
            {
                codigo = btnSender.Tag?.ToString() ?? string.Empty;

                if (btnSender.Content is StackPanel panel && panel.Children.Count > 2 && panel.Children[2] is TextBlock textBlock)
                {
                    item = textBlock.Text;
                }
            }

            DataTable dt = itemsSel.getRecetaDetalle(codigo);

            winGeneral win = new winGeneral
            {
                Title = "Eliminación de ingredientes en el pedido",
                Width = 650,
                Height = 630,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            ctlQuitaIngredientes ctl = new ctlQuitaIngredientes(codigo, item, dt)
            {
                contenedor = win
            };

            ctl.agregaItem += Quita_Ingredientes;
            win.Contenido.Children.Add(ctl);
            win.ShowDialog();
        }

        public void Quita_Ingredientes(String codigo, int cantidad, List<mIngredientes> lista, string observacion)
        {
            sbActivaBtnPedd(true);
            barcode.Visibility = Visibility.Visible;
            barcode.Focus();

            Items itemsSel = new Items(General.connectionString);
            DataTable dti = itemsSel.getById(codigo, this.pedido.codCrri);

            if (dti == null || dti.Rows.Count == 0) return;

            DataRow filaItem = dti.Rows[0];
            string codigoArticulo = filaItem["codigo"].ToString();

            var itemExistente = dgvPedd.Items.OfType<ProdPedd>().FirstOrDefault(i => i.codigo == codigoArticulo);
            bool existe = itemExistente != null;

            if (existe)
            {
                itemExistente.cantidad += cantidad;
                dgvPedd.Items.Refresh();
            }

            double precioUnidad = Math.Round(General.StringToDouble(filaItem["Precio"].ToString()), 6);
            double ivaUnidad = Math.Round(Math.Abs(General.StringToDouble(filaItem["IVA"].ToString()) - General.StringToDouble(filaItem["precio"].ToString())), 6);
            bool usaIva = (bool)filaItem["usaIva"];
            bool gravaIva = (bool)filaItem["gravaIva"];
            string nombreProducto = filaItem["producto"].ToString();

            ProdPedd prodPedd = existe ? null : new ProdPedd
            {
                codigo = codigoArticulo,
                producto = nombreProducto,
                cantidad = cantidad,
                usaIva = usaIva,
                gravaIva = gravaIva,
                precio = precioUnidad,
                iva = ivaUnidad
            };

            if ((bool)filaItem["itemCoc"])
            {
                var prodCocina = new ProdPedd
                {
                    codigo = codigoArticulo,
                    producto = nombreProducto,
                    cantidad = cantidad,
                    usaIva = usaIva,
                    gravaIva = gravaIva,
                    precio = precioUnidad,
                    iva = ivaUnidad,
                    noIngredientes = lista,
                    observacion = observacion
                };

                if (!existe) dgvPedd.Items.Add(prodPedd);

                cocina.Add(prodCocina);
            }

            calcularTotalMostrar();
        }

        public void MuestraOpcionesCarrier()
        {
            winGeneral win = new winGeneral
            {
                Title = "Datos del Carrier",
                Width = 620,
                Height = 520,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            ctlCarrierData ctl = new ctlCarrierData
            {
                contenedor = win,
                parent = this
            };

            ctl.pedidoCarrier += Genera_Carrier;

            win.Contenido.Children.Add(ctl);
            win.ShowDialog();
        }

        public void MuestraOpcionesDomicilio(Boolean paraCarrier)
        {
            winGeneral win = new winGeneral
            {
                Title = "Datos de pedido a domicilio",
                Width = 750,
                Height = 530,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            ctlDelivery ctl = new ctlDelivery
            {
                Contenedor = win,
                parent = this,
                paraCarrier = paraCarrier
            };

            ctl.pedidoDomicilio += Genera_Pedido;
            win.Contenido.Children.Add(ctl);
            win.ShowDialog();
        }

        private void creaGrupoMesas(string titulo, string codalt)
        {
            bool esNormal = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal;

            double alto = esNormal ? 60 : 40;
            double ancho = esNormal ? 100 : 70;
            var tamanio = esNormal ? ctlButtonNI.tamanioButton.Normal : ctlButtonNI.tamanioButton.Small;


            ctlButtonNI btn = new ctlButtonNI
            {
                Alto = alto,
                Ancho = ancho,
                SetTamanioButton = tamanio,
                ShowCodigo = false,
                CentrarTexto = true,
                Codigo = "T",
                CodigoAlterno = codalt,
                Titulo = titulo,
                TipoBotones = "ST"
            };

            btn._ctlButtonClick += ctlButton_Click;
            gProd.addToGroup(btn);
        }

        private void MuestraBarcodeYBuscador()
        {
            if (General.PERMITE_BUSCAR_ITEMS)
            {
                buscaItem.Margin = new Thickness(3, 3, 98, 3);
            }

            if (General.PERMITE_USO_BARCODE)
            {
                int margenDerecho = General.PERMITE_BUSCAR_ITEMS ? 300 : 3;
                barcode.Margin = new Thickness(3, 3, margenDerecho, 3);
            }
        }

        public void ctlButton_Click(object sender, RoutedEventArgs e, string tipo)
        {
            try
            {
                setTipoPedido(tipo);
                string codAlterno = (sender as Button)?.Tag?.ToString() ?? string.Empty;
                var tamanioBoton = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal
                                    ? ctlButton.TamanioButton.Normal
                                    : ctlButton.TamanioButton.Small;

                switch (tipo)
                {
                    case "L":
                        // Cuando selecciona un tipo de pedido LOCAL
                        // Se van a generar las mesas
                        pedidoContainer.Children.Clear();
                        pedidoContainer.Columns = 0;

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

                            if (General.CANTIDAD_MESAS_ALTERNA > 0) creaGrupoMesas("SALÓN ALTERNO", "AL");
                            if (General.CANTIDAD_MESAS_EXTERIOR > 0) creaGrupoMesas("EXTERIORES", "EX");
                            if (General.CANTIDAD_MESAS_TERRAZA > 0) creaGrupoMesas("TERRAZA", "TR");
                            if (General.CANTIDAD_MESAS_2DO_PISO > 0) creaGrupoMesas("SEGUNDO PISO", "SP");
                        }
                        else
                        {
                            for (int i = 1; i <= General.CANTIDAD_MESAS; i++)
                            {
                                pedidoContainer.Children.Add(CrearBotonMesa(i, 0, tamanioBoton));
                            }
                        }

                        break;
                    case "T":
                        // Cuando se selecciona un grupo de mesas (localidad)
                        gProd.clearProduct();
                        var (factor, cantidad) = codAlterno switch
                        {
                            "PR" => (100, General.CANTIDAD_MESAS),
                            "AL" => (200, General.CANTIDAD_MESAS_ALTERNA),
                            "EX" => (300, General.CANTIDAD_MESAS_EXTERIOR),
                            "TR" => (400, General.CANTIDAD_MESAS_TERRAZA),
                            "SP" => (500, General.CANTIDAD_MESAS_2DO_PISO),
                            _ => (100, 0)
                        };

                        for (int i = 1; i <= cantidad; i++)
                        {
                            gProd.addToProduct(CrearBotonMesa(i, factor, tamanioBoton));
                        }
                        break;
                    case "D":
                        // Cuando selecciona un tipo de pedido a DOMICILIO
                        MuestraOpcionesDomicilio(false);
                        break;
                    case "C":
                        // Cuando selecciona un tipo de pedido para LLEVAR
                        pedido.msaPedd = "";
                        creaGrupos();
                        VerificarVisibilidadControlesBasicos();
                        MuestraBarcodeYBuscador();

                        break;
                    case "E":
                        // Cuando selecciona un tipo de pedido por carrier
                        //MuestraOpcionesDomicilio(true);
                        pedido.codCrri = "";
                        pedido.crrCodi = "";
                        pedido.crrObsv = "";
                        pedido.nbrCrri = "";

                        MuestraOpcionesCarrier();
                        break;
                    case "R":
                        // Define tipo de pedido para retiros
                        pedido.msaPedd = "";
                        creaGrupos();
                        VerificarVisibilidadControlesBasicos();
                        break;
                    case "M":
                        // Cuando selecciona una mesa
                        lblMesa.Content = codAlterno;
                        pedido.msaPedd = codAlterno;
                        
                        gProd.setTituloGrupo("Grupo");
                        gProd.setTituloProducto("Productos");
                        creaGrupos();
                        VerificarVisibilidadControlesBasicos();

                        MuestraBarcodeYBuscador();
                        pedidoContainer.Columns = 1;
                        gProd.SetColumnasGrid = 11;
                        break;
                    case "F":
                        // Finalización de un pedido
                        MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desea finalizar el Pedido?");
                        if (res == MessageBoxResult.Yes)
                        {
                            ObjComportamiento.showMuestraMensaje(pedido.finalizar(codAlterno));
                            btnCancelar_Click(sender, e);
                        }
                        break;
                    case "U":
                        // Reversión de un pedido
                        MessageBoxResult res1 = ObjComportamiento.showMsgYesNo("¿Desea revertir el Pedido?");
                        if (res1 == MessageBoxResult.Yes)
                        {
                            ObjComportamiento.showMuestraMensaje(pedido.revertir(codAlterno));
                            btnCancelar_Click(sender, e);
                        }
                        break;
                    case "CR":
                        pedido.codCrri = codAlterno;
                        pedido.nbrCrri = ((TextBlock)((StackPanel)((StackPanel)((Button)sender).Content).Children[1]).Children[1]).Text;
                        pedido.msaPedd = "";
                        creaGrupos();
                        VerificarVisibilidadControlesBasicos();
                        MuestraBarcodeYBuscador();

                        break;
                    case "P":
                        // Cuando se selecciona un pedido
                        String codigo = codAlterno;
                        pedido.getPedido(codigo);
                        existeEnCocina(codigo, pedido.getPedidoCocina(codigo), "C");
                        setTipoPedido(pedido.tipPedd);
                        lblMesa.Content = pedido.msaPedd;
                        txtPedido.Content = pedido.sclPedd.ToString().PadLeft(10, '0');

                        dgvPedd.Items.Clear();
                        Double total = 0;
                        btnChgMesa.IsEnabled = true;

                        for (int i = 0; i < pedido.detPedd.Count; i++)
                        {
                            var item = new ProdPedd
                            {
                                cantidad = pedido.detPedd[i].cntprd,
                                codigo = pedido.detPedd[i].codprd,
                                iva = pedido.detPedd[i].ivaprd,
                                precio = pedido.detPedd[i].prcprd,
                                producto = pedido.detPedd[i].nbrprd,
                                usaIva = pedido.detPedd[i].usaiva,
                                DelFromCoc = pedido.detPedd[i].itemdelcoc,
                                gravaIva = pedido.detPedd[i].itemgiva
                            };

                            total += Math.Round((item.precio + item.iva) * item.cantidad, 2);

                            dgvPedd.Items.Add(item);
                        }

                        txtTotal.Content = String.Format("{0:0.00}", total);
                        creaGrupos();
                        //gProd.SetColumnasGrid = 8;
                        VerificarVisibilidadControlesBasicos();
                        MuestraBarcodeYBuscador();

                        if (PUEDE_ELIMINAR_PEDIDO)
                            btnEliminar.IsEnabled = true;

                        cambiaTipoPedido.IsEnabled = true;
                        btnImpFact.IsEnabled = true;
                        pedidoContainer.Columns = 1;
                        gProd.SetColumnasGrid = 11;
                        break;
                    case "G":
                        // Cuando se selecciona un grupo
                        gProd.clearProduct();

                        MuestraBarcodeYBuscador();

                        Items items = new Items(General.connectionString);
                        DataTable dtp = null;

                        if (sender.GetType() == typeof(Button))
                        {
                            dtp = items.getItemsByGroup(codAlterno);
                        }
                        else if (sender.GetType() == typeof(ctlButtonNI))
                        {
                            dtp = items.getItemsByGroup(((ctlButtonNI)sender).CodigoAlterno.ToString());
                        }

                        if(dtp != null)
                        {
                            bool esNormal = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal;
                            double alto = esNormal ? 60 : 50;
                            double ancho = esNormal ? 120 : 110;
                            var tipoTamanio = esNormal ? ctlButtonNI.tamanioButton.Normal : ctlButtonNI.tamanioButton.Small;

                            foreach (DataRow dr in dtp.Rows)
                            {
                                ctlButtonNI btn = new ctlButtonNI {
                                    Alto = alto,
                                    Ancho = ancho,
                                    SetTamanioButton = tipoTamanio,
                                    ShowCodigo = false,
                                    CentrarTexto = true,
                                    Codigo = "I",
                                    CodigoAlterno = dr["codigo"].ToString(),
                                    Titulo = dr["descripcion"].ToString(),
                                    MuestraEdicion = (bool)dr["ParaCocina"],
                                    TipoBotones = General.TIPO_BOTONES,
                                    Imagen = dr["imagen"] != DBNull.Value ? (byte[])dr["imagen"] : null
                                };

                                if (btn.Titulo.Contains(buscaItem.Text) && buscaItem.Text.Trim().Length > 0)
                                {
                                    btn.setTituloBold(true);
                                    btn.setTituloColor(true);
                                }

                                btn._ctlButtonClick += ctlButton_Click;
                                btn._ctlButtonDobleClick += ctlButton_DobleClick;
                                btn._ctlButtonEditClick += ctlButton_EditClick;

                                gProd.addToProduct(btn);
                            }
                        }

                        
                        VerificarVisibilidadControlesBasicos();
                        break;
                    case "I":
                        // Cuando se selecciona un item
                        sbActivaBtnPedd(true);
                        VerificarVisibilidadControlesBasicos();
                        MuestraBarcodeYBuscador();

                        if (General.AGREGA_SERVICIO && General.VALOR_PAGO_SERVICIO > 0)
                        {
                            valorServicio = General.VALOR_PAGO_SERVICIO;
                        }

                        Items itemsSel = new Items(General.connectionString);

                        DataTable dti = itemsSel.getById(codAlterno, pedido.codCrri);

                        // Buscar si existe el item agregado
                        bool existe = false;
                        var Row = dti.Rows[0];
                        var RowIVA = Row["IVA"].ToString();
                        var RowPrecio = Row["precio"].ToString();
                        var codigoRow = Row["codigo"].ToString();
                        var usaIVA = (bool)Row["usaIva"];
                        var grabaIVA = (bool)Row["gravaIva"];
                        var producto = Row["producto"].ToString();

                        var itemExistente = dgvPedd.Items.OfType<ProdPedd>().FirstOrDefault(i => i.codigo == codigoRow);

                        if (itemExistente != null)
                        {
                            itemExistente.cantidad++;
                            existe = true;
                            dgvPedd.Items.Refresh();
                        }

                        // Buscar si existe el item en control
                        var itemCocina = cocina.FirstOrDefault(c => c.codigo == codigoRow || c.noIngredientes.Count > 0);
                        bool existeControl = itemCocina != null;

                        if (existeControl)
                        {
                            itemCocina.cantidad++;
                        }

                        var itemBar = bar.FirstOrDefault(b => b.codigo == codigoRow);
                        bool existeControlBar = itemBar != null;

                        if (existeControlBar) itemBar.cantidad++;

                        ProdPedd prodPedd = new ProdPedd
                        {
                            codigo = codigoRow,
                            producto = producto,
                            cantidad = 1,
                            usaIva = usaIVA,
                            gravaIva = grabaIVA,
                            precio = Math.Round(General.StringToDouble(Row["Precio"].ToString()), 6),
                            iva = Math.Round(Math.Abs(General.StringToDouble(RowIVA) - General.StringToDouble(RowPrecio)), 6)
                        };

                        ProdPedd prodCocina = null;
                        ProdPedd prodBar = null;

                        if ((bool)Row["itemCoc"] == true)
                        {
                            prodCocina = new ProdPedd
                            {
                                codigo = codigoRow,
                                producto = producto,
                                cantidad = 1,
                                usaIva = usaIVA,
                                gravaIva = grabaIVA,
                                precio = Math.Round(General.StringToDouble(Row["Precio"].ToString()), 6),
                                iva = Math.Round(Math.Abs(General.StringToDouble(RowIVA) - General.StringToDouble(RowPrecio)), 6)
                            };
                        }

                        if ((bool)Row["itemBar"] == true)
                        {
                            prodBar = new ProdPedd
                            {
                                codigo = codigoRow,
                                producto = producto,
                                cantidad = 1,
                                usaIva = usaIVA,
                                gravaIva = grabaIVA,
                                precio = Math.Round(General.StringToDouble(Row["Precio"].ToString()), 6),
                                iva = Math.Round(Math.Abs(General.StringToDouble(RowIVA) - General.StringToDouble(RowPrecio)), 6)
                            };
                        }

                        if (!existe && prodPedd != null) dgvPedd.Items.Add(prodPedd);
                        if (!existeControl && prodCocina != null) cocina.Add(prodCocina);
                        if (!existeControlBar && prodBar != null) bar.Add(prodBar);

                        // Calculamos el total
                        calcularTotalMostrar();

                        bool hayServ = false;
                        var itemServicio = dgvPedd.Items.OfType<ProdPedd>().FirstOrDefault(p => p.codigo == "9999");
                        var valorTotal = dgvPedd.Items.OfType<ProdPedd>().Where(c => c.codigo != "9999").Sum(o => o.precio * o.cantidad);
                        double Precio = General.FORMA_PAGO_SERVICIO == "P" ?
                                                (valorTotal * (valorServicio / 100)) :
                                                (valorTotal + valorServicio);

                        if (itemServicio != null)
                        {
                            itemServicio.cantidad = 1;
                            itemServicio.precio = Precio;
                            hayServ = true;
                            dgvPedd.Items.Refresh();
                        }

                        if (General.AGREGA_SERVICIO)
                        {
                            if (!hayServ) {
                                dgvPedd.Items.Add(new ProdPedd
                                {
                                    codigo = "9999",
                                    cantidad = 1,
                                    gravaIva = false,
                                    usaIva = false,
                                    precio = Precio,
                                    iva = 0,
                                    producto = "Servicio de Atención",
                                    total = Precio
                                });
                            }

                            calcularTotalMostrar();
                        }
                        
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private ctlButton CrearBotonMesa(int indice, int factor, ctlButton.TamanioButton tamanio) 
        { 
            string identificador = (indice + factor).ToString().PadLeft(3, '0'); 
            var btn = new ctlButton { 
                SetTamanioButton = tamanio, 
                Codigo = "M", 
                CodigoAlterno = identificador, 
                Titulo = identificador, 
                Imagen = "pack://application:,,,/images/table.png" 
            }; 
            btn._ctlButtonClick += ctlButton_Click; 
            return btn; 
        }

        private void VerificarVisibilidadControlesBasicos() 
        { 
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
        }

        private Dictionary<string, string> mapasIconosDeliveries()
        {
            return new Dictionary<string, string>
            {
                { "CBF", "pack://application:,,,/images/cabify.png" },
                { "GLV", "pack://application:,,,/images/glovo.png" },
                { "RPI", "pack://application:,,,/images/rappi.png" },
                { "SES", "pack://application:,,,/images/super-easy.png" },
                { "TPT", "pack://application:,,,/images/tipti.png" },
                { "UBR", "pack://application:,,,/images/uber-eats.png" },
                { "PEY", "pack://application:,,,/images/pedidosya.png" },
                { "DID", "pack://application:,,,/images/DiDiFood.png" },
                { "PIC", "pack://application:,,,/images/picker.png" },
                { "FOT", "pack://application:,,,/images/foodtook.png" },
                { "TAD", "pack://application:,,,/images/tada.png" }
            };
        }

        private ctlButton generaBotonDelivery(string codigo, string carrier, ctlButton.TamanioButton tamanioBoton, 
            Dictionary<string, string> mapasIconos, byte[] imagen=null, string icono=null)
        {
            var btn = new ctlButton
            {
                Codigo = "CR",
                CodigoAlterno = codigo,
                Titulo = carrier,
                SetTamanioButton = tamanioBoton
            };

            if (imagen != null)
            {
                btn.Icono = imagen;
            }
            else if (icono != null && mapasIconos.TryGetValue(icono, out string rutaImagen))
            {
                btn.Imagen = rutaImagen;
            }

            btn._ctlButtonClick += ctlButton_Click;

            return btn;
        }

        public void Genera_Carrier(String cliNid, String cliObv)
        {
            pedido.crrCodi = cliNid;
            pedido.crrObsv = cliObv;

            if (this.pedido.tipPedd == "E")
            {
                pedidoContainer.Children.Clear();

                Carriers carriers = new Carriers(General.connectionString);
                List<mCarrier> listaCarriers = carriers.buscar("", "A");

                var mapasIconos = mapasIconosDeliveries();

                var tamanioBoton = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal
                    ? ctlButton.TamanioButton.Normal
                    : ctlButton.TamanioButton.Small;

                foreach (var carrier in listaCarriers)
                {
                    var btn = generaBotonDelivery(carrier.codigo, carrier.carrier, tamanioBoton, mapasIconos, carrier.imagen, carrier.icono);
                    
                    pedidoContainer.Children.Add(btn);
                }
            }

        }

        public void Genera_Pedido(String cliNid, String cliNbr, String cliDir, String cliTlf, String cliRef)
        {
            pedido.cliDir = cliDir;
            pedido.cliNbr = cliNbr;
            pedido.cliNid = cliNid;
            pedido.cliRef = cliRef;
            pedido.cliTlf = cliTlf;

            var mapasIconos = mapasIconosDeliveries();
            
            if (this.pedido.tipPedd == "D")
            {
                creaGrupos();
                VerificarVisibilidadControlesBasicos();
                MuestraBarcodeYBuscador();
            }
            else if (this.pedido.tipPedd == "E")
            {
                pedidoContainer.Children.Clear();
                Carriers carriers = new Carriers(General.connectionString);
                List<mCarrier> listaCarriers = carriers.buscar("", "A");

                var tamanioBoton = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal
                                ? ctlButton.TamanioButton.Normal
                                : ctlButton.TamanioButton.Small;

                foreach (var carrier in listaCarriers)
                {
                    var btn = new ctlButton
                    {
                        Codigo = "CR",
                        CodigoAlterno = carrier.codigo,
                        Titulo = carrier.carrier,
                        SetTamanioButton = tamanioBoton
                    };

                    if (carrier.imagen != null)
                    {
                        btn.Icono = carrier.imagen;
                    }
                    else if (carrier.icono != null && mapasIconos.TryGetValue(carrier.icono, out string rutaImagen))
                    {
                        btn.Imagen = rutaImagen;
                    }

                    btn._ctlButtonClick += ctlButton_Click;

                    pedidoContainer.Children.Add(btn);
                }
            }
        }

        private void btnImpFact_Click(object sender, RoutedEventArgs e)
        {
            if (dgvPedd.Items.Count > 0)
            {
                MessageBoxResult resultadoCocina = MessageBox.Show(
                    "¿Desea imprimir el pedido en cocina? Yes[Si] = Todo, No = Nuevos, Cancel = Cancela Impresión",
                    "Información",
                    MessageBoxButton.YesNoCancel);

                if (resultadoCocina == MessageBoxResult.Yes || resultadoCocina == MessageBoxResult.No)
                {
                    string sec = this.pedido.sclPedd.ToString();
                    string peddScl = sec.PadLeft(10, '0');

                    List<ProdPedd> tmpCocina = this.cocina.ToList();

                    if (!string.IsNullOrEmpty(sec) && resultadoCocina == MessageBoxResult.Yes)
                    {
                        if (Principal.pedidosCocina.ContainsKey(peddScl))
                        {
                            existeEnCocina(peddScl, this.cocina, "A");

                            cocina = Principal.pedidosCocina[peddScl].ToList();
                        }
                        else
                        {
                            var nuevosItemsCocina = dgvPedd.Items.OfType<ProdPedd>().Select(item => new ProdPedd
                            {
                                codigo = item.codigo,
                                producto = item.producto,
                                cantidad = item.cantidad,
                                usaIva = item.usaIva,
                                precio = item.precio,
                                iva = item.iva,
                                noIngredientes = item.noIngredientes,
                                observacion = item.observacion
                            });

                            cocina.AddRange(nuevosItemsCocina);
                        }
                    }

                    paraCocinar(peddScl);

                    if (tmpCocina.Any(x => x != null))
                    {
                        cocina = tmpCocina;
                    }
                    else
                    {
                        this.cocina.Clear();
                    }
                }
            }
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            gProd.altura = dgvPedd.ActualHeight + 100;
            gProd.ancho = General.getCurrentScreenWidth((Window)((Grid)((DockPanel)((Grid)this.Parent).Parent).Parent).Parent) - 300;

            gProd.setHeight();
            gProd.setWidth();
        }

        private void btnBscItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(buscaItem.Text))
            {
                Items itemsSel = new Items(General.connectionString);

                string orden = General.FORMA_ORDEN_ITEMS_GRPS switch
                {
                    "A" => "ALIAS",
                    "N" => "PRODUCTO",
                    _ => "CODIGO"
                };

                var result = itemsSel.buscarPorNombre(buscaItem.Text, orden);

                gProd.resetFilter();

                var grupos = result.Select(o => o.grupo).Distinct().ToHashSet();

                ctlButtonNI btnFirst = null;

                foreach (var itmsGrp in gProd.getGrupoContainer())
                {
                    if (itmsGrp is ctlButtonNI btn && grupos.Contains(btn.Titulo))
                    {
                        btn.setTituloBold(true);
                        btn.setTituloColor(true);

                        if (btnFirst == null) btnFirst = btn;
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
                        gProd.clearProduct();
                    }
                }
            }

            VerificarVisibilidadControlesBasicos();
        }

        private void btnBscCls_Click(object sender, RoutedEventArgs e)
        {
            gProd.resetFilter();
            barcode.Focus();
            buscaItem.Text = "";
            gProd.clearProduct();
        }

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (nKeyboard != null)
                {
                    nKeyboard.retornaTexto -= TextoNumerico_Retorna;
                }
                this.KeyUp -= ctlPedidos_KeyUp;

                if (pedidoContainer != null)
                {
                    pedidoContainer.Children.Clear();
                }

                if (dgvPedd != null)
                {
                    dgvPedd.Items.Clear();
                }

                if (cocina != null) cocina.Clear();
                if (bar != null) bar.Clear();

                dtTipPedd = null;
                pedido = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);   
        }
        #endregion

    }
}
