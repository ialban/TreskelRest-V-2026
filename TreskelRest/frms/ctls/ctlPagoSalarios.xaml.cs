using Entidades;
using LibKeys;
using LibPrint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlPagoSalarios.xaml
    /// </summary>
    public partial class ctlPagoSalarios : UserControl
    {
        private clsTextBox clsEmp;
        private bool disposedValue;
        private bool ConfirmacionAceptada = false;
        private string caja;

        private winGeneral win = new winGeneral();
        public Window contenedor { get; set; }

        private int anio = DateTime.Now.Year;
        private int mes = DateTime.Now.Month;
        private int dia = DateTime.Now.Day;
        private int mesActual = DateTime.Now.Month;

        private NumKeyboard nKeyboard = new NumKeyboard();

        private Empleados empleados = new Empleados(General.connectionString);
        private string codUsuario = "";
        private int origenSelected = -1;

        public ctlPagoSalarios()
        {
            InitializeComponent();

            clsEmp = new clsTextBox(txtNid, General.USA_TECLADO_PANTALLA);

            clsEmp.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsEmp.permiteEspacios = true;
            clsEmp.setMaxLength(50);

            txtAnio.setValue(anio.ToString());

            General.selectedByTagCombo(cboMes, General.obtnieneMesPorNumero(mes, true));
            win.Topmost = true;

            txtDia.ValorMaximo = DateTime.DaysInMonth(anio, mes);
            txtDia.ValorMinimo = 1;
            txtDia.setValue(dia.ToString());

            nKeyboard.retornaTexto += new LibKeys.NumKeyboard.RetornaTexto(TextoNumerico_Retorna);

            txtNid.IsEnabled = false;
            grbDatos.IsEnabled = false;
            btnBuscar.IsEnabled = false;
            btnClsEmp.IsEnabled = false;

            caja = General.CAJA;

            Bancos bancos = new Bancos(General.connectionString);

            List<Entidades.modelos.lBancos> lBancos = bancos.buscar("", "A");
            GeneraRadioButton(OpcOrigen, "Caja Chica", "CC-0", "caja.png");
            foreach (var bco in lBancos)
            {
                string imagen = "";
                switch (bco.acronimo)
                {
                    case "BA":
                        imagen = "baustro.png";
                        break;
                    case "BB":
                        imagen = "bbolivariano.png";
                        break;
                    case "BG":
                        imagen = "bgquil.png";
                        break;
                    case "BI":
                        imagen = "binternacional.png";
                        break;
                    case "BM":
                        imagen = "bmachala.png";
                        break;
                    case "PB":
                        imagen = "bpacifico.png";
                        break;
                    case "BP":
                        imagen = "bpichincha.png";
                        break;
                    case "BD":
                        imagen = "produbanco.png";
                        break;
                    case "BR":
                        imagen = "bruminahui.png";
                        break;
                    case "BS":
                        imagen = "bsolidario.png";
                        break;
                    case "MP":
                        imagen = "mutPichincha.png";
                        break;
                    case "MA":
                        imagen = "mutAmbato.png";
                        break;
                    case "CP":
                        imagen = "coopPN.png";
                        break;
                    case "CO":
                        imagen = "coopOscus.png";
                        break;
                    case "29":
                        imagen = "coop29Oct.png";
                        break;
                    case "BE":
                        imagen = "banecuador.png";
                        break;
                }
                GeneraRadioButton(OpcOrigen, System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(bco.nombre.ToLower()), 
                    bco.acronimo + "-" + bco.codigo, imagen);
            }
            
        }

        public void GeneraRadioButton(WrapPanel wpnl, string nombre, string acronimo, string imagen)
        {
            ToggleButton rb = new ToggleButton();
            rb.Tag = acronimo;
            rb.Height = 70;
            rb.Width = 90;
            rb.Checked += new RoutedEventHandler(ToggleOrigen_Checked);
            rb.Unchecked += new RoutedEventHandler(ToggleOrigen_UnChecked);

            StackPanel pnl = new StackPanel();
            pnl.Orientation = Orientation.Vertical;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri($"pack://application:,,,/images/{imagen}")); ;
            img.Width = 40;
            img.Height = 40;

            Label lbl = new Label();
            lbl.Content = nombre;
            lbl.FontSize = 9;

            pnl.Children.Add(img);
            pnl.Children.Add(lbl);
            rb.Content = pnl;

            wpnl.Children.Add(rb);
        }

        private void ToggleOrigen_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleButton tgb = ((ToggleButton)sender);
                string[] acrocod = tgb.Tag.ToString().Split('-');
                string acronimo = acrocod[0];
                origenSelected = int.Parse(acrocod[1]);
                WrapPanel wpn = (WrapPanel)tgb.Parent;

                foreach (ToggleButton t in wpn.Children)
                {
                    if (t.Tag.ToString() != acronimo + "-" + origenSelected)
                        t.IsChecked = false;
                }
            }
            catch(Exception er)
            {

            }   
        }

        private void ToggleOrigen_UnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                int cantidad = 0;
                foreach (ToggleButton t in OpcOrigen.Children)
                {
                    if (t.IsChecked == true)
                        cantidad++;
                }

                if (cantidad == 0)
                {
                    origenSelected = -1;
                }
            }
            catch (Exception er)
            {

            }

        }

        private void TextoNumerico_Retorna(Object txt, String texto)
        {
            if (texto.Equals(""))
            {
                ObjComportamiento.showMsgError("Debe ingresar una clave para poder registrar un pago de salario");
                btnCajero.IsChecked = false;
                return;
            }

            Usuarios usuario = new Usuarios(General.connectionString);

            string codigo = usuario.getCodMesero(texto, General.USA_CLAVE_NUMERICA);

            if (codigo != "")
            {
                var rcodigo = codigo.Split(';');
                codUsuario = rcodigo[0];

                txtNid.IsEnabled = true;
                grbDatos.IsEnabled = true;
                btnBuscar.IsEnabled = true;
                btnClsEmp.IsEnabled = true;
            }
            else
            {
                ObjComportamiento.showMsgError("Clave ingresada no válida");
            }
        }

        public void TextoNid_GotFocus()
        {
            txtNid.Focus();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = "";

            if (lblNid.Tag.ToString() == "")
            {
                mensaje += "- No se ha seleccionado a un empleado" + Environment.NewLine;
            }

            if (txtValor.Text != "")
            {
                if (double.Parse(txtValor.Text) == 0)
                {
                    mensaje += "- El valor a pagar deber ser mayor a 0" + Environment.NewLine;
                }

                var numero = txtValor.Text.Replace(",", ".").Split('.');
                if (numero.Length > 1)
                {
                    if (numero[1].Length > 2)
                    {
                        mensaje += "- El valor ingresado solo puede tener dos dígitos decimales, el formato es ####.##" + Environment.NewLine;
                    }
                }
            }
            else
            {
                mensaje += "- El campo Valor no puede estar en blanco" + Environment.NewLine;
            }

            if (double.Parse(txtValor.Text) == double.Parse(txtSaldo.Text) && double.Parse(txtSaldo.Text) == 0)
            {
                mensaje += "- No existen saldos que deban ser cancelados" + Environment.NewLine;
            }

            if (double.Parse(txtValor.Text) != double.Parse(txtSaldo.Text) && rbPagoTotal.IsChecked == true)
            {
                mensaje += "- El valor a pagar debe ser igual al saldo" + Environment.NewLine;
            }

            if (double.Parse(txtValor.Text) > double.Parse(txtSaldo.Text) && rbPagoParcial.IsChecked == true)
            {
                mensaje += "- El valor a pagar no puede ser mayor al saldo" + Environment.NewLine;
            }

            if (origenSelected == -1)
            {
                mensaje += "- No ha seleccionado un origen de la fuente de pago" + Environment.NewLine;
            }

            if (mensaje != "")
            {
                ObjComportamiento.showMsgError(mensaje);
                return;
            }

            if (ObjComportamiento.showMsgYesNo("¿Está seguro de realizar este pago de salario?") == MessageBoxResult.Yes)
            {

                if (General.CONFIRMA_RECEPTOR)
                {
                    ConfirmaPin cls = new ConfirmaPin(General.connectionString);
                    cls.msgpin = General.generaPin();
                    cls.msgatv = "A";
                    cls.msgtip = "C"; //Confirma Pago de Salario por parte del Empleado Receptor.
                    cls.tiporg = "V";
                    cls.usucod = codUsuario;
                    cls.secorg = "0".PadLeft(10, '0');

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
                else
                {
                    realizarPago();
                }
            }
        }

        private void acepta_confirmacion(bool acepta)
        {
            ConfirmacionAceptada = acepta;

            realizarPago();
        }

        private void realizarPago(string codigoAcepta = "")
        {
            if (!General.CONFIRMA_RECEPTOR)
            {
                ConfirmacionAceptada = true;
            }

            if (ConfirmacionAceptada)
            {
                int dia = int.Parse(txtDia.getValue());
                string codigo = lblNid.Tag.ToString();
                string empleado = lblNid.Content.ToString();
                string motivo = rbPagoTotal.IsChecked == true ? "C" : "P";
                double valor = General.StringToDouble(txtValor.Text);
                string usuario = codUsuario;
                string codapro = codigoAcepta;

                var resultado = empleados.pagarSalario(codigo, motivo, anio, mes, dia, valor, usuario, codapro, origenSelected, caja);

                if (resultado.resultado)
                {
                    ObjComportamiento.showMsgInfo(resultado.mensaje);

                    // Imprimir commprobante de pago
                    string motivoPago = $"SALARIO MES DE {General.obtnieneMesPorNumero(mes, false)} del {anio}";
                    string tipo = motivo == "C" ? "PAGO COMPLETO": "PAGO PARCIAL";

                    ImprimeComprobante(codigo, empleado, valor, DateTime.Now, motivoPago, tipo);

                    limpiar();
                }
                else
                {
                    ObjComportamiento.showMsgError(resultado.mensaje);
                }
            }
            
        }

        private void ImprimeComprobante(string empleadoId, string empleado, double cantidad, 
                                        DateTime fechaHora, string motivo, string tipo)
        {
            string fecha = fechaHora.Date.ToString();
            string hora = fechaHora.TimeOfDay.ToString();
            int cnt = General.CONFIRMA_RECEPTOR ? 1 : 2;

            for (int i = 0; i < cnt; i++){
                EposPrint cls = new EposPrint();
                cls.TipoImpresora = General.TIPO_IMPRESORAS;
                Respuesta resp = cls.setPrinter(General.PRINTER_FACTURA);

                if (resp.resultado == false)
                {
                    ObjComportamiento.showMsgError(resp.mensaje);
                    cls.Dispose();
                    return;
                }

                cls.cantidadColumnas = General.CANTIDAD_COLUMNAS_IMPRESION;
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
                cls.usaEspaciosInterlineado = General.USA_ESPACIOS_INTERLINEADO;

                cls.ComprobantePago(General.EMPRESA, General.DIRECCION_LOCAL, General.RUC_EMPRESA,
                    empleadoId, empleado, string.Format("{0:0.00}",cantidad), fecha, hora, motivo, tipo);

                cls.sbIpr();
                cls.Dispose();
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            var empds = empleados.buscarEmpleado(txtNid.Text, "A");

            if (empds.Count > 1)
            {
                winGeneral win = new winGeneral();
                win.Title = "Selección de empleado";
                win.Width = 810;
                win.Height = 480;

                ctlBscEmpleado crea = new ctlBscEmpleado(empds);
                crea.contenedor = win;
                crea.aceptaEmpleado += new ctlBscEmpleado.AceptaEmpleado(empleado_Seleccionado);

                win.Contenido.Children.Add(crea);

                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();

            }
            else if(empds.Count == 1)
            {
                var empd = empds.First();
                lblNid.Content = empd.empleado;
                lblNid.Tag = empd.codigo;

                obtieneInfoSalarios();
            }
            else
            {
                ObjComportamiento.showMsgInfo("No se ha encontrado empleados que coincida con la búsqueda");
                lblNid.Content = "No hay Empleado Seleccionado";
                lblNid.Tag = "";
            }
        }

        public void empleado_Seleccionado(string codigo, string empleado)
        {
            if (codigo != "")
            {
                lblNid.Content = empleado;
                lblNid.Tag = codigo;

                obtieneInfoSalarios();
            }
            else
            {
                lblNid.Content = "No hay Empleado Seleccionado";
                lblNid.Tag = "";
            }
        }

        private void cboMotivo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void obtieneInfoSalarios()
        {
            var dataSalario = empleados.buscarEmpleadoSalario(lblNid.Tag.ToString(), anio, mes);

            txtSalario.Text = string.Format("{0:0.00}", dataSalario.salario);
            txtDescuento.Text = string.Format("{0:0.00}", dataSalario.descuento);
            txtSaldo.Text = string.Format("{0:0.00}", dataSalario.saldo);
            txtSaldoPrev.Text = string.Format("{0:0.00}", dataSalario.saldoPrevio);

            if (rbPagoTotal.IsChecked == true)
                txtValor.Text = string.Format("{0:0.00}", dataSalario.saldo);
            else
                txtValor.Text = string.Format("{0:0.00}", "0.00");

            if (dataSalario.salario == 0)
            {
                ObjComportamiento.showMsgInfo("El empleado no tiene definido un salario, primero defina el salario, la ventana se CERRARÁ");
                contenedor.Close();
            }
        }

        private void btnClsVal_Click(object sender, RoutedEventArgs e)
        {
            txtValor.Text = "0.00";
        }

        private void limpiar()
        {
            ConfirmacionAceptada = false;
            txtNid.Text = "";
            lblNid.Content = "No hay Empleado Seleccionado";
            txtSalario.Text = "0.00";
            txtDescuento.Text = "0.00";
            txtSaldoPrev.Text = "0.00";
            txtSaldo.Text = "0.00";
            txtValor.Text = "0.00";
            txtAnio.setValue(anio.ToString());
            General.selectedByTagCombo(cboMes, General.obtnieneMesPorNumero(mes, true));
            rbPagoTotal.IsChecked = true;

            origenSelected = -1;
            foreach (ToggleButton t in OpcOrigen.Children)
            {
                t.IsChecked = false;
            }
        }

        private void btnClsEmp_Click(object sender, RoutedEventArgs e)
        {
            limpiar();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            obtieneInfoSalarios();
        }

        private void cboMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMes != null)
            {
                var fechaActual = DateTime.Now;
                var horaActual = DateTime.Now.Hour;
                var mesSelected = cboMes.SelectedIndex + 1;

                if (new DateTime(int.Parse(txtAnio.getValue()), mesSelected, horaActual) > fechaActual)
                {
                    ObjComportamiento.showMsgError("No puede realizar pagos de salario a futuro");
                    return;
                }

                if (mesSelected != mesActual)
                {
                    if (mesSelected < mesActual)
                    {
                        txtDia.ValorMaximo = DateTime.DaysInMonth(anio, mesSelected);
                        txtDia.setValue(txtDia.ValorMaximo.ToString());
                    }
                    
                    lblDia.Visibility = Visibility.Visible;
                    txtDia.Visibility = Visibility.Visible;
                }
                else
                {
                    txtDia.ValorMaximo = DateTime.DaysInMonth(anio, mesActual);
                    txtDia.setValue(DateTime.Now.Day.ToString());
                    lblDia.Visibility = Visibility.Hidden;
                    txtDia.Visibility = Visibility.Hidden;
                }

                mes = cboMes.SelectedIndex + 1;
            }
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
            txtNid.IsEnabled = false;
            grbDatos.IsEnabled = false;
            btnBuscar.IsEnabled = false;
            btnClsEmp.IsEnabled = false;
            limpiar();
        }

        private void SeleccionMotivo_Checked(object sender, RoutedEventArgs e)
        {
            if (rbPagoTotal != null && txtValor != null)
            {
                if (rbPagoTotal.IsChecked == true)
                {
                    txtValor.Text = txtSaldo.Text;
                }
                else
                {
                    txtValor.Text = "0.00";
                    txtValor.Select(0, txtValor.Text.Length);
                    txtValor.Focus();
                }
            }
        }

        private void txtNid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                btnBuscar.Focus();
            }
        }
    }
}
