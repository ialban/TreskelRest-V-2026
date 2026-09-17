using Entidades;
using LibConfig;
using LibConfig.cls;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Input;
using TreskelRest.cls;

namespace TreskelRest.frms.config
{
    /// <summary>
    /// Interaction logic for winConfig.xaml
    /// </summary>
    public partial class winConfig : Window
    {
        public clsTextBox clsSvd;
        public clsTextBox clsBsd;
        public clsTextBox clsPto;
        public clsTextBox clsUsr;
        public clsTextBox clsPwd;
        public clsTextBox clsCja;

        private string origen = "";

        private List<String> printerList()
        {
            List<String> impresoras = new List<string>();

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                impresoras.Add(printer);
            }
            return impresoras;
        }

        public winConfig()
        {
            InitializeComponent();

            Definicion();
        }

        public winConfig(string origen)
        {
            InitializeComponent();

            Definicion();
            if (origen == "C" && General.USUARIO != "admin")
            {
                this.origen = origen;
                tabDatabase.IsEnabled = false;
            }

            cargar();

            if (General.USUARIO != "admin")
            {
                tabReset.IsEnabled = false;
            }
        }

        public void Definicion()
        {
            clsSvd = new clsTextBox(txtServer, General.USA_TECLADO_PANTALLA);
            clsBsd = new clsTextBox(txtDbName, General.USA_TECLADO_PANTALLA);
            clsPto = new clsTextBox(txtPuerto, General.USA_TECLADO_PANTALLA);
            clsUsr = new clsTextBox(txtUsuario, General.USA_TECLADO_PANTALLA);
            clsPwd = new clsTextBox(txtClave, General.USA_TECLADO_PANTALLA);
            clsCja = new clsTextBox(txtCaja, General.USA_TECLADO_PANTALLA);

            clsSvd.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.SERVIDOR;
            clsPto.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsBsd.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsUsr.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.USUARIOS;
            clsPwd.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.CLAVES;
            clsCja.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;

            clsSvd.setMaxLength(20);
            clsPto.setMaxLength(5);
            clsBsd.setMaxLength(20);
            clsUsr.setMaxLength(100);
            clsPwd.setMaxLength(20);
            clsCja.setMaxLength(4);

            List<String> impresoras = this.printerList();

            foreach (String impresora in impresoras)
            {
                cboFacturacion.Items.Add(impresora);
                cboProforma.Items.Add(impresora);
                cboCocina.Items.Add(impresora);
                cboCocinaAlt.Items.Add(impresora);
                cboCocinaBar.Items.Add(impresora);
            }

            txtServer.PreviewTextInput += new TextCompositionEventHandler(txtServer_PreviewTextInput);
            txtDbName.PreviewTextInput += new TextCompositionEventHandler(txtDbName_PreviewTextInput);
            txtPuerto.PreviewTextInput += new TextCompositionEventHandler(txtPuerto_PreviewTextInput);
            txtUsuario.PreviewTextInput += new TextCompositionEventHandler(txtUsuario_PreviewTextInput);
            txtClave.PreviewTextInput += new TextCompositionEventHandler(txtClave_PreviewTextInput);
            txtCaja.PreviewTextInput += new TextCompositionEventHandler(txtCaja_PreviewTextInput);
        }

        private void txtCaja_PreviewTextInput(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = ObjComportamiento.PreviewTextInput(ObjComportamiento.tiposEntradas.soloNumeros, e);
        }

        private void txtServer_PreviewTextInput(Object sender, TextCompositionEventArgs e)
        {
            e.Handled=ObjComportamiento.PreviewTextInput(ObjComportamiento.tiposEntradas.nombreServidor, e);
        }

        private void txtDbName_PreviewTextInput(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = ObjComportamiento.PreviewTextInput(ObjComportamiento.tiposEntradas.nombreBaseDatos, e);            
        }

        private void txtPuerto_PreviewTextInput(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = ObjComportamiento.PreviewTextInput(ObjComportamiento.tiposEntradas.soloNumeros, e);
        }

        private void txtUsuario_PreviewTextInput(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = ObjComportamiento.PreviewTextInput(ObjComportamiento.tiposEntradas.usuario, e);
        }

        private void txtClave_PreviewTextInput(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = ObjComportamiento.PreviewTextInput(ObjComportamiento.tiposEntradas.contrasenia, e);
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Está seguro de salir de la pantalla de configuración?");            
            if (res == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void cargar()
        {
            Configuracion config = new Configuracion();

            if (config.ExistConfig())
            {
                Config actual = config.readConfig();

                chkTouch.IsChecked = actual.system.UsaTouch; 

                General.selectedByContentCombo(cboFacturacion, actual.system.PrinterFactura);
                General.selectedByContentCombo(cboProforma, actual.system.PrinterProforma);
                General.selectedByContentCombo(cboCocina, actual.system.PrinterCocina);
                General.selectedByContentCombo(cboCocinaAlt, actual.system.PrinterParrilla);
                General.selectedByContentCombo(cboCocinaBar, actual.system.PrinterBar);
                txtCaja.Text = actual.system.Caja;

                // --------------------------------------------------
                txtServer.Text = actual.database.Server;
                txtDbName.Text = actual.database.DbName;
                txtPuerto.Text = actual.database.Puerto.ToString();
                txtUsuario.Text = actual.database.Usuario;
                txtClave.Password = actual.database.Clave;
            }
        }

        private void Grabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if (txtServer.Text.Trim().Length == 0)
            {
                msg += "- El campo servidor no puede estar en blanco"+((char) 13);
            }

            if (txtDbName.Text.Trim().Length == 0)
            {
                msg += "- El campo Base de datos no puede estar en blanco" + ((char)13);
            }

            if (txtPuerto.Text.Trim().Length == 0)
            {
                msg += "- El campo Puerto no puede estar en blanco" + ((char)13);
            }

            if (!(int.Parse(txtPuerto.Text)>100 && int.Parse(txtPuerto.Text) < 65535))
            {
                msg += "- El campo Puerto debe tener un valor entre 100 y 65535" + ((char)13);
            }

            if (txtUsuario.Text.Trim().Length == 0)
            {
                msg += "- El campo Usuario no puede estar en blanco" + ((char)13);
            }

            if (txtClave.Password.Trim().Length == 0)
            {
                msg += "- El campo Clave no puede estar en blanco" + ((char)13);
            }

            if (txtCaja.Text.Trim().Length == 0)
            {
                msg += "- El campo Caja no puede estar en blanco" + ((char)13);
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desar grabar la configuración indicada?");
            if (res == MessageBoxResult.Yes)
            {
                Configuracion config = new Configuracion();

                Config nuevo = new Config();

                nuevo.database.Server=txtServer.Text.ToString();
                nuevo.database.DbName=txtDbName.Text.ToString();
                nuevo.database.Puerto= int.Parse(txtPuerto.Text.ToString());
                nuevo.database.Usuario = txtUsuario.Text.ToString();
                nuevo.database.Clave = txtClave.Password.ToString();

                nuevo.system.UsaTouch = (Boolean) chkTouch.IsChecked;

                nuevo.system.PrinterFactura = cboFacturacion.SelectedIndex == 0 ? "Ninguna" : cboFacturacion.SelectedItem.ToString();
                nuevo.system.PrinterProforma = cboProforma.SelectedIndex == 0 ? "Ninguna" : cboProforma.SelectedItem.ToString();
                nuevo.system.PrinterCocina = cboCocina.SelectedIndex == 0 ? "Ninguna" :  cboCocina.SelectedItem.ToString();
                nuevo.system.PrinterParrilla = cboCocinaAlt.SelectedIndex == 0 ? "Ninguna" :  cboCocinaAlt.SelectedItem.ToString();
                nuevo.system.PrinterBar = cboCocinaBar.SelectedIndex == 0 ? "Ninguna" : cboCocinaBar.SelectedItem.ToString();
                nuevo.system.Caja = txtCaja.Text;

                if (config.creaConfig(nuevo))
                {
                    // Verificar si el archivo de configuración existe
                    if (this.origen == "C")
                    {
                        General.loadParametros();
                        General.generaPrintersConfig(nuevo);
                    }
                    ObjComportamiento.showMsgInfo("Configuración grabada correctamente");
                }else
                {
                    ObjComportamiento.showMsgError("Configuración no pudo ser grabada!, " + ((char)13) + "comuniquese con el administrador!");
                }
            }
        }

        private void TestDb_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "server=" + txtServer.Text.ToString() + ";" +
                                      "port="+ txtPuerto.Text.ToString() + ";" + 
                                      "database="+ txtDbName.Text.ToString() + ";" +
                                      "user id="+ txtUsuario.Text.ToString() + ";" +
                                      "password="+ txtClave.Password.ToString() + ";";

            TestDbConnection test = new TestDbConnection(connectionString);

            if (test.Test())
            {
                ObjComportamiento.showMsgInfo("Conexión a la base de datos CORRECTA");
            }
            else
            {
                ObjComportamiento.showMsgError("No se puedo establecer conexión con la base de datos, configuración incorrecta o puerto de conexión bloqueado por Firewall");
            }
        }

        private void CheckReset(bool value)
        {
            try
            {
                rstCaja.IsChecked = value;
                rstCierre.IsChecked = value;
                rstInventario.IsChecked = value;
                rstItems.IsChecked = value;
                rstPedidos.IsChecked = value;
                rstUsuarios.IsChecked = value;
                rstVentas.IsChecked = value;
                rstEmpresa.IsChecked = value;
            }
            catch (Exception e)
            {

            }
        }

        private void ResetAll_Checked(object sender, RoutedEventArgs e)
        {
            if (ResetAll != null)
            {
                CheckReset(true);
            }
        }

        private void ResetAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ResetAll != null)
            {
                CheckReset(false);
            }
        }

        private void ResetDb_Click(object sender, RoutedEventArgs e)
        {
            var resetmsg = ObjComportamiento.showMsgYesNo("Resetear la base de datos conlleva perder información de Pedidos, Ventas y otros." + System.Environment.NewLine +
                "¿Desea continuar con el proceso de RESET");

            if (resetmsg == MessageBoxResult.Yes)
            {
                string connectionString = "server=" + txtServer.Text.ToString() + ";" +
                                          "port=" + txtPuerto.Text.ToString() + ";" +
                                          "database=" + txtDbName.Text.ToString() + ";" +
                                          "user id=" + txtUsuario.Text.ToString() + ";" +
                                          "password=" + txtClave.Password.ToString() + ";";

                EntidadBase test = new EntidadBase(connectionString);

                Respuesta resp = test.clearDatabase((bool)rstCaja.IsChecked, (bool)rstCierre.IsChecked, (bool)rstInventario.IsChecked,
                    (bool)rstItems.IsChecked, (bool)rstPedidos.IsChecked, (bool)rstUsuarios.IsChecked, (bool)rstVentas.IsChecked,
                    (bool)rstEmpresa.IsChecked);

                ObjComportamiento.showMuestraMensaje(resp);
            }
        }
    }
}
