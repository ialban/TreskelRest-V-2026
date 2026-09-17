using Entidades;
using Entidades.modelos;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlIngresaCoord.xaml
    /// </summary>
    public partial class ctlIngresaCoord : Window
    {
        private int minuto = 5;
        private int segundo = 0;
        private Thread tiempo = null;
        private int maxLength = 6;
        private mCoordenada _coordenada = new mCoordenada { 
            coord1 = null, 
            coord2 = null, 
            coord3 = null 
        };
        public string codigoEmpleado;
        private string codigoOrigen;
        
        private int coordScl = 0;
        private string tipo = "";
        private string origen = "";

        private bool continuar = true;
        private bool pausa = false;
        private TextBox lastFocus;
        private TextBox currFocus;
        private bool isBack = false;

        public event AceptaCoord aceptaCoord;
        public delegate void AceptaCoord(bool aceptado);

        public mCoordenada SetearCoordenada
        {
            set
            {
                _coordenada = value;

                lblCoord1.Content = _coordenada.lcoor1;
                lblCoord2.Content = _coordenada.lcoor2;
                lblCoord3.Content = _coordenada.lcoor3;
            }
        }

        public int SetearCoordSecuencial
        {
            set
            {
                coordScl = value;
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

        public string SetearCodigoOrigen
        {
            set
            {
                codigoOrigen = value;
            }
        }

        public ctlIngresaCoord()
        {
            InitializeComponent();

            minuto = General.SEGUNDOS_ESPERA / 60;
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
            catch (Exception e)
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
            coord1.Text = "";
            coord2.Text = "";
            coord3.Text = "";
            minuto = General.SEGUNDOS_ESPERA / 60;
            segundo = 0;

            setTiempo();
            Thread.Sleep(1000);
            pausa = false;
        }

        private void coord1_KeyUp(object sender, KeyEventArgs e)
        {
            isBack = false;
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                enter.Focus();
            }

            if (e.Key == Key.Back)
            {
                isBack = true;
            }

            if (((TextBox) sender).Text.Length == 2)
            {
                coord2.Focus();
            }
        }

        private void coord2_KeyUp(object sender, KeyEventArgs e)
        {
            isBack = false;
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                enter.Focus();
            }

            if (e.Key == Key.Back)
            {
                isBack = true;
            }

            if (((TextBox)sender).Text.Length == 2)
            {
                coord3.Focus();
            }
        }

        private void coord3_KeyUp(object sender, KeyEventArgs e)
        {
            isBack = false;
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                enter.Focus();
            }

            if (e.Key == Key.Back)
            {
                isBack = true;
            }
        }

        private void coord1_LostFocus(object sender, RoutedEventArgs e)
        {
            lastFocus = (TextBox)sender;
        }

        private void coord2_LostFocus(object sender, RoutedEventArgs e)
        {
            lastFocus = (TextBox)sender;
        }

        private void coord3_LostFocus(object sender, RoutedEventArgs e)
        {
            lastFocus = (TextBox)sender;
        }

        private void coord1_GotFocus(object sender, RoutedEventArgs e)
        {
            currFocus = coord1;
            if (coord1.Text.Length >= 2 && !isBack)
            {
                coord2.Focus();
            }
        }

        private void coord2_GotFocus(object sender, RoutedEventArgs e)
        {
            currFocus = coord2;
            if (coord2.Text.Length >= 2 && !isBack)
            {
                coord3.Focus();
            }
        }

        private void coord3_GotFocus(object sender, RoutedEventArgs e)
        {
            currFocus = coord3;
            if (coord3.Text.Length >= 2 && !isBack)
            {
                enter.Focus();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BotonKeyUp(sender);
        }

        private void btnPto_Click(object sender, RoutedEventArgs e)
        {
            isBack = false;

            //Grabar la coordenada generada y el estatus en S
            Coordenada newCoord = new Coordenada(General.connectionString);

            mCoordenada valor = newCoord.Generar("R", _coordenada.coordscl, tipo, origen, codigoOrigen, codigoEmpleado, "A");

            _coordenada = valor;

            lblCoord1.Content = _coordenada.lcoor1;
            lblCoord2.Content = _coordenada.lcoor2;
            lblCoord3.Content = _coordenada.lcoor3;

            coordScl = _coordenada.coordscl;

            reset();
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            isBack = false;

            if (contador.Content.ToString() == "00:00")
            {
                ObjComportamiento.showMsgError("El tiempo para ingresar la COORDENADA válida ha CADUCADO");
                return;
            }

            if (string.IsNullOrEmpty(coord1.Text) ||
                string.IsNullOrEmpty(coord2.Text) ||
                string.IsNullOrEmpty(coord3.Text))
            {
                ObjComportamiento.showMsgError("Debe Ingresar los valores de las COORDENADAS, para poder continuar");
                return;
            }

            // Validar PIN y volver a pantalla principal
            if (_coordenada.lcoor1 == lblCoord1.Content.ToString() &&
                _coordenada.coord1 == coord1.Text &&
                _coordenada.lcoor2 == lblCoord2.Content.ToString() &&
                _coordenada.coord2 == coord2.Text &&
                _coordenada.lcoor3 == lblCoord3.Content.ToString() &&
                _coordenada.coord3 == coord3.Text)
            {
                Coordenada newCoord = new Coordenada(General.connectionString);
                //Verificar si la coordenada sigue activo
                var estado = newCoord.CoordenadaActive(coordScl);
                //Grabar la coordenada generada y el estatus en S
                switch (estado)
                {
                    case "A":
                        //Cambiar el estado de la coordenada a S
                        newCoord.Generar("M", coordScl, "", "", "", codigoEmpleado, "S");
                        //Retorna a la pantalla principal
                        aceptaCoord(true);
                        break;
                    case "I":
                        ObjComportamiento.showMsgError("La COORDENADA ingresada ya ha CADUCADO. Genere uno nuevo y REINTENTE");
                        break;
                    case "S":
                        ObjComportamiento.showMsgError("La COORDENADA ingresada ya no se lo considera VÁLIDO. Genere uno nuevo y REINTENTE");
                        break;
                }

                mCoordenada valor = newCoord.Generar("M", _coordenada.coordscl, "", "", "", "", "S");
                //Cierra la pantalla
                cancelar_Click(sender, e);
            }
            else
            {
                ObjComportamiento.showMsgError("La COORDENADA ingresada es INVÁLIDA");
                return;
            }
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            isBack = false;
            coord1.Text = "";
            coord2.Text = "";
            coord3.Text = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            isBack = true;
            String _txt = currFocus.Text.ToString();
            int _idx = currFocus.SelectionStart;

            if (lastFocus.SelectionLength > 0)
            {
                currFocus.Text = currFocus.Text.Remove(_idx, currFocus.SelectionLength);
                currFocus.Focus();
                return;
            }

            if (_txt.Length > 0)
            {
                if (_idx > 0 && _idx < currFocus.Text.Length)
                {
                    String txt1 = currFocus.Text.Substring(0, _idx);
                    String txt2 = currFocus.Text.Substring(_idx);

                    txt1 = txt1.Remove(txt1.Length - 1, 1);
                    currFocus.Text = txt1 + txt2;
                }
                else
                {
                    if (_idx == 0) return;
                    currFocus.Text = _txt.Substring(0, _txt.Length - 1);
                }
                currFocus.SelectionStart = (_idx - 1) > 0 ? _idx - 1 : 0;
            }
            else
            {
                if (currFocus.Name == "coord3") {
                    coord2.Focus();

                }else if (currFocus.Name == "coord2") {
                    coord1.Focus();
                }
            }
            currFocus.Focus();
        }

        private TextBox getFocusable()
        {
            return lastFocus;
        }

        private void BotonKeyUp(object sender)
        {
            isBack = false;
            TextBox texto = getFocusable();

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

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            tiempo = null;
            this.Close();
        }

        private void UniformGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Boolean)e.NewValue)
            {
                coord1.Focus();
            }
        }

        
    }
}
