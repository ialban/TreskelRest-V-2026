using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace LibControls.buttons
{
    /// <summary>
    /// Interaction logic for ctlButton.xaml
    /// </summary>
    public partial class ctlButton : UserControl
    {
        private String _codigo;
        private String _titulo;
        private String _imagen;
        private byte[] _icono;
        private String _tag;
        private String _secTag = "";

        private double _ancho;
        private double _alto;

        private Boolean _ShowImage = true;
        private Boolean _ShowCodigo = true;
        private Boolean _CentrarTexto = false;

        public enum TamanioButton
        {
            Normal = 1,
            Small = 2
        }

        private TamanioButton _setTamanioButton { get; set; } = TamanioButton.Normal;

        public delegate void ctlButton_Click(object sender, RoutedEventArgs e, string tipo);
        public event ctlButton_Click _ctlButtonClick;

        public TamanioButton SetTamanioButton
        {
            set
            {
                _setTamanioButton = value;

                if (value == TamanioButton.Normal)
                {
                    codigo.FontSize = 14;
                    titulo.FontSize = 14;
                    Boton.Width = 195;
                    Boton.Height = 80;
                }
                else
                {
                    codigo.FontSize = 10;
                    titulo.FontSize = 10;
                    Boton.Width = 160;
                    Boton.Height = 60;
                }
            }
        }

        public Boolean CentrarTexto
        {
            get
            {
                return _CentrarTexto;
            }
            set
            {
                _CentrarTexto = value;
                if (_CentrarTexto)
                {
                    titulo.TextAlignment = TextAlignment.Center;
                }
                else
                {
                    titulo.TextAlignment = TextAlignment.Left;
                }

            }
        }

        public Boolean ShowCodigo
        {
            get
            {
                return _ShowCodigo;
            }
            set
            {
                _ShowCodigo = value;
                if (_ShowCodigo)
                {
                    codigo.Visibility = Visibility.Visible;
                }
                else
                {
                    codigo.Visibility = Visibility.Hidden;
                    codigo.Height = 0;
                    codigo.Width = 0;
                }

            }
        }

        public Boolean ShowImage
        {
            get
            {                
                return _ShowImage;
            }
            set
            {
                _ShowImage = value;
                if (_ShowImage)
                {
                    imagen.Visibility = Visibility.Visible;
                }else
                {
                    imagen.Visibility = Visibility.Hidden;
                    imagen.Width = 0;
                    imagen.Height = 0;
                }
                
            }
        }

        public String Codigo{
            get {
                _codigo = codigo.Content.ToString();
                return _codigo;
            }
            set
            {
                _codigo = value;
                codigo.Content = _codigo;
            } 
        }

        public String CodigoAlterno
        {
            get
            {
                _tag = Boton.Tag.ToString();
                return _tag;
            }
            set
            {
                _tag = value;
                Boton.Tag = _tag;
            }
        }

        public String CodigoSubAlterno
        {
            get
            {
                _secTag = Boton.Uid;
                return _secTag;
            }
            set
            {
                Boton.Uid = value;
                _secTag = value;
            }
        }

        public double Ancho
        {
            get
            {
                _ancho = Boton.Width;
                return _ancho;
            }
            set
            {
                _ancho  = value;
                Boton.Width = _ancho;
            }
        }

        public double Alto
        {
            get
            {
                _alto = Boton.Height;
                return _alto;
            }
            set
            {
                _alto = value;
                Boton.Height = _alto;
            }
        }

        public String Titulo
        {
            get
            {
                _titulo = titulo.Text.ToString();
                return _titulo;
            }
            set
            {
                _titulo = value;
                titulo.Text = _titulo;
                if(_ShowImage)
                    stackTexto.Width = Boton.Width-53;
                else
                    stackTexto.Width = Boton.Width-5;
            }
        }

        public String Imagen
        {
            get
            {
                return _imagen;
            }
            set
            {
                _imagen = value;
                imagen.Source = new BitmapImage(new Uri(_imagen));
            }
        }

        public byte[] Icono
        {
            get
            {
                return _icono;
            }
            set
            {
                _icono = value;
                
                imagen.Source = LoadImage(_icono);
            }
        }


        public ctlButton()
        {
            InitializeComponent();
        }

        public void setBackgroundColor(Brush valor)
        {
            Boton.Background = valor;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this._ctlButtonClick(sender, e, _codigo);
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
