using Entidades;
using Entidades.modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TreskelRest.cls;
using TreskelRest.frms.config;
using TreskelRest.frms.ctls;

namespace TreskelRest.frms
{
    /// <summary>
    /// Interaction logic for Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        public static Dictionary<string, List<ProdPedd>> pedidosCocina = new Dictionary<string, List<ProdPedd>>();
        public static List<string> secPedds = new List<string>();
        private bool usaPedFac = false;

        private static readonly BitmapImage ImagenPedidos = new BitmapImage(new Uri("pack://application:,,,/images/pedidos.png"));
        private static readonly BitmapImage ImagenBilling = new BitmapImage(new Uri("pack://application:,,,/images/billing.png"));

        public Principal()
        {
            InitializeComponent();

            this.Loaded += Principal_Loaded;
        }

        private async void Principal_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                General.loadParametros();
            });

            this.Title = $"Treskel System: {General.EMPRESA}";

            if (!string.IsNullOrEmpty(General.LOGO_EMPRESA))
            {
                try
                {
                    byte[] imageBytes = General.base64ToByteArray(General.LOGO_EMPRESA);
                    imgFondo.ImageSource = General.BtyeArrayToBitmapImage(imageBytes);
                }
                catch {  }
            }

            List<mPermiso> permisos = null;
            await Task.Run(() =>
            {
                Usuarios usuario = new Usuarios(General.connectionString);
                permisos = usuario.getPermisoUsuario(General.CODUSUARIO);
            });

            if (permisos == null) permisos = new List<mPermiso>();

            if (!General.SISTEMA_VALIDADO || !General.SISTEMA_REGISTRADO)
            {
                this.activarOpciones(false);
                string tipoFalta = !General.SISTEMA_VALIDADO ? "validado" : "registrado";

                ObjComportamiento.showMsgInfo($"Su sistema no se encuentra {tipoFalta}, realice ésta operación en la opción configuración, para ello debe usar la cuenta ADMIN, si no la posee contacte al proveedor");

                if (General.USUARIO == "admin")
                {
                    menuConfig.IsEnabled = true;
                }

                hideLoading();

                return;
            }

            if (General.USUARIO != "admin")
            {
                this.activarMenu(permisos);

                if (menu.Items[0] is MenuItem menuArchivo)
                {
                    var menuImpresoras = menuArchivo.Items.OfType<MenuItem>().FirstOrDefault(m => m.Name == "menuConfigImpresoras");
                    var menuFactElec = menuArchivo.Items.OfType<MenuItem>().FirstOrDefault(m => m.Name == "menuTestFactura");
                    var menuImportar = menuArchivo.Items.OfType<MenuItem>().FirstOrDefault(m => m.Name == "menuImportarDatos");

                    if (menuImpresoras != null) menuArchivo.Items.Remove(menuImpresoras);
                    if (menuFactElec != null) menuArchivo.Items.Remove(menuFactElec);
                    if (menuImportar != null) menuArchivo.Items.Remove(menuImportar);
                }
            }

            CambioRapido.Visibility = (General.CAMBIO_RAPIDO == 1) ? Visibility.Visible : Visibility.Hidden;

            bool tienePermisoMKT = permisos.Any(x => x.tag == "MKT" && x.estado == true);

            if (General.MODO_PEDIDO_FACTURACION == "FP" || tienePermisoMKT)
            {
                if (menu.Items[2] is MenuItem menuOperaciones)
                {
                    var mnuPedidos = menuOperaciones.Items.OfType<MenuItem>().FirstOrDefault(m => m.Name == "menuPedidos");
                    if (mnuPedidos != null) menuOperaciones.Items.Remove(mnuPedidos);
                }

                if (tienePermisoMKT)
                {
                    usaPedFac = true;
                    CambioRapido.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                CambioRapido.Visibility = Visibility.Visible;
            }

            hideLoading();
        }

        private void hideLoading()
        {
            LoadingOverlay.Visibility = Visibility.Collapsed;
            ContenedorGeneral.Visibility = Visibility.Visible;
        }

        private void activarOpciones(Boolean activar)
        {
            menuInicioCaja.IsEnabled = activar;
            menuCierre.IsEnabled = activar;
            menuValeIn.IsEnabled = activar;
            menuValeOut.IsEnabled = activar;
            menuReceta.IsEnabled = activar;

            menuConfig.IsEnabled = activar;
            menuConfigPC.IsEnabled = activar;

            menuPedidos.IsEnabled = activar;
            menuBilling.IsEnabled = activar;
            menuCompras.IsEnabled = activar;
            menuEmployee.IsEnabled = activar;
            menuSeguridad.IsEnabled = activar;
            menuInvAdapt.IsEnabled = activar;
            menuGrupos.IsEnabled = activar;
            menuSubGrupos.IsEnabled = activar;
            menuCarriers.IsEnabled = activar;

            menuRptControCaja.IsEnabled = activar;
            menuRptPedidos.IsEnabled = activar;
            menuRptVentas.IsEnabled = activar;
            menuRptRegalos.IsEnabled = activar;
            menuRptAnulaciones.IsEnabled = activar;
            menuRptCobrar.IsEnabled = activar;
            menuRptInventario.IsEnabled = activar;
            menuRptSRI.IsEnabled = activar;

            menuBackup.IsEnabled = activar;
            menuClientes.IsEnabled = activar;
            menuProducts.IsEnabled = activar;
            menuProviders.IsEnabled = activar;
            menuMateriaPrima.IsEnabled = activar;
            menuBackup.IsEnabled = activar;
            menuRetencion.IsEnabled = activar;
            menuRetenciones.IsEnabled = activar;
            menuNotaCredito.IsEnabled = activar;
            menuCaja.IsEnabled = activar;
            menuReservaciones.IsEnabled = activar;
            menuCobroCredito.IsEnabled = activar;
        }

        private void ocultarMenuItem(MenuItem mnu, Boolean accion)
        {
            if (accion)
                mnu.Visibility = Visibility.Collapsed;
            else
                mnu.Visibility = Visibility.Visible;
        }

        private void activarMenu(List<mPermiso> permisos)
        {
            this.activarOpciones(false);

            this.ocultarMenuItem(menuBackup, true);
            var permisosActivos = permisos.Where(p => p != null && p.estado);

            foreach (mPermiso permiso in permisosActivos)
            {
                switch (permiso.tag)
                {
                    case "VING":
                        menuValeIn.IsEnabled = true;
                        break;

                    case "VEGR":
                        menuValeOut.IsEnabled = true;
                        break;

                    case "RCTA":
                        menuReceta.IsEnabled = true;
                        break;

                    case "CAJA":
                        menuInicioCaja.IsEnabled = true;
                        menuCierre.IsEnabled = true;
                        menuValeIn.IsEnabled = true;
                        menuValeOut.IsEnabled = true;
                        break;

                    case "CONF":
                        menuConfig.IsEnabled = true;
                        menuBackup.IsEnabled = true;
                        menuConfigPC.IsEnabled = true;
                        menuCaja.IsEnabled = true;
                        break;

                    case "PEDD":
                        menuPedidos.IsEnabled = true;
                        menuReservaciones.IsEnabled = true;
                        break;

                    case "VENT":
                        menuBilling.IsEnabled = true;
                        menuClientes.IsEnabled = true;
                        menuRetencion.IsEnabled = true;
                        menuRetenciones.IsEnabled = true;
                        menuNotaCredito.IsEnabled = true;
                        FactElect.IsEnabled = true;
                        menuCobroCredito.IsEnabled = true;
                        break;

                    case "COMP":
                        menuCompras.IsEnabled = true;
                        menuProviders.IsEnabled = true;
                        menuMateriaPrima.IsEnabled = true;
                        break;

                    case "USRE":
                        menuSeguridad.IsEnabled = true;
                        menuEmployee.IsEnabled = true;
                        break;

                    case "INVT":
                        menuInvAdapt.IsEnabled = true;
                        break;

                    case "GSGR":
                        menuGrupos.IsEnabled = true;
                        menuSubGrupos.IsEnabled = true;
                        menuProducts.IsEnabled = true;
                        menuCarriers.IsEnabled = true;
                        break;

                    case "RPTE":
                        menuRptControCaja.IsEnabled = true;
                        menuRptVentas.IsEnabled = true;
                        menuRptPedidos.IsEnabled = true;
                        menuRptRegalos.IsEnabled = true;
                        menuRptAnulaciones.IsEnabled = true;
                        menuRptCobrar.IsEnabled = true;
                        menuRptInventario.IsEnabled = true;
                        menuRptSRI.IsEnabled = true;
                        break;

                    case "BKPS":
                        menuBackup.IsEnabled = true;
                        break;

                    // Propiedades estáticas globales de la clase General
                    case "DSCT": General.PUEDE_DAR_DESCUENTO = true; break;
                    case "GIFT": General.PUEDE_DAR_REGALO = true; break;
                    case "DPED": General.PUEDE_ELIMINAR_PEDIDO = true; break;
                    case "AFCT": General.PUEDE_ANULAR_FACTURA = true; break;
                }
            }
        }

        private void cerrar()
        {
            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Está seguro de finalizar la aplicación?");
            if (res == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void menuClientes_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlClientes(), contenido);
        }

        private void menuSalir_Click(object sender, RoutedEventArgs e)
        {
            cerrar();
        }

        private void menuConfig_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlConfiguracion(), contenido);
        }

        private void menuProducts_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlItems(), contenido);
        }

        private void menuProviders_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlProveedor(), contenido);
        }

        private void menuEmployee_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlEmpleados(), contenido);
        }

        private void menuSeguridad_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlUsuarios(), contenido);
        }

        private void menuPedidos_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlPedidos(), contenido);
            this.CambioRapido.Header = "Cambio a Ventas";
            imgCambioRapido.Source = ImagenBilling;

        }

        private void menuBilling_Click(object sender, RoutedEventArgs e)
        {
            if (General.MODO_PEDIDO_FACTURACION == "F-P" && !usaPedFac)
            {
                General.loadWindow(new ctlVenta(), contenido);
                CambioRapido.Header = "Cambio a Pedidos";
                imgCambioRapido.Source = ImagenPedidos;
            }
            else
            {
                General.loadWindow(new ctlFacturacion(), contenido);
            }
        }

        private void CambioRapido_Click(object sender, RoutedEventArgs e)
        {
            Action<object, RoutedEventArgs> ejecutarClick = General.TipoVentana switch
            {
                "P" => menuBilling_Click,
                _ => menuPedidos_Click // Cualquier otro caso (por defecto "Pedidos")
            };

            ejecutarClick(sender, e);
        }

        private void menuCompras_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlCompras(), contenido);
        }

        private void menuEnvio_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AbrirFormularioVale(UserControl objeto, string accion, string titulo, int width, int height, Action<winGeneral> configurarVentana = null)
        {
            // 1. Inicialización estructurada de la ventana cascarón
            winGeneral win = new winGeneral
            {
                Title = titulo,
                Width = width,
                Height = height,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            try
            {
                // Usamos dynamic para asignarle la ventana al control sin importar su tipo específico
                dynamic controlDinamico = objeto;
                controlDinamico.contenedor = win;
                if (accion != "")
                    controlDinamico.accion = accion;
            }
            catch
            { 
            
            }

            // 3. Inyección y apertura modal
            win.Contenido.Children.Add(objeto);

            // 4. Ejecucion de eventos personalizados
            configurarVentana?.Invoke(win);

            win.ShowDialog();
        }

        private void menuValeIn_Click(object sender, RoutedEventArgs e)
        {
            ctlValeIngEgr crea = new ctlValeIngEgr("I", General.CAJA);

            AbrirFormularioVale(crea, "N", "Crea nuevo Vale de Ingreso", 600, 400);
        }

        private void menuValeOut_Click(object sender, RoutedEventArgs e)
        {
            ctlValeIngEgr crea = new ctlValeIngEgr("E", General.CAJA);
            AbrirFormularioVale(crea, "N", "Crea nuevo Vale de Egreso", 600, 400);
        }

        private void menuBackup_Click(object sender, RoutedEventArgs e)
        {
            ctlBackup crea = new ctlBackup();
            AbrirFormularioVale(crea, "N", "Crea nuevo Vale de Egreso", 920, 670);
        }

        private void menuSistema_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuCierre_Click(object sender, RoutedEventArgs e)
        {
            ctlCierreCaja crea = new ctlCierreCaja("C", General.CAJA);
            AbrirFormularioVale(crea, "", "Cierre de Caja", 720, 435);
        }

        private void menuInicioCaja_Click(object sender, RoutedEventArgs e)
        {
            ctlCierreCaja crea = new ctlCierreCaja("I", General.CAJA);
            AbrirFormularioVale(crea, "", "Inicio de Caja", 720, 435);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
           
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Está seguro de finalizar la aplicación?");
            if (res == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }else
            {
                e.Cancel = true;
            }
        }

        private void menuGrupos_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlGrupos(), contenido);
        }

        private void menuSubGrupos_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlSubgrupos(), contenido);
        }

        private void menuMateriaPrima_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlMateriaPrima(), contenido);
        }

        private void menuInvAdapt_Click(object sender, RoutedEventArgs e)
        {
            ctlInvAjus crea = new ctlInvAjus();
            AbrirFormularioVale(crea, "", "Ajuste de Inventario", 810, 530);
        }

        private void menuRecetas_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlReceta(), contenido);
        }

        private void menuRptControCaja_Click(object sender, RoutedEventArgs e)
        {
            ctrlReporteCaja crea = new ctrlReporteCaja();
            AbrirFormularioVale(crea, "", "Reporte de Control de Caja", 610, 448);
        }

        private void menuRptVentas_Click(object sender, RoutedEventArgs e)
        {
            ctlReporteVentas crea = new ctlReporteVentas();
            AbrirFormularioVale(crea, "", "Reporte de Ventas", 610, 358);
        }

        private void menuRptPedidos_Click(object sender, RoutedEventArgs e)
        {
            ctlReportePedidos crea = new ctlReportePedidos();
            AbrirFormularioVale(crea, "", "Reporte de Pedidos", 610, 458);
        }

        private void menuRptRegalos_Click(object sender, RoutedEventArgs e)
        {
            ctlReporteRegalos crea = new ctlReporteRegalos();
            AbrirFormularioVale(crea, "", "Reporte de Regalo de Pedidos", 610, 458);
        }

        private void menuCarriers_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlCarriers(), contenido);
        }

        private void menuRptInventario_Click(object sender, RoutedEventArgs e)
        {
            ctlReporteInventario crea = new ctlReporteInventario();
            AbrirFormularioVale(crea, "", "Reporte de Inventarios", 610, 500);
        }

        private void menuRptAnulaciones_Click(object sender, RoutedEventArgs e)
        {
            ctlReporteAnulaciones crea = new ctlReporteAnulaciones();
            AbrirFormularioVale(crea, "", "Reporte de Anulaciones/Eliminaciones", 610, 358);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void menuTestFactElec_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlTestFactElect(), contenido);
        }

        private void menuRetencion_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlRetenciones(), contenido);
        }

        private void menuRetenciones_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlTraRetencion(), contenido);
        }

        private void menuCaja_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlCajas(), contenido);
        }

        private void menuConfigPC_Click(object sender, RoutedEventArgs e)
        {
            winConfig WinConfig = new winConfig("C");
            WinConfig.ShowDialog();
        }

        private void menuReservaciones_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlReservaciones(), contenido);
        }

        private void FactElect_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlFacturaSRI(), contenido);
        }

        private void menuImportacion_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlImportacion(), contenido);
        }

        private void menuRptSRI_Click(object sender, RoutedEventArgs e)
        {
            ctlReporteSRI crea = new ctlReporteSRI();
            AbrirFormularioVale(crea, "", "Reporte de Documentos SRI", 610, 480);
        }

        private void SearchFactElect_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlViewFacturas(), contenido);
        }

        private void menuRptCobrar_Click(object sender, RoutedEventArgs e)
        {
            ctlRptPorCobrar crea = new ctlRptPorCobrar();
            AbrirFormularioVale(crea, "", "Reporte de Pedidos por Cobrar", 810, 480);
        }

        private void menuCobroCredito_Click(object sender, RoutedEventArgs e)
        {
            ctlIngPagosCrdto crea = new ctlIngPagosCrdto();
            AbrirFormularioVale(crea, "", "Ingreso de pagos de clientes crédito", 810, 580);
        }

        private void menuRptInconsistencia_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuNotaCredito_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlNotasCredito(), contenido);
        }

        private void menuTestEmail_Click(object sender, RoutedEventArgs e)
        {
            ctlTestEmail crea = new ctlTestEmail();
            AbrirFormularioVale(crea, "", "Test de correo electrónico", 810, 450);
        }

        private void menuCuentas_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlPlanCuentas(), contenido);
        }

        private void menuFeriados_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlFeriados(), contenido);
        }

        private void menuPagoSalarios_Click(object sender, RoutedEventArgs e)
        {
            ctlPagoSalarios crea = new ctlPagoSalarios();
            AbrirFormularioVale(crea, "", "Pago de Salarios", 910, 580, win =>
            {
                win.ejecutaWindowsActivate += new winGeneral.EjecutaWindowsActivate(ActivaVentana);
            });

            void ActivaVentana(object ventana)
            {
                ((ctlPagoSalarios)ventana).TextoNid_GotFocus();
            }
        }

        private void menuBancos_Click(object sender, RoutedEventArgs e)
        {
            General.loadWindow(new ctlBancos(), contenido);
        }

        private void menuRptSalarios_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuRptSalariosPendientes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuRptBancos_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuTransBancaria_Click(object sender, RoutedEventArgs e)
        {
            ctlMovBcos crea = new ctlMovBcos();
            AbrirFormularioVale(crea, "", "Registro de Movimientos Bancarios", 910, 580);
        }
    }
}
