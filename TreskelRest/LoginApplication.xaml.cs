using Entidades;
using Entidades.modelos;
using LibConfig;
using LibConfig.cls;
using LibKeys;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TreskelRest.cls;
using TreskelRest.frms.config;

namespace TreskelRest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginApplication : Window
    {
        private FullKeyboard keyboard = new FullKeyboard();
        private NumKeyboard nKeyboard = new NumKeyboard();
        private bool inicioConfig=false;

        private clsTextBox clsUsuario = null;
        private clsTextBox clsClave = null;
        private string imgFondo = "fondo.png";

        private void leeConfig()
        {
            Configuracion config = new Configuracion();

            if (config.ExistConfig()){
                Config actual = config.readConfig();

                General.generaConnectionString(actual);
                General.generaPrintersConfig(actual);
            }else{
                ObjComportamiento.showMsgError("No ha configurado el sistema, Reinicie la aplicación!");
            }

            config.Dispose();
        }

        private void checkSetting()
        {
            Configuracion config = new Configuracion();            

            if (config.ExistConfig()){
                Config actual = config.readConfig();

                General.generaConnectionString(actual);
                General.generaPrintersConfig(actual);

                ConfiguracionSys confSys = new ConfiguracionSys(General.connectionString);

                DataTable dt = confSys.loadConfigBase();

                clsUsuario.tecladoPantalla = General.USA_TECLADO_PANTALLA;
                clsClave.tecladoPantalla = General.USA_TECLADO_PANTALLA;

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (int.Parse(dr["confNid"].ToString()) == 3) General.USA_TECLADO_PANTALLA = dr["confVal"].ToString().Equals("1") ? true : false;
                        if (int.Parse(dr["confNid"].ToString()) == 13) General.CANTIDAD_CARACTERES_PASSWORD = int.Parse(dr["confVal"].ToString());
                        if (int.Parse(dr["confNid"].ToString()) == 20) General.USA_CLAVE_NUMERICA = dr["confVal"].ToString().Equals("1") ? true : false;
                        if (int.Parse(dr["confNid"].ToString()) == 21) General.USA_DESCUENTOS_PERSONALIZADOS = dr["confVal"].ToString().Equals("1") ? true : false;
                        if (int.Parse(dr["confNid"].ToString()) == 161) General.USA_FONDO_RANDOM = dr["confVal"].ToString().Equals("1") ? true : false;
                        if (int.Parse(dr["confNid"].ToString()) == 162) General.IMAGEN_FONDO_SELECC = dr["confVal"].ToString();
                        if (int.Parse(dr["confNid"].ToString()) == 163) General.FONDO_TEMA_LOGIN = dr["confVal"].ToString();

                        clsUsuario.tecladoPantalla = General.USA_TECLADO_PANTALLA;
                        clsClave.tecladoPantalla = General.USA_TECLADO_PANTALLA;

                        if (General.USA_CLAVE_NUMERICA)
                            clsClave.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
                        else
                            clsClave.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.CLAVES;

                        clsClave.setMaxLength(General.CANTIDAD_CARACTERES_PASSWORD);
                    }
                }else
                {
                    ObjComportamiento.showMsgError("No se puede establecer comunicación con el servidor de base de datos, contacte al servicio técnico");
                    btnLogin.IsEnabled = false;
                }                
            }
            else{
                inicioConfig = true;
                winConfig WinConfig = new winConfig();
                WinConfig.ShowDialog();
            }

            config.Dispose();
        }

        public LoginApplication()
        {
            InitializeComponent();

            
        }

        private void Window_loaded(object sender, RoutedEventArgs e)
        {
            clsUsuario = new clsTextBox(txtUsuario, true);
            clsClave = new clsTextBox(txtClave, true);

            clsUsuario.setMaxLength(20);
            clsClave.setMaxLength(20);

            checkSetting();

            txtUsuario.Tag = "USER";
            txtClave.Tag = "PASSWORD";

            txtUsuario.GotFocus += new RoutedEventHandler(textBox_GotFocus);
            txtUsuario.LostFocus += new RoutedEventHandler(textBox_LostFocus);
            txtClave.GotFocus += new RoutedEventHandler(textBox_GotFocus);
            keyboard.retornaTexto += new LibKeys.FullKeyboard.RetornaTexto(Texto_Retorna);
            //nKeyboard.retornaTexto += new LibKeys.NumKeyboard.RetornaTexto(TextoNumerico_Retorna);
            nKeyboard.retornaTexto += new LibKeys.NumKeyboard.RetornaTexto(Texto_Retorna);

            clsUsuario.muestraTeclado += new clsTextBox.MuestraTeclado(MuestraTecladoPantalla);
            clsClave.muestraTeclado += new clsTextBox.MuestraTeclado(MuestraTecladoPantalla);

            clsUsuario.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.USUARIOS;
            clsUsuario.permiteEspacios = false;
            clsClave.permiteEspacios = false;

            muestraImagenFondo();
        }

        private void muestraImagenFondo()
        {
            if (General.USA_FONDO_RANDOM)
            {
                int numAleatorio = General.generaNumeroAleatorio(1, 10);
                string numero = numAleatorio == 1 ? "" : numAleatorio.ToString();

                (lblUser.Foreground, lblClave.Foreground) = numAleatorio switch
                {
                    5 or 8 => (Brushes.Black, Brushes.Black),
                    6 => (Brushes.White, Brushes.Black),
                    _ => (lblUser.Foreground, lblClave.Foreground)
                };

                string[] prefijosTemas = { "fondo", "fondomusi", "fondosoft" };
                if (int.TryParse(General.FONDO_TEMA_LOGIN, out int indexTema) && indexTema >= 0 && indexTema < prefijosTemas.Length)
                {
                    string path = $"pack://application:,,,/images/{prefijosTemas[indexTema]}{numero}.png";
                    imgBackground.ImageSource = new BitmapImage(new Uri(path));
                }
            }
            else
            {
                string numeroImg = General.IMAGEN_FONDO_SELECC;
                if (numeroImg == "1" || string.IsNullOrEmpty(numeroImg)) numeroImg = "";

                string pathEstatico = $"pack://application:,,,/images/fondo{numeroImg}.png";
                imgBackground.ImageSource = new BitmapImage(new Uri(pathEstatico));

                (lblUser.Foreground, lblClave.Foreground) = numeroImg switch
                {
                    "5" or "8" => (Brushes.Black, Brushes.Black),
                    "6" => (Brushes.White, Brushes.Black),
                    _ => (Brushes.White, Brushes.White)
                };
            }
        }

        private void Texto_Retorna(Object txt, String texto)
        {
            if (txt is TextBox textBox)
            {
                textBox.Text = texto;
                textBox.SelectionStart = texto?.Length ?? 0;
            }
            else if (txt is PasswordBox passwordBox)
            {
                passwordBox.Password = texto;
            }

        }

        //private void TextoNumerico_Retorna(Object txt, String texto)
        //{
        //    if (txt.GetType() == typeof(PasswordBox))
        //    {
        //        ((PasswordBox)txt).Password = texto;
        //    }

        //    if (txt.GetType() == typeof(TextBox))
        //    {
        //        ((TextBox)txt).Text = texto;
        //        ((TextBox)txt).SelectionStart = txtUsuario.Text.Length;
        //    }

        //}

        private void MuestraTecladoPantalla(object sender)
        {
            string tag = "";
            bool usaCantPasswd = false;

            if (sender is TextBox textBox)
            {
                tag = textBox.Tag?.ToString() ?? "";
            }
            else if (sender is PasswordBox passwordBox)
            {
                tag = passwordBox.Tag?.ToString() ?? "";
                usaCantPasswd = true;
            }

            if (General.USA_CLAVE_NUMERICA && tag == "PASSWORD")
            {
                nKeyboard.MaxLength = General.CANTIDAD_CARACTERES_PASSWORD;

                if (sender is PasswordBox) nKeyboard.setTextBox = (PasswordBox)sender;
                else if (sender is TextBox) nKeyboard.setTextBox = (TextBox)sender;

                nKeyboard.ShowDialog();
            }
            else
            {
                keyboard.MaxLength = usaCantPasswd ? General.CANTIDAD_CARACTERES_PASSWORD : 20;

                if (sender is PasswordBox) keyboard.setTextBox = (PasswordBox)sender;
                else if (sender is TextBox) keyboard.setTextBox = (TextBox)sender;

                keyboard.setUsuario();
                keyboard.ShowDialog();
            }
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                clsUsuario.GotFocus(sender, e);
            }
            else if (sender is PasswordBox)
            {
                clsClave.GotFocus(sender, e);
            }
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Text == "admin")
                {
                    clsClave.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
                    clsClave.setMaxLength(20);
                }
                else
                {
                    if (General.USA_CLAVE_NUMERICA)
                    {
                        if (!General.parseInt(txtClave.Password))
                        {
                            txtClave.Password = "";
                        }
                        clsClave.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
                    }

                    clsClave.setMaxLength(General.CANTIDAD_CARACTERES_PASSWORD);
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Está seguro de finalizar la aplicación?");                               
            if(res == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }            
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Usuarios usuarios = new Usuarios(General.connectionString);
            lUsuario lusuario = usuarios.autenticar(txtUsuario.Text, txtClave.Password);

            if (lusuario != null)
            {
                General.USUARIO = lusuario.usuario;
                General.CODUSUARIO = lusuario.codigo;

                General.principal = new frms.Principal();
                General.principal.Show();

                this.Close();
            }
            else
            {
                ObjComportamiento.showMsgError("Usuario o clave incorrecto, REINTENTE!");
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (inicioConfig)
            {

            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Boolean)e.NewValue)
            {
                txtUsuario.Focus();
            }
        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                txtClave.Focus();
            }
        }

        private void txtClave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnLogin.Focus();
            }
        }

        private void txtClave_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtClave != null)
            {
                if (txtClave.Password.Length > 0)
                {
                    if (e.Key == Key.Return || e.Key == Key.Enter)
                    {
                        btnLogin.Focus();
                    }
                }
            }
            
        }

        private void clearUsuario_Click(object sender, RoutedEventArgs e)
        {
            txtUsuario.Clear();
            txtUsuario.Focus();
        }

        private void clearClave_Click(object sender, RoutedEventArgs e)
        {
            txtClave.Clear();
            txtClave.Focus();
        }
    }
}
