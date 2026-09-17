using DeviceId;
using Entidades;
using Entidades.modelos;
using LibConfig;
using LibConfig.cls;
using LibEncrypt;
using LibHttpClient;
using LibKeys;
using LibPrint;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using TreskelRest.cls;
using TreskelRest.cls.Common;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlConfiguracion.xaml
    /// </summary>
    public partial class ctlConfiguracion : UserControl, IDisposable
    {
        public clsTextBox clsSvd;
        public clsTextBox clsPto;
        public clsTextBox clsUsr;
        public clsTextBox clsPwd;
        public clsTextBox clsPri;
        public clsTextBox clsSec;
        public clsTextBox clsTer;
        public clsTextBox clsCua;
        public clsTextBox clsEmp;
        public clsTextBox clsRuc;
        public clsTextBox clsRsc;
        public clsTextBox clsDlc;
        public clsTextBox clsDmt;
        public clsTextBox clsEst;
        public clsTextBox clsMin;
        public clsTextBox clsDev;

        private Principal principal;
        private string _img = "";
        private const int ProductCode = 1;

        private NumKeyboard nKeyboard = new NumKeyboard();

        private gridKeysControl gridKeyboard = new gridKeysControl();

        private bool ticketCantidadIzq = false;
        private bool OrganizacionHorizontal = true;
        private string tipoBotones = "ST";
        private bool usaImpresoraParrilla = false;
        private bool usaImpresoraProforma = false;
        private int fondoLogin = 1;

        private string ubicacionLogoOrig = "";

        ImpresoraDestino impresoras = new ImpresoraDestino();

        public void setPrincipal(Principal principal)
        {
            this.principal = principal;
        }

        private void CrearTextBox()
        {
            clsSvd = new clsTextBox(txtServer, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.SERVIDOR, 20);
            clsPto = new clsTextBox(txtPuerto, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS, 5);
            clsUsr = new clsTextBox(txtUsuario, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.CORREO, 100);
            clsPwd = new clsTextBox(txtClave, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.CLAVES, 20);
            clsPri = new clsTextBox(txtCorreo1, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.CORREO, 50);
            clsSec = new clsTextBox(txtCorreo2, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.CORREO, 50);
            clsTer = new clsTextBox(txtCorreo3, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.CORREO, 50);
            clsCua = new clsTextBox(txtCorreo4, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.CORREO, 50);
            clsEmp = new clsTextBox(txtEmpName, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS, 50);
            clsRuc = new clsTextBox(txtEmpRuc, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS, 13);
            clsRsc = new clsTextBox(txtRazonSocial, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS, 50, true, "-.");
            clsDmt = new clsTextBox(txtDirMatriz, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS, 80, true, "-.");
            clsDlc = new clsTextBox(txtDirLocal, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS, 80, true, "-.");
            clsEst = new clsTextBox(txtEstablecimiento, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS, 3);
            clsMin = new clsTextBox(txtMinutos, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS, 5);
            clsDev = new clsTextBox(txtRucDevelop, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS, 13);
        }

        private void InicializarControles()
        {
            txtMinutos.Text = "0";
            txtProdID.Text = General.PRODUCT_CODE;
        }

        private void ConfigurarControlNumerico(dynamic control, int minimo, int maximo, string valor = null)
        {
            control.ValorMinimo = minimo;
            control.ValorMaximo = maximo;

            if (valor != null) control.setValue("0");
        }

        private void ConfigurarPermisos()
        {
            if (General.USUARIO != "admin") return;

            System.Windows.Controls.Control[] controles =
            {
                txtEmpName,
                txtEmpRuc,
                txtRazonSocial,
                txtEstablecimiento,
                txtNumFact,
                txtDirLocal,
                txtDirMatriz,
                chkRetencion,
                chkContabilidad,
                tabEmpresa,
                tabActivacion,
                tabFactElect,
                cboTipoLic,
                txtMinutos,
                horaInicio,
                txtRucDevelop
            };

            foreach (var control in controles)
                control.IsEnabled = true;

            txtMinutos.IsEnabled = false;
            horaInicio.IsEnabled = false;
        }

        private void ConfigurarEventos()
        {
            nKeyboard.retornaTexto += new LibKeys.NumKeyboard.RetornaTexto(TextoNumerico_Retorna);
            gridKeyboard.retornaValidacion += new LibKeys.gridKeysControl.RetornaValidacion(RetornaValidacion);
        }

        private int ObtenerIndiceLicencia(LicenseType tipo)
        {
            return tipo switch
            {
                LicenseType.TRIAL => 0,
                LicenseType.FULL => 1,
                LicenseType.ANUAL => 2,
                _ => -1
            };
        }

        private void CargarLicencia()
        {
            if (!General.SISTEMA_REGISTRADO)
                return;

            CryptLicense cript = new CryptLicense();
            LicenseInfo lic = cript.leer(txtProdID.Text);

            txtProdKey.Text = lic.LicenseKey;

            cboTipoLic.SelectedIndex = ObtenerIndiceLicencia(lic.licenseType);
        }

        private void AjustarResolucion()
        {
            if (General.TAMANIO_BOTONES_FACT != TamanioBotonesMenu.Small &&
                General.TAMANIO_BOTONES_PEDD != TamanioBotonesMenu.Small)
                return;

            foreach (TabItem tab in tabControl.Items)
                tab.FontSize = 12;
        }

        public ctlConfiguracion()
        {
            try
            {
                InitializeComponent();

                CrearTextBox();
                InicializarControles();

                ConfigurarControlNumerico(txtIva, 1, 100);
                ConfigurarControlNumerico(txtMesas, 3, 100);
                ConfigurarControlNumerico(txtInicioScl, 1, 999999);
                ConfigurarControlNumerico(txtCntPswd, 4, 15);
                ConfigurarControlNumerico(txtCntCopias, 1, 5, "1");
                ConfigurarControlNumerico(txtCntCols, 0, 4);
                ConfigurarControlNumerico(txtValServ, 0, 20, "0");
                ConfigurarControlNumerico(txtCadMinutos, 1, 5);
                ConfigurarControlNumerico(txtStockMin, 0, 100);

                load();

                ConfigurarPermisos();

                ConfigurarEventos();

                CargarLicencia();

                AjustarResolucion();
            }
            catch (Exception e)
            {
                ObjComportamiento.showMsgError(e.Message);
            }
        }

        private void cargarConfiguracionGeneral(int id, string valor)
        {
            switch (id)
            {
                case 1:
                    // Precio del producto incluye el IVA
                    chkPrcIva.IsChecked = valor.Equals("1");
                    break;
                case 2:
                    // Valor del IVA ordinario
                    txtIva.setValue(valor);
                    break;
                case 3:
                    // Teclado en pantalla
                    chkTouch.IsChecked = valor.Equals("1");
                    break;
                case 12:
                    // Permite definir si se usa un control de inventario
                    chkCtrlInv.IsChecked = valor.Equals("1");
                    break;
                case 21:
                    // Permite descuento manual
                    chkDsctManual.IsChecked = valor.Equals("1");
                    break;
                case 22:
                    General.TEMA_REPORTE = int.Parse(valor);

                    foreach (var tgb in tabTemas.Children.OfType<ToggleButton>())
                    {
                        //ToggleButton tgb = (ToggleButton)control;
                        tgb.Checked -= toggle_Checked;
                        tgb.Unchecked -= toggle_UnChecked;

                        tgb.Checked += toggle_Checked;
                        tgb.Unchecked += toggle_UnChecked;

                        tgb.IsChecked = Convert.ToInt32(tgb.Tag) == General.TEMA_REPORTE;
                    }
                    break;
                case 42:
                    // Permite cierre de caja acumulativo
                    chkCierreAcum.IsChecked = valor.Equals("1");
                    break;
                case 48:
                    // Define el formato decimal a utilizar
                    cboFormatDec.SelectedIndex = valor.Equals("###,###.##") ? 0 : 1;
                    break;
                case 98:
                    // Permite la creación del documento Factura o Proforma
                    DocuCrea.Valor = valor;
                    break;
                case 140:
                    // Permite otorgar crédito a los comensales
                    chkAllowCredito.IsChecked = valor.Equals("1");
                    break;
                case 142:
                    // Permite definir el tiempo de corte para los créditos otorgados y realizar los cobros.
                    cboTiempoCorte.SelectedIndex = valor.Equals("Q") ? 0 : 1;
                    break;
                case 161:
                    // Permite definir si el fondo del login se genera de forma randomica
                    chkFondoRandom.IsChecked = valor.Equals("1") ? true : false;
                    break;
                case 162:
                    // Permite definir el fondo a utilizar en el login

                    int num = int.TryParse(valor, out int v) && v >= 1 && v <= 10 ? v : 1;

                    // Buscar el control dinámicamente por su nombre (ejemplo para WPF)
                    var fondo = this.FindName($"Fondo{num}") as RadioButton; 
                    if (fondo != null) fondo.IsChecked = true;
                    break;
                case 163:
                    // Permite identificar si el login se aplicará para un restaurat o para otro tipo de entidad.
                    cboTemaLogin.SelectedIndex = int.Parse(valor);
                    break;
                case 165:
                    // Valor del IVA para Feriados
                    string ivaFeriado = valor;
                    if (string.IsNullOrEmpty(ivaFeriado)) ivaFeriado = txtIva.getValue();
                    if (ivaFeriado == "0") ivaFeriado = txtIva.getValue();

                    txtIvaFeriado.setValue(ivaFeriado);
                    break;
                case 167:
                    // Valor de la cantidad mínima de stock disponible
                    valor = valor ?? "0";
                    if (int.Parse(valor) < 0) valor = "0";
                    txtStockMin.setValue(valor);
                    break;
                case 168:
                    chkIncPrc.IsChecked = valor.Equals("1") ? true : false;
                    break;
                case 169:
                    txtRucDevelop.Text = valor;
                    break;
            }
        }

        public void cargarConfiguracionMesas(int id, string valor)
        {
            switch (id)
            {
                case 4:
                    // Cantidad de mesas
                    txtMesas.setValue(valor);
                    break;
                case 50:
                    // Definir si usa mesas alternas y su cantidad
                    chkAlterna.IsChecked = int.Parse(valor) > 0 ? true: false;
                    txtMesasAlt.setValue(valor);
                    break;
                case 51:
                    // Definir si usa mesas externas y su cantidad
                    chkExterior.IsChecked = int.Parse(valor) > 0 ? true : false;
                    txtMesasExt.setValue(valor);
                    break;
                case 52:
                    // Definir si usa mesas en terrazas y su cantidad
                    chkTerraza.IsChecked = int.Parse(valor) > 0 ? true : false;
                    txtMesasTerr.setValue(valor);
                    break;
                case 53:
                    // Definir si usa mesas en segundo piso y su cantidad
                    chk2Piso.IsChecked = int.Parse(valor) > 0 ? true : false;
                    txtMesasSPis.setValue(valor);
                    break;
            }
        }

        private void cargarConfiguracionPedidosVentas(int id, string valor)
        {
            switch (id)
            {
                case 23:
                    // Permite definir si la facturación y pedidos se generan en una única pantalla
                    chkCambioRapido.IsChecked = valor.Equals("1");

                    if (chkCambioRapido.IsChecked == true)
                    {
                        chkModoFact.IsChecked = false;
                        chkModoFact.IsEnabled = false;
                        chkCambioRapido.IsEnabled = true;
                    }
                    else
                    {
                        chkModoFact.IsEnabled = true;
                    }
                    break;
                case 24:
                    // Define el valor con el cual iniciará el secuencial de pedidos/ventas, es importante cuando se tiene una base de datos previa.
                    txtInicioScl.setValue(valor);
                    break;
                case 25:
                    // Imprime la hora en las comandas de cocina
                    chkImpHoraCocina.IsChecked = valor.Equals("1");
                    break;
                case 27:
                    // Definir la forma en la que se va a generar el ticket de cocina
                    bool seleccionado = valor.Equals("1");
                    ticketCocina1.IsChecked = !seleccionado;
                    ticketCocina2.IsChecked = seleccionado;
                    break;
                case 28:
                    // Imprime el ticket de carries / deliverier
                    chkImpTicketCarrier.IsChecked = valor.Equals("1");
                    break;
                case 39:
                    // Permite definir si se presenta el botón de cambio rápido entre las pantallas de pedidos y ventas
                    chkModoFact.IsChecked = valor.Equals("F-P");
                    if (chkModoFact.IsChecked == true)
                    {
                        chkCambioRapido.IsEnabled = true;
                        chkModoFact.IsEnabled = true;
                    }
                    else
                    {
                        chkCambioRapido.IsEnabled = false;
                        chkCambioRapido.IsChecked = false;
                    }
                    break;
                case 40:
                    btnTextOnly.IsChecked = valor == "ST";
                    btnTextOnlyEdit.IsChecked = valor == "STE";
                    btnImageOnly.IsChecked = valor == "SI";
                    btnImageEdit.IsChecked = valor == "SIE";
                    btnImageText.IsChecked = valor == "IT";

                    // El último botón se marca si no coincide con ninguno de los anteriores (tu bloque 'else')
                    btnImageTextEdit.IsChecked = valor != "ST" && valor != "STE" && valor != "SI" && valor != "SIE" && valor != "IT";

                    break;
                case 41:
                    // Permite definir si el cierre de caja permitirá incluir los deliveries
                    chkIncCarrieCCajar.IsChecked = valor.Equals("1");
                    break;
                case 45:
                    // Define si los items de pedidos se mostrarán de forma horizontal o vertical
                    bool organiza = valor.Equals("H");
                    organizaProductosHor.IsChecked = organiza;
                    organizaProductosVer.IsChecked = !organiza;
                    break;
                case 69:
                    bool samllbutt = valor.Equals("1");
                    tamBotPedd1.IsChecked = samllbutt;
                    tamBotPedd2.IsChecked = !samllbutt;
                    break;
                case 70:
                    bool tambott = valor.Equals("1");
                    tamBotFact1.IsChecked = tambott;
                    tamBotFact2.IsChecked = !tambott;
                    break;
                case 71:
                    // Cantidad de dígitos que tendrá la parte decimal del valor en el código de barras
                    txtParteDecimal.setValue(valor);
                    break;
                case 72:
                    // Define el costo que tendrá el servicio en el restaurant
                    txtValServ.setValue(valor);
                    break;
                case 73:
                    // Permite definir si el valor del servicio será un cálculo porcentual o un valor fijo
                    cboCalcServ.SelectedIndex = valor.Equals("P") ? 0 : 1;
                    break;
                case 74:
                    // Código que permite identificar el producto en un código de barras
                    txtIndicador.Text = valor;
                    break;
                case 75:
                    // Cantidad de dígitos que tendrá el código definido previamente
                    if (int.Parse(valor) > 4) valor = "4";
                    txtCodigo.setValue(valor);
                    break;
                case 76:
                    // Cantidad de dígitos de la parte entera del código
                    txtParteEntera.setValue(valor);
                    break;
                case 77:
                    // Define el tipo de código de barras a utilizar
                    cboTipo.SelectedIndex = int.Parse(valor) - 1;
                    break;
                case 97:
                    // Cantidad de columnas que se presentarán en los pedidos
                    txtCntCols.setValue(valor);
                    break;
                case 113:
                    // Permite definir si la pantalla de pedidos se mostrarán los items en forma vertical u horizontal
                    chkPeddVert.IsChecked = valor.Equals("1");
                    break;
                case 138:
                    // Permite imprimir las comandas al momento de facturar
                    chkImpComandaCocina.IsChecked = valor.Equals("1");
                    break;
                case 139:
                    // Permite indica si en market se permite seleccionar mesas
                    chkSelMesasMarket.IsChecked = valor.Equals("1");
                    break;
                case 141:
                    // Permite indicar la forma en la cual se ordenarán los productos
                    cboOrderBy.SelectedIndex = valor switch
                    {
                        "C" => 0,
                        "N" => 1,
                        "A" => 2,
                        _ => 0 // Caso por defecto
                    };

                    break;
                case 144:
                    // Permite mostrar el cliente para llevar al momento de imprimir
                    chkMuestraClieLlevar.IsChecked = valor.Equals("1");
                    break;
                case 146:
                    // Permite definir si se pueden seleccionar productos usando código de barras
                    chkSelProdByBC.IsChecked = valor.Equals("1");
                    break;
                case 147:
                    // Permite realizar la búsqueda de productos
                    chkBscItems.IsChecked = valor.Equals("1");
                    break;
                case 148:
                    // Permite seleccionar productos del primer grupo
                    chkBscGrpSel.IsChecked = valor.Equals("1");
                    break;
                case 159:
                    // Permite que los tickets de las comandas presenten un secuencia que se reinicia cada día
                    chkSecPorPeriodo.IsChecked = valor.Equals("1");
                    break;
                case 164:
                    // Permite indicar que permitirá únicamente ventas locales
                    chkLocalOnly.IsChecked = valor.Equals("1");
                    break;
            }
        }

        private void cargarConfiguracionSeguridad(int id, string valor)
        {
            switch (id)
            {
                case 13:
                    // Cantidad de caracteres que tendrá el password
                    txtCntPswd.setValue(valor);
                    break;
                case 20:
                    // Permite saber si usará un password numérico
                    chkPswdNum.IsChecked = valor.Equals("1");
                    break;
                case 143:
                    // Modo de confirmación de la acción
                    //      "G" dará posición 1, "P" dará 2, "H" dará 3, "C" dará 4. Cualquier otra cosa dará -1.
                    int indice = "GPHC".IndexOf(valor) + 1;

                    //      Si el valor era vacío o "N", forzamos el índice 0. Si no coincide con nada, también cae en 0.
                    cboMdoConfir.SelectedIndex = (valor == "" || valor == "N" || indice == 0) ? 0 : indice;

                    break;
                case 157:
                    // Permite definir si se usará tarjeta de coordenadas
                    chkAlterCoord.IsChecked = valor.Equals("1");
                    break;
                case 158:
                    // Tiempo de vigencia del password
                    if (string.IsNullOrEmpty(valor)) valor = "60";
                    txtCadMinutos.setValue((int.Parse(valor) / 60).ToString());
                    break;
                case 166:
                    // Receptor debe ingresar confirmación
                    chkValidRecep.IsChecked = valor.Equals("1");
                    break;
            }
        }

        public void cargarConfiguracionImpresoras(int id, string valor)
        {
            switch (id)
            {
                case 121:
                    impresoras.Facturacion.tipo = valor;
                    break;
                case 122:
                    impresoras.Cocina.tipo = valor;
                    break;
                case 123:
                    impresoras.Parrilla.tipo = valor;
                    break;
                case 124:
                    impresoras.Bar.tipo = valor;
                    break;
                case 125:
                    impresoras.Facturacion.marca = valor;
                    break;
                case 126:
                    impresoras.Cocina.marca = valor;
                    break;
                case 127:
                    impresoras.Parrilla.marca = valor;
                    break;
                case 128:
                    impresoras.Bar.marca = valor;
                    break;
                case 129:
                    impresoras.Facturacion.usaEspacios = valor;
                    break;
                case 130:
                    impresoras.Cocina.usaEspacios = valor;
                    break;
                case 131:
                    impresoras.Parrilla.usaEspacios = valor;
                    break;
                case 132:
                    impresoras.Bar.usaEspacios = valor;
                    break;
            }
        }

        private void cargarConfiguracionImpresion(int id, string valor)
        {
            switch (id)
            {
                case 46:
                    // Permite la impresión de proformas
                    chkAllowProforma.IsChecked = valor.Equals("1");
                    break;
                case 47:
                    // Permite imprimir comandas en parrilla
                    chkAllowParrilla.IsChecked = valor.Equals("1");
                    break;
                case 49:
                    // Permite imprimir talonarios o comandas en el bar
                    chkAllowImpBar.IsChecked = valor.Equals("1");
                    break;
                case 78:
                    // Define el separado en las impresiones
                    //      "." da 0 (+1 = 1) | "*" da 1 (+1 = 2) | "=" da 2 (+1 = 3) | Si no existe da -1 (+1 = 0)
                    cboTipoSep.SelectedIndex = ".*=".IndexOf(valor) + 1;

                    break;
                case 79:
                    // Cantidad de columnas que tendrá el talonario a imprimir
                    // Intentamos convertir de forma segura. Si falla o es nulo, asignamos un valor que caiga en el default (por ejemplo, -1)
                    int confVal = int.TryParse(valor, out int parsed) ? parsed : -1;

                    cboCntCols.SelectedIndex = confVal switch
                    {
                        40 => 0,
                        23 => 2,
                        64 => 3,
                        _ => 1 // Caso por defecto
                    };

                    break;
                case 80:
                    // Imprime el nombre de la empresa
                    chkImpEmpr.IsChecked = valor.Equals("1");
                    break;
                case 81:
                    // Imprime la dirección de la empresa
                    chkImpDirEmp.IsChecked = valor.Equals("1");
                    break;
                case 82:
                    // Imprimie el ruc de la empresa
                    chkImpRuc.IsChecked = valor.Equals("1");
                    break;
                case 83:
                    // Imprime la cabecera en negrita
                    chkHeadBold.IsChecked = valor.Equals("1");
                    break;
                case 84:
                    // Determina la alineación de la cabecera
                    cboAlignHeader.SelectedIndex = valor switch
                    {
                        "I" => 0,
                        "D" => 2,
                        _ => 1 // Caso por defecto 
                    };

                    break;
                case 85:
                    // Imprime un separador en la cabecera
                    chkSepHead.IsChecked = valor.Equals("1");
                    break;
                case 86:
                    chkHeadTablaBold.IsChecked = valor.Equals("1");
                    break;
                case 87:
                    txtCntLineas.setValue(valor);
                    break;
                case 88:
                    chkSepFinItems.IsChecked = valor.Equals("1");
                    break;
                case 89:
                    txtCntDecItems.setValue(valor);
                    break;
                case 90:
                    // Imprime forma de pago en el talonario
                    chkFormPago.IsChecked = valor.Equals("1");
                    break;
                case 91:
                    // Incluye los datos del mesero en la factura
                    chkIncMesero.IsChecked = valor.Equals("1");
                    break;
                case 92:
                    // Incluye agradecimiento en la impresión
                    chkIncThanks.IsChecked = valor.Equals("1");
                    break;
                case 93:
                    // Permite imprimir un ticket al momento del cierre de caja
                    chkImprimeCierreCaja.IsChecked = valor.Equals("1");
                    break;
                case 95:
                    cboTipoImpresora.SelectedIndex = valor.Equals("I") ? 0 : 1;
                    break;
                case 96:
                    // Permite imprimir las cabeceras en las comandas
                    chkImpHeadCmds.IsChecked = valor.Equals("1");
                    CambiaImagenCabeceraComanda((bool)chkImpHeadCmds.IsChecked);
                    break;
                case 103:
                    // Permite imprimir saltos de línea en la cabecera
                    chkSalHeadCmds.IsChecked = valor.Equals("1");
                    CambiaImagenCabeceraSalto((bool)chkSalHeadCmds.IsChecked);
                    break;
                case 108:
                    // Imprime la opción de descarga de factura en el talonario de facturación
                    chkImpDownload.IsChecked = valor.Equals("1");
                    break;
                case 110:
                    // Imprime un talonario de retencíon
                    chkImpRetencion.IsChecked = valor.Equals("1");
                    break;
                case 119:
                    // Permite eliminar mostrar eliminaciones y anulaciones en el cierre de caja
                    chkIncElim.IsChecked = valor.Equals("1");
                    break;
                case 120:
                    chkEspBar.IsChecked = valor.Equals("1");
                    break;
                case 133:
                    txtCntCopias.setValue(valor);
                    break;
                case 134:
                    // Imprime la cabecera en formato expandido
                    chkCabExpand.IsChecked = valor.Equals("1");
                    break;
                case 145:
                    // Permite mostrar el nombre del cliente en la comanda
                    chkImpClieCmd.IsChecked = valor.Equals("1");
                    break;
                case 160:
                    // Imprime el secuencial temporal en el talonario
                    chkImpTmpSec.IsChecked = valor.Equals("1");
                    break;
            }
        }

        private void cargarConfiguracionSMTP(int id, string valor)
        {
            switch (id)
            {
                case 6:
                    // Servidor de correo electrónico
                    txtServer.Text = valor;
                    break;
                case 7:
                    // Puerto del servidor de correo
                    txtPuerto.Text = valor;
                    break;
                case 8:
                    // Usuario del correo electrónico
                    txtUsuario.Text = valor;
                    break;
                case 9:
                    // Clave del usuario de correo electrónico
                    txtClave.Password = valor;
                    break;
                case 11:
                    // Habilita TLS en el envío de mensajes de correo electrónico
                    chkTls.IsChecked = valor.Equals("1");
                    break;;
                case 14:
                    // Dirección de correo electrónico primario
                    txtCorreo1.Text = valor;
                    break;
                case 15:
                    // Habilita el envío de copias de correo electrónico a direcciones secundarias
                    chkCopiaEmail.IsChecked = valor.Equals("1");
                    break;
                case 16:
                    // Dirección de correo electrónico secundario
                    txtCorreo2.Text = valor;
                    break;
                case 17:
                    // Drección de correo electrónico terciario
                    txtCorreo3.Text = valor;
                    break;
                case 18:
                    // Dirección de correo electrónico cuaternario
                    txtCorreo4.Text = valor;
                    break;
                case 19:
                    // Habilita el envío de correos en la plataforma
                    chkEnvEmail.IsChecked = valor.Equals("1");
                    break;
                case 111:
                    // Utiliza la plataforma de mail relay para envío de correo electrónico
                    chkUsaRelay.IsChecked = valor.Equals("1");
                    break;
                case 112:
                    // Token del servidor mail relay
                    txtTokenMail.Text = valor;
                    break;
                case 149:
                    // Dirección URL del servidor Mail Relay
                    txtTSvrMailRelay.Text = valor;
                    break;
                case 150:
                    // Envía mensajes vía SMS
                    chkUsaSMS.IsChecked = valor.Equals("1");
                    break;
                case 151:
                    // Token del servidor para envío de Whatsapp
                    txtTokenWhatsapp.Text = valor;
                    break;
                case 152:
                    // Direción URL del API para envio de mensajes de Whatsapp
                    txtSvrWhatsapp.Text = valor;
                    break;
                case 153:
                    // Token del servidor para envío de mensajes SMS
                    txtTokenSms.Text = valor;
                    break;
                case 154:
                    // Direccion URL del API para envio de mensajes SMS
                    txtSvrSms.Text = valor;
                    break;
                case 155:
                    // Habilita el envío de mensajes vía whatsapp
                    chkUsaWhatsapp.IsChecked = valor.Equals("1");
                    break;
                case 156:
                    // Utiliza la plataforma de mail relay para envío de correo electrónico
                    chkUsaRelay.IsChecked = valor.Equals("1");
                    break;
            }
        }

        private void cargarConfiguracionFacturacion(int id, string valor)
        {
            switch (id)
            {
                case 54:
                    // Usa facturación electrónica
                    chkUseFactElec.IsChecked = valor.Equals("1");
                    break;
                case 55:
                    // Usa el envío de facturas al sri de forma automática
                    chkUseOnline.IsChecked = valor.Equals("1");
                    break;
                case 56:
                    // Usa el envio de las facturas al sri de manera online
                    chkUseOnline.IsChecked = valor.Equals("1");
                    break;
                case 57:
                    // Dirección o path del archivo de firma electrónica, incluye el nombre del archivo de firma eletrónica
                    txtArchFirma.Text = valor;
                    break;
                case 58:
                    // Hora de envio de las facturas de forma automática al SRI
                    if (!string.IsNullOrEmpty(valor))
                    {
                        horaInicio.setValue(valor);
                    }
                    break;
                case 59:
                    // Minutos para el reintento en caso de fallo del envio al SRI
                    txtMinutos.Text = valor;
                    break;
                case 60:
                    // Identificador de la empresa
                    txtEmpresaGuid.Text = valor;
                    break;
                case 61:
                    // Dirección URL del API de facturación electrónica
                    txtURLWS.Text = valor;
                    break;
                case 62:
                    // Usuario del API de facturación electrónica
                    txtUsuarioWS.Text = valor;
                    break;
                case 63:
                    // Clave del usuario del API de facturación electrónica
                    txtClaveWS.Text = valor;
                    break;
                case 64:
                    // Clave de la firma electrónica
                    txtClaveFirma.Text = valor;
                    break;
                case 65:
                    // Ruta en la que se generarán los archivos xml de las facturas, retenciones o notas de crédito
                    txtFolder.Text = valor;
                    break;
                case 66:
                    // Ruta, path y nobre del archivo que representan el logo de la empresa
                    txtLogo.Text = valor;
                    this.ubicacionLogoOrig = valor;
                    break;
                case 109:
                    txtURLApp.Text = valor;
                    break;
            }
        }

        private void cargarConfiguracionEmpresa(int id, string valor)
        {
            switch (id)
            {
                case 67:
                    txtNumFact.Text = valor ?? "001";
                    break;
                case 68:
                    chkRimpe.IsChecked = valor.Equals("1");
                    break;
                case 9989:
                    chkContabilidad.IsChecked = valor.Equals("1");
                    break;
                case 9990:
                    chkRetencion.IsChecked = valor.Equals("1");
                    break;
                case 9991:
                    // Definición de la razón social de la empresa
                    txtRazonSocial.Text = valor;
                    break;
                case 9992:
                    txtEstablecimiento.Text = valor;
                    break;
                case 9993:
                    txtDirMatriz.Text = valor;
                    break;
                case 9994:
                    txtDirLocal.Text = valor;
                    break;
                case 9995:
                    // Definición del logotipo de la empresa
                    // 1. Evaluar y asignar el origen de la imagen en un solo paso
                    imgLogo.Source = !string.IsNullOrEmpty(valor)
                        ? General.BtyeArrayToBitmapImage(General.base64ToByteArray(valor))
                        : new BitmapImage(new Uri("pack://application:,,,/images/no-image-available.png"));

                    // 2. Asignar el estado usando lógica condicional simplificada
                    this._img = !string.IsNullOrEmpty(valor) ? "*" : this._img;

                    break;
                case 9998:
                    // Definición del RUC de una empresa
                    txtEmpRuc.Text = valor;
                    break;
                case 9999:
                    // Nombre de la empresa
                    txtEmpName.Text = valor;
                    break;
            }
        }

        public void load()
        {
            ConfiguracionSys config = new ConfiguracionSys(General.connectionString);

            DataTable dt = config.load();

            foreach (DataRow dr in dt.Rows)
            {
                int id = dr.Field<int>("confNid");
                string valor = dr["confVal"]?.ToString() ?? "";

                switch (id)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 12:
                    case 21:
                    case 22:
                    case 42:
                    case 48:
                    case 98:
                    case 140:
                    case 142:
                    case 161:
                    case 162:
                    case 163:
                    case 165:
                    case 167:
                    case 168:
                        cargarConfiguracionGeneral(id, valor);
                        break;
                    case 4:
                    case 50:
                    case 51:
                    case 52:
                    case 53:
                        cargarConfiguracionMesas(id, valor);
                        break;
                    case 23:
                    case 24:
                    case 25:
                    case 27:
                    case 28:
                    case 39:
                    case 40:
                    case 41:
                    case 45:
                    case 69:
                    case 70:
                    case 71:
                    case 72:
                    case 73:
                    case 74:
                    case 75:
                    case 76:
                    case 77:
                    case 97:
                    case 113:
                    case 138:
                    case 139:
                    case 141:
                    case 144:
                    case 146:
                    case 147:
                    case 148:
                    case 159:
                    case 164:
                        cargarConfiguracionPedidosVentas(id, valor);
                        break;
                    case 13:
                    case 20:
                    case 143:
                    case 157:
                    case 158:
                    case 166:
                        cargarConfiguracionSeguridad(id, valor);
                        break;
                    case 121:
                    case 122:
                    case 123:
                    case 124:
                    case 125:
                    case 126:
                    case 127:
                    case 128:
                    case 129:
                    case 130:
                    case 131:
                    case 132:
                        cargarConfiguracionImpresoras(id, valor);
                        break;
                    case 46:
                    case 47:
                    case 49:
                    case 78:
                    case 79:
                    case 80:
                    case 81:
                    case 82:
                    case 83:
                    case 84:
                    case 85:
                    case 86:
                    case 87:
                    case 88:
                    case 89:
                    case 90:
                    case 91:
                    case 92:
                    case 93:
                    case 95:
                    case 96:
                    case 103:
                    case 108:
                    case 110:
                    case 119:
                    case 120:
                    case 133:
                    case 134:
                    case 145:
                    case 160:
                        cargarConfiguracionImpresion(id, valor);
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 11:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 111:
                    case 112:
                    case 149:
                    case 150:
                    case 151:
                    case 152:
                    case 153:
                    case 154:
                    case 155:
                    case 156:
                        cargarConfiguracionSMTP(id, valor);
                        break;
                    case 54:
                    case 55:
                    case 56:
                    case 57:
                    case 58:
                    case 59:
                    case 60:
                    case 61:
                    case 62:
                    case 63:
                    case 64:
                    case 65:
                    case 66:
                    case 109:
                        cargarConfiguracionFacturacion(id, valor);
                        break;
                    case 67:
                    case 68:
                    case 9989:
                    case 9990:
                    case 9991:
                    case 9992:
                    case 9993:
                    case 9994:
                    case 9995:
                    case 9998:
                    case 9999:
                        cargarConfiguracionEmpresa(id, valor);
                        break;
                    
                }
            }
        }

        private void toggle(string tag, Boolean activa)
        {
            foreach (System.Windows.Controls.Control control in tabTemas.Children)
            {
                ToggleButton tgb = (ToggleButton)control;

                if (!tgb.Tag.Equals(tag))
                {
                    tgb.IsChecked = activa;
                }
            }

            General.TEMA_REPORTE = int.Parse(tag);

        }

        public void toggle_Checked(object sender, RoutedEventArgs e)
        {
            this.toggle(((ToggleButton)sender).Tag.ToString(), false);
        }

        public void toggle_UnChecked(object sender, RoutedEventArgs e)
        {
            //this.toggle(((ToggleButton)sender).Tag.ToString(), false);
        }

        private async void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Inicializamos una lista para acumular errores de forma limpia y agregar saltos de línea al final
                var errores = new List<string>();

                if (chkEnvEmail.IsChecked == true)
                {
                    if (chkUsaRelay.IsChecked == false)
                    {
                        string servidor = txtServer.Text.Trim();
                        string puertoText = txtPuerto.Text.Trim();
                        string usuario = txtUsuario.Text;
                        string clave = txtClave.Password;

                        if (string.IsNullOrEmpty(servidor))
                            errores.Add("El campo servidor de correo no puede estar en blanco.");

                        if (string.IsNullOrEmpty(puertoText))
                        {
                            errores.Add("El puerto no puede estar en blanco.");
                        }
                        // General.parseInt debe validar el formato; usamos int.TryParse para extraer el número de forma segura
                        else if (!int.TryParse(puertoText, out int puerto) || puerto < 1 || puerto > 65535)
                        {
                            errores.Add("El número del puerto es inválido, debe ser un entero entre 1 y 65535.");
                        }

                        if (string.IsNullOrEmpty(usuario))
                            errores.Add("El campo usuario no puede estar en blanco.");
                        else if (usuario.Length < 4)
                            errores.Add("El usuario debe tener más de 4 caracteres.");

                        if (string.IsNullOrEmpty(clave))
                            errores.Add("El campo contraseña o clave no puede estar en blanco.");
                        else if (clave.Length < 4)
                            errores.Add("La contraseña o clave debe tener más de 4 caracteres.");
                    }
                    else if (txtTokenMail.Text.Length < 4)
                    {
                        errores.Add("El token del Mail no es válido.");
                    }

                    // Simplificación de la validación del correo principal
                    string correo1 = txtCorreo1.Text;
                    if (!string.IsNullOrEmpty(correo1) && !General.validaEmail(correo1))
                    {
                        errores.Add("La dirección de correo principal no es válida.");
                    }

                    if (chkCopiaEmail.IsChecked == true)
                    {
                        // Validamos correos secundarios solo si no están vacíos
                        if (!string.IsNullOrEmpty(txtCorreo2.Text) && !General.validaEmail(txtCorreo2.Text))
                            errores.Add("La dirección de correo secundaria 1 no es válida.");

                        if (!string.IsNullOrEmpty(txtCorreo3.Text) && !General.validaEmail(txtCorreo3.Text))
                            errores.Add("La dirección de correo secundaria 2 no es válida.");

                        if (!string.IsNullOrEmpty(txtCorreo4.Text) && !General.validaEmail(txtCorreo4.Text))
                            errores.Add("La dirección de correo secundaria 3 no es válida.");
                    }
                }

                // === VALIDACIONES DE EMPRESA ===
                string ruc = txtEmpRuc.Text.Trim();
                if (string.IsNullOrEmpty(ruc))
                {
                    errores.Add("El Ruc de la empresa no puede estar en blanco.");
                }
                else
                {
                    AlgoritmosValidacion val = new AlgoritmosValidacion();
                    if (!val.validaRuc(ruc)) errores.Add("El Número de Ruc no es válido.");
                }

                if (string.IsNullOrEmpty(txtEmpName.Text))
                {
                    errores.Add("El Nombre de la empresa no puede estar en blanco.");
                }

                if (chkUseFactElec.IsChecked == true)
                {
                    if (string.IsNullOrEmpty(txtArchFirma.Text))
                        errores.Add("El campo para el archivo de firma digital no puede estar en blanco.");

                    if (string.IsNullOrEmpty(txtEmpresaGuid.Text))
                        errores.Add("El campo identificador de empresa no puede estar en blanco.");
                }

                // === VALIDACIONES DE CÓDIGO DE BARRAS (BARCODE) ===
                // Parseos seguros con TryParse para evitar caídas del sistema si los campos están vacíos
                int parteEntera = int.TryParse(txtParteEntera.getValue(), out int pe) ? pe : 0;
                int parteDecimal = int.TryParse(txtParteDecimal.getValue(), out int pd) ? pd : 0;
                int codigoBC = int.TryParse(txtCodigo.getValue(), out int cbc) ? cbc : 0;

                int totalDecimalBC = parteEntera + parteDecimal;
                int totalIdentificadorBC = txtIndicador.Text.Length;
                int totalBC = totalDecimalBC + totalIdentificadorBC + codigoBC;

                if (totalDecimalBC > 5)
                {
                    errores.Add("El identificador de peso definido por la Parte Entera y Decimal deben sumar un máximo de 5 caracteres.");
                }

                // CORRECCIÓN LÓGICA: Se cambió == por != para que dispare el error cuando NO cumpla la longitud requerida
                int longitudRequerida = cboTipo.SelectedIndex == 0 ? 13 : 12;
                if (totalBC != longitudRequerida)
                {
                    errores.Add($"No cumple con el total de dígitos que debe tener el código de barras. Debe tener {longitudRequerida} dígitos.");
                }

                // === EVALUACIÓN FINAL ===
                if (errores.Any())
                {
                    // Une todos los errores agregando un salto de línea entre cada uno para que se lea perfectamente
                    string mensajeFinal = string.Join("\n", errores);
                    ObjComportamiento.showMsgError(mensajeFinal);
                    return;
                }

                ConfiguracionSys config = new ConfiguracionSys(General.connectionString);

                List<mConfig> data = new List<mConfig> {
                    new mConfig { codigo = 1, valor = chkPrcIva.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 2, valor = txtIva.getValue() },
                    new mConfig { codigo = 3,  valor = chkTouch.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 4,  valor = txtMesas.getValue() },

                    new mConfig { codigo = 6,  valor = txtServer.Text },
                    new mConfig { codigo = 7,  valor = txtPuerto.Text },
                    new mConfig { codigo = 8,  valor = txtUsuario.Text },
                    new mConfig { codigo = 9,  valor = txtClave.Password }, // Password ya devuelve un string
                    new mConfig { codigo = 11, valor = chkTls.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 12, valor = chkCtrlInv.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 13, valor = txtCntPswd.getValue() },
                    new mConfig { codigo = 14, valor = txtCorreo1.Text },
                    new mConfig { codigo = 15, valor = chkCopiaEmail.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 16, valor = txtCorreo2.Text },
                    new mConfig { codigo = 17, valor = txtCorreo3.Text },
                    new mConfig { codigo = 18, valor = txtCorreo4.Text },
                    new mConfig { codigo = 19, valor = chkEnvEmail.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 20, valor = chkPswdNum.IsChecked == true ? "1" : "0" },

                    new mConfig { codigo = 21, valor = chkDsctManual.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 22, valor = General.TEMA_REPORTE.ToString() },
                    new mConfig { codigo = 23, valor = chkCambioRapido.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 24, valor = txtInicioScl.getValue() },
                    new mConfig { codigo = 25, valor = chkImpHoraCocina.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 27, valor = ticketCantidadIzq ? "1" : "0" }, 
                    new mConfig { codigo = 28, valor = chkImpTicketCarrier.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 39, valor = chkModoFact.IsChecked == true ? "F-P" : "FP" },
                    new mConfig { codigo = 40, valor = tipoBotones },
                    new mConfig { codigo = 41, valor = chkIncCarrieCCajar.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 42, valor = chkCierreAcum.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 45, valor = organizaProductosHor.IsChecked == true ? "H" : "V" },
                    new mConfig { codigo = 46, valor = usaImpresoraProforma ? "1" : "0" },
                    new mConfig { codigo = 47, valor = usaImpresoraParrilla ? "1" : "0" },
                    new mConfig { codigo = 48, valor = cboFormatDec.SelectedIndex == 0 ? "###,###.##" : "###.###,##" },
                    new mConfig { codigo = 49, valor = chkAllowImpBar.IsChecked == true ? "1" : "0" },

                    new mConfig { codigo = 50, valor = chkAlterna.IsChecked == true ? txtMesasAlt.getValue() : "0" },
                    new mConfig { codigo = 51, valor = chkExterior.IsChecked == true ? txtMesasExt.getValue() : "0" },
                    new mConfig { codigo = 52, valor = chkTerraza.IsChecked == true ? txtMesasTerr.getValue() : "0" },
                    new mConfig { codigo = 53, valor = chk2Piso.IsChecked == true ? txtMesasSPis.getValue() : "0" },

                    new mConfig { codigo = 54, valor = chkUseFactElec.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 55, valor = chkUseAuto.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 56, valor = chkUseOnline.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 57, valor = txtArchFirma.Text },
                    new mConfig { codigo = 58, valor = horaInicio.getValue() },
                    new mConfig { codigo = 59, valor = txtMinutos.Text },
                    new mConfig { codigo = 60, valor = txtEmpresaGuid.Text },
                    new mConfig { codigo = 61, valor = txtURLWS.Text },
                    new mConfig { codigo = 62, valor = txtUsuarioWS.Text },
                    new mConfig { codigo = 63, valor = txtClaveWS.Text },
                    new mConfig { codigo = 64, valor = txtClaveFirma.Text },
                    new mConfig { codigo = 65, valor = txtFolder.Text },
                    new mConfig { codigo = 66, valor = txtLogo.Text },
                    new mConfig { codigo = 67, valor = txtNumFact.Text },
                    new mConfig { codigo = 68, valor = chkRimpe.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 69, valor = tamBotPedd1.IsChecked == true ? "1" : "2" },
                    new mConfig { codigo = 70, valor = tamBotFact1.IsChecked == true ? "1" : "2" },
                    new mConfig { codigo = 71, valor = txtParteDecimal.getValue() },
                    new mConfig { codigo = 72, valor = txtValServ.getValue() },
                    new mConfig { codigo = 73, valor = cboCalcServ.SelectedIndex == 0 ? "P" : "V" },
                    new mConfig { codigo = 74, valor = txtIndicador.Text },
                    new mConfig { codigo = 75, valor = txtCodigo.getValue() },
                    new mConfig { codigo = 76, valor = txtParteEntera.getValue() },

                    new mConfig { codigo = 78, valor = cboTipoSep.SelectedIndex switch { 1 => ".", 2 => "*", 3 => "=", _ => "-" } },

                    new mConfig { codigo = 79, valor = cboCntCols.SelectedIndex switch { 0 => "40", 2 => "23", 3 => "64", _ => "48" } },

                    new mConfig { codigo = 80, valor = chkImpEmpr.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 81, valor = chkImpDirEmp.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 82, valor = chkImpRuc.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 83, valor = chkHeadBold.IsChecked == true ? "1" : "0" },

                    new mConfig { codigo = 84, valor = cboAlignHeader.SelectedIndex switch { 0 => "I", 2 => "D", _ => "C" } },

                    new mConfig { codigo = 85, valor = chkSepHead.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 86, valor = chkHeadTablaBold.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 87, valor = txtCntLineas.getValue() },
                    new mConfig { codigo = 88, valor = chkSepFinItems.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 89, valor = txtCntDecItems.getValue() },
                    new mConfig { codigo = 90, valor = chkFormPago.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 91, valor = chkFormPago.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 92, valor = chkIncThanks.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 95, valor = cboTipoImpresora.SelectedIndex == 0 ? "I" : "T" },
                    new mConfig { codigo = 96, valor = chkImpHeadCmds.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 97, valor = txtCntCols.getValue() },
                    new mConfig { codigo = 98, valor = DocuCrea.Valor },
                    new mConfig { codigo = 103, valor = chkSalHeadCmds.IsChecked == true ? "1" : "0" },

                    new mConfig { codigo = 108, valor = chkImpDownload.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 109, valor = txtURLApp.Text },
                    new mConfig { codigo = 110, valor = chkImpRetencion.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 111, valor = chkUsaRelay.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 112, valor = txtTokenMail.Text },
                    new mConfig { codigo = 113, valor = chkPeddVert.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 119, valor = chkIncElim.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 120, valor = chkEspBar.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 133, valor = txtCntCopias.getValue() },
                    new mConfig { codigo = 134, valor = chkCabExpand.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 138, valor = chkImpComandaCocina.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 139, valor = chkSelMesasMarket.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 140, valor = chkAllowCredito.IsChecked == true ? "1" : "0" },

                    new mConfig { codigo = 141, valor = cboOrderBy.SelectedIndex switch { 1 => "N", 2 => "A", _ => "C" } },
                    new mConfig { codigo = 142, valor = cboTiempoCorte.SelectedIndex == 0 ? "Q" : "M" },
                    new mConfig { codigo = 143, valor = cboMdoConfir.SelectedIndex switch { 1 => "G", 2 => "P", 3 => "H", 4 => "C", _ => "N" } },
                    new mConfig { codigo = 144, valor = chkMuestraClieLlevar.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 145, valor = chkImpClieCmd.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 146, valor = chkSelProdByBC.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 147, valor = chkBscItems.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 148, valor = chkBscGrpSel.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 149, valor = txtTSvrMailRelay.Text },
                    new mConfig { codigo = 151, valor = txtTokenWhatsapp.Text },
                    new mConfig { codigo = 152, valor = txtSvrWhatsapp.Text },
                    new mConfig { codigo = 153, valor = txtTokenSms.Text },
                    new mConfig { codigo = 154, valor = txtSvrSms.Text },
                    new mConfig { codigo = 155, valor = chkUsaWhatsapp.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 156, valor = chkUsaRelay.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 157, valor = chkAlterCoord.IsChecked == true ? "1" : "0" },

                    new mConfig { codigo = 158, valor = (60 * (int.TryParse(txtCadMinutos.getValue(), out int m) ? m : 0)).ToString() },
                    new mConfig { codigo = 159, valor = chkSecPorPeriodo.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 160, valor = chkImpTmpSec.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 161, valor = chkFondoRandom.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 162, valor = fondoLogin.ToString() },
                    new mConfig { codigo = 163, valor = cboTemaLogin.SelectedIndex.ToString() },
                    new mConfig { codigo = 164, valor = chkLocalOnly.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 165, valor = (string.IsNullOrEmpty(txtIvaFeriado.getValue()) || txtIvaFeriado.getValue() == "0") ? txtIva.getValue() : txtIvaFeriado.getValue() },
                    new mConfig { codigo = 166, valor = chkValidRecep.IsChecked == true ? "1" : "0" },
                    new mConfig { codigo = 167, valor = txtStockMin.getValue() },
                    new mConfig { codigo = 168, valor = chkIncPrc.IsChecked == true ? "1": "0" },
                    new mConfig { codigo = 169, valor = txtRucDevelop.Text },

                    // -----------------------------------------------------------------------------------------------
                    new mConfig() { codigo = 9988, valor = General.ImageToBase64(General.bitmapImageToBytes(txtLogo.Text)) },
                    new mConfig() { codigo = 9989, valor = chkContabilidad.IsChecked == true ? "1" : "0" },
                    new mConfig() { codigo = 9990, valor = chkRetencion.IsChecked == true ? "1" : "0" },
                    new mConfig() { codigo = 9991, valor = txtRazonSocial.Text },
                    new mConfig() { codigo = 9992, valor = txtEstablecimiento.Text },
                    new mConfig() { codigo = 9993, valor = txtDirMatriz.Text },
                    new mConfig() { codigo = 9994, valor = txtDirLocal.Text },
                    new mConfig { codigo = 9995, valor = this._img == "" ? "" : (string.IsNullOrEmpty(this._img) ? "" : General.ImageToBase64(General.bitmapImageToBytes(this._img))) },
                };

                if (cboTipoImpresora.SelectedIndex == 2)
                {
                    data.AddRange(
                        new[] { 
                            new mConfig { codigo = 121, valor = impresoras.Facturacion.tipo }, 
                            new mConfig { codigo = 122, valor = impresoras.Cocina.tipo }, 
                            new mConfig { codigo = 123, valor = impresoras.Parrilla.tipo }, 
                            new mConfig { codigo = 124, valor = impresoras.Bar.tipo }, 
                            new mConfig { codigo = 125, valor = impresoras.Facturacion.marca }, 
                            new mConfig { codigo = 126, valor = impresoras.Cocina.marca }, 
                            new mConfig { codigo = 127, valor = impresoras.Parrilla.marca }, 
                            new mConfig { codigo = 128, valor = impresoras.Bar.marca }, 
                            new mConfig { codigo = 129, valor = impresoras.Facturacion.usaEspacios }, 
                            new mConfig { codigo = 130, valor = impresoras.Cocina.usaEspacios }, 
                            new mConfig { codigo = 131, valor = impresoras.Parrilla.usaEspacios }, 
                            new mConfig { codigo = 132, valor = impresoras.Bar.usaEspacios }, 
                            new mConfig { codigo = 135, valor = impresoras.Cocina.letraGrande }, 
                            new mConfig { codigo = 136, valor = impresoras.Parrilla.letraGrande }, 
                            new mConfig { codigo = 137, valor = impresoras.Bar.letraGrande } 
                        }
                    );
                }


                AlgoritmosValidacion validador = new AlgoritmosValidacion();
                GeneradorID numgen = validador.generaValidador(txtEmpRuc.Text);

                data.AddRange(
                    new[]
                    {
                        new mConfig { codigo = 9996, valor = numgen.numero },
                        new mConfig { codigo = 9997, valor = numgen.validador },
                        new mConfig { codigo = 9998, valor = txtEmpRuc.Text },
                        new mConfig { codigo = 9999, valor = txtEmpName.Text }
                    }    
                );

                Respuesta respuesta = config.grabar(data);
                ObjComportamiento.showMuestraMensaje(respuesta);

                Boolean sistValidado = General.SISTEMA_VALIDADO;

                General.loadParametros();

                Configuracion configXml = new Configuracion();
                Config actual = configXml.readConfig();
                General.generaPrintersConfig(actual);

                bool debeSubirLogo = !string.IsNullOrEmpty(txtLogo.Text)
                                        && respuesta.resultado
                                        && chkUseFactElec.IsChecked == true
                                        && txtLogo.Text != ubicacionLogoOrig;

                Boolean sistValidadoLoad = General.SISTEMA_VALIDADO;

                if (debeSubirLogo)
                {
                    try
                    {
                        uploading.Visibility = Visibility.Visible;
                        General.PROCESANDO = true;

                        HttpClienteAPI client = new HttpClienteAPI { TrsklUrl = General.URL_WS };

                        // 1. Ejecución asíncrona real y limpia sin congelar la interfaz (Eliminado Task.Run().Result)
                        var resp = await Task.Run(() => client.Login(
                            General.USUARIO_WS,
                            General.CLAVE_WS,
                            General.RUC_EMPRESA,
                            General.EMPRESA_GUID
                        ));

                        client.Token = resp.Token;

                        byte[] logo = await Task.Run(() => File.ReadAllBytes(txtLogo.Text));

                        LibHttpClient.Model.UsuarioEmpresa payload = new LibHttpClient.Model.UsuarioEmpresa
                        {
                            Ruc = General.RUC_EMPRESA,
                            EmpresaId = General.EMPRESA_GUID,
                            logotipo = logo
                        };

                        // 3. Envío al API esperando la respuesta de forma asíncrona
                        await Task.Run(() => client.sendLogoFactura(payload));

                    }
                    catch (Exception)
                    {
                        ObjComportamiento.showMsgError("El logotipo de la factura no pudo ser enviado al servidor");
                    }
                    finally
                    {
                        // 4. El bloque finally asegura que el indicador se oculte SIEMPRE, incluso si ocurre un error
                        uploading.Visibility = Visibility.Hidden;
                        General.PROCESANDO = false;
                    }
                }

                if (sistValidado != sistValidadoLoad)
                {
                    ObjComportamiento.showMsgInfo("Puesto que el sistema fue configurado y validado por primera vez, Ud, debe proceder a reiniciar el sistema, para que los cambios realizados se actualicen");
                }
            }
            catch (Exception er)
            {
                ObjComportamiento.showMsgError(er.Message);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private void chkCopiaEmail_Checked(object sender, RoutedEventArgs e)
        {
            ActivaCopiaMail(true);
        }

        private void chkCopiaEmail_Unchecked(object sender, RoutedEventArgs e)
        {
            ActivaCopiaMail(false);
        }

        private void chkEnvEmail_Checked(object sender, RoutedEventArgs e)
        {
            ActivaOpcEmail(true);
        }

        private void chkEnvEmail_Unchecked(object sender, RoutedEventArgs e)
        {
            ActivaOpcEmail(false);
        }

        public void ActivaOpcEmail(Boolean valor)
        {
            txtServer.IsEnabled = valor;
            txtClave.IsEnabled = valor;
            txtUsuario.IsEnabled = valor;
            txtPuerto.IsEnabled = valor;
            txtCorreo1.IsEnabled = valor;
            chkCopiaEmail.IsEnabled = valor;
            chkTls.IsEnabled = valor;
        }

        public void ActivaCopiaMail(Boolean valor)
        {
            txtCorreo2.IsEnabled = valor;
            txtCorreo3.IsEnabled = valor;
            txtCorreo4.IsEnabled = valor;
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
        // ~ctlConfiguracion() {
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

        private void btnScl_Click(object sender, RoutedEventArgs e)
        {
            nKeyboard.MaxLength = 999999999;

            nKeyboard.setTextBox = txtInicioScl;

            nKeyboard.ShowDialog();
        }

        private void TextoNumerico_Retorna(Object txt, String texto)
        {
            if (texto.Equals(""))
            {
                txtInicioScl.setValue("1");
                return;
            }

            txtInicioScl.setValue(texto);
        }

        private void ticketCocina1_Checked(object sender, RoutedEventArgs e)
        {
            this.ticketCantidadIzq = false;
        }

        private void ticketCocina2_Checked(object sender, RoutedEventArgs e)
        {
            this.ticketCantidadIzq = true;
        }

        private void srcLogo_Click(object sender, RoutedEventArgs e)
        {
            _img = General.getImageFromFile(imgLogo);
        }

        private void delLogo_Click(object sender, RoutedEventArgs e)
        {
            this._img = "";
            imgLogo.Source = new BitmapImage(new Uri("pack://application:,,,/images/no-image-available.png"));
        }

        private void btnGetKey_Click(object sender, RoutedEventArgs e)
        {
            string ruc = txtEmpRuc.Text;
            string producto = "";
            string licencia = ((ComboBoxItem)cboTipoLic.SelectedItem).Content.ToString();
            string mainboard = General.MAINBOARD_SERIAL;


            string mensaje = "Se va a solicitar la clave vía correo, verifique que su equipo tiene conexión a internet, ¿Desea continuar?";
            if (ObjComportamiento.showMsgYesNo(mensaje) == MessageBoxResult.Yes)
            {
                String titulo = "Solicitud de Número de Licencia.";
                mensaje = "<h1>Solicitud de Número de Licencia.</h1>";
                mensaje += "<hr>";
                mensaje += "<b>Empresa   : </b>" + txtEmpName.Text + "<br>";
                mensaje += "<b>Ruc       : </b>" + txtEmpRuc.Text + "<br>";
                mensaje += "<b>Correo    : </b>" + txtCorreo1.Text + "<br>";
                mensaje += "<b>Codigo PC : </b>" + txtProdID.Text + "<br>";
                mensaje += "<b>Licencia  : </b>" + ((ComboBoxItem)cboTipoLic.SelectedItem).Content.ToString() + "<br>";

                Email correo = new Email(General.CORREO_HOST, General.CORREO_PORT, General.CORREO_USER,
                                         General.CORREO_PASSWORD, General.CORREO_AUTH, General.CORREO_UTLS,
                                         General.SUCURSAL);
                correo.subject = titulo;
                correo.body = mensaje;


                Thread hiloCorreo = new Thread(new ThreadStart(correo.solicitarLicencia));
                hiloCorreo.Start();

                btnPasteKey.IsEnabled = true;
            }

        }

        private void generaLicencia()
        {
            LicenseInfo info = new LicenseInfo();
            info.PCName = Environment.MachineName;
            info.OperatingSystem = Environment.OSVersion.ToString();
            info.Version = "V4.0";

            CryptLicense license = new CryptLicense();
            DeviceIdManager devId = new DeviceIdManager();

            switch (cboTipoLic.SelectedIndex)
            {
                case 0:
                    info.licenseType = LicenseType.TRIAL;
                    info.ExpirationDate = DateTime.Now.Date.AddDays(30).Date.ToString();
                    break;
                case 1:
                    info.licenseType = LicenseType.FULL;
                    info.ExpirationDate = null;
                    break;
                case 2:
                    info.licenseType = LicenseType.ANUAL;
                    info.ExpirationDate = DateTime.Now.Date.AddYears(1).Date.ToString();
                    break;
            }

            if (!license.generar(info, txtProdID.Text))
            {
                ObjComportamiento.showMsgError("No fue posible generar la clave");
            }

            txtProdKey.Text = license.ClaveActivacion;

        }

        private void btnRegisterKey_Click(object sender, RoutedEventArgs e)
        {
            LicenseInfo info = new LicenseInfo();


        }

        private void btnGenerateKey_Click(object sender, RoutedEventArgs e)
        {
            if (cboTipoLic.SelectedIndex >= 0)
            {
                gridKeyboard.ShowDialog();
            }
            else
            {
                ObjComportamiento.showMsgError("Debe seleccionar el tipo de licenciamiento");
            }
        }

        private void RetornaValidacion(Boolean respuesta)
        {
            if (respuesta)
            {
                this.generaLicencia();
            }
            else
            {
                ObjComportamiento.showMsgError("Clave para generación de Licencia incorrecta");
            }
        }

        private void btnPasteKey_Click(object sender, RoutedEventArgs e)
        {
            string texto = Clipboard.GetText();
            if (texto.Length == 35)
            {
                string[] aTexto = texto.Split('-');

                if (aTexto.Length == 6)
                {
                   /* KeyManager km = new KeyManager(txtProdID.Text);
                    if (km.ValidKey(ref texto))
                    {
                        txtProdKey.Text = Clipboard.GetText();
                    }
                    else
                    {
                        ObjComportamiento.showMsgError("El código ingresado no es correcto");
                    }*/
                }
                else
                {
                    ObjComportamiento.showMsgError("El código ingresado no tiene el formato correcto");
                }
            }
            else
            {
                ObjComportamiento.showMsgError("El código ingresado no tiene la longitud necesaria");
            }
        }

        private void chkModoFact_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkModoFact.IsChecked == false)
            {
                if (chkCambioRapido != null)
                {
                    chkCambioRapido.IsEnabled = false;
                    chkCambioRapido.IsChecked = false;
                    chkImpComandaCocina.IsChecked = false;
                    chkImpComandaCocina.IsEnabled = true;
                    chkSelMesasMarket.IsEnabled = true;
                    chkSelMesasMarket.IsChecked = false;
                }
            }
        }

        private void chkModoFact_Checked(object sender, RoutedEventArgs e)
        {
            if (chkModoFact.IsChecked == true)
            {
                if (chkCambioRapido != null)
                {
                    chkCambioRapido.IsChecked = true;
                    chkCambioRapido.IsEnabled = true;
                    chkImpComandaCocina.IsChecked = false;
                    chkImpComandaCocina.IsEnabled = false;
                    chkSelMesasMarket.IsEnabled = false;
                    chkSelMesasMarket.IsChecked = false;
                }
            }
        }

        private void chkCambioRapido_Checked(object sender, RoutedEventArgs e)
        {
            if (chkCambioRapido.IsChecked == true)
            {
                if (chkModoFact != null)
                {
                    chkModoFact.IsChecked = true;
                    chkModoFact.IsEnabled = true;
                }
            }
        }

        private void chkCambioRapido_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkCambioRapido.IsChecked == false)
            {
                if (chkModoFact != null)
                    chkModoFact.IsEnabled = true;
            }
        }

        private void organizaProductosHor_Checked(object sender, RoutedEventArgs e)
        {
            OrganizacionHorizontal = true;

            if (organizaProductosHor != null && OpcColsGrp != null && txtCntCols != null)
            {
                if (organizaProductosHor.IsChecked == true)
                {
                    OpcColsGrp.IsEnabled = false;
                    txtCntCols.ValorMinimo = 0;
                    txtCntCols.setValue("0");
                }
                else
                {
                    OpcColsGrp.IsEnabled = true;
                    txtCntCols.ValorMinimo = 1;
                    txtCntCols.setValue("1");
                }
            }
        }

        private void organizaProductosVer_Checked(object sender, RoutedEventArgs e)
        {
            OrganizacionHorizontal = false;

            if (organizaProductosVer != null && OpcColsGrp != null && txtCntCols != null)
            {
                if (organizaProductosVer.IsChecked == true)
                {
                    OpcColsGrp.IsEnabled = true;
                    txtCntCols.ValorMinimo = 1;
                    txtCntCols.setValue("1");
                }
                else
                {
                    OpcColsGrp.IsEnabled = false;
                    txtCntCols.ValorMinimo = 0;
                    txtCntCols.setValue("0");
                }
            }
        }

        private void btnTextOnly_Checked(object sender, RoutedEventArgs e)
        {
            tipoBotones = "ST";
        }

        private void btnImageOnly_Checked(object sender, RoutedEventArgs e)
        {
            tipoBotones = "SI";
        }

        private void btnImageText_Checked(object sender, RoutedEventArgs e)
        {
            tipoBotones = "IT";
        }

        private void btnTextOnlyEdit_Checked(object sender, RoutedEventArgs e)
        {
            tipoBotones = "STE";
        }

        private void btnImageEdit_Checked(object sender, RoutedEventArgs e)
        {
            tipoBotones = "SIE";
        }

        private void btnImageTextEdit_Checked(object sender, RoutedEventArgs e)
        {
            tipoBotones = "ITE";
        }

        private void BtnLeeQR_Click(object sender, RoutedEventArgs e)
        {
            /*Bitmap BitmapImage = new Bitmap(@"C:\Users\Ignacio\Pictures\QR-test.png");
            QRCodeReader reader = new QRCodeReader();
            try
            {
                LuminanceSource source;
                source = new BitmapLuminanceSource(BitmapImage);
                BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                Result result = new MultiFormatReader().decode(bitmap);
                Console.WriteLine(result.ToString());
            }
            catch
            {

            }*/

        }

        private static byte[] ToByteArray(System.Drawing.Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        private void chkAllowProforma_Checked(object sender, RoutedEventArgs e)
        {
            usaImpresoraProforma = true;
        }

        private void chkAllowProforma_Unchecked(object sender, RoutedEventArgs e)
        {
            usaImpresoraProforma = false;
        }

        private void chkAllowParrilla_Checked(object sender, RoutedEventArgs e)
        {
            usaImpresoraParrilla = true;
        }

        private void chkAllowParrilla_Unchecked(object sender, RoutedEventArgs e)
        {
            usaImpresoraParrilla = false;
        }

        private void cboFormatDec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void chkAlterna_Checked(object sender, RoutedEventArgs e)
        {
            txtMesasAlt.IsEnabled = true;
        }

        private void chkAlterna_Unchecked(object sender, RoutedEventArgs e)
        {
            txtMesasAlt.IsEnabled = false;
        }

        private void chkExterior_Checked(object sender, RoutedEventArgs e)
        {
            txtMesasExt.IsEnabled = true;
        }

        private void chkExterior_Unchecked(object sender, RoutedEventArgs e)
        {
            txtMesasExt.IsEnabled = false;
        }

        private void chkTerraza_Checked(object sender, RoutedEventArgs e)
        {
            txtMesasTerr.IsEnabled = true;
        }

        private void chkTerraza_Unchecked(object sender, RoutedEventArgs e)
        {
            txtMesasTerr.IsEnabled = false;
        }

        private void chk2Piso_Checked(object sender, RoutedEventArgs e)
        {
            txtMesasSPis.IsEnabled = true;
        }

        private void chk2Piso_Unchecked(object sender, RoutedEventArgs e)
        {
            txtMesasSPis.IsEnabled = false;
        }

        private void chkUseFactElec_Checked(object sender, RoutedEventArgs e)
        {
            if (chkUseFactElec != null)
            {
                chkUseAuto.IsEnabled = true;
            }
        }

        private void chkUseFactElec_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkUseFactElec != null)
            {
                chkUseAuto.IsEnabled = false;
                chkUseAuto.IsChecked = false;
                txtArchFirma.Text = "";
            }
        }

        private void chkUseAuto_Checked(object sender, RoutedEventArgs e)
        {
            if (chkUseAuto != null)
            {
                chkUseOnline.IsEnabled = true;
                chkUseOnline.IsChecked = true;
            }
        }

        private void chkUseAuto_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkUseAuto != null)
            {
                chkUseOnline.IsChecked = false;
                chkUseOnline.IsEnabled = false;
                horaInicio.IsEnabled = false;
                txtMinutos.IsEnabled = false;
            }
        }

        private void chkUseOnline_Checked(object sender, RoutedEventArgs e)
        {
            if (chkUseOnline != null)
            {
                horaInicio.IsEnabled = false;
                horaInicio.setValue("00:00");
                txtMinutos.IsEnabled = false;
                txtMinutos.Text = "0";
            }
        }

        private void chkUseOnline_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkUseOnline != null)
            {
                if (chkUseAuto.IsChecked == true)
                {
                    horaInicio.IsEnabled = true;
                    txtMinutos.IsEnabled = true;
                }
                else
                {
                    horaInicio.IsEnabled = false;
                    txtMinutos.IsEnabled = false;
                }
            }
        }

        private void btnFirma_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Selección de Firma Digital";
            openFileDialog.Filter = "Imagenes (*.p12;*.cert)|*.p12;*.cert";
            if (openFileDialog.ShowDialog() == true)
            {
                txtArchFirma.Text = openFileDialog.FileName;
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                txtFolder.Text = fbd.SelectedPath;
            }

        }

        private void cboTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboTipo != null)
            {
                if (txtCodigo == null) return;
                if (txtParteEntera == null) return;
                if (txtParteDecimal == null) return;

                switch (cboTipo.SelectedIndex)
                {
                    case 0:
                        txtCodigo.ValorMaximo = 5;
                        txtParteEntera.ValorMaximo = 3;
                        txtParteDecimal.ValorMaximo = 3;
                        imgBarcode.Source = new BitmapImage(new Uri("pack://application:,,,/images/bc-EAN-13.png"));
                        break;
                    case 1:
                        txtCodigo.ValorMaximo = 4;
                        txtParteEntera.ValorMaximo = 3;
                        txtParteDecimal.ValorMaximo = 2;
                        imgBarcode.Source = new BitmapImage(new Uri("pack://application:,,,/images/bc-UPC-A.png"));
                        break;
                }
            }
        }

        private void TestPrinter_Click(object sender, RoutedEventArgs e)
        {
            EposPrint printer = new EposPrint();

            Respuesta resp = printer.setPrinter(General.PRINTER_FACTURA);
            if (resp.resultado == false)
            {
                ObjComportamiento.showMsgError(resp.mensaje);
                return;
            }
            printer.TestImpresion("TEXTO DE PRUEBA DE IMPRESION");
        }

        private void btnLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Selección del logotipo para la factura";
            openFileDialog.Filter = "Imagenes (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;

                try
                {
                    BitmapImage img = new BitmapImage(new Uri(fileName));

                    int ancho = img.PixelWidth;
                    int alto = img.PixelHeight;

                    MessageBoxResult resp = MessageBoxResult.Yes;

                    if (ancho != 480 && alto != 150)
                    {
                        resp = ObjComportamiento.showMsgYesNo("El tamaño recomendado de la imagen para la facturación es de 480 x 150 pixeles. ¿Desea continuar?");

                        if (resp == MessageBoxResult.Yes)
                        {
                            txtLogo.Text = fileName;
                        }
                    }
                    else
                    {
                        txtLogo.Text = fileName;
                    }
                }
                catch (OutOfMemoryException err)
                {
                    ObjComportamiento.showMsgError(err.Message);
                }
                catch (Exception err)
                {
                    ObjComportamiento.showMsgError(err.Message);
                }
            }
        }

        private void cboMdoConfir_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMdoConfir != null && lblModoConfirInfo != null)
            {
                chkAlterCoord.IsEnabled = false;
                switch (cboMdoConfir.SelectedIndex)
                {
                    case 0:
                        lblModoConfirInfo.Text = "";
                        if (chkAlterna.IsChecked == true) chkAlterCoord.IsChecked = false;
                        break;
                    case 1:
                        lblModoConfirInfo.Text = "Los usuarios asignados poseerán una tarjeta de coordenadas. Cada usuario es responsable del uso de su tarjeta.";
                        if (chkAlterna.IsChecked == true) chkAlterCoord.IsChecked = false;
                        break;
                    case 2:
                        // Necesita de Internet para funcionar.
                        lblModoConfirInfo.Text = "El sistema le enviará al celular del usuario un PIN (número) de cuatro dígitos.";
                        chkAlterCoord.IsEnabled = true;
                        break;
                    case 3:
                        lblModoConfirInfo.Text = "El sistema pedirá que proceda a validar la huella digital del usuario.";
                        if (chkAlterna.IsChecked == true) chkAlterCoord.IsChecked = false;
                        break;
                    case 4:
                        lblModoConfirInfo.Text = "El sistema pedirá que proceda a validar usando tarjeta CONTACTLESS.";
                        if (chkAlterna.IsChecked == true) chkAlterCoord.IsChecked = false;
                        break;
                }
            }
        }

        private void chkUsaRelay_Checked(object sender, RoutedEventArgs e)
        {
            if (chkUsaRelay != null && titOpcServ != null && titOpcRealy != null)
            {
                titOpcServ.IsEnabled = false;
                titOpcRealy.IsEnabled = true;
                titOpcRealy.IsSelected = true;
            }
        }

        private void chkUsaRelay_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkUsaRelay != null && titOpcServ != null && titOpcRealy != null)
            {
                titOpcServ.IsEnabled = true;
                titOpcRealy.IsEnabled = false;
                titOpcServ.IsSelected = true;
            }
        }

        private void btnTipImp_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Listado de Descuentos a aplicar";
            win.Width = 740;
            win.Height = 430;

            ctlTipPrints crea = new ctlTipPrints();

            crea.contenedor = win;
            crea.ValoresIniciales = impresoras;

            crea.aceptaTiposImpresoras += new ctlTipPrints.AceptaTiposImpresoras(Crea_AceptaTiposImpresoras);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void Crea_AceptaTiposImpresoras(ImpresoraDestino impresorasDestino)
        {
            impresoras = impresorasDestino;
        }

        private void cboTipoImpresora_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool esCustom = false;
            if (btnTipImp != null)
            {
                if (cboTipoImpresora.SelectedIndex == 2)
                    esCustom = true;
                else
                {
                    impresoras.Facturacion.tipo = "";
                    impresoras.Facturacion.marca = "";
                    impresoras.Facturacion.usaEspacios = "";

                    impresoras.Cocina.tipo = "";
                    impresoras.Cocina.marca = "";
                    impresoras.Cocina.usaEspacios = "";

                    impresoras.Parrilla.tipo = "";
                    impresoras.Parrilla.marca = "";
                    impresoras.Parrilla.usaEspacios = "";

                    impresoras.Bar.tipo = "";
                    impresoras.Bar.marca = "";
                    impresoras.Bar.usaEspacios = "";
                }

                btnTipImp.IsEnabled = esCustom;
            }
        }

        private void chkUsaWhatsapp_Checked(object sender, RoutedEventArgs e)
        {
            if (chkUsaWhatsapp != null && titOpcServ != null && titOpcWhatsapp != null)
            {
                titOpcWhatsapp.IsEnabled = true;
                titOpcWhatsapp.IsSelected = true;
                chkUsaSMS.IsChecked = false;
                titOpcSMS.IsEnabled = false;
            }
        }

        private void chkUsaWhatsapp_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkUsaWhatsapp != null && titOpcServ != null && titOpcWhatsapp != null)
            {
                titOpcSMS.IsEnabled = false;
                titOpcWhatsapp.IsEnabled = false;
                titOpcWhatsapp.IsSelected = false;
                if (titOpcServ.IsEnabled)
                    titOpcServ.IsSelected = true;
                else
                    titOpcRealy.IsSelected = true;
            }
        }

        private void chkUsaSMS_Checked(object sender, RoutedEventArgs e)
        {
            if (chkUsaSMS != null && titOpcServ != null && titOpcSMS != null)
            {
                titOpcSMS.IsEnabled = true;
                titOpcSMS.IsSelected = true;
                titOpcWhatsapp.IsEnabled = false;
                titOpcWhatsapp.IsSelected = false;
            }
        }

        private void chkUsaSMS_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkUsaSMS != null && titOpcServ != null && titOpcSMS != null)
            {
                titOpcSMS.IsEnabled = false;
                titOpcSMS.IsSelected = false;
                if (titOpcServ.IsEnabled)
                    titOpcServ.IsSelected = true;
                else
                    titOpcRealy.IsSelected = true;
            }
        }

        private void Fondo_Checked(object sender, RoutedEventArgs e)
        {
            if (Fondo1 != null && Fondo1.IsChecked == true) fondoLogin = 1;
            if (Fondo2 != null && Fondo2.IsChecked == true) fondoLogin = 2;
            if (Fondo3 != null && Fondo3.IsChecked == true) fondoLogin = 3;
            if (Fondo4 != null && Fondo4.IsChecked == true) fondoLogin = 4;
            if (Fondo5 != null && Fondo5.IsChecked == true) fondoLogin = 5;
            if (Fondo6 != null && Fondo6.IsChecked == true) fondoLogin = 6;
            if (Fondo7 != null && Fondo7.IsChecked == true) fondoLogin = 7;
            if (Fondo8 != null && Fondo8.IsChecked == true) fondoLogin = 8;
            if (Fondo9 != null && Fondo9.IsChecked == true) fondoLogin = 9;
            if (Fondo10 != null && Fondo10.IsChecked == true) fondoLogin = 10;
        }

        private void CambiaImagenCabeceraComanda(bool valor)
        {
            if (chkImpHeadCmds != null && imgCabComanda != null)
            {
                if (valor)
                {
                    imgCabComanda.Source = new BitmapImage(new Uri("pack://application:,,,/images/comandaCab.png"));
                }
                else
                {
                    imgCabComanda.Source = new BitmapImage(new Uri("pack://application:,,,/images/comandaNoCab.png"));
                }
            }
        }

        private void CambiaImagenCabeceraSalto(bool valor)
        {
            if (chkSalHeadCmds != null && imgCabSalto != null)
            {
                if (valor)
                {
                    imgCabSalto.Source = new BitmapImage(new Uri("pack://application:,,,/images/comandaSalto.png"));
                }
                else
                {
                    imgCabSalto.Source = new BitmapImage(new Uri("pack://application:,,,/images/comandaNoSalto.png"));
                }
            }
        }

        private void chkImpHeadCmds_Checked(object sender, RoutedEventArgs e)
        {
            CambiaImagenCabeceraComanda(true);
        }

        private void chkImpHeadCmds_Unchecked(object sender, RoutedEventArgs e)
        {
            CambiaImagenCabeceraComanda(false);
        }

        private void chkSalHeadCmds_Checked(object sender, RoutedEventArgs e)
        {
            CambiaImagenCabeceraSalto(true);
        }

        private void chkSalHeadCmds_Unchecked(object sender, RoutedEventArgs e)
        {
            CambiaImagenCabeceraSalto(false);
        }

        private void cboTemaLogin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboTemaLogin != null)
            {
                try
                {
                    switch (cboTemaLogin.SelectedIndex)
                    {
                        case 0:
                            if (imgFondo1 != null) imgFondo1.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondo.png"));
                            if (imgFondo2 != null) imgFondo2.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondo2.png"));
                            if (imgFondo3 != null) imgFondo3.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondo3.png"));
                            if (imgFondo4 != null) imgFondo4.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondo4.png"));
                            if (imgFondo5 != null) imgFondo5.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondo5.png"));
                            if (imgFondo6 != null) imgFondo6.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondo6.png"));
                            if (imgFondo7 != null) imgFondo7.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondo7.png"));
                            if (imgFondo8 != null) imgFondo8.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondo8.png"));
                            if (imgFondo9 != null) imgFondo9.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondo9.png"));
                            if (imgFondo10 != null) imgFondo10.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondo10.png"));
                            break;
                        case 1:
                            if (imgFondo1 != null) imgFondo1.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondomusi.png"));
                            if (imgFondo2 != null) imgFondo2.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondomusi2.png"));
                            if (imgFondo3 != null) imgFondo3.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondomusi3.png"));
                            if (imgFondo4 != null) imgFondo4.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondomusi4.png"));
                            if (imgFondo5 != null) imgFondo5.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondomusi5.png"));
                            if (imgFondo6 != null) imgFondo6.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondomusi6.png"));
                            if (imgFondo7 != null) imgFondo7.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondomusi7.png"));
                            if (imgFondo8 != null) imgFondo8.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondomusi8.png"));
                            if (imgFondo9 != null) imgFondo9.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondomusi9.png"));
                            if (imgFondo10 != null) imgFondo10.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondomusi10.png"));
                            break;
                        case 2:
                            if (imgFondo1 != null) imgFondo1.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondosoft.png"));
                            if (imgFondo2 != null) imgFondo2.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondosoft2.png"));
                            if (imgFondo3 != null) imgFondo3.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondosoft3.png"));
                            if (imgFondo4 != null) imgFondo4.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondosoft4.png"));
                            if (imgFondo5 != null) imgFondo5.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondosoft5.png"));
                            if (imgFondo6 != null) imgFondo6.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondosoft6.png"));
                            if (imgFondo7 != null) imgFondo7.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondosoft7.png"));
                            if (imgFondo8 != null) imgFondo8.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondosoft8.png"));
                            if (imgFondo9 != null) imgFondo9.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondosoft9.png"));
                            if (imgFondo10 != null) imgFondo10.Source = new BitmapImage(new Uri("pack://application:,,,/images/fondosoft10.png"));
                            break;
                    }
                }
                catch(Exception ex)
                {

                }
                
            }
        }

    }
}
