using Entidades;
using LibHttpClient;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlIngresaPIN.xaml
    /// </summary>
    public partial class ctlIngresaPIN : Window
    {
        private int minuto = General.SEGUNDOS_ESPERA / 60;
        private int segundo = 0;
        private Thread tiempo = null;
        private int maxLength = 6;
        private int pinScl = 0;
        private string PIN = "";
        private string tipo = "";
        private string origen = "";

        public string codigoEmpleado;
        public string codigo;

        private bool continuar = true;
        private bool pausa = false;

        public event AceptaPIN aceptaPin;
        public delegate void AceptaPIN(bool aceptado);

        public string SetearPin
        {
            set
            {
                PIN = value;
            }
        }

        public string SetearTipo
        {
            set
            {
                tipo = value;
            }
        }

        public string SetearOrigen
        {
            set
            {
                origen = value;
            }
        }

        public int SetearPinSecuencial
        {
            set
            {
                pinScl = value;
            }
        }

        public ctlIngresaPIN()
        {
            InitializeComponent();

            setTiempo();

            tiempo = new Thread(() => { iniciaCountDown(); });

            tiempo.Start();
        }

        private void setTiempo()
        {
            try
            {
                if (contador != null)
                {
                    contador.Content = string.Format("{0:00}", minuto) + ":" + string.Format("{0:00}", segundo);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void updateClockImage(string tipo)
        {
            switch (tipo)
            {
                case "O":
                    TimeDefcon.Source = new BitmapImage(new Uri("pack://application:,,,/images/timeOK.png"));
                    break;
                case "W":
                    TimeDefcon.Source = new BitmapImage(new Uri("pack://application:,,,/images/TimeD1.png"));
                    break;
                case "R":
                    TimeDefcon.Source = new BitmapImage(new Uri("pack://application:,,,/images/TimeD2.png"));
                    break;
            }
            System.Windows.Forms.Application.DoEvents();
        }

        private void ActivaGenerar(bool value)
        {
            btnPto.IsEnabled = value;
        }

        private void iniciaCountDown()
        {
            //Definir rangos por porcentaje
            int limite1 = 0;
            int limite2 = 0;
            General.retornaLimites(30, out limite1);
            General.retornaLimites(10, out limite2);

            while (continuar)
            {
                if (!pausa)
                {
                    while (minuto > 0 || segundo > 0)
                    {
                        Thread.Sleep(1000);
                        if (segundo == 0)
                        {
                            minuto--;
                            segundo = 59;
                        }
                        else
                        {
                            segundo--;
                        }

                        Dispatcher.BeginInvoke((Action)(() => setTiempo()));

                        var tiempo = minuto * 100 + segundo;

                        if (tiempo > limite1)
                        {
                            Dispatcher.BeginInvoke((Action)(() => updateClockImage("O")));
                        }

                        if (tiempo > limite2 && tiempo <= limite1)
                        {
                            Dispatcher.BeginInvoke((Action)(() => updateClockImage("W")));
                        }

                        if (tiempo <= limite2)
                        {
                            Dispatcher.BeginInvoke((Action)(() => updateClockImage("R")));
                        }
                    }

                    Dispatcher.BeginInvoke((Action)(() => setTiempo()));
                    Dispatcher.BeginInvoke((Action)(() => ActivaGenerar(true)));

                    pausa = true;
                }
            }
        }

        private void reset()
        {
            updateClockImage("O");
            btnPto.IsEnabled = false;
            texto.Text = "";
            minuto = General.SEGUNDOS_ESPERA / 60;
            segundo = 0;

            setTiempo();
            Thread.Sleep(1000);
            pausa = false;
        }

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            tiempo = null;
            this.Close();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            texto.Text = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            String _txt = texto.Text.ToString();
            int _idx = texto.SelectionStart;

            if (texto.SelectionLength > 0)
            {
                texto.Text = texto.Text.Remove(_idx, texto.SelectionLength);
                texto.Focus();
                return;
            }

            if (_txt.Length > 0)
            {
                if (_idx > 0 && _idx < texto.Text.Length)
                {
                    String txt1 = texto.Text.Substring(0, _idx);
                    String txt2 = texto.Text.Substring(_idx);

                    txt1 = txt1.Remove(txt1.Length - 1, 1);
                    texto.Text = txt1 + txt2;
                }
                else
                {
                    if (_idx == 0) return;
                    texto.Text = _txt.Substring(0, _txt.Length - 1);
                }
                texto.SelectionStart = (_idx - 1) > 0 ? _idx - 1 : 0;
            }
            texto.Focus();
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            if (contador.Content.ToString() == "00:00")
            {
                ObjComportamiento.showMsgError("El tiempo para ingresar un PIN válido ha CADUCADO");
                return;
            }

            if (string.IsNullOrEmpty(texto.Text))
            {
                ObjComportamiento.showMsgError("Debe Ingresar un valor de PIN, para poder continuar");
                return;
            }

            // Validar PIN y volver a pantalla principal
            if (PIN == texto.Text)
            {
                ConfirmaPin cls = new ConfirmaPin(General.connectionString);
                //Verificar si el PIN sigue activo
                var estado = cls.PinActive(pinScl);

                switch (estado)
                {
                    case "A":
                        //Cambiar el estado del PIN a S
                        cls.msgatv = "S";
                        cls.msgtip = tipo;
                        cls.tiporg = origen;
                        cls.msgscl = pinScl;
                        cls.grabar("M");
                        //Retorna a la pantalla principal
                        aceptaPin(true);
                        break;
                    case "I":
                        ObjComportamiento.showMsgError("El PIN ingresado ya ha CADUCADO. Genere uno nuevo y REINTENTE");
                        break;
                    case "S":
                        ObjComportamiento.showMsgError("El PIN ingresado ya no se lo considera VÁLIDO. Genere uno nuevo y REINTENTE");
                        break;
                }
                
                //Cierra la pantalla
                cancelar_Click(sender, e);
            }
            else
            {
                ObjComportamiento.showMsgError("El PIN ingresado es INVÁLIDO");
                return;
            }
        }

        private void texto_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                enter.Focus();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (texto.Text.Length == maxLength)
            {
                texto.Focus();
                return;
            }

            int _idx = texto.SelectionStart;
            string txt1 = "";
            string txt2 = "";

            if (_idx >= 0 && _idx < texto.Text.Length)
            {
                txt1 = texto.Text.Substring(0, _idx);
                txt2 = texto.Text.Substring(_idx);

                if (!((Button)sender).Tag.ToString().Equals(""))
                {
                    texto.Text = txt1 + ((Button)sender).Tag.ToString() + txt2;
                    texto.SelectionStart = _idx + ((Button)sender).Tag.ToString().Length;
                }
                else
                {
                    texto.Text = txt1 + ((Button)sender).Content.ToString() + txt2;
                    texto.SelectionStart = _idx + ((Button)sender).Content.ToString().Length;
                }
            }
            else
            {
                if (!((Button)sender).Tag.ToString().Equals(""))
                {
                    texto.Text += ((Button)sender).Tag.ToString();
                    texto.SelectionStart = _idx + ((Button)sender).Tag.ToString().Length;
                }
                else
                {
                    texto.Text += ((Button)sender).Content.ToString();
                    texto.SelectionStart = _idx + ((Button)sender).Content.ToString().Length;
                }
            }
            texto.Focus();
        }

        private void UniformGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Boolean)e.NewValue)
            {
                texto.Focus();
            }
        }

        private void btnPto_Click(object sender, RoutedEventArgs e)
        {
            // Actualiza estado de PIN a Inactivo

            ConfirmaPin cls = new ConfirmaPin(General.connectionString);
            cls.msgpin = General.generaPin();
            cls.msgatv = "A";
            cls.msgtip = tipo;
            cls.tiporg = origen;
            cls.usucod = codigoEmpleado;
            cls.secorg = codigo;

            PIN = cls.msgpin;

            cls.grabar("R");

            // envía el PIN al usuario vía Whatsapp
            General.enviarWhatsapp("Ingrese el PIN #" + cls.msgpin + " Para confirmar la acción de eliminacion del Pedido.", "+539984990123");
            HttpClienteAPI client = new HttpClienteAPI();

            reset();
        }
    }
}
