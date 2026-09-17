using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibControls.switchButton
{
    /// <summary>
    /// Lógica de interacción para ctlSwitchButton.xaml
    /// </summary>
    public partial class ctlSwitchButton : UserControl
    {
        private double cornerRadiusProperty;
        private bool _IsEnabled;
        private TemaColores _temaColores;
        private string _texto;
        private string _alterno;
        //private bool _activo = true;
        private Brush colorPreview;
        private string _valor = "";

        public enum TemaForma
        {
            Circular = 1,
            Cuadrado = 2,
            BordesRedondos = 3
        }

        public enum TemaColores
        {
            VerdeBlanco = 1,
            NaranjaBlanco = 2,
            RojoIndioBlanco = 3,
            TurquesaBlanco = 4,
            AzulBlanco = 5
        }

        public double cornerRadius
        {
            get { return cornerRadiusProperty; }
            set
            {
                CornerRadius prop = new CornerRadius(value, value, value, value);
                cornerRadiusProperty = value;
                
                BtnInterno.CornerRadius = prop;
                fondo.CornerRadius = prop;
            }
        }

        public string Valores
        {
            set
            {
                var valores = value.Split('/');
                if (valores.Length == 1)
                {
                    _valor = value + "/ ";
                }
                if (valores.Length > 2)
                {
                    _valor = valores[0] + "/" + valores[1];
                }
                _valor = value;
            }
        }

        public string texto
        {
            set
            {
                _texto = value;
                Texto.Text = value;
            }
        }

        public string textoAlterno
        {
            set
            {
                _alterno = value;
            }
        }

        public bool isEnabled
        {
            get { return _IsEnabled; }
            set {
                _IsEnabled = value;
                if (value)
                {
                    Colores =_temaColores;
                }
                else
                {
                    fondo.Background = Brushes.LightGray;
                }
            }
        }

        public TemaForma Forma
        {
            set
            {
                switch (value)
                {
                    case TemaForma.Circular:
                        BtnInterno.CornerRadius = new CornerRadius(50, 50, 50, 50);
                        fondo.CornerRadius = new CornerRadius(25, 25, 25, 25);
                        break;
                    case TemaForma.Cuadrado:
                        BtnInterno.CornerRadius = new CornerRadius(0, 0, 0, 0);
                        fondo.CornerRadius = new CornerRadius(0, 0, 0, 0);
                        break;
                    case TemaForma.BordesRedondos:
                        BtnInterno.CornerRadius = new CornerRadius(10, 10, 10, 10);
                        fondo.CornerRadius = new CornerRadius(10, 10, 10, 10);
                        break;
                }
            }
        }

        public TemaColores Colores
        {
            set
            {
                _temaColores = value;
                switch (value)
                {
                    case TemaColores.VerdeBlanco:
                        fondo.Background = Brushes.Green;
                        Texto.Foreground = Brushes.White;
                        break;
                    case TemaColores.NaranjaBlanco:
                        fondo.Background = Brushes.Orange;
                        Texto.Foreground = Brushes.Black;
                        break;
                    case TemaColores.RojoIndioBlanco:
                        fondo.Background = Brushes.IndianRed;
                        Texto.Foreground = Brushes.White;
                        break;
                    case TemaColores.TurquesaBlanco:
                        fondo.Background = Brushes.Turquoise;
                        Texto.Foreground = Brushes.Black;
                        break;
                    case TemaColores.AzulBlanco:
                        fondo.Background = Brushes.Blue;
                        Texto.Foreground = Brushes.White;
                        BtnInterno.Background = Brushes.LightGreen;
                        colorPreview = BtnInterno.Background;
                        break;
                }
            }
        }

        public ctlSwitchButton()
        {
            InitializeComponent();

            BtnInterno.MouseDown += new MouseButtonEventHandler(Button_Click);
            BtnInterno.MouseEnter += new MouseEventHandler(Button_Over);
            BtnInterno.MouseLeave += new MouseEventHandler(Button_Leave);
        }

        private void clickButton()
        {
            if (BtnInterno.HorizontalAlignment == HorizontalAlignment.Left)
            {
                BtnInterno.HorizontalAlignment = HorizontalAlignment.Right;
                Texto.Text = _texto;
                Texto.SetValue(Grid.ColumnProperty, 0);
                BtnInterno.Background = Brushes.LightGreen;
            }
            else
            {
                BtnInterno.HorizontalAlignment = HorizontalAlignment.Left;
                Texto.Text = _alterno;
                Texto.SetValue(Grid.ColumnProperty, 1);
                BtnInterno.Background = Brushes.LightCoral;
            }
            colorPreview = BtnInterno.Background;
        }

        private void Button_Click(object sender, MouseButtonEventArgs e)
        {
            clickButton();
        }

        private void Button_Over(object sender, MouseEventArgs e )
        {
            colorPreview = BtnInterno.Background;
            BtnInterno.Background = Brushes.LightBlue;
        }

        private void Button_Leave(object sender, MouseEventArgs e)
        {
            BtnInterno.Background = colorPreview;
        }

        public string Valor
        {
            get
            {
                string[] valores = _valor.Split('/');
                if (BtnInterno.HorizontalAlignment == HorizontalAlignment.Left)
                {
                    return valores[1];
                }
                else
                {
                    return valores[0];
                }
            }
            set
            {
                string[] valores = _valor.Split('/');
                if (value == valores[0])
                {
                    BtnInterno.HorizontalAlignment = HorizontalAlignment.Left;
                }
                else
                {
                    BtnInterno.HorizontalAlignment = HorizontalAlignment.Right;
                }
                clickButton();
            }
        }
    }
}
